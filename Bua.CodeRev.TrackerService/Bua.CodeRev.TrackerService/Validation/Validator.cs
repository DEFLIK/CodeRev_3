using System.ComponentModel.DataAnnotations;
using Bua.CodeRev.TrackerService.Primitives;

namespace Bua.CodeRev.TrackerService.Validation;

public static class Validator
{
    public static void NotNull<T>(T? value, string name, string? errorMassage)
    {
        if (!Ensure.NotNull(value))
            throw new ValidationException($"param Error: {name}, {nameof(errorMassage)}: {errorMassage}");
    }
}