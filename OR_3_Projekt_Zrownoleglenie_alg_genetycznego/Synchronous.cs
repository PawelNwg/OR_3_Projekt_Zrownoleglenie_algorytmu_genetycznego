// zalozenia: stałe wartośći przy zmiennych x algorytow np S = 3, 3*x1^2 + 1*x2^2 

using System.Diagnostics;
using System.Text;

int polynominalsCount = 1024; // S
int algorithmIterations = 1024;// (int)(1024 * 1024 / polynominalsCount) - (polynominalsCount + 10); // N

double percentPopulationToMutate = 0.1;
double percentPopulationToCrossover = 0.1;
double percentPopulationToPromoteToNextGeneration = 0.8;

Random random = new Random();
Polynominal[] polynominals = new Polynominal[polynominalsCount];

var stopwatch = new Stopwatch();
stopwatch.Start();

for (int algorithmIteration = 0, k = 1; algorithmIteration < algorithmIterations; algorithmIteration++, k++)
{
    if (k == 1)
        polynominals = PrepareFirstGeneration();
    
    polynominals = MautatePolynominals(polynominals);
    polynominals = CrossoverPolynominals(polynominals);
    polynominals = CalculateFitness(polynominals);
    polynominals = SelectBestPolynominals(polynominals);
    polynominals = FillPolynominals(polynominals, polynominals.First().Elements.Select(x => x.Coefficient).ToList());
    polynominals = CalculateFitness(polynominals); // only to display, can be removed
    PrintPolynominals(polynominals);
}

stopwatch.Stop();
Console.WriteLine("Time taken: " + stopwatch.Elapsed.ToString(@"m\:ss\.fff" + "s"));

Polynominal[] FillPolynominals(Polynominal[] polynominals, List<Double> coefficients)
{
    if (polynominals.Length == polynominalsCount)
        return polynominals;

    var polynominalsToFill = polynominalsCount - polynominals.Length;
    Polynominal[] newPolynominals = new Polynominal[polynominalsToFill];
    for (int i = 0; i < polynominalsToFill; i++)
    {
        var newPolynominal =  new Polynominal();
        for (int j = 0; j < polynominalsCount - 1; j++)
        {
            newPolynominal.Elements.Add(new PolynominalElement()
            { Coefficient = coefficients[j], Exponent = random.NextDouble() * (1.0 - (-1.0)) + (-1.0) });
        }
        newPolynominals[i] = newPolynominal;
    }
    return polynominals.Union(newPolynominals).ToArray();
}

Polynominal[] SelectBestPolynominals(Polynominal[] polynominals)
{
    int numberOfPolynominalsToPromoteToNextGeneration = (int)(polynominals.Length * percentPopulationToPromoteToNextGeneration);
    if (numberOfPolynominalsToPromoteToNextGeneration == 0)
        return polynominals;

    return polynominals.OrderBy(x => x.FintessValue).Take(numberOfPolynominalsToPromoteToNextGeneration).ToArray();
}

Polynominal[] CrossoverPolynominals(Polynominal[] polynominals)
{
    int numberOfPolynominalsToCrossover = (int)(polynominals.Length * percentPopulationToCrossover);
    if (numberOfPolynominalsToCrossover == 0)
        return polynominals;

    for (int i = 0; i < numberOfPolynominalsToCrossover; i++)
    {
        polynominals[i] = CrossoverPolynominal(polynominals.OrderBy(x => random.Next()).Take(2).ToList());
    }
    return polynominals;
}

Polynominal CrossoverPolynominal(List<Polynominal> polynominals)
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

Polynominal[] MautatePolynominals(Polynominal[] polynominals)
{
    int numberOfPolynominalsToMutate = (int)(polynominals.Length * percentPopulationToMutate);
    if (numberOfPolynominalsToMutate == 0)
        return polynominals;

    for (int i = 0; i < numberOfPolynominalsToMutate; i++)
    {
        var index = random.Next(0, polynominals.Length);
        polynominals[index] = MutatePolynominal(polynominals[i]);
    }
    return polynominals;
}

Polynominal MutatePolynominal(Polynominal polynominal)
{
    foreach (var element in polynominal.Elements)
    {
        element.Exponent = random.NextDouble() * (1.0 - (-1.0)) + (-1.0);
    }
    return polynominal;
}

Polynominal[] CalculateFitness(Polynominal[] polynominals)
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

Polynominal[] PrepareFirstGeneration()
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

void PrintPolynominals(Polynominal[] polynominals)
{
    StringBuilder stringBuilder = new StringBuilder();
    for (int i = 0; i < polynominalsCount; i++)
    {
        stringBuilder.Append($"Wielomian {i}: ");
        for (int j = 0; j < polynominals[i].Elements.Count; j++)
        {
            stringBuilder.Append($"{Math.Round(polynominals[i].Elements[j].Coefficient, 2)} * x^{j} + ");
        }
        stringBuilder.Append($", fitness: {Math.Round(polynominals[i].FintessValue, 3)}");
        stringBuilder.Append("\n");
    }
    Console.WriteLine(stringBuilder.ToString());
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

public static class EnumExtension
{
    public static IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> self)
       => self.Select((item, index) => (item, index));
}