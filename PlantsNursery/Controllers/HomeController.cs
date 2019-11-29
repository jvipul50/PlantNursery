using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PlantsNursery.DAL;

namespace PlantsNursery.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        //Get All the plants
        public JsonResult GetAllPlants() {
            var plantsManager = new PlantsManager();
            plantsManager.RefreshPlantsState();
            var plants = plantsManager.GetAllPlants();

            return Json(plants, JsonRequestBehavior.AllowGet);
        }

        //Start Watering
        public JsonResult StartWatering(int plantId) {
            var plantsManager = new PlantsManager();
            plantsManager.RefreshPlantsState(); // refresh current state of plants before start watering
            var result = plantsManager.UpdatePlantState(plantId, Common.Enums.PlantState.Watering.ToString());

            if (result)
                return Json("success", JsonRequestBehavior.AllowGet);
            else
                return Json("Try after some time", JsonRequestBehavior.AllowGet);
        }

        //Stop Watering
        public JsonResult StopWatering(int plantId) {
            var plantsManager = new PlantsManager();
            plantsManager.RefreshPlantsState();
            var plant = plantsManager.GetPlantById(plantId);

            // if plant has start watering time and also 10 seconds has passed then update plan watered last time property
            if (plant.WateringStartTime.HasValue && (DateTime.Now - plant.WateringStartTime.Value).TotalSeconds > 10) {
                plantsManager.UpdatePlantWateredLastTime(plantId, DateTime.Now);
            }
            plantsManager.UpdatePlantState(plantId, Common.Enums.PlantState.Normal.ToString());
            
            return Json("success", JsonRequestBehavior.AllowGet);

        }
    }
}