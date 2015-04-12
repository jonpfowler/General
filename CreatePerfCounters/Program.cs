using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;

namespace CreatePerfCounters
{
    class Program
    {
        private static string performanceCounterCategoryName;
        private static string performanceCounterCategoryHelp;
        private static int performanceCounterCategoryRate;
        private static string counterFile = "Counters.txt";
        private static string[] counterFileText;
        private static List<string> counterText;
        private static Random random;

        static void Main(string[] args)
        {
            string action = "read";
            if(args.Length > 0)
            {
                if (args.Length == 2)
                {
                    if (args[0].ToLower() == "delete")
                    {
                        action = "delete";
                        counterFile = args[1];
                    }
                }
                else
                {
                    counterFile = args[0];
                }
            }
            switch(action.ToLower())
            {
                case "read":
                    random = new Random(DateTime.Now.Millisecond);
                    ReadCounterFile(counterFile);
                    CreatePerformanceCounterCategory();
                    AddPerformanceCounterInstances();
                    break;
                case "delete":
                    ReadCounterFile(counterFile);
                    DeletePerformanceCounterCategory();
                    break;
            }
        }

        private static void ReadCounterFile(string counterFile)
        {
            if(!File.Exists(counterFile))
            {
                throw new FileNotFoundException(string.Format("{0} not found.", counterFile));
            }

            counterFileText = File.ReadAllLines(counterFile);

            string[] categoryInfo;
            counterText = new List<string>();

            foreach (string line in counterFileText)
            {
                if (line != string.Empty && !line.Trim().StartsWith("//"))
                {
                    if (string.IsNullOrEmpty(performanceCounterCategoryName))
                    {
                        categoryInfo = line.Split(new char[] { ',' });
                        performanceCounterCategoryName = categoryInfo[0].Trim();
                        performanceCounterCategoryHelp = categoryInfo[1].Trim();
                        performanceCounterCategoryRate = int.Parse(categoryInfo[2].Trim());
                        Console.WriteLine();
                        Console.WriteLine(counterFile);
                        Console.WriteLine("Category Name:\t{0}", performanceCounterCategoryName);
                        Console.WriteLine("Category Help:\t{0}", performanceCounterCategoryHelp);
                        Console.WriteLine("Category Rate:\t{0}", performanceCounterCategoryRate);
                        Console.WriteLine();
                        Console.WriteLine("Counters:");
                    }
                    else
                    {
                        Console.WriteLine(line);
                        counterText.Add(line);
                    }
                }
            }
            Console.WriteLine();
        }

        static void AddPerformanceCounterInstances()
        {
            if (PerformanceCounterCategory.Exists(performanceCounterCategoryName))
            {
                Console.WriteLine("Press Ctrl+C to quit.");
                Console.WriteLine();
                Console.WriteLine("Values:");

                while(true)
                {
                    foreach (string line in counterText)
                    {
                        string[] counterInfo = line.Split(new char[] { ',' });

                        string counterName = counterInfo[0].Trim();
                        string counterInstance = counterInfo[1].Trim();

                        PerformanceCounter myCounter = new PerformanceCounter(performanceCounterCategoryName, counterName, counterInstance, false);
                        int minValue = int.Parse(counterInfo[4]);
                        int maxValue = int.Parse(counterInfo[5]);
                        int variance = int.Parse(counterInfo[6]);
                        SetNextCounterValue(myCounter, minValue, maxValue, variance);
                        Console.WriteLine("{0}\t{1}\t{2}\t{3}", DateTime.Now, myCounter.CounterName, counterInstance, myCounter.RawValue);
                    }

                    System.Threading.Thread.Sleep(performanceCounterCategoryRate);
                }
            }
        }

        private static void SetNextCounterValue(PerformanceCounter myCounter, int minValue, int maxValue, int variance)
        {
            if(myCounter.CounterType == PerformanceCounterType.RateOfCountsPerSecond32 ||
                myCounter.CounterType == PerformanceCounterType.RateOfCountsPerSecond64)
            {
                myCounter.IncrementBy(10);
                return;
            }

            long lastValue = myCounter.RawValue;
            long newValue = 0;
            long adjustAmount;
            while (true)
            {
                adjustAmount = random.Next(0, variance);
                if (random.Next(0, 2) == 0)
                {
                    //Positive
                    //adjustAmount = 1 * adjustAmount; //redundant
                    //newValue = lastValue + adjustAmount;
                }
                else
                {
                    //Negative
                    adjustAmount = -1 * adjustAmount;
                    //newValue = lastValue - adjustAmount;
                }
                //adjustAmount could be positive or negative
                newValue = lastValue + adjustAmount;
                
                //Validate it fits within min/max
                if(newValue >= minValue && newValue <= maxValue)
                {
                    break;
                }
            }
            switch(myCounter.CounterType)
            {
                case PerformanceCounterType.NumberOfItems32:
                case PerformanceCounterType.NumberOfItems64:
                    myCounter.IncrementBy(adjustAmount);
                    break;
                case PerformanceCounterType.RateOfCountsPerSecond32:
                case PerformanceCounterType.RateOfCountsPerSecond64:
                    //myCounter.IncrementBy(newValue);
                    myCounter.RawValue = newValue;
                    break;
            }
            //myCounter.RawValue = newValue;

        }

        static void CreatePerformanceCounterCategory()
        {
            if (!PerformanceCounterCategory.Exists(performanceCounterCategoryName))
            {
                string[] counterInfo;
                // Create a collection of type CounterCreationDataCollection.
                CounterCreationDataCollection counterData = new CounterCreationDataCollection();

                foreach (string line in counterText)
                {
                    counterInfo = line.Split(new char[] { ',' });

                    CounterCreationData counter = new CounterCreationData();
                    counter.CounterName = counterInfo[0].Trim();
                    counter.CounterType = GetPerformanceCounterType(counterInfo[2].Trim());
                    counter.CounterHelp = counterInfo[3].Trim();
                    counterData.Add(counter);
                }

                // Create the category and pass the collection to it.
                PerformanceCounterCategory.Create(performanceCounterCategoryName, performanceCounterCategoryHelp, PerformanceCounterCategoryType.MultiInstance, counterData);
            }

        }
        static void DeletePerformanceCounterCategory()
        {
            if (PerformanceCounterCategory.Exists(performanceCounterCategoryName))
            {
                Console.WriteLine("Removing Performance Category: {0}", performanceCounterCategoryName);
                PerformanceCounterCategory.Delete(performanceCounterCategoryName);
                Console.WriteLine("Removed Performance Category: {0}", performanceCounterCategoryName);
            }
            else
            {
                Console.WriteLine("Performance Category to remove not found: {0}", performanceCounterCategoryName);
            }
        }

        private static PerformanceCounterType GetPerformanceCounterType(string performanceCounterType)
        {
            switch (performanceCounterType.ToLower())
            {
                case "numberofitems32":
                case "count":
                    return PerformanceCounterType.NumberOfItems32;
                case "numberofitems64":
                    return PerformanceCounterType.NumberOfItems64;
                case "rateofcountspersecond32":
                case "rate/sec":
                    return PerformanceCounterType.RateOfCountsPerSecond32;
                case "rateofcountspersecond64":
                    return PerformanceCounterType.RateOfCountsPerSecond64;
                case "averagetimer":
                case "average":
                    return PerformanceCounterType.AverageTimer32;
                default:
                    throw new Exception(string.Format("Counter name : \"{0}\" not understood. Please use \"Count\", \"Rate/Sec\", or \"Average\".", performanceCounterType));
            }
        }

    }
}
