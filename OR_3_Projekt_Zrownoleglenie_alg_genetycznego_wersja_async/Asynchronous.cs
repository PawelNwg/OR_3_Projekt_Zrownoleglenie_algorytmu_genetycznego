using OR_3_Projekt_Zrownoleglenie_alg_genetycznego_wersja_async;
using OR_3_Projekt_Zrownoleglenie_alg_genetycznego_wersja_async.Processing;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text;

int threadCount = 5;
int elementsPerChunk = Configuration.polynominalsCount / threadCount;

Random random = new Random();

Polynominal[] polynominals = new Polynominal[Configuration.polynominalsCount];
int k = 1;

var stopwatch = new Stopwatch();
stopwatch.Start();

//polynominals = Preparation.PrepareFirstGeneration(Configuration.polynominalsCount);
//polynominals = Mutation.MautatePolynominals(polynominals);

//var res = await Task.Factory.StartNew(Work_part_1);

List<List<Polynominal>> polynominal = new List<List<Polynominal>>();
List<Thread> threads = new List<Thread>();
for (int i = 0; i < threadCount; i++)
{
    Thread t = new Thread(() => { polynominal.Add(Work_part_1()); });
    t.Start();
    threads.Add(t);
}

foreach (Thread thread in threads)
{
    thread.Join();
}

polynominals = polynominal.SelectMany(x => x).ToArray();
polynominals = Crossover.CrossoverPolynominals(polynominals);

var p = polynominal.ChunkBy(threadCount);
// split
polynominals = Fitness.CalculateFitness(polynominals);


stopwatch.Stop();
Console.WriteLine("Time taken: " + stopwatch.Elapsed.ToString(@"m\:ss\.fff" + "s"));
;
List<Polynominal> Work_part_1()
{
    Polynominal[] polynominals = new Polynominal[elementsPerChunk];
    if (k == 1)
        polynominals = Preparation.PrepareFirstGeneration(elementsPerChunk);
    polynominals = Mutation.MautatePolynominals(polynominals);
    return polynominals.ToList();
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