using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OR_3_Projekt_Zrownoleglenie_alg_genetycznego_wersja_async.Processing
{
    public static class Crossover
    {
        static readonly Random random = new Random();
        public static Polynominal[] CrossoverPolynominals(Polynominal[] polynominals)
        {
            int numberOfPolynominalsToCrossover = (int)(polynominals.Length * Configuration.percentPopulationToCrossover);
            if (numberOfPolynominalsToCrossover == 0)
                return polynominals;

            for (int i = 0; i < numberOfPolynominalsToCrossover; i++)
            {
                polynominals[i] = CrossoverPolynominal(polynominals.OrderBy(x => random.Next()).Take(2).ToList());
            }
            return polynominals;
        }

        private static Polynominal CrossoverPolynominal(List<Polynominal> polynominals)
        {
            var parentOne = polynominals[0];
            var parentTwo = polynominals[1];
            int numberOfElements = parentOne.Elements.Count;
            int placeOfCrossover = numberOfElements / 2;
            int numerOfDataFromParentOne = placeOfCrossover;
            int numerOfDataFromParentTwo = numberOfElements - placeOfCrossover;
            List<PolynominalElement> parentOneData = parentOne.Elements.GetRange(0, numerOfDataFromParentOne);
            List<PolynominalElement> parentTwoData = parentTwo.Elements.GetRange(numerOfDataFromParentOne, numerOfDataFromParentTwo);

            return new Polynominal() { Elements = parentOneData.Union(parentTwoData).ToList(), FintessValue = 0.0 };
        }
    }
}
