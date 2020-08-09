using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ComponentProcessingMicroservice.Models;

namespace ComponentProcessingMicroservice.DependencyInjection
{
   public interface IComponentProcessingMicroservice
    {
        public int PackagingDelivery(string Item, int Count);

        public string CardDetails(CardDetails details);

        public int ProcessId();

        public int ProcessingCharge();

        public string GetRequest(string json);

        public string GetUserMessage(ReturnOrderPortal.Models.Submission message);
        public DateTime DeliveryDate();
    }
}
