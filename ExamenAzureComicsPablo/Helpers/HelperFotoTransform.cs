namespace AyudaExamenViernes.Helpers
{
    public class HelperFotoTransform
    {
        private IWebHostEnvironment hostEnvironment;

        public HelperFotoTransform(IWebHostEnvironment hostEnvironment)
        {
            this.hostEnvironment = hostEnvironment;
        }

        // 1. Método original: Convierte IFormFile a byte[]
        public async Task<byte[]> ConvertirImagenABytesAsync(IFormFile imagen)
        {
            if (imagen == null || imagen.Length == 0) return null;

            using (MemoryStream memoryStream = new MemoryStream())
            {
                await imagen.CopyToAsync(memoryStream);
                return memoryStream.ToArray();
            }
        }

        // NUEVO: Convierte un Base64 (opcionalmente con prefijo data:image/..;base64,) a byte[]
        public byte[] ConvertirBase64ABytes(string base64)
        {
            if (string.IsNullOrWhiteSpace(base64)) return null;

            // Permite recibir data URLs desde Angular: "data:image/png;base64,AAAA..."
            var comaIndex = base64.IndexOf(',');
            if (comaIndex != -1)
            {
                base64 = base64[(comaIndex + 1)..];
            }

            try
            {
                return Convert.FromBase64String(base64);
            }
            catch (FormatException)
            {
                return null;
            }
        }

        // 2. NUEVO MÉTODO: Guarda el byte[] en un archivo físico y devuelve el nombre
        public async Task<string> GuardarArchivoByteAsync(byte[] datosImagen, string nombreOriginal, string carpeta)
        {
            if (datosImagen == null)
            {
                return null;
            }

            // Generamos un nombre único para no sobreescribir archivos
            string nombreArchivo = Guid.NewGuid().ToString() + "_" + nombreOriginal;

            // Construimos la ruta hacia la carpeta (ej. imagenes en la raíz del proyecto)
            string path = Path.Combine(this.hostEnvironment.ContentRootPath, carpeta, nombreArchivo);

            // Escribimos el arreglo de bytes directamente en un archivo físico
            await System.IO.File.WriteAllBytesAsync(path, datosImagen);

            // Retornamos el nombre para guardarlo en la base de datos
            return nombreArchivo;
        }
    }
}