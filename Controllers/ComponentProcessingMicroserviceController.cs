using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ComponentProcessingMicroservice.Models;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Text;
using ComponentProcessingMicroservice.Processing;
using ReturnOrderPortal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;

namespace ComponentProcessingMicroservice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ComponentProcessingMicroserviceController : ControllerBase
    {
        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(ComponentProcessingMicroserviceController));

        readonly int Limit = 50000;

        public static ProcessRequest RequestObject = new ProcessRequest();

        public static ProcessResponse ResponseObject = new ProcessResponse();

        private readonly IConfiguration _config;
        public ComponentProcessingMicroserviceController( IConfiguration config)
        {
            
            _config = config;
        }

        [Route("DeliverDate")]
        [HttpGet]
        public dynamic DeliveryDate()
        {
            try
            {
                DateTime date = DateTime.Now;
                if (RequestObject.IsPriorityRequest == true && RequestObject.ComponentType == "Integral")
                {
                    return date.AddDays(2);
                }
                else
                {
                    return date.AddDays(5);
                }
            }
            catch(Exception)
            {
                return BadRequest("Exception from DeliveryDate");
            }
            
        }

        [HttpGet]
        [Route("PackagingDelivery")]
        public dynamic PackagingDelivery(string Item, int Count)
        {
            try
            {
                string PackigingDeliveryCharge;
                var query = "?item=" + Item + "&count=" + Count;
                HttpClient client = new HttpClient();
                HttpResponseMessage result = client.GetAsync(_config["Links:PackagingAndDeliveryMicroService"] + "/GetPackagingDeliveryCharge" + query).Result;


                if (result.IsSuccessStatusCode)
                {
                    PackigingDeliveryCharge = result.Content.ReadAsStringAsync().Result;
                }
                else
                {
                    PackigingDeliveryCharge = "0";
                }
                int charge = int.Parse(PackigingDeliveryCharge);
                return charge;
            }
            catch(Exception)
            {
                return BadRequest("Exception from PackagingDelivery");
            }
            
        }

        [HttpPost]
        [Route("CardDetails")]
        public dynamic CardDetails(CardDetails details)
        {
            try
            {
                PaymentDetails Response;
                var data = JsonConvert.SerializeObject(details);
                var value = new StringContent(data, Encoding.UTF8, "application/json");
                using var client = new HttpClient();
                var response = client.PostAsync(_config["Links:PaymentMicroService"] + "/ProcessPayment", value).Result;

                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadAsStringAsync().Result;
                    Response = JsonConvert.DeserializeObject<PaymentDetails>(result);
                    if (Response.Message == "Successful")
                    {
                        return "Successful";
                    }
                    else
                    {
                        return "Failed";
                    }
                }
                else
                {
                    return "Failed";
                }
            }
            catch (Exception)
            {
                return BadRequest( "Exception from CardDetails");
            }
            
           
        }
      
        [Route("ProcessId")]
        [HttpGet]
        public dynamic ProcessId()
        {
            try
            {
                int n;
                Random _random = new Random();
                n = _random.Next(500, 1000);
                return n;
            }
            catch (Exception)
            {
                return BadRequest("Exception from ProcessId");
            }
          
        }

        [Route("Workflow")]
        [HttpGet]
        public dynamic ProcessingCharge()
        {
            try
            {
                int ProcessingCharge;
                string Workflow = RequestObject.ComponentType;
                if (Workflow == "Integral")
                {
                    IntegralWorkflow integral = new IntegralWorkflow();
                    ProcessingCharge = integral.ProcessingCharge(RequestObject.IsPriorityRequest);
                }
                else
                {
                    AccessoryWorkflow accessory = new AccessoryWorkflow();
                    ProcessingCharge = accessory.ProcessingCharge(false);
                }
                return ProcessingCharge;
            }
            catch (Exception)
            {
                return BadRequest("Exception from ProcessingCharge");
            }
            
        }

       
        [HttpGet]
        [Authorize]
        
        public dynamic GetRequest(string json)
        {

            try
            {
                _log4net.Info("GetRequest() called with json input");
                RequestObject = JsonConvert.DeserializeObject<ProcessRequest>(json);

                RequestObject = new ProcessRequest
                {
                    Name = RequestObject.Name,
                    ContactNumber = RequestObject.ContactNumber,
                    CreditCardNumber = RequestObject.CreditCardNumber,
                    ComponentType = RequestObject.ComponentType,
                    ComponentName = RequestObject.ComponentName,
                    Quantity = RequestObject.Quantity,
                    IsPriorityRequest = RequestObject.IsPriorityRequest

                };
                int Processing = ProcessId();



                ResponseObject = new ProcessResponse
                {
                    RequestId = Processing,
                    ProcessingCharge = ProcessingCharge(),
                    PackagingAndDeliveryCharge = PackagingDelivery(RequestObject.ComponentType, RequestObject.Quantity),
                    DateOfDelivery = DeliveryDate()

                };

                var ResponseString = JsonConvert.SerializeObject(ResponseObject);
                return ResponseString;

            }
            catch (Exception)
            {
                return BadRequest("Exception from GetRequest");
            }
            
            
        }

        [HttpPost]
        public dynamic GetUserMessage(Submission message)
        {
            try
            {
                _log4net.Info("GetUserMessage() called with user message as input");
                if (message.Result == "True")
                {
                    CardDetails detail = new CardDetails()
                    {
                        CreditCardNumber = RequestObject.CreditCardNumber,
                        CreditLimit = Limit,
                        ProcessingCharge = ResponseObject.ProcessingCharge + ResponseObject.PackagingAndDeliveryCharge
                    };
                    var result = CardDetails(detail);
                    if (result == "Successful")
                    {
                        return "Success";
                    }
                    else
                    {
                        return "Failed";
                    }
                }
                else
                {
                    return "Payment not initiated";
                }
            }
            catch (Exception)
            {
                return BadRequest("Exception from GetUserMessage");
            }
            
        } 
    }
}
