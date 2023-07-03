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
        [HttpGet]
        public IActionResult ActivityGet(int page = 0)
        {
            int? idUser = null;

            if (User.FindFirst(ClaimTypes.NameIdentifier)?.Value != null)
            {
                idUser = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            }

            try
            {
                Dictionary<string, object> args = new Dictionary<string, object> {
                    {"pPage",page},
                    {"pIdUser",idUser}
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
            int? idUser = null;

            if (User.FindFirst(ClaimTypes.NameIdentifier)?.Value != null)
            {
                idUser = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            }

            try
            {
                Dictionary<string, object> args = new Dictionary<string, object> {
                    {"pId",id},
                    {"pIdUser",idUser}
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
            int? idUser = null;

            if (User.FindFirst(ClaimTypes.NameIdentifier)?.Value != null)
            {
                idUser = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            }

            try
            {
                Dictionary<string, object> args = new Dictionary<string, object> {
                    {"pIdUser",idUser}
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
            int? idUser = null;

            if (User.FindFirst(ClaimTypes.NameIdentifier)?.Value != null)
            {
                idUser = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            }

            try
            {
                Dictionary<string, object> args = new Dictionary<string, object> {
                    {"pTitle",title},
                    {"pIdUser",idUser}
                };
                return Ok(DBHelper.callProcedureReader("spActivityGetByTitle", args));
            }
            catch
            {
                return StatusCode(400, "Titulo no encontrado. Por favor intente denuevo");
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

                return StatusCode(400, "Se enviaron campos incompletos o erroneos");

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
        public async Task<IActionResult> Create([FromForm] Activity activity)
        {
            string success = "Error al crear la actividad";

            int? idUser = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value); ;

            try
            {
                if (activity.Description != null && activity.Title != null && activity.Contact != null && activity.ActivityDateFinished != null && activity.File.FileName != null)
                {
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "WWWRoot", "ActivityPhotos", activity.File.FileName);

                    var originalFilePath = filePath;
                    var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(originalFilePath);
                    var extension = Path.GetExtension(originalFilePath);

                    DateTime currentDate = DateTime.Today;

                    if (extension != ".jpg" && extension != ".png" && extension != ".gif" && extension != ".jpeg")
                    {
                        return StatusCode(400, "El formato del archivo no es aceptado. Por favor verifique que sea .PNG / .JPG / .GIF / .JPEG");
                    }

                    if (activity.ActivityDateFinished < currentDate)
                    {
                        return StatusCode(400, "La fecha de finalizacion de la actividad es invalida. Vuelve a ingresar una pasada la fecha actual.");
                    }

                    var counter = 1;

                    const int maxFileSizeMB = 15;
                    FileInfo fileInfo = new FileInfo(filePath);

                    // Obtener el tamaño del archivo en bytes
                    long fileSizeInBytes = activity.File.Length;

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
                        filePath = Path.Combine("WWWRoot", "ActivityPhotos", newFileName);
                        counter++;
                    }

                    Dictionary<string, object> args = new Dictionary<string, object> {
                         {"pTitle",activity.Title},
                         {"pDescription",activity.Description},
                         {"pContact",activity.Contact},
                         {"pDateFinishActivity",activity.ActivityDateFinished},
                         {"pURLphotos","/ActivityPhotos/" + newFileName},
                         {"pIdUser",idUser}
                    };

                    success = DBHelper.CallNonQuery("spActivityCreate", args);

                    if (success == "2")
                    {
                        using (var stream = System.IO.File.Create(filePath))
                        {
                            await activity.File.CopyToAsync(stream);
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
                return StatusCode(500, "Error al crear la actividad");
            }

        }

        // UPDATE
        // Ejemplo: (PUT) localhost:7004/activity

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Update([FromForm] Activity activity, int id)
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

                    DateTime currentDate = DateTime.Today;

                    if (extension != ".jpg" && extension != ".png" && extension != ".gif" && extension != ".jpeg")
                    {
                        return StatusCode(400, "El formato del archivo no es aceptado. Por favor verifique que sea .PNG / .JPG / .GIF / .JPEG");
                    }

                    if (activity.ActivityDateFinished < currentDate)
                    {
                        return StatusCode(400, "La fecha de finalizacion de la actividad es invalida. Vuelve a ingresar una pasada la fecha actual.");
                    }

                    var counter = 1;

                    const int maxFileSizeMB = 15;
                    FileInfo fileInfo = new FileInfo(filePath);

                    // Obtener el tamaño del archivo en bytes
                    long fileSizeInBytes = activity.File.Length;

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
                        filePath = Path.Combine("WWWRoot", "ActivityPhotos", newFileName);
                        counter++;
                    }

                    Dictionary<string, object> args = new Dictionary<string, object> {
                         {"pTitle",activity.Title},
                         {"pDescription",activity.Description},
                         {"pContact",activity.Contact},
                         // {"pActivityDate",activity.ActivityDate},
                         {"pDateFinishActivity",activity.ActivityDateFinished},
                         {"pURLphotos","/ActivityPhotos/" + newFileName},
                         {"pId",id},
                         {"pIdUser",idUser}
                    };

                    success = DBHelper.CallNonQuery("spActivityUpdate", args);

                    if (success == "1")
                    {
                        using (var stream = System.IO.File.Create(filePath))
                        {
                            await activity.File.CopyToAsync(stream);
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
                return StatusCode(500, "Error al crear la actividad");
            }
            
        }

        // ACTIVITY JOIN
        // Ejemplo: (POST) localhost:7004/activity/join

        [HttpPost("join/{id}")]
        public IActionResult ActivityManager(int id)
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
                    return StatusCode(400, "Se enviaron campos incompletos o erroneos");
                }

            }
            catch
            {
                return StatusCode(500, "Error al crear la actividad");
            }
            

        }

        public class Timers
        {
            private static System.Timers.Timer _timer;

            public static void Start()
            {
                _timer = new System.Timers.Timer();

                //Calcula cuánto tiempo falta para las 00:00
                DateTime now = DateTime.Now;
                DateTime midnight = new DateTime(now.Year, now.Month, now.Day + 1, 0, 0, 0);
                double msUntilMidnight = (midnight - now).TotalMilliseconds;

                //Establece el intervalo al número calculado de milisegundos hasta la medianoche.
                _timer.Interval = msUntilMidnight;

                //Configura el evento para que se ejecute en el intervalo.
                _timer.Elapsed += new System.Timers.ElapsedEventHandler(TimerElapsed);

                //Inicia el temporizador.
                _timer.Start();
            }

            private static void TimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
            {
                //Store procedure a ejecutar
                DBHelper.callProcedureReader("spActivityCleaner", new Dictionary<string, object> { });

                //Reinicia el intervalo a 24 horas.
                _timer.Interval = TimeSpan.FromHours(24).TotalMilliseconds;
            }
        }

    }
}
