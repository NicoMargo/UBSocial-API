using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UbSocial.Models;
using UbSocial.Models.Helpers;

namespace UBSocial.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class DownloadableContentController : ControllerBase
    {
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

        [HttpDelete("{id}")]
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

        [HttpPost]
        [Authorize]

        public IActionResult Create(DownloadableContent downloadableContent)
        {
            string success = "Error al crear el contenido";
            try
            {
                if (downloadableContent.Title != null && downloadableContent.Description != null && downloadableContent.URL != null && downloadableContent.DownloadableContentDate != null)
                {
                    Dictionary<string, object> args = new Dictionary<string, object> {
                         {"pTitle",downloadableContent.Title},
                         {"pDescription",downloadableContent.Description},
                         {"pURLPhotos",downloadableContent.URL},
                         {"pDownloadableContentDate",downloadableContent.DownloadableContentDate},
                    };
                    success = DBHelper.CallNonQuery("spDownloadableContentCreate", args);
                    if (success == "1")
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

        [HttpPut]
        public IActionResult Update(DownloadableContent downloadableContent)
        {
            string success = "Error al modificar el contenido";
            try
            {
                if (downloadableContent.Title != null && downloadableContent.Description != null && downloadableContent.URL != null && downloadableContent.DownloadableContentDate != null && downloadableContent.Id != null)
                {
                    Dictionary<string, object> args = new Dictionary<string, object> {
                        {"pTitle",downloadableContent.Title},
                        {"pDescription",downloadableContent.Description},
                        {"pURLPhotos",downloadableContent.URL},
                        {"pDownloadableContentDate",downloadableContent.DownloadableContentDate},
                        {"pId",downloadableContent.Id},


                    };

                    success = DBHelper.CallNonQuery("spDownloadableContentUpdate", args);

                    if (success == "1")
                    {
                        return Ok();
                    }
                    else
                    {
                        return StatusCode(500, success);
                    }
                }
                else
                {
                    success = "Error al intentar actualizar los datos. Revise nuevemente lo introducido.";
                }
            }
            catch
            {
            }
            return StatusCode(500, success);
        }

    }
}
