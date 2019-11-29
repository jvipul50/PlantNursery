using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PlantsNursery.Common
{
    public  class Enums
    {
        public enum PlantState
        {
            Dried, //when plant is not watered for more that 6 hrs
            Watering, // when plant is being watered
            Flooded, // after 10 seconds of watering
            Normal // when plant is fully watered at least once
        }
    }
}