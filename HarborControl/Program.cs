using HarborControl.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HarborControl
{
    class Program
    {        
        private static Queue<Boat> boats = new Queue<Boat>();
        private static Wind currentWind = new Wind();

        static void Main(string[] args)
        {
            WeatherService ws = new WeatherService();            
            currentWind = ws.GetCurrentWeather().wind;            

            Console.WriteLine("Harbor Control Console Application");
            Console.WriteLine("------------------------------------------");
            Console.WriteLine("Current wind speed at Durban harbor: {0}", currentWind.speed);

            // Snapshot example: 2 speedboats, 1 sailboat, 1 cargo 
            generateInitialBoats();

            // allow boat into the perimeter
            var allowBoat = PromptStaffToAllowBoat();
            while (!allowBoat)
            {
                allowBoat = PromptStaffToAllowBoat();
            }
            checkIfBoatIsAllowed();
            var boatWithinPerimeter = boats.Dequeue();
            displayBoatWithinPerimeter(boatWithinPerimeter);
            var timeToDock = GetTime(boatWithinPerimeter.speed);
            Task.Delay(timeToDock).Wait();
            displayBoatDocked(boatWithinPerimeter);
            ScheduleBoats();

            // TODO: Random boat generator method to be called. Could not be completed due to time constraint
        }
        
        private static bool checkIfBoatIsAllowed()
        {            
            bool isAllowed = true;
            float belowWindSpeed = 10;
            float aboveWindSpeed = 30;
            var firstBoatIntheQueue = boats.Peek();
            if(firstBoatIntheQueue.type == "Sailboat" &&
                (currentWind.speed < belowWindSpeed || currentWind.speed > aboveWindSpeed))
            {                
                // move boat to the last on the queue and check other boats
                var boat = boats.Dequeue();
                boats.Enqueue(boat);
                Console.WriteLine("Sailboat cannot be allowed within the perimeter. Current wind speed is {0}", currentWind.speed);

                isAllowed = false;
            }
            return isAllowed;
        }

        private static void generateInitialBoats()
        {
            var cargo = new Boat() { id = 1, type = "Cargo", speed = 5 };
            boats.Enqueue(cargo);
            var speedBoat1 = new Boat() { id = 2, type = "Speedboat", speed = 30 };
            boats.Enqueue(speedBoat1);
            var speedBoat2 = new Boat() { id = 3, type = "Speedboat", speed = 30 };
            boats.Enqueue(speedBoat2);
            var sailBoat = new Boat() { id = 4, type = "Sailboat", speed = 15 };
            boats.Enqueue(sailBoat);
        }

        private static void displayBoatDocked(Boat boat)
        {            
            Console.WriteLine("-> {0} has docked", boat.type);            
        }

        private static void displayBoatWithinPerimeter(Boat boat)
        {
            Console.WriteLine("-> {0} is within the perimeter", boat.type);
        }

        private static void ScheduleBoats()
        {
            var allowBoat = PromptStaffToAllowBoat();
            while (!allowBoat)
            {
                allowBoat = PromptStaffToAllowBoat();
            }
            if (boats.Count != 0)
            {
                var isBoatAllowed = checkIfBoatIsAllowed();
                if (isBoatAllowed)
                {
                    var boatWithinPerimeter = boats.Dequeue();
                    displayBoatWithinPerimeter(boatWithinPerimeter);
                    var timeToDock = GetTime(boatWithinPerimeter.speed);
                    Task.Delay(timeToDock).Wait();
                    displayBoatDocked(boatWithinPerimeter);
                }

                ScheduleBoats();
            }
        }

        private static bool PromptStaffToAllowBoat()
        {
            Console.WriteLine("\nPress 'Enter' to allow boat to enter the harbor perimeter and wait for the boat to dock before allowing another one:");
            return (Console.ReadKey().Key == ConsoleKey.Enter);
        }

        private static int GetTime(int boatSpeed)
        {
            double distance = 10;
            double timeToTake = distance / (double)boatSpeed * 3600;            
            return Convert.ToInt32(timeToTake);
        }
    }
}
