using System.Diagnostics;
using System.Reflection;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using UbSocial.Models;
using UbSocial.Models.Helpers;

namespace UBSocial.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class DownloadableContentController : ControllerBase
    {

        // GET ALL
        // Ejemplo: (PUT) localhost:7004/downloadableContent

        [HttpGet]
        public IActionResult DownloadableContentGet()
        {
            try
            {
                return Ok(DBHelper.callProcedureReader("spDownloadableContentGetAll", new Dictionary<string, object> { }));
            }
            catch
            {
                return StatusCode(500, "Error al obtener la informacion del contenido");
            }
        }

        // GET BY ID
        // Ejemplo: (GET) localhost:7004/downloadableContent/1

        [HttpGet("{id}")]
        public IActionResult DownloadableContentGetById(int id)
        {
            try
            {
                Dictionary<string, object> args = new Dictionary<string, object> {
                    {"pId",id}
                };
                return Ok(DBHelper.callProcedureReader("spDownloadableContentGetById", args));
            }
            catch
            {
                return StatusCode(500, "Error al obtener la informacion del contenido");
            }
        }

        // GET BY Id User
        // Ejemplo: (GET) localhost:7004/downloadableContent/current

        [HttpGet("current")]
        [Authorize]
        public IActionResult DownloadableContentGetByIdUser()
        {
            int? userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            try
            {
                Dictionary<string, object> args = new Dictionary<string, object> {
                    {"pIdUser",userId}
                };
                return Ok(DBHelper.callProcedureReader("spDownloadableContentGetByIdUser", args));
            }
            catch
            {
                return StatusCode(500, "Error al obtener la informacion del contenido");
            }
        }

        // GET BY SUBJECT
        // Ejemplo: (GET) localhost:7004/downloadableContent/subject/1

        [HttpGet]
        [Route("subject/{id}")]
        public IActionResult DownloadableContentGetBySubject(int id)
        {
            try
            {
                Dictionary<string, object> args = new Dictionary<string, object> {
                    {"pIdSubject",id}
                };
                return Ok(DBHelper.callProcedureReader("spDownloadableContentGetBySubject", args));
            }
            catch
            {
                return StatusCode(500, "Error al obtener la informacion del contenido");
            }
        }

        // DELETE BY ID
        // Ejemplo: (DELETE) localhost:7004/downloadableContent/1

        [HttpDelete("{id}")]
        [Authorize]
        public IActionResult Delete(int id)
        {
            string success = "Error al eliminar el contenido";
            int idUser = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            try
            {
                if (id >= 0)
                {

                    Dictionary<string, object> args = new Dictionary<string, object> {
                        {"pId",id},
                        {"pIdUser",idUser}
                    };

                    success = DBHelper.CallNonQuery("spDownloadableContentDelete", args);

                    if (success == "1")
                    {
                        return Ok();
                    }

                }

                return StatusCode(400, "Se enviaron campos incompletos");

            }
            catch
            {
                return StatusCode(500, "Error al intentar eliminar el contenido descargable");
            }
        }

        // CREATE
        // Ejemplo: (POST) localhost:7004/downloadableContent

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromForm] DownloadableContent downloadableContent)
        {
            string success = "Error al crear el contenido";
            int idUser = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            try
            {
                if (downloadableContent.Title != null && downloadableContent.File.FileName != null  && downloadableContent.IdSubject != null)
                {
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "WWWRoot", "Content", downloadableContent.File.FileName);

                    var originalFilePath = filePath;
                    var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(originalFilePath);
                    var extension = Path.GetExtension(originalFilePath);
                    var counter = 1;

                    const int maxFileSizeMB = 15;
                    FileInfo fileInfo = new FileInfo(filePath);

                    // Obtener el tamaño del archivo en bytes
                    long fileSizeInBytes = downloadableContent.File.Length;

                    // Convertir a Kilobytes
                    double fileSizeInKB = fileSizeInBytes / 1024;

                    // Convertir a Megabytes
                    double fileSizeInMB = fileSizeInKB / 1024;

                    // Verificar si el tamaño del archivo es mayor que el máximo permitido
                    if (fileSizeInMB > maxFileSizeMB)
                    {
                        success = ($"El archivo es demasiado grande. Debe ser menor de {maxFileSizeMB} MB.");
                        return StatusCode(400, success);
                    }

                    var newFileName = $"{fileNameWithoutExtension}{extension}";

                    while (System.IO.File.Exists(filePath))
                    {
                        newFileName = $"{fileNameWithoutExtension}({counter}){extension}";
                        filePath = Path.Combine("WWWRoot", "Content", newFileName);
                        counter++;
                    }

                    Dictionary<string, object> args = new Dictionary<string, object> {
                         {"pTitle",downloadableContent.Title},
                         {"pURL","Content\\" + newFileName},
                         {"pIdSubject",downloadableContent.IdSubject},
                         {"pIdUser",idUser}
                    };

                    success = DBHelper.CallNonQuery("spDownloadableContentCreate", args);

                    if (success == "3")
                    {

                        using (var stream = System.IO.File.Create(filePath))
                        {
                            await downloadableContent.File.CopyToAsync(stream);
                        }
                        return Ok();
                    }

                    else
                    {
                        return StatusCode(400, "Se enviaron campos incompletos o erroneos");
                    }
                }

                return StatusCode(400, "Se enviaron campos incompletos o erroneos");

            }
            catch (NullReferenceException ex)
            {
                return StatusCode(400, "Se enviaron campos incompletos");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error al crear el contenido descargable");
            }

        }

        // DOWNLOAD
        // Ejemplo: (GET) localhost:7004/downloadableContent/download/pato.gif

        [HttpGet]
        [Authorize]
        [Route("download")]
        public IActionResult Download(string URL)
        {
            string success;
            int idUser = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            try
            {
                // Combina la ruta de la carpeta wwwroot con el nombre del archivo para obtener la ruta completa del archivo.
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "WWWRoot", URL);

                // Verifica si el archivo existe
                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound();
                }

                Dictionary<string, object> args = new Dictionary<string, object> {
                    {"pId",idUser}
                };

                success = DBHelper.callProcedureReader("spCanDownloadableContent", args);

                if (success == "True")
                {                    

                    // Obtiene el tipo MIME del archivo (importante para saber cómo el navegador debe manejar la descarga)
                    var mimeType = GetMimeType(filePath);

                    // Retorna el archivo para descarga
                    return PhysicalFile(filePath, mimeType, URL);

                }
                else
                {
                    return StatusCode(429, "Debes subir un archivo para poder descargar otros 3 archivos");
                }

            }
            catch (NullReferenceException ex)
            {
                return StatusCode(400, "Se enviaron campos incompletos");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error al crear el contenido descargable");
            }

        }
        private string GetMimeType(string filePath)
        {
            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(filePath, out var mimeType))
            {
                mimeType = "application/octet-stream"; // Tipo MIME por defecto si no se puede determinar
            }
            return mimeType;
        }

    }
}
