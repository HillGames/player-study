using NUnit.Framework;
using PlayerStudy.Core.Common.Math;

namespace PlayerStudy.Test.Common.Math;

[TestFixture]
public sealed class Vector2Test
{
    [Test]
    public void ZeroVectorHasZeroLength()
    {
        var vector = Vector2.Zero;

        Assert.That(vector.X, Is.EqualTo(0.0f));
        Assert.That(vector.Y, Is.EqualTo(0.0f));
        Assert.That(vector.Length, Is.EqualTo(0.0f));
    }

    [Test]
    public void CalculatesLength()
    {
        var vector = new Vector2(3.0f, 4.0f);

        Assert.That(vector.Length, Is.EqualTo(5.0f));
    }

    [Test]
    public void NormalizedVectorHasUnitLength()
    {
        var vector = new Vector2(3.0f, 4.0f);

        var normalized = vector.Normalized();

        Assert.That(normalized.Length, Is.EqualTo(1.0f).Within(0.0001f));
    }

    [Test]
    public void NormalizingZeroReturnsZero()
    {
        var normalized = Vector2.Zero.Normalized();

        Assert.That(normalized, Is.EqualTo(Vector2.Zero));
    }

    [Test]
    public void AddsVectors()
    {
        var left = new Vector2(2.0f, 3.0f);
        var right = new Vector2(4.0f, 5.0f);

        var result = left + right;

        Assert.That(result, Is.EqualTo(new Vector2(6.0f, 8.0f)));
    }

    [Test]
    public void SubtractsVectors()
    {
        var left = new Vector2(5.0f, 8.0f);
        var right = new Vector2(2.0f, 3.0f);

        var result = left - right;

        Assert.That(result, Is.EqualTo(new Vector2(3.0f, 5.0f)));
    }

    [Test]
    public void MultipliesVectorByScalar()
    {
        var vector = new Vector2(2.0f, 3.0f);

        var result = vector * 2.0f;

        Assert.That(result, Is.EqualTo(new Vector2(4.0f, 6.0f)));
    }

    [Test]
    public void SupportsEquality()
    {
        var first = new Vector2(2.0f, 3.0f);
        var second = new Vector2(2.0f, 3.0f);

        Assert.That(first, Is.EqualTo(second));
        Assert.That(first == second, Is.True);
    }
}