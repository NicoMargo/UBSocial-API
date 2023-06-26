using Microsoft.AspNetCore.Mvc;
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

                    string success = DBHelper.callProcedureReader("spUserLogin", args);
                    if (success == "true")
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
                if (user.Password != null && user.Email != null && user.Name != null && user.Surname != null)
                {
                    Dictionary<string, object> args = new Dictionary<string, object> {
                    {"pEmail",user.Email},
                    {"pPassword",user.Password},
                    {"pName",user.Name},
                    {"pSurname",user.Surname}
                    };


                    success = DBHelper.CallNonQuery("spUserCreate", args);

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
                    return BadRequest();
                }
            }
            catch
            {
                return StatusCode(500, success);
            }
        }

            //localhost:5665/User/1
            [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            string success = "";
            try
            {
                if (id != null)
                {
                    Dictionary<string, object> args = new Dictionary<string, object> {
                         {"pId",id}
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
