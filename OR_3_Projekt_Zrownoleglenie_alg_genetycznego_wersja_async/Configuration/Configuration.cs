using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OR_3_Projekt_Zrownoleglenie_alg_genetycznego_wersja_async
{
    public static class Configuration
    {
        public static readonly int polynominalsCount = 1024; // S
        public static readonly int algorithmIterations = 512;// (int)(1024 * 1024 / polynominalsCount) - (polynominalsCount + 10); // N
        public static readonly double percentPopulationToMutate = 0.1;
        public static readonly double percentPopulationToCrossover = 0.1;
        public static readonly double percentPopulationToPromoteToNextGeneration = 0.8;
        public static readonly int threadCount = 4;
        public static readonly int elementsPerChunk = Configuration.polynominalsCount / threadCount;
    }
}
