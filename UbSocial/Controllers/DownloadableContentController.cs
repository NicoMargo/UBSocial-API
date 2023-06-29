﻿using System.Reflection;
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
        // Ejemplo: (PUT) localhost:5665/downloadableContent

        [HttpGet]
        [Authorize]
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
        // Ejemplo: (GET) localhost:5665/downloadableContent/1

        [HttpGet("{id}")]
        [Authorize]
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

        // GET BY SUBJECT
        // Ejemplo: (GET) localhost:5665/downloadableContent/1

        [HttpGet]
        [Route("subject/{id}")]
        [Authorize]
        public IActionResult DownloadableContentGetBySubject(int id)
        {
            try
            {
                Dictionary<string, object> args = new Dictionary<string, object> {
                    {"pId",id}
                };
                return Ok(DBHelper.callProcedureReader("spDownloadableContentGetBySubject", args));
            }
            catch
            {
                return StatusCode(500, "Error al obtener la informacion del contenido");
            }
        }

        // DELETE BY ID
        // Ejemplo: (DELETE) localhost:5665/downloadableContent/1

        [HttpDelete("{id}")]
        [Authorize]
        public IActionResult Delete(int id)
        {
            string success = "Error al eliminar el contenido";
            try
            {
                if (id >= 0)
                {

                    Dictionary<string, object> args = new Dictionary<string, object> {

                    {"pId",id}

                    };

                    success = DBHelper.CallNonQuery("spDownloadableContentDelete", args);

                    if (success == "1")
                    {
                        return Ok();
                    }

                }

                return StatusCode(500, success);

            }
            catch
            {
                return StatusCode(500, success);
            }
        }

        // CREATE
        // Ejemplo: (POST) localhost:5665/downloadableContent

        [HttpPost]
        [Authorize]

        public IActionResult Create([FromForm] DownloadableContent downloadableContent)
        {
            string success = "Error al crear el contenido";
            try
            {
                if (downloadableContent.File.FileName != null)
                {
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "WWWRoot", "Content", downloadableContent.File.FileName);

                    var originalFilePath = filePath;
                    var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(originalFilePath);
                    var extension = Path.GetExtension(originalFilePath);
                    var counter = 1;

                    while (System.IO.File.Exists(filePath))
                    {
                        var newFileName = $"{fileNameWithoutExtension}({counter}){extension}";
                        filePath = Path.Combine("WWWRoot", "Content", newFileName);
                        counter++;
                    }

                    using (var stream = System.IO.File.Create(filePath))
                    {
                        downloadableContent.File.CopyToAsync(stream);
                    }

                    Dictionary<string, object> args = new Dictionary<string, object> {
                         {"pTitle",downloadableContent.File.FileName},
                         {"pURL","/Content/" + downloadableContent.File.FileName},
                         {"pIdSubject",downloadableContent.IdSubject},
                         {"pIdUser",downloadableContent.IdUser}
                    };

                    success = DBHelper.CallNonQuery("spDownloadableContentCreate", args);

                    if (success == "3")
                    {
                        return Ok();
                    }

                    else
                    {
                        return StatusCode(500, success);
                    }
                }
            }
            catch
            {
            }
            return StatusCode(500, success);
        }

        // DOWNLOAD
        // Ejemplo: (GET) localhost:5665/downloadableContent/download/pato.gif/1

        [HttpGet]
        [Authorize]
        [Route("download/{URL}/{id}")]
        public IActionResult Download(string URL, int id)
        {
            string success = "Error al crear el contenido";

            try
            {
                Dictionary<string, object> args = new Dictionary<string, object> {
                    {"pId",id}
                };

                success = DBHelper.callProcedureReader("spCanDownloadableContent", args);

                if (success == "True")
                {
                    // Combina la ruta de la carpeta wwwroot con el nombre del archivo para obtener la ruta completa del archivo.
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "WWWRoot/Content", URL);

                    // Verifica si el archivo existe
                    if (!System.IO.File.Exists(filePath))
                    {
                        return NotFound();
                    }

                    // Obtiene el tipo MIME del archivo (importante para saber cómo el navegador debe manejar la descarga)
                    var mimeType = GetMimeType(filePath);

                    // Retorna el archivo para descarga
                    return PhysicalFile(filePath, mimeType, URL);

                }

                else
                {
                    success = "Debe subir un archivo para poder descargar el contenido.";
                    return StatusCode(500, success);
                }

            }
            catch
            {
                return StatusCode(500, success);
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