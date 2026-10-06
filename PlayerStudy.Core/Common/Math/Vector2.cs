namespace PlayerStudy.Core.Common.Math;

public readonly struct Vector2 : IEquatable<Vector2>
{
    public static readonly Vector2 Zero = new(0.0f, 0.0f);
    public float X { get; }
    public float Y { get; }
    public float LengthSquared => (X * X) + (Y * Y);
    public float Length => MathF.Sqrt(LengthSquared);
	
    public Vector2(float x, float y)
    {
        X = x;
        Y = y;
    }

    public Vector2 Normalized()
    {
        var length = Length;

        if (length <= float.Epsilon)
            return Zero;

        return new Vector2(
            X / length,
            Y / length);
    }

    public static Vector2 operator +(Vector2 left, Vector2 right)
    {
        return new Vector2(
            left.X + right.X,
            left.Y + right.Y);
    }

    public static Vector2 operator -(Vector2 left, Vector2 right)
    {
        return new Vector2(
            left.X - right.X,
            left.Y - right.Y);
    }

    public static Vector2 operator *(Vector2 vector, float scalar)
    {
        return new Vector2(
            vector.X * scalar,
            vector.Y * scalar);
    }

    public static Vector2 operator *(float scalar, Vector2 vector)
    {
        return vector * scalar;
    }

    public static Vector2 operator /(Vector2 vector, float scalar)
    {
        if (MathF.Abs(scalar) <= float.Epsilon)
            throw new DivideByZeroException();

        return new Vector2(
            vector.X / scalar,
            vector.Y / scalar);
    }

    public bool Equals(Vector2 other)
    {
        return X.Equals(other.X) &&
               Y.Equals(other.Y);
    }

    public override bool Equals(object? obj)
    {
        return obj is Vector2 other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y);
    }

    public static bool operator ==(Vector2 left, Vector2 right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Vector2 left, Vector2 right)
    {
        return !left.Equals(right);
    }

    public override string ToString()
    {
        return $"({X}, {Y})";
    }
}