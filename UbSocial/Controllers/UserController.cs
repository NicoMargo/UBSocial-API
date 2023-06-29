using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Security.Claims;
using System.Text.Json;
using UbSocial.Models;
using UbSocial.Models.Helpers;

namespace UbSocial.Controllers
{

    [Route("[controller]")]
    [ApiController]
    public class UserController : Controller
    {

        [HttpPost]
        [Route("userLogin")]
        //await
        public async Task<IActionResult> Login(User user)
        {
            string token;
            try
            {
                if (user.Password != null && user.Email != null)
                {
                    Dictionary<string, object> args = new Dictionary<string, object> {
                     {"pEmail",user.Email},
                     {"pPassword",user.Password},
                    };
                    string json = DBHelper.callProcedureReader("spUserLogin", args);
                    user = JsonSerializer.Deserialize<User>(json);
                    if (user.Id != null)
                    {
                        token = JWT.GenerateToken(user);
                        RefreshToken refreshToken = JWT.GenerateRefreshToken();

                        return Ok(token);
                    }
                }
                return NotFound("Usuario no encontrado");
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet]
        public IActionResult UserGet()
        {
            try
            {
                return Ok(DBHelper.callProcedureReader("spUserGetAll", new Dictionary<string, object> { }));
            }
            catch
            {
                return StatusCode(500, "Error al obtener la informacion de los usuarios");
            }
        }

        [HttpGet("{id}")]
        public IActionResult UserGetById(int id)
        {
            try
            {
                Dictionary<string, object> args = new Dictionary<string, object> {
                    {"pId",id}
                };
                return Ok(DBHelper.callProcedureReader("spUserGetById", args));
            }
            catch
            {
                return StatusCode(500, "Error al obtener la informacion del usuario");
            }
        }

        [HttpPost]
        public IActionResult Create(User user)
        {
            string success = "";

            try
            {
                if (user.Password != null && user.Email != null && user.Name != null && user.Surname != null && user.Admin != null)
                {
                    Dictionary<string, object> args = new Dictionary<string, object> {
                    {"pEmail",user.Email},
                    {"pPassword",user.Password},
                    {"pName",user.Name},
                    {"pSurname",user.Surname},
                    {"pAdmin",user.Admin}
                    };


                    success = DBHelper.CallNonQuery("spUserCreate", args);

                    if (success == "1")
                    {
                        return Ok();
                    }
                    else
                    {
                        success = "El email proporcionado ya existe en otro usuario.";
                        return StatusCode(500, success);
                    }
                }
                else
                {
                    return BadRequest("El email, nombre, apellido y contraseña no pueden ser nulos" + user.Password + user.Name + user.Surname + user.Name + " " + user);
                }
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpDelete]
        [Authorize]
        public IActionResult Delete()
        {
            int? userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            string success = "";
            try
            {
                if (userId > 0)
                {
                    Dictionary<string, object> args = new Dictionary<string, object> {
                         {"pId",userId}
                    };

                    success = DBHelper.CallNonQuery("spUserDelete", args);
                    if(success == "1")
                    {
                        return Ok();

                    }
                }
                return BadRequest();
            }
            catch
            {
                return StatusCode(500, success);
            }

        }


        [HttpPut]
        [Authorize]
        public IActionResult Update(User user)
        {
            string success = "Error al modificar el usuario";
            try
            {
                if (user.Email != null && user.Password != null && user.Name != null && user.Surname != null && user.Id != null)
                {
                    Dictionary<string, object> args = new Dictionary<string, object> {
                         {"pEmail",user.Email},
                         {"pPassword",user.Password},
                         {"pName",user.Name},
                         {"pSurname",user.Surname},
                         {"pId",user.Id}
                    };
                    success = DBHelper.CallNonQuery("spUserUpdate", args);
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
                    success = "Hay campos que no pueden estar vacios";
                }
            }
            catch
            {
            }
            return StatusCode(500, success);
        }
    }
}
