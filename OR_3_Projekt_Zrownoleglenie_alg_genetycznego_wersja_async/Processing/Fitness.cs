using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OR_3_Projekt_Zrownoleglenie_alg_genetycznego_wersja_async.Processing
{
    public static class Fitness
    {
        public static Polynominal[] CalculateFitness(Polynominal[] polynominals)
        {
            foreach (var (polynominal, index) in polynominals.WithIndex())
            {
                var tempSum = 0.0;
                foreach (var element in polynominal.Elements)
                {
                    tempSum += element.Coefficient * element.Exponent;
                }
                polynominal.FintessValue = tempSum;
            }

            return polynominals;
        }
    }
}
