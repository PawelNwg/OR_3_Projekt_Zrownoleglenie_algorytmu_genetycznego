using System.Collections.Concurrent;
using System.Diagnostics;

int polynominalsCount = 10000; // S
int algorithmIterations = 10024;// (int)(1024 * 1024 / polynominalsCount) - (polynominalsCount + 10); // N

double percentPopulationToMutate = 0.1;
double percentPopulationToCrossover = 0.1;
double percentPopulationToPromoteToNextGeneration = 0.8;

Random random = new Random();

var stopwatch = new Stopwatch();
stopwatch.Start();

Polynominal[] polynominals = new Polynominal[polynominalsCount];

polynominals = PrepareFirstGenerationParallel_v3();


stopwatch.Stop();
Console.WriteLine("Time taken: " + stopwatch.Elapsed.ToString(@"m\:ss\.fff" + "s"));

Polynominal[] PrepareFirstGenerationParallel_v1() // Time taken: 0:17.33817 per 10000 iterations
{
    var polynominals = new ConcurrentBag<Polynominal>();

    Parallel.For(0, polynominalsCount, new ParallelOptions { MaxDegreeOfParallelism = 30 }, (i, state) =>
     {
         Polynominal polynominal = new Polynominal();

         var polynominalElements = new ConcurrentBag<PolynominalElement>();
         Parallel.For(0, polynominalsCount - 1, new ParallelOptions { MaxDegreeOfParallelism = 30 }, j =>
         {
             polynominalElements.Add(new PolynominalElement()
             { Coefficient = random.NextDouble(), Exponent = random.NextDouble() * (1.0 - (-1.0)) + (-1.0) });
         });

         polynominal.Elements = polynominalElements.ToList();
         polynominals.Add(polynominal);
     });

    return polynominals.ToArray();
}

Polynominal[] PrepareFirstGenerationParallel_v2() // Time taken: 0:16.15816 per 1000 iterations
{
    var polynominals = new ConcurrentBag<Polynominal>();

    Parallel.For(0, polynominalsCount, (i, state) =>
    {
        Polynominal polynominal = new Polynominal();

        var polynominalElements = new ConcurrentBag<PolynominalElement>();
        for (int j = 0; j < polynominalsCount - 1; j++)
        {
            polynominalElements.Add(new PolynominalElement()
            { Coefficient = random.NextDouble(), Exponent = random.NextDouble() * (1.0 - (-1.0)) + (-1.0) });
        }

        polynominal.Elements = polynominalElements.ToList();
        polynominals.Add(polynominal);
    });

    return polynominals.ToArray();
}

Polynominal[] PrepareFirstGenerationParallel_v3() // Time taken: 0:11.97411 per 1000 iterations
{
    var polynominals = new ConcurrentBag<Polynominal>();

    Parallel.For(0, polynominalsCount, i =>
    {
        polynominals.Add(CreatePolynominalParallel_v3());
    });

    return polynominals.ToArray();
}


async Task<Polynominal[]> PrepareFirstGenerationParallel_v4()
{
    Polynominal[] polynominals = await Task.WhenAll(CreatePolynominalParallel_v1());

    return polynominals;
}

Task<Polynominal> CreatePolynominalParallel_v1()
{
    Polynominal polynominal = new Polynominal();
    for (int j = 0; j < polynominalsCount - 1; j++)
    {
        polynominal.Elements.Add(new PolynominalElement()
        { Coefficient = random.NextDouble(), Exponent = random.NextDouble() * (1.0 - (-1.0)) + (-1.0) });
    }
    return Task.FromResult(polynominal);
}

Polynominal CreatePolynominalParallel_v2() // Time taken: 0:11.97411 per 1000 iterations
{
    Polynominal polynominal = new Polynominal();
    for (int j = 0; j < polynominalsCount - 1; j++)
    {
        polynominal.Elements.Add(new PolynominalElement()
        { Coefficient = random.NextDouble(), Exponent = random.NextDouble() * (1.0 - (-1.0)) + (-1.0) });
    }
    return polynominal;
}


Polynominal CreatePolynominalParallel_v3() // Time taken: 0:01.0991 per 1000 iterations
{
    Polynominal polynominal = new Polynominal();

    var polynominalElements = new ConcurrentBag<PolynominalElement>();

    var source = Enumerable.Range(0, polynominalsCount - 1).ToArray();
    var rangePartitioner = Partitioner.Create(0, source.Length);

    Parallel.ForEach(rangePartitioner, (range, loopState) =>
    {
        polynominalElements.Add(new PolynominalElement()
        { Coefficient = random.NextDouble(), Exponent = random.NextDouble() * (1.0 - (-1.0)) + (-1.0) });
    });

    polynominal.Elements = polynominalElements.ToList();
    return polynominal;
}

Polynominal[] PrepareFirstGeneration() // Time taken: 0:12.37812 per 1000 iterations
{
    Polynominal[] polynominals = new Polynominal[polynominalsCount];
    for (int i = 0; i < polynominalsCount; i++)
    {
        polynominals[i] = new Polynominal();
        for (int j = 0; j < polynominalsCount - 1; j++)
        {
            polynominals[i].Elements.Add(new PolynominalElement()
            { Coefficient = random.NextDouble(), Exponent = random.NextDouble() * (1.0 - (-1.0)) + (-1.0) });
        }
    }

    return polynominals;
}


public class Polynominal
{
    public List<PolynominalElement> Elements = new List<PolynominalElement>();
    public double FintessValue { get; set; }
}

public class PolynominalElement
{
    public double Coefficient { get; set; }
    public double Exponent { get; set; }
}
