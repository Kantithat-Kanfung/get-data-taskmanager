using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MonitorDemo
{
    class Program
    {
        static int nC_Cores = 0;
        static List<PerformanceCounter> aoPerformanceCounter = new List<PerformanceCounter>();
        static PerformanceCounter oCPUCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
        static PerformanceCounter oDiskCounter = new PerformanceCounter("PhysicalDisk", "% Disk Time", "_Total");

        static void Main(string[] args)
        {
            int nProcessCount;

            nC_Cores = C_GETnCoresCPU();

            nProcessCount = System.Environment.ProcessorCount;

            for (int nI = 0; nI < nProcessCount; nI++)
            {
                PerformanceCounter oPc = new PerformanceCounter("Processor", "% Processor Time", nI.ToString());
                aoPerformanceCounter.Add(oPc);
            }

            while (true)
            {
                Console.WriteLine($"CPU: {C_GETnCPUUsage().ToString()}%");
                Console.WriteLine($"RAM: {C_GETnRamUsage().ToString()}%");
                Console.WriteLine($"DISK: {C_GETnDiskUsage().ToString()}%");
                Console.WriteLine();
                Thread.Sleep(3000);
            }
        }

        public static int C_GETnCPUUsage()
        {
            float cSum = 0;
            foreach (PerformanceCounter oCounter in aoPerformanceCounter) cSum += oCounter.NextValue();
            int nPercent = (int)Math.Round(cSum / nC_Cores, MidpointRounding.AwayFromZero);
            return nPercent > 100 ? 100 : nPercent;
        }

        public static int C_GETnRamUsage()
        {
            Int64 nAvailableRam;
            Int64 nTotalRam;
            decimal cPercentFree;
            decimal cPercentOccupied;
            int nPercent;

            nAvailableRam = cPerformanceInfo.C_GETnPhysicalAvailableMemoryInMiB();
            nTotalRam = cPerformanceInfo.C_GETnTotalMemoryInMiB();
            cPercentFree = ((decimal)nAvailableRam / (decimal)nTotalRam) * 100;
            cPercentOccupied = 100 - cPercentFree;
            nPercent = (int)Math.Round(cPercentOccupied, MidpointRounding.AwayFromZero);
            return nPercent > 100 ? 100 : nPercent;
        }

        public static int C_GETnDiskUsage()
        {
            int nPercent = (int)Math.Round(oDiskCounter.NextValue(), MidpointRounding.AwayFromZero);
            return nPercent > 100 ? 100 : nPercent;
        }

        public static int C_GETnCoresCPU()
        {
            int nCores = 0;

            foreach (var item in new System.Management.ManagementObjectSearcher("Select * from Win32_Processor").Get())
            {
                nCores += int.Parse(item["NumberOfCores"].ToString());
            }

            return nCores;
        }
    }
}
