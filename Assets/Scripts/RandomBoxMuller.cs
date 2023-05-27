using UnityEngine;

/// <summary>
/// Box–Muller's method
/// </summary>
public class RandomBoxMuller
{
    public static float Range(float min, float max)
    {
        while (true)
        {
            // N(range *0.16f, range.Center)
            // Almost Result will be for min to max
            float v = GetNext((max - min) * 0.24f, (min + max) * 0.5f); //for the first value, (sigma) its going to be higher numbers max the range more spread
            if (min <= v && v <= max) return v;
        }
    }

    /// <summary>
    /// N(mu,sigma)
    /// </summary>
    /// <returns></returns>
    public static float GetNext(float sigma = 1f, float mu = 0)
    {
        float rand1, rand2;
        while ((rand1 = Random.value) == 0) ;
        while ((rand2 = Random.value) == 0) ;
        return Mathf.Sqrt(-2f * Mathf.Log(rand1)) * Mathf.Cos(2f * Mathf.PI * rand2) * sigma + mu;
    }

    private static float DestabilizeRange(float min, float max, float v) => (min + max) * v;
}