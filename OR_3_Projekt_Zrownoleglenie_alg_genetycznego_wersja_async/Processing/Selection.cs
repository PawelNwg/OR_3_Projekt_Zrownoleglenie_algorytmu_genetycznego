using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OR_3_Projekt_Zrownoleglenie_alg_genetycznego_wersja_async.Processing
{
    public static class Selection
    {
        public static Polynominal[] SelectBestPolynominals(Polynominal[] polynominals)
        {
            int numberOfPolynominalsToPromoteToNextGeneration = (int)(polynominals.Length * Configuration.percentPopulationToPromoteToNextGeneration);
            if (numberOfPolynominalsToPromoteToNextGeneration == 0)
                return polynominals;

            return polynominals.OrderBy(x => x.FintessValue).Take(numberOfPolynominalsToPromoteToNextGeneration).ToArray();
        }
    }
}
