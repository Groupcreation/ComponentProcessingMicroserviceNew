using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using ComponentProcessingMicroservice.DependencyInjection;
using ComponentProcessingMicroservice.Models;

using ReturnOrderPortal.Models;

namespace ComponentProcessingMicroservice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ComponentProcessingMicroservice2Controller : ControllerBase
    {
      readonly  IComponentProcessingMicroservice ob;
      public  ComponentProcessingMicroservice2Controller(IComponentProcessingMicroservice _ob)
        {
            ob = _ob;
        }

        [HttpGet]
        public IActionResult CardDetails(CardDetails details)
        {
            try
            {
                var result = ob.CardDetails(details);
                if (result == null)
                {
                    return NotFound();
                }
                return Ok(result);
            }
            catch (Exception)
            {
                return BadRequest("Exception from CardDetails");
            }
           
        }

        [HttpGet]
        [Route("PackagingDelivery")]
        public IActionResult PackagingDelivery(string Item, int Count)
        {
            try
            {
                var Item1 = Item; var Count1 = Count;
                var Result = ob.PackagingDelivery(Item1, Count1);
                if (Result >= 0)
                {
                    return Ok(Result);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception)
            {
                return BadRequest("Exeception from PackagingDelivery");
            }
           
        }
        [Route("Processid")]
        [HttpGet]
        public IActionResult ProcessId()
        {
            try
            {
                int Result = ob.ProcessId();
                if (Result >= 500 || Result <= 1000)
                {
                    return Ok(Result);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception)
            {
                return BadRequest( "Exception from ProcessId");
            }
            
        }

        [Route("ProcessingCharge")]
        [HttpGet]
        public IActionResult ProcessingCharge()
        {
            try
            {
                int Result = ob.ProcessingCharge();
                if (Result >= 300 || Result <= 700)
                {
                    return Ok(Result);
                }
                else
                {
                    return NotFound();
                }
            }
            catch(Exception)
            {
                return BadRequest("Exception from ProcessingCharge");
            }
            
        }

        [Route("GetUserMessage")]
        [HttpGet]
        public IActionResult GetUserMessage(ReturnOrderPortal.Models.Submission message)
        {
            try
            {
                var Result = ob.GetUserMessage(message);
                if (message != null)
                {
                    return Ok(Result);
                }
                else
                {
                    return NotFound();
                }
            }
            catch(Exception)
            {
                return BadRequest("Exception from GetUserMessage");
            }
           
        }


        [Route("GetDeliveryDate")]
        [HttpGet]
        public IActionResult DeliveryDate()
        {
            try
            {
                DateTime Date;
                Date = ob.DeliveryDate();
                if (ob != null)
                {
                    return Ok(Date);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception)
            {
                return BadRequest("Exception from GetDeliveryDate");
            }
            
        }

        [Route("GetRequest")]
        [HttpGet]
        public IActionResult GetRequest(string json)
        {
            try
            {
                var ResponseString = ob.GetRequest(json);
                if (ResponseString != null)
                {
                    return Ok(ResponseString);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception)
            {
                return BadRequest("Exception from GetRequest");
            }
           
        }
    }
}