using OR_3_Projekt_Zrownoleglenie_alg_genetycznego_wersja_async;
using OR_3_Projekt_Zrownoleglenie_alg_genetycznego_wersja_async.Processing;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text;
using static System.Linq.Enumerable;

Polynominal[] polynominals = new Polynominal[Configuration.polynominalsCount];
ParallelOptions options = new ParallelOptions {MaxDegreeOfParallelism = Configuration.threadCount};
int k = 0;
Random random = new Random();
var stopwatch = new Stopwatch();
stopwatch.Start();

foreach (var i in Range(0, Configuration.algorithmIterations - 1))
{
    k++;
    polynominals = IslandModelParallelForeach();
}

stopwatch.Stop();
Console.WriteLine("Time taken: " + stopwatch.Elapsed.ToString(@"m\:ss\.fff" + "s"));

// model wyspowy 
// PARALLEL FOREACH START
Polynominal[] IslandModelParallelForeach()
{
    ConcurrentBag<List<Polynominal>> polynominalBag = new ConcurrentBag<List<Polynominal>>();
    
    if (k == 1)
        polynominals = Preparation.PrepareFirstGeneration(Configuration.polynominalsCount);
    
    var chunkedPolinominals = polynominals.Chunk(Configuration.elementsPerChunk).ToArray();

    Parallel.ForEach(chunkedPolinominals, options,(line, _, _) =>
    {
        polynominalBag.Add(ParallelWork(line));
    });
    
    return polynominalBag.SelectMany(x => x).ToArray();
}

List<Polynominal> ParallelWork(Polynominal[] polynominalsChunked)
{
    polynominalsChunked = MautatePolynominals(polynominalsChunked);
    polynominalsChunked = Crossover.CrossoverPolynominals(polynominalsChunked);
    polynominalsChunked = CalculateFitnessV2(polynominalsChunked);
    polynominalsChunked = Selection.SelectBestPolynominals(polynominalsChunked);
    Console.WriteLine($"Pokolenie {k}, najlepszy osobnik {polynominalsChunked.First().FintessValue}, wyspa {Thread.CurrentThread.ManagedThreadId}"); // only for Debug
    polynominalsChunked = FillPolynominals(polynominalsChunked, polynominalsChunked.First().Elements.Select(x => x.Coefficient).ToList());

    return polynominalsChunked.ToList();
}

Polynominal[] MautatePolynominals(Polynominal[] polynominals)
{
    int numberOfPolynominalsToMutate = (int)(polynominals.Length * Configuration.percentPopulationToMutate);
    if (numberOfPolynominalsToMutate == 0)
        return polynominals;

    for (int i = 0; i < numberOfPolynominalsToMutate; i++)
    {
        var index3 = random.Next(0, polynominals.Length);
        polynominals[index3] = MutatePolynominal(polynominals[i]);
    }
    return polynominals;
}

Polynominal MutatePolynominal(Polynominal polynominal)
{
    Parallel.ForEach(polynominal.Elements, options, (element, _, _) =>
    {
        element.Exponent = random.NextDouble() * (1.0 - (-1.0)) + (-1.0);
    });
    
    return polynominal;
}

Polynominal[] CalculateFitness(Polynominal[] polynominals)
{
    Parallel.ForEach(polynominals, options,(element, _, _) =>
    {
        var tempSum = 0.0;
        foreach (var elementElement in element.Elements)
        {
            tempSum += elementElement.Coefficient * elementElement.Exponent;
        }
        element.FintessValue = tempSum;
    });
    
    return polynominals;
}

Polynominal[] CalculateFitnessV2(Polynominal[] polynominals)
{
    Parallel.ForEach(polynominals, options,(polinominal, _, _) =>
    {
        ConcurrentBag<double> bag = new ConcurrentBag<double>();

        Parallel.ForEach(polinominal.Elements, options,(element, _, _) =>
        {
            bag.Add(element.Coefficient * element.Exponent);
        });

        polinominal.FintessValue = bag.Sum();
    });
    
    return polynominals;
}

Polynominal[] FillPolynominals(Polynominal[] polynominals, List<Double> coefficients)
{
    if (polynominals.Length == Configuration.polynominalsCount)
        return polynominals;

    var polynominalsToFill = Configuration.elementsPerChunk - polynominals.Length;
    Polynominal[] newPolynominals = new Polynominal[polynominalsToFill];



    for (int i = 0; i < polynominalsToFill; i++)
    {
        var newPolynominal = new Polynominal();
        var range = Range(0, Configuration.polynominalsCount - 1);

        ConcurrentBag<PolynominalElement> polynominalElements = new ConcurrentBag<PolynominalElement>();

        Parallel.ForEach(range, options, (polynominal, _, index) =>
        {
            var polynominalElement = new PolynominalElement()
                {Coefficient = coefficients[(int) index], Exponent = 50.0 * (1.0 - (-1.0)) + (-1.0)};

            polynominalElements.Add(polynominalElement);
        });
        newPolynominal.Elements.AddRange(polynominalElements.ToArray());
        newPolynominals[i] = newPolynominal;
    }

    return polynominals.Concat(newPolynominals).ToArray();
}
// PARALLEL FOREACH STOP

Polynominal[] IslandModelParallel()
{
    ConcurrentBag<List<Polynominal>> polynominal = new ConcurrentBag<List<Polynominal>>();
    List<Thread> threads = new List<Thread>();

    if (k == 1)
        polynominals = Preparation.PrepareFirstGeneration(Configuration.polynominalsCount);

    var chunkedPolinominals = polynominals.Chunk(Configuration.elementsPerChunk).ToArray();

    foreach (var index in Range(0, Configuration.threadCount))
        threads.Add(new Thread(() => { polynominal.Add(ParallelThreadWork(chunkedPolinominals[index])); }));

    threads.ForEach(x => x.Start());
    threads.ForEach(x => x.Join());
    return polynominal.SelectMany(x => x).ToArray();
}

List<Polynominal> ParallelThreadWork(Polynominal[] polynominalsChunked)
{
    polynominalsChunked = Mutation.MautatePolynominals(polynominalsChunked);
    polynominalsChunked = Crossover.CrossoverPolynominals(polynominalsChunked);
    polynominalsChunked = Fitness.CalculateFitness(polynominalsChunked);
    polynominalsChunked = Selection.SelectBestPolynominals(polynominalsChunked);
    Console.WriteLine($"Pokolenie {k}, najlepszy osobnik {polynominalsChunked.First().FintessValue}, wyspa {Thread.CurrentThread.ManagedThreadId}"); // only for Debug
    polynominalsChunked = Population.FillPolynominals(polynominalsChunked, polynominalsChunked.First().Elements.Select(x => x.Coefficient).ToList());

    return polynominalsChunked.ToList();
}

public static class ListExtensions
{
    public static List<List<T>> ChunkBy<T>(this List<T> source, int chunkSize)
    {
        return source
            .Select((x, i) => new { Index = i, Value = x })
            .GroupBy(x => x.Index / chunkSize)
            .Select(x => x.Select(v => v.Value).ToList())
            .ToList();
    }
}
public static class EnumExtension
{
    public static IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> self)
       => self.Select((item, index) => (item, index));
}
