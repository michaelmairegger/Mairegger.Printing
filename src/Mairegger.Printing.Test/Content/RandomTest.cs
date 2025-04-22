using Bogus;

namespace Mairegger.Printing.Tests.Content;

internal class RandomTest
{
    private static readonly Faker s_faker = new();

    public static IEnumerable<TheoryDataRow<int>> NumberList(int min, int max, int testCount)
    {
        for(int i = 1; i <= testCount; i++)
        {
            yield return new TheoryDataRow<int>(s_faker.Random.Int(min, max));
        }
    }

    public static IEnumerable<TheoryDataRow<int, int>> NumberList2(int min1, int max1, int min2, int max2, int testCount)
    {
        for(int i = 1; i <= testCount; i++)
        {
            yield return new TheoryDataRow<int, int>(s_faker.Random.Int(min1, max1), s_faker.Random.Int(min2, max2));
        }
    }

    public static IEnumerable<TheoryDataRow<double, double>> NumberList2Double(double min1, double max1, double min2, double max2, int testCount)
    {
        for(int i = 1; i <= testCount; i++)
        {
            yield return new TheoryDataRow<double, double>(s_faker.Random.Double(min1, max1),s_faker.Random.Double(min2, max2));
        }
    }
}
