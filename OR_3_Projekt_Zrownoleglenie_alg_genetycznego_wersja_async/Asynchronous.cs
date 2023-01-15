using OR_3_Projekt_Zrownoleglenie_alg_genetycznego_wersja_async;
using OR_3_Projekt_Zrownoleglenie_alg_genetycznego_wersja_async.Processing;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text;
using static System.Linq.Enumerable;

Polynominal[] polynominals = new Polynominal[Configuration.polynominalsCount];
int k = 1;
var stopwatch = new Stopwatch();
stopwatch.Start();

for (int algorithmIteration = 0; algorithmIteration < Configuration.algorithmIterations; algorithmIteration++, k++)
    polynominals = IslandModelParallel();

stopwatch.Stop();
Console.WriteLine("Time taken: " + stopwatch.Elapsed.ToString(@"m\:ss\.fff" + "s"));

// model wyspowy 

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
