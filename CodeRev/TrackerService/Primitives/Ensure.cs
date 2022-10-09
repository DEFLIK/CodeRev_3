namespace TrackerService.Primitives;

public static class Ensure
{
    public static bool NotNull<T>(T? ob)
    {
        return ob != null;
    }

    public static bool GreaterThanOrEqualTo(decimal value, decimal bound)
    {
        return value > bound || Math.Abs(value - bound) < 1e-9m;
    }
}