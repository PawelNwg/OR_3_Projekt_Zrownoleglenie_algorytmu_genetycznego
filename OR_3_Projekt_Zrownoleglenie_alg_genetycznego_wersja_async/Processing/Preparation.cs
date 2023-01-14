using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OR_3_Projekt_Zrownoleglenie_alg_genetycznego_wersja_async
{
    public static class Preparation
    {
        static readonly Random random = new Random();

        public static Polynominal[] PrepareFirstGenerationParallel_v1() // Time taken: 0:17.33817 per 10000 iterations
        {
            var polynominals = new ConcurrentBag<Polynominal>();

            Parallel.For(0, Configuration.polynominalsCount, new ParallelOptions { MaxDegreeOfParallelism = 30 }, (i, state) =>
            {
                Polynominal polynominal = new Polynominal();

                var polynominalElements = new ConcurrentBag<PolynominalElement>();
                Parallel.For(0, Configuration.polynominalsCount - 1, new ParallelOptions { MaxDegreeOfParallelism = 30 }, j =>
                {
                    polynominalElements.Add(new PolynominalElement()
                    { Coefficient = random.NextDouble(), Exponent = random.NextDouble() * (1.0 - (-1.0)) + (-1.0) });
                });

                polynominal.Elements = polynominalElements.ToList();
                polynominals.Add(polynominal);
            });

            return polynominals.ToArray();
        }

        public static Polynominal[] PrepareFirstGenerationParallel_v2() // Time taken: 0:16.15816 per 1000 iterations
        {
            var polynominals = new ConcurrentBag<Polynominal>();

            Parallel.For(0, Configuration.polynominalsCount, (i, state) =>
            {
                Polynominal polynominal = new Polynominal();

                var polynominalElements = new ConcurrentBag<PolynominalElement>();
                for (int j = 0; j < Configuration.polynominalsCount - 1; j++)
                {
                    polynominalElements.Add(new PolynominalElement()
                    { Coefficient = random.NextDouble(), Exponent = random.NextDouble() * (1.0 - (-1.0)) + (-1.0) });
                }

                polynominal.Elements = polynominalElements.ToList();
                polynominals.Add(polynominal);
            });

            return polynominals.ToArray();
        }

        public static Polynominal[] PrepareFirstGenerationParallel_v3() // Time taken: 0:11.97411 per 1000 iterations
        {
            var polynominals = new ConcurrentBag<Polynominal>();

            Parallel.For(0, Configuration.polynominalsCount, i =>
            {
                polynominals.Add(CreatePolynominalParallel_v3());
            });

            return polynominals.ToArray();
        }


        public static async Task<Polynominal[]> PrepareFirstGenerationParallel_v4()
        {
            Polynominal[] polynominals = await Task.WhenAll(CreatePolynominalParallel_v1());

            return polynominals;
        }

        public static Task<Polynominal> CreatePolynominalParallel_v1()
        {
            Polynominal polynominal = new Polynominal();
            for (int j = 0; j < Configuration.polynominalsCount - 1; j++)
            {
                polynominal.Elements.Add(new PolynominalElement()
                { Coefficient = random.NextDouble(), Exponent = random.NextDouble() * (1.0 - (-1.0)) + (-1.0) });
            }
            return Task.FromResult(polynominal);
        }

        public static Polynominal CreatePolynominalParallel_v2() // Time taken: 0:11.97411 per 1000 iterations
        {
            Polynominal polynominal = new Polynominal();
            for (int j = 0; j < Configuration.polynominalsCount - 1; j++)
            {
                polynominal.Elements.Add(new PolynominalElement()
                { Coefficient = random.NextDouble(), Exponent = random.NextDouble() * (1.0 - (-1.0)) + (-1.0) });
            }
            return polynominal;
        }


        public static Polynominal CreatePolynominalParallel_v3() // Time taken: 0:01.0991 per 1000 iterations
        {
            Polynominal polynominal = new Polynominal();

            var polynominalElements = new ConcurrentBag<PolynominalElement>();

            var source = Enumerable.Range(0, Configuration.polynominalsCount - 1).ToArray();
            var rangePartitioner = Partitioner.Create(0, source.Length);

            Parallel.ForEach(rangePartitioner, (range, loopState) =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    polynominalElements.Add(new PolynominalElement()
                    { Coefficient = random.NextDouble(), Exponent = random.NextDouble() * (1.0 - (-1.0)) + (-1.0) });
                }
            });

            polynominal.Elements = polynominalElements.ToList();
            return polynominal;
        }

        public static Polynominal[] PrepareFirstGeneration(int polinominalsToGenerate) // Time taken: 0:12.37812 per 1000 iterations
        {
            Polynominal[] polynominals = new Polynominal[polinominalsToGenerate];
            for (int i = 0; i < polinominalsToGenerate; i++)
            {
                polynominals[i] = new Polynominal();
                for (int j = 0; j < Configuration.polynominalsCount - 1; j++)
                {
                    polynominals[i].Elements.Add(new PolynominalElement()
                    { Coefficient = random.NextDouble(), Exponent = random.NextDouble() * (1.0 - (-1.0)) + (-1.0) });
                }
                //Console.WriteLine(i);
            }

            return polynominals;
        }
    }
}
