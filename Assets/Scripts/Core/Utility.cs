using UnityEngine;

public class Utility
{
    public static T GetRandomEnum<T>()
    {
        var values = System.Enum.GetValues(typeof(T));
        return (T)values.GetValue(UnityEngine.Random.Range(0, values.Length));
    }

    public static T GetRandomEnum<T>(int minInclusive, int maxExclusive)
    {
        var values = System.Enum.GetValues(typeof(T));
        if (minInclusive < 0)
        {
            minInclusive = 0;
        }
            
        if(maxExclusive > values.Length)
        {
            maxExclusive = values.Length;
        }
            
        return (T)values.GetValue(UnityEngine.Random.Range(minInclusive, maxExclusive));
    }


}
