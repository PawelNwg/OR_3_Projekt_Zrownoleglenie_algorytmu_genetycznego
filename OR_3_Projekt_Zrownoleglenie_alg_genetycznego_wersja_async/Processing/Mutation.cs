using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OR_3_Projekt_Zrownoleglenie_alg_genetycznego_wersja_async.Processing
{
    public static class Mutation
    {
        static readonly Random random = new Random();
        public static Polynominal[] MautatePolynominalsParallel(Polynominal[] polynominals)
        {
            int numberOfPolynominalsToMutate = (int)(polynominals.Length * Configuration.percentPopulationToMutate);
            if (numberOfPolynominalsToMutate == 0)
                return polynominals;

            Parallel.For(0, numberOfPolynominalsToMutate, i =>
            {
                var index = random.Next(0, polynominals.Length);
                polynominals[index] = MutatePolynominalParallel(polynominals[i]);
            });

            return polynominals;
        }

        private static Polynominal MutatePolynominalParallel(Polynominal polynominal)
        {
            var source = Enumerable.Range(0, Configuration.polynominalsCount - 1).ToArray();
            var rangePartitioner = Partitioner.Create(0, source.Length);

            Parallel.ForEach(rangePartitioner, (range, loopState) =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    polynominal.Elements[i].Exponent = random.NextDouble() * (1.0 - (-1.0)) + (-1.0);
                }
            });

            return polynominal;
        }

        public static Polynominal[] MautatePolynominals(Polynominal[] polynominals)
        {
            int numberOfPolynominalsToMutate = (int)(polynominals.Length * Configuration.percentPopulationToMutate);
            if (numberOfPolynominalsToMutate == 0)
                return polynominals;

            for (int i = 0; i < numberOfPolynominalsToMutate; i++)
            {
                var index = random.Next(0, polynominals.Length);
                polynominals[index] = MutatePolynominal(polynominals[i]);
            }
            return polynominals;
        }

        private static Polynominal MutatePolynominal(Polynominal polynominal)
        {
            foreach (var element in polynominal.Elements)
            {
                element.Exponent = random.NextDouble() * (1.0 - (-1.0)) + (-1.0);
            }
            return polynominal;
        }


    }
}

