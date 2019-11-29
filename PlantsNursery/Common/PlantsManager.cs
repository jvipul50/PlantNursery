using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PlantsNursery.Common;
using PlantsNursery.Models;

namespace PlantsNursery.DAL
{
    /// <summary>
    /// Manage plants and their states
    /// </summary>
    public class PlantsManager
    {
        // InMemory collection of plants. Requirements was for 5 plants. So five plants are created. List can incorporate more if needed.
        static List<Plant> Plants = new List<Plant>() {
                    new Plant() { Id = 1, Title = "Sugar Maple", State = Enums.PlantState.Dried.ToString(), WateredLastTime = DateTime.Today, WateringStartTime = null} ,
                    new Plant() { Id = 2, Title = "White Pine", State = Enums.PlantState.Dried.ToString(), WateredLastTime = DateTime.Today, WateringStartTime = null} ,
                    new Plant() { Id = 3, Title = "Paper Birch", State = Enums.PlantState.Dried.ToString(), WateredLastTime = DateTime.Today, WateringStartTime = null} ,
                    new Plant() { Id = 4, Title = "Douglas Fir", State = Enums.PlantState.Dried.ToString(), WateredLastTime = DateTime.Today, WateringStartTime = null} ,
                    new Plant() { Id = 5, Title = "Small sundrops", State = Enums.PlantState.Dried.ToString(), WateredLastTime = DateTime.Today, WateringStartTime = null}
                };
        public List<Plant> GetAllPlants()
        {
            return Plants;
        }

        /// <summary>
        /// Update state of the plan given the plantId and desired state
        /// </summary>
        /// <param name="plantId"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public bool UpdatePlantState(int plantId, string state)
        {
            var plant = Plants.SingleOrDefault(x => x.Id.Equals(plantId)); // find the plant by id
            lock (plant) // sync-lock plants object before updating state to avoid race condition
            {
                
                if (state == Enums.PlantState.Watering.ToString())
                {
                    if ((DateTime.Now - plant.WateredLastTime).TotalSeconds > 30) // don't allow watering a plant again for next 30 seconds
                    {
                        plant.WateringStartTime = DateTime.Now;
                        plant.State = state;
                        return true;
                    }
                    else
                        return false;
                }
                else
                {
                    // if user is already watering this plant and 10 seconds has passed then it means  he wants to stop watering. So we update the WateredLastTime field along with reseting WateringStartTime
                    if (plant.WateringStartTime.HasValue && (DateTime.Now - plant.WateringStartTime.Value).TotalSeconds > 10)
                    {
                        plant.WateringStartTime = null;
                        plant.WateredLastTime = DateTime.Now;
                        plant.State = state;
                    }
                    else
                    {
                        plant.WateringStartTime = null;                        
                        plant.State = state;
                    }

                    
                    return true;
                }
            }
        }

        /// <summary>
        /// Get the plant object by using its Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Plant GetPlantById(int id)
        {
            return Plants.SingleOrDefault(x => x.Id.Equals(id));
        }

        public void UpdatePlantWateredLastTime(int plantId, DateTime targetDateTime)
        {
            var targetPlant = Plants.SingleOrDefault(x => x.Id.Equals(plantId));
            lock (targetPlant) // sync-lock plants object before updating state to avoid race condition
            {
                
                targetPlant.WateringStartTime = null;
                targetPlant.WateredLastTime = targetDateTime;
            }
        }

        /// <summary>
        /// Iterates through all plants and updates state and other properties based on business logic
        /// </summary>
        public void RefreshPlantsState()
        {

            foreach (var plant in Plants)
            {
                lock (plant) // sync-lock plants object before updating state to avoid race condition
                {
                    // if plant is being watered for more than 10 seconds then its status is flooded
                    if (plant.WateringStartTime.HasValue && (DateTime.Now - plant.WateringStartTime.Value).TotalSeconds > 10)
                    {
                        plant.State = Enums.PlantState.Flooded.ToString();
                    }
                    else if ((DateTime.Now - plant.WateredLastTime).TotalSeconds > 6 * 60 * 60) // if plant is not fully watered for 6 hours
                    {
                        if (plant.State == Enums.PlantState.Normal.ToString()) // if it's in normal state (NOT in watering or flooded state)
                        {
                            plant.WateringStartTime = null;
                            plant.State = Enums.PlantState.Dried.ToString();

                        }

                    }
                }
            }


        }
    }
}