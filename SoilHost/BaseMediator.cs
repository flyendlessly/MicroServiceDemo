using System;
using System.Collections.Generic;
using System.Text;

namespace SoilHost
{
    public abstract class BaseMediator
    {
        List<string> RelatedDevices { get; set; }

        public BaseMediator(List<string> RelatedDevices)
        {
            this.RelatedDevices = RelatedDevices;
        }

        public abstract void AddDevices(List<string> RelatedDevices);
        
    }

    //public class ConcreteMediator : BaseMediator
    //{
    //    public override void AddDevices(List<string> RelatedDevices)
    //    {
            
    //        if (RelatedDevices != null && RelatedDevices.Count > 0)
    //             RelatedDevices.AddRange(RelatedDevices);
    //    }
    //}
}
