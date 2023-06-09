﻿using System.Security.Claims;
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
        // Ejemplo: (GET) localhost:7004/proposal

        [HttpGet]
        [Route("{page}")]
        [Route("")]
        public IActionResult ProposalGet(int page = 0)
        {
            try
            {
                Dictionary<string, object> args = new Dictionary<string, object> {
                    {"pPage",page}
                };
                return Ok(DBHelper.callProcedureReader("spProposalGetAll", args));
            }
            catch
            {
                return StatusCode(500, "Error al obtener la informacion de las propuestas");
            }
        }

        // GET BY ID
        // Ejemplo: (GET) localhost:7004/proposal/1

        //[HttpGet("{id}")]
        //public IActionResult ProposalGetById(int id)
        //{
            
        //    try
        //    {
        //        Dictionary<string, object> args = new Dictionary<string, object> {
        //            {"pId",id}
        //        };
        //        return Ok(DBHelper.callProcedureReader("spProposalGetById", args));
        //    }
        //    catch
        //    {
        //        return StatusCode(500, "Error al obtener la informacion de las propuestas");
        //    }
        //}



        // GET BY Id User
        // Ejemplo: (GET) localhost:7004/proposal/current

        [HttpGet("current")]
        [Authorize]
        public IActionResult ProposalGetByIdUser()
        {
            int? userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            try
            {
                Dictionary<string, object> args = new Dictionary<string, object> {
                    {"pIdUser",userId}
                };
                return Ok(DBHelper.callProcedureReader("spProposalGetByIdUser", args));
            }
            catch
            {
                return StatusCode(500, "Error al obtener la informacion de las propuestas");
            }
        }

        // DELETE BY ID
        // Ejemplo: (DELETE) localhost:7004/proposal/1

        [HttpDelete("{id}")]
        [Authorize]
        public IActionResult Delete(int id)
        {
            string success = "Error al eliminar la propuesta";
            int idUser = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            try
            {
                if (id >= 0)
                {

                    Dictionary<string, object> args = new Dictionary<string, object> {

                    {"pId",id},
                    {"pIdUser",idUser}

                    };

                    success = DBHelper.CallNonQuery("spProposalDelete", args);

                    if (success == "1")
                    {
                        return Ok();
                    }

                }

                return StatusCode(400, "Se enviaron campos incompletos o erroneos");

            }
            catch
            {
                return StatusCode(500, "Error al intentar eliminar la propuesta");
            }
        }

        // CREATE
        // Ejemplo: (POST) localhost:7004/proposal

        [HttpPost]
        [Authorize]

        public IActionResult Create(Proposal proposal)
        {
            string success = "Error al crear la propuesta";
            int idUser = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            try
            {
                if (proposal.Title != null && proposal.Description != null)
                {
                    Dictionary<string, object> args = new Dictionary<string, object> {
                         {"pTitle",proposal.Title},
                         {"pDescription",proposal.Description},
                         {"pIdUser",idUser}
                    };
                    success = DBHelper.CallNonQuery("spProposalCreate", args);
                    if (success == "1")
                    {
                        return Ok();
                    }
                    else
                    {
                        return StatusCode(400, "Se enviaron campos incompletos o erroneos");
                    }
                }

                return StatusCode(400, "Se enviaron campos incompletos o erroneos");

            }
            catch
            {
                return StatusCode(500, success);
            }
            
        }

        // UPDATE
        // Ejemplo: (PUT) localhost:7004/proposal

        [HttpPut]
        [Authorize]
        public IActionResult Update(Proposal proposal)
        {
            string success = "Error al modificar la propuesta";
            int idUser = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            try
            {
                if (proposal.Title != null && proposal.Description != null && proposal.Id != null)
                {
                    Dictionary<string, object> args = new Dictionary<string, object> {
                         {"pTitle",proposal.Title},
                         {"pDescription",proposal.Description},
                         {"pId", proposal.Id},
                         {"pIdUser",idUser}
                    };
                    success = DBHelper.CallNonQuery("spProposalUpdate", args);
                    if (success == "1")
                    {
                        return Ok();
                    }
                    else
                    {
                        return StatusCode(400, "Se enviaron campos incompletos o erroneos");
                    }
                }
                else
                {
                    return StatusCode(400, "Se enviaron campos incompletos o erroneos");
                }

            }
            catch
            {
                return StatusCode(500, success);
            }
            
        }

    }
}
