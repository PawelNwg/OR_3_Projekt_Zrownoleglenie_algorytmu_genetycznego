using OR_3_Projekt_Zrownoleglenie_alg_genetycznego_wersja_async;
using OR_3_Projekt_Zrownoleglenie_alg_genetycznego_wersja_async.Processing;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text;
using static System.Linq.Enumerable;

Random random = new Random();

Polynominal[] polynominals = new Polynominal[Configuration.polynominalsCount];
int k = 1;
var stopwatch = new Stopwatch();
stopwatch.Start();

for (int algorithmIteration = 0; algorithmIteration < Configuration.algorithmIterations; algorithmIteration++, k++)
{
     polynominals = ParallelPart_1();
}

stopwatch.Stop();
Console.WriteLine("Time taken: " + stopwatch.Elapsed.ToString(@"m\:ss\.fff" + "s"));

// mozna zasosowac model wyspowy i zlaczyc tylko na koncu 

Polynominal[] ParallelPart_1()
{
    List<List<Polynominal>> polynominal = new List<List<Polynominal>>();
    List<Thread> threads = new List<Thread>();

    if (k == 1)
        polynominals = Preparation.PrepareFirstGeneration(Configuration.polynominalsCount);

    var chunkedPolinominals = polynominals.Chunk(Configuration.elementsPerChunk).ToArray();
    Console.WriteLine(polynominals.Count() + " " + k);

    foreach (var index in Range(0, Configuration.threadCount))
    {
        threads.Add(new Thread(() => { polynominal.Add(ParallelThreadWork_1(chunkedPolinominals[index])); }));
    }

    threads.ForEach(x => x.Start());
    threads.ForEach(x => x.Join());
    while (polynominal.Count != Configuration.threadCount)
        Thread.Sleep(1);
    return polynominal.SelectMany(x => x).ToArray();
}

List<Polynominal> ParallelThreadWork_1(Polynominal[] polynominalsChunked)
{
    polynominalsChunked = Mutation.MautatePolynominals(polynominalsChunked);
    polynominalsChunked = Crossover.CrossoverPolynominals(polynominalsChunked);
    polynominalsChunked = Fitness.CalculateFitness(polynominalsChunked);
    polynominalsChunked = Selection.SelectBestPolynominals(polynominalsChunked);
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

//var res = await Task.Factory.StartNew(Work_part_1);
//var p = polynominals.ToList().ChunkBy(threadCount);
// split