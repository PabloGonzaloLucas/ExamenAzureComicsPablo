using ApiExamenAzureComics.Models;
using AyudaExamenViernes.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace ApiExamenAzureComics.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestingFilesController : ControllerBase
    {
        private readonly HelperFotoTransform helper;

        public TestingFilesController(HelperFotoTransform helper)
        {
            this.helper = helper;
        }

        [HttpPost]
        public async Task<ActionResult> UploadFile ([FromBody] FileModel fileModel)
        {
            if (fileModel == null || string.IsNullOrWhiteSpace(fileModel.FileName) || string.IsNullOrWhiteSpace(fileModel.FileContent))
            {
                return BadRequest("Debe enviar FileName y FileContent (Base64). ");
            }

            byte[] imagenBytes = this.helper.ConvertirBase64ABytes(fileModel.FileContent);
            if (imagenBytes == null)
            {
                return BadRequest("El contenido Base64 no es válido.");
            }

            string nombreArchivo = await this.helper.GuardarArchivoByteAsync(imagenBytes, fileModel.FileName, "Imagenes");
            if (string.IsNullOrWhiteSpace(nombreArchivo))
            {
                return BadRequest("No se pudo guardar la imagen.");
            }

            return Ok(nombreArchivo);
        }

        [HttpGet]
        [Route("Images")]
        public ActionResult<List<ApiExamenAzureComics.Models.FileInfo>> Images()
        {
            string carpeta = "Imagenes";
            string pathCarpeta = Path.Combine(Directory.GetCurrentDirectory(), carpeta);

            if (!Directory.Exists(pathCarpeta))
            {
                return Ok(new List<ApiExamenAzureComics.Models.FileInfo>());
            }

            // Build the base URL dynamically based on the current request
            var scheme = Request.Scheme;
            var host = Request.Host;
            string baseUrl = $"{scheme}://{host}";
            const string requestPath = "/imagenes/";

            var archivos = Directory
                .GetFiles(pathCarpeta)
                .Select(ruta => Path.GetFileName(ruta))
                .Where(nombre => !string.IsNullOrWhiteSpace(nombre))
                .Select(nombre => new ApiExamenAzureComics.Models.FileInfo
                {
                    FileName = nombre,
                    UrlPath = $"{baseUrl}{requestPath}{nombre}"
                })
                .ToList();

            return Ok(archivos);
        }

       
    }
}
