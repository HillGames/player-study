using PlayerStudy.Core.Common.Math;
namespace PlayerStudy.Core.Player.Movement;

public sealed class PlayerMovement
{
	private readonly PlayerMovementSettings _settings;
	public Vector2 Velocity {get; private set;}

	public PlayerMovement(PlayerMovementSettings settings)
	{
		_settings = settings ?? throw new ArgumentNullException(nameof(settings));
		Velocity = Vector2.Zero;
	}

	public void Update(Vector2 input, float deltaTime)
	{
		if(deltaTime <0)
			throw new ArgumentOutOfRangeException(nameof(deltaTime));
		
		var inputMagnitude = input.Length;
		if(inputMagnitude > 1.0f)
		{
			input = input.Normalized();
			inputMagnitude = 1.0f;

		}
		        // No input: decelerate toward zero.
        if (inputMagnitude <= float.Epsilon)
        {
            Velocity = MoveTowards(
                Velocity,
                Vector2.Zero,
                _settings.Deceleration * deltaTime);

            return;
        }
		
		 // Separate direction from input strength.
        var direction = input.Normalized();

        // Analog input controls the target speed.
        var targetSpeed =
            inputMagnitude * _settings.MaxSpeed;

        var targetVelocity =
            direction * targetSpeed;

        // Momentum: gradually approach target velocity.
        Velocity = MoveTowards(
            Velocity,
            targetVelocity,
            _settings.Acceleration * deltaTime);

	}
	private static Vector2 MoveTowards(
        Vector2 current,
        Vector2 target,
        float maxDelta)
    {
        var difference = target - current;
        var distance = difference.Length;

        if (distance <= maxDelta || distance <= float.Epsilon)
            return target;

        return current +
               difference.Normalized() * maxDelta;
    }
}