using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PlantsNursery.Models
{
    public class Plant
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime WateredLastTime { get; set; }
        public String State { get; set; }
        public  DateTime? WateringStartTime { get; set; }
    }
}