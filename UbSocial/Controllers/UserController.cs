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
            catch
            {
                return NotFound("Usuario no encontrado");
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
                    success = user.Create(user);
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
    }
}
