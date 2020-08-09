using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using ComponentProcessingMicroservice.Processing;
using ComponentProcessingMicroservice.Models;
using ReturnOrderPortal.Models;

namespace ComponentProcessingMicroservice.DependencyInjection
{
    public class ComponentProcessingMicroserviceCore
    {

        ProcessRequest RequestObject = new ProcessRequest();
        ProcessResponse ResponseObject = new ProcessResponse();
        public string CardDetails(CardDetails details)
        {
            PaymentDetails Response;
            var data = JsonConvert.SerializeObject(details);
            var value = new StringContent(data, Encoding.UTF8, "application/json");
            using var client = new HttpClient();
            var response = client.PostAsync("http://localhost:57287/api/ProcessPayment", value).Result;

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


        public int PackagingDelivery(string Item, int Count)
        {
            string PackigingDeliveryCharge ;
            var query = "?item=" + Item + "&count=" + Count;
            HttpClient client = new HttpClient();
            HttpResponseMessage result = client.GetAsync("http://localhost:53642/GetPackagingDeliveryCharge" + query).Result;


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


        public int ProcessId()
        {
            int n ;
            Random _random = new Random();
            n = _random.Next(500, 1000);
            return n;
        }

        public int ProcessingCharge()
        {
            int ProcessingCharge;
            string Workflow = RequestObject.ComponentType;
            if (Workflow == "Integral")
            {
                
                ProcessingCharge = 700;
            }
            else
            {
            
                ProcessingCharge = 300;
            }
            return ProcessingCharge;
        }


        public string GetUserMessage(Submission message)
        {

            if (message.Result == "True")
            {
                CardDetails detail = new CardDetails()
                {
                    CreditCardNumber = 123456789012,
                    CreditLimit = 100000,
                    ProcessingCharge = 7000
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

        public DateTime DeliveryDate()
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



        public string GetRequest(string json)
        {




            RequestObject = JsonConvert.DeserializeObject<ProcessRequest>(json);



            RequestObject = new ProcessRequest
            {
                Name = "Namit",
                ContactNumber = "99999888",
                CreditCardNumber = 123456789012,
                ComponentType = "Integral",
                ComponentName = "Wheel",
                Quantity = 2,
                IsPriorityRequest = true

            };
            // int Processing = ProcessId();



            ResponseObject = new ProcessResponse
            {
                RequestId = 550,
                ProcessingCharge = 400,
                PackagingAndDeliveryCharge = 100,
                DateOfDelivery = DateTime.Now

            };

            var ResponseString = JsonConvert.SerializeObject(ResponseObject);
            return ResponseString;

        }

    }
}
