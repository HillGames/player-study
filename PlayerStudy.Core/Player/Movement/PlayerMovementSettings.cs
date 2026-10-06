namespace PlayerStudy.Core.Player.Movement;

public sealed class PlayerMovementSettings
{
	public float MaxSpeed {get;}
	public float Acceleration {get;}
	public float Deceleration {get;}

	public PlayerMovementSettings(float maxSpeed, 
				float acceleration, 
				float deceleration)
	{
		if(maxSpeed <0)
			throw new ArgumentOutOfRangeException(nameof(maxSpeed));
		if(acceleration <0)
			throw new ArgumentOutOfRangeException(nameof(acceleration));
		if(deceleration <0)
			throw new ArgumentOutOfRangeException(nameof(deceleration));
		MaxSpeed = maxSpeed;
		Acceleration = acceleration;
		Deceleration = deceleration;
		
	}
}
