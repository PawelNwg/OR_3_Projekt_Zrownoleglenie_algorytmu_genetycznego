using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OR_3_Projekt_Zrownoleglenie_alg_genetycznego_wersja_async.Processing
{
    public static class Population
    {
        static readonly Random random = new Random();
        public static Polynominal[] FillPolynominals(Polynominal[] polynominals, List<Double> coefficients)
        {
            if (polynominals.Length == Configuration.polynominalsCount)
                return polynominals;

            var polynominalsToFill = Configuration.elementsPerChunk - polynominals.Length;
            Polynominal[] newPolynominals = new Polynominal[polynominalsToFill];
            for (int i = 0; i < polynominalsToFill; i++)
            {
                var newPolynominal = new Polynominal();
                for (int j = 0; j < Configuration.polynominalsCount - 1; j++)
                {
                    newPolynominal.Elements.Add(new PolynominalElement()
                    { Coefficient = coefficients[j], Exponent = random.NextDouble() * (1.0 - (-1.0)) + (-1.0) });
                }
                newPolynominals[i] = newPolynominal;
            }
            return polynominals.Concat(newPolynominals).ToArray();
        }
    }
}
