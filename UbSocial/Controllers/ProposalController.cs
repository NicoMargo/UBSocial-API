using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UbSocial.Models;
using UbSocial.Models.Helpers;

namespace UBSocial.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ProposalController : ControllerBase
    {
        [HttpGet]
        public IActionResult ProposalGet()
        {
            try
            {
                return Ok(DBHelper.callProcedureReader("spProposalGetAll", new Dictionary<string, object> { }));
            }
            catch
            {
                return StatusCode(500, "Error al obtener la informacion de las propuestas");
            }
        }

        [HttpGet("{id}")]
        public IActionResult ProposalGetById(int id)
        {
            try
            {
                Dictionary<string, object> args = new Dictionary<string, object> {
                    {"pId",id}
                };
                return Ok(DBHelper.callProcedureReader("spProposalGetById", args));
            }
            catch
            {
                return StatusCode(500, "Error al obtener la informacion de las propuestas");
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            string success = "Error al eliminar la propuesta";
            try
            {
                Dictionary<string, object> args = new Dictionary<string, object> {
                    {"pId",id}
                };

                success = DBHelper.CallNonQuery("spProposalDelete", args);

                if (success == "1")
                {
                    return Ok();
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

        public IActionResult Create(Proposal proposal)
        {
            string success = "Error al crear la propuesta";
            try
            {
                if (proposal.Title != null && proposal.Description != null)
                {
                    Dictionary<string, object> args = new Dictionary<string, object> {
                         {"pTitle",proposal.Title},
                         {"pDescription",proposal.Description}
                    };
                    success = DBHelper.CallNonQuery("spProposalCreate", args);
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
        public IActionResult Update(Proposal proposal)
        {
            string success = "Error al modificar la propuesta";
            try
            {
                if (proposal.Title != null && proposal.Description != null)
                {
                    Dictionary<string, object> args = new Dictionary<string, object> {
                         {"pTitle",proposal.Title},
                         {"pDescription",proposal.Description}
                    };
                    success = DBHelper.CallNonQuery("spProposalUpdate", args);
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
                    success = "El titulo y la descripcion no pueden estar vacios";
                }
            }
            catch
            {
            }
            return StatusCode(500, success);
        }

    }
}
