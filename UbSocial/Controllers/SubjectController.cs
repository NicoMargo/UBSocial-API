using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UbSocial.Models;
using UbSocial.Models.Helpers;

namespace UBSocial.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class SubjectController : ControllerBase
    {
        // GET ALL
        // Ejemplo: (GET) localhost:5665/Subject

        [HttpGet]
        public IActionResult SubjectGet()
        {
            try
            {
                return Ok(DBHelper.callProcedureReader("spSubjectGetAll", new Dictionary<string, object> { }));
            }
            catch
            {
                return StatusCode(500, "Error al obtener la informacion de las materias");
            }
        }

        // GET ALL
        // Ejemplo: (GET) localhost:5665/Subject/1

        [HttpGet("{id}")]
        public IActionResult SubjectGetById(int id)
        {
            try
            {
                Dictionary<string, object> args = new Dictionary<string, object> {
                    {"pId",id}
                };
                return Ok(DBHelper.callProcedureReader("spSubjectGetById", args));
            }
            catch
            {
                return StatusCode(500, "Error al obtener la informacion de la materia");
            }
        }
    }
}
