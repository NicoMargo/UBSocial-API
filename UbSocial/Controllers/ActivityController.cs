using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UbSocial.Models;
using UbSocial.Models.Helpers;

namespace UBSocial.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ActivityController : ControllerBase
    {
        // GET ALL
        // Ejemplo: (GET) localhost:7004/activity

        [Route("{page}")]
        [Route("")]
        public IActionResult ActivityGet(int page = 0)
        {
            try
            {
                Dictionary<string, object> args = new Dictionary<string, object> {
                    {"pPage",page}
                };
                return Ok(DBHelper.callProcedureReader("spActivityGetAll", args));
            }
            catch
            {
                return StatusCode(500, "Error al obtener la informacion de las actividades");
            }
        }

        // GET BY ID
        // Ejemplo: (GET) localhost:7004/activity/ActivityIdentifier/1

        [HttpGet("ActivityIdentifier/{id}")]
        public IActionResult ActivityGetById(int id)
        {
            try
            {
                Dictionary<string, object> args = new Dictionary<string, object> {
                    {"pId",id}
                };
                return Ok(DBHelper.callProcedureReader("spActivityGetById", args));
            }
            catch
            {
                return StatusCode(500, "Error al obtener la informacion de las actividades");
            }
        }

        // GET BY Id User
        // Ejemplo: (GET) localhost:7004/activity/current

        [HttpGet("current")]
        [Authorize]
        public IActionResult ActivityGetByIdUser()
        {
            int? userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            try
            {
                Dictionary<string, object> args = new Dictionary<string, object> {
                    {"pIdUser",userId}
                };
                return Ok(DBHelper.callProcedureReader("spActivityGetByIdUser", args));
            }
            catch
            {
                return StatusCode(500, "Error al obtener la informacion de las actividades");
            }
        }

        // GET BY TITLE
        // Ejemplo: (GET) localhost:7004/activity/currentTitle/valorant

        [HttpGet("currentTitle/{title}")]
        public IActionResult ActivityGetByTitle(string title)
        {
            try
            {
                Dictionary<string, object> args = new Dictionary<string, object> {
                    {"pTitle",title}
                };
                return Ok(DBHelper.callProcedureReader("spActivityGetByTitle", args));
            }
            catch
            {
                return StatusCode(500, "Error al obtener la informacion de las actividades");
            }
        }

        // DELETE BY ID
        // Ejemplo: (DELETE) localhost:7004/activity/1

        [HttpDelete("{id}")]
        [Authorize]
        public IActionResult Delete(int id)
        {
            string success = "Error al eliminar la actividad";
            int idUser = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            try
            {
                if (id >= 0)
                {

                    Dictionary<string, object> args = new Dictionary<string, object> {

                        {"pId",id},
                        {"pIdUser",idUser}

                    };

                    success = DBHelper.CallNonQuery("spActivityDelete", args);

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
        // Ejemplo: (POST) localhost:7004/activity

        [HttpPost]
        [Authorize]
        public IActionResult Create([FromForm] Activity activity)
        {
            string success = "Error al crear la actividad";
            int idUser = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            try
            {
                if (activity.File.FileName != null)
                {
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "WWWRoot", "ActivityPhotos", activity.File.FileName);

                    var originalFilePath = filePath;
                    var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(originalFilePath);
                    var extension = Path.GetExtension(originalFilePath);
                    var counter = 1;

                    const int maxFileSizeMB = 15;
                    FileInfo fileInfo = new FileInfo(filePath);

                    // Obtener el tamaño del archivo en bytes
                    long fileSizeInBytes = fileInfo.Length;

                    // Convertir a Megabytes
                    double fileSizeInMB = (double)fileSizeInBytes / 1024 / 1024;

                    // Verificar si el tamaño del archivo es mayor que el máximo permitido
                    if (fileSizeInMB > maxFileSizeMB)
                    {
                        throw new Exception($"El archivo es demasiado grande. Debe ser menor de {maxFileSizeMB} MB.");
                    }

                    while (System.IO.File.Exists(filePath))
                    {
                        var newFileName = $"{fileNameWithoutExtension}({counter}){extension}";
                        filePath = Path.Combine("WWWRoot", "ActivityPhotos", newFileName);
                        counter++;
                    }

                    using (var stream = System.IO.File.Create(filePath))
                    {
                        activity.File.CopyToAsync(stream);
                    }

                    Dictionary<string, object> args = new Dictionary<string, object> {
                         {"pTitle",activity.Title},
                         {"pDescription",activity.Description},
                         {"pContact",activity.Contact},
                         // {"pActivityDate",activity.ActivityDate},
                         {"pDateFinishActivity",activity.ActivityDateFinished},
                         {"pURLphotos","/ActivityPhotos/" + activity.File.FileName},
                         {"pIdUser",idUser}
                    };

                    success = DBHelper.CallNonQuery("spActivityCreate", args);

                    if (success == "1")
                    {
                        return Ok();
                    }

                    else
                    {
                        return StatusCode(500, success);
                    }
                }

                return StatusCode(500, success);
            }
            catch
            {
                return StatusCode(500, success);
            }

        }

        // UPDATE
        // Ejemplo: (PUT) localhost:7004/activity

        [HttpPut("{id}")]
        [Authorize]
        public IActionResult Update([FromForm] Activity activity, int id)
        {
            string success = "Error al modificar la actividad";
            int idUser = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            try
            {
                if (activity.File.FileName != null && activity.Title != null && activity.Description != null && id != null && activity.Contact != null && activity.ActivityDateFinished != null && idUser != null)
                {
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "WWWRoot", "ActivityPhotos", activity.File.FileName);

                    var originalFilePath = filePath;
                    var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(originalFilePath);
                    var extension = Path.GetExtension(originalFilePath);
                    var counter = 1;

                    while (System.IO.File.Exists(filePath))
                    {
                        var newFileName = $"{fileNameWithoutExtension}({counter}){extension}";
                        filePath = Path.Combine("WWWRoot", "ActivityPhotos", newFileName);
                        counter++;
                    }

                    using (var stream = System.IO.File.Create(filePath))
                    {
                        activity.File.CopyToAsync(stream);
                    }

                    Dictionary<string, object> args = new Dictionary<string, object> {
                         {"pTitle",activity.Title},
                         {"pDescription",activity.Description},
                         {"pContact",activity.Contact},
                         // {"pActivityDate",activity.ActivityDate},
                         {"pDateFinishActivity",activity.ActivityDateFinished},
                         {"pURLphotos","/ActivityPhotos/" + activity.File.FileName},
                         {"pId",id},
                         {"pIdUser",idUser}
                    };

                    success = DBHelper.CallNonQuery("spActivityUpdate", args);

                    if (success == "1")
                    {
                        return Ok();
                    }

                    else
                    {
                        return StatusCode(500, success);
                    }
                }

                return StatusCode(500, success);
            }
            catch
            {
                return StatusCode(500, success);
            }
            
        }

        // ACTIVITY JOIN
        // Ejemplo: (POST) localhost:7004/activity/join

        [HttpPost("join/{id}")]
        public IActionResult ActivityJoin(int id)
        {
            string success = "Error al unirse a la actividad";
            int idUser = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            try
            {
                Dictionary<string, object> args = new Dictionary<string, object> {
                         {"pIdActivity",id},
                         {"pIdUser",idUser}
                };

                success = DBHelper.CallNonQuery("spActivityJoin", args);

                if (success == "2")
                {
                    return Ok();
                }

                else
                {
                    return StatusCode(500, success);
                }

            }
            catch
            {
                return StatusCode(500, success);
            }
            

        }

    }
}
