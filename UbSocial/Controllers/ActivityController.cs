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
        // Ejemplo: (GET) localhost:5665/activity

        [Route("{page}")]
        [Route("")]
        [HttpGet]
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
        // Ejemplo: (GET) localhost:5665/activity/1

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

        // GET BY TOKEN
        // Ejemplo: (GET) localhost:5665/activity/current

        [HttpGet("current")]
        [Authorize]
        public IActionResult ActivityGetByToken()
        {
            int? userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            try
            {
                Dictionary<string, object> args = new Dictionary<string, object> {
                    {"pId",userId}
                };
                return Ok(DBHelper.callProcedureReader("spActivityGetById", args));
            }
            catch
            {
                return StatusCode(500, "Error al obtener la informacion de las actividades");
            }
        }

        // GET BY TITLE
        // Ejemplo: (GET) localhost:5665/activity/currentTitle/valorant

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
        // Ejemplo: (DELETE) localhost:5665/activity/1

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
        // Ejemplo: (POST) localhost:5665/activity

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
                         // {"pActivityDateFinished",activity.ActivityDateFinished},
                         {"pURL","/ActivityPhotos/" + activity.File.FileName},
                         {"pIdSubject",activity.IdActivity},
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
        // Ejemplo: (PUT) localhost:5665/activity

        [HttpPut]
        [Authorize]
        public IActionResult Update([FromForm] Activity activity)
        {
            string success = "Error al modificar la actividad";
            int idUser = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            try
            {
                if (activity.File.FileName != null && activity.Title != null && activity.Description != null && activity.Id != null && activity.Contact != null && activity.ActivityDate != null && activity.URLPhotos != null && activity.IdUser != null)
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
                         // {"pActivityDateFinished",activity.ActivityDateFinished},
                         {"pURL","/ActivityPhotos/" + activity.File.FileName},
                         {"pIdSubject",activity.IdActivity},
                         {"pIdUser",idUser}
                    };

                    success = DBHelper.CallNonQuery("spActivityUpdate", args);

                    if (success == "4")
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

        // ACTIVITY MEMBERS
        // Ejemplo: (POST) localhost:5665/activity

        [HttpPost]
        public IActionResult ActivityMember(Activity activity)
        {
            string success = "Error al mostrar los usuarios de la actividad";
            try
            {
                Dictionary<string, object> args = new Dictionary<string, object> {
                         {"pIdActivity",activity.IdActivity},
                         {"pIdUser",activity.IdUser}
                };

                success = DBHelper.CallNonQuery("spActivityMembers", args);

                if (success == "1")
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
