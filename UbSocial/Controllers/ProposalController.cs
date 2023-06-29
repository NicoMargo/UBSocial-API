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
        // GET ALL
        // Ejemplo: (GET) localhost:5665/proposal

        [HttpGet]
        [Authorize]
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

        // GET BY ID
        // Ejemplo: (GET) localhost:5665/proposal/1

        [HttpGet("{id}")]
        [Authorize]
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

        // DELETE BY ID
        // Ejemplo: (DELETE) localhost:5665/proposal/1

        [HttpDelete("{id}")]
        [Authorize]
        public IActionResult Delete(int id)
        {
            string success = "Error al eliminar la propuesta";
            try
            {
                if (id >= 0)
                {

                    Dictionary<string, object> args = new Dictionary<string, object> {

                    {"pId",id}

                    };

                    success = DBHelper.CallNonQuery("spProposalDelete", args);

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
        // Ejemplo: (POST) localhost:5665/proposal

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

        // UPDATE
        // Ejemplo: (PUT) localhost:5665/proposal

        [HttpPut]
        [Authorize]
        public IActionResult Update(Proposal proposal)
        {
            string success = "Error al modificar la propuesta";
            try
            {
                if (proposal.Title != null && proposal.Description != null && proposal.Id != null)
                {
                    Dictionary<string, object> args = new Dictionary<string, object> {
                         {"pTitle",proposal.Title},
                         {"pDescription",proposal.Description},
                         {"pId", proposal.Id},
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
