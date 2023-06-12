﻿using Microsoft.AspNetCore.Authorization;
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
        [HttpGet]
        public IActionResult ActivityGet()
        {
            try
            {
                return Ok(DBHelper.callProcedureReader("spActivityGetAll", new Dictionary<string, object> { }));
            }
            catch
            {
                return StatusCode(500, "Error al obtener la informacion de las actividades");
            }
        }

        [HttpGet("{id}")]
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

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            string success = "Error al eliminar la actividad";
            try
            {
                if (id >= 0)
                {

                    Dictionary<string, object> args = new Dictionary<string, object> {

                    {"pId",id}

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

        [HttpPost]
        [Authorize]

        public IActionResult Create(Activity activity)
        {
            string success = "Error al crear la actividad";
            try
            {
                if (activity.Title != null && activity.Description != null && activity.Id != null && activity.Contact != null && activity.URLPhotos != null)
                {
                    Dictionary<string, object> args = new Dictionary<string, object> {
                         {"pTitle",activity.Title},
                         {"pDescription",activity.Description},
                         {"pContact",activity.Contact},
                         {"pURLPhotos",activity.URLPhotos},
                         {"pActivityDate",activity.ActivityDate},
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
            }
            catch
            {
            }
            return StatusCode(500, success);
        }

        [HttpPut]
        public IActionResult Update(Activity activity)
        {
            string success = "Error al modificar la actividad";
            try
            {
                if (activity.Title != null && activity.Description != null && activity.Id != null && activity.Contact != null && activity.ActivityDate != null && activity.URLPhotos != null) 
                {
                    Dictionary<string, object> args = new Dictionary<string, object> {
                         {"pTitle",activity.Title},
                         {"pDescription",activity.Description},
                         {"pId", activity.Id},
                         {"pContact",activity.Contact},
                         {"pURLPhotos",activity.URLPhotos},
                         {"pActivityDate",activity.ActivityDate},
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
