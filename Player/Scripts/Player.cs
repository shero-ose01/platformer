using Godot;

public partial class Player : CharacterBody2D
{
	// Movement State Enum for the StateMachine
	private enum MovementState
	{
		Idle = 0,
		Walking = 1,
		Jumping = 2,
		InAir = 3,
	}
	
	// current state of the player
	private MovementState _movementState = MovementState.Idle;
	
	// Movement values
	private float _maxSpeed = 200;
	private float _minSpeed = 0.5f;
	private float _accelaration = 50;
	private float _jumpStrength = 300;
	private float _gravity = 9;
	private float _friction = 4;
	private float _aerialFriction = 0.01f;
	private float _jumpStrengthFactor = 1f;
	private float _maxJumpStrengthFactor = 2f;
	
	private AnimatedSprite2D _animation;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_movementState = MovementState.Idle;
		_animation = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		_animation.Animation = "0_Idle";
		_animation.Play();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		// determine Movementstate and apply movement

		if (!IsOnFloor())
		{
			_movementState = MovementState.InAir;
		}
		else
		{
			_movementState = MovementState.Idle;
		}

		Move(delta); 
	}

	// Calculates and applies movement to keep the Process function clean
	private void Move(double delta)
	{
		Vector2 inputDirection = new Vector2();

		// Process gravity and check if is in air for correct animation
		// Also if play is in air we don't want new movement besides gravity
		if (_movementState == MovementState.InAir)
		{
			_animation.Animation = "2_Jump";
			_animation.Frame = 2;
			
			float aerialFrictionHorizontal = Mathf.Lerp(Velocity.X, 0, _aerialFriction * (float)delta);
			
			Velocity = new Vector2(aerialFrictionHorizontal, Velocity.Y+_gravity);
			
			MoveAndSlide();
			return;
		}
		
		// Calculate simple friction and apply it before new movement input.
		// Friction is only applied to the horizontal movement since gravity does a similiar job for vertical movement
		float horizontalVelocity = Mathf.Lerp(Velocity.X, 0, _friction * (float)delta);
		Velocity = new Vector2(horizontalVelocity, Velocity.Y);
		
		// for a better "feel" we cut off speeds that are too low since our current friction calculation doesnt reach 0	
		if (Mathf.Abs(Velocity.X) < _minSpeed)
		{
			Velocity = new Vector2(0, Velocity.Y);
		}
		
		// clamp speed to a max speed since we constantly apply accelaration instead of a constant speed
		if (Mathf.Abs(Velocity.X) > _maxSpeed)
		{
			Velocity = new Vector2(_maxSpeed*Velocity.Normalized().X , Velocity.Y);
		}

		// process input and add it to our direction
		if (Input.IsActionPressed("move_right"))
			inputDirection.X += _accelaration;
		if (Input.IsActionPressed("move_left"))
			inputDirection.X -= _accelaration;
		
		// process jump direction and strength
		if (Input.IsActionJustReleased("jump"))
		{
			if (Mathf.Abs(inputDirection.X) > 0)
			{
				inputDirection.Y = -_jumpStrength * 0.75f * _jumpStrengthFactor;
				inputDirection.X *= 2f * _jumpStrengthFactor;
			}
			else
				inputDirection.Y = -_jumpStrength * _jumpStrengthFactor;
			_animation.Animation = "2_Jump";
			_animation.Frame = 2;
		}
		
		// check if space bar is being pressed for correct animation and state
		if(Input.IsActionPressed("jump"))
		{
			_movementState = MovementState.Jumping;
			_animation.Animation = "2_Jump";
			_animation.Frame = 1;

			_jumpStrengthFactor += (float)delta;
			if (_jumpStrengthFactor > _maxJumpStrengthFactor)
			{
				_jumpStrengthFactor = _maxJumpStrengthFactor;
			}
		}
		else
		{
			_jumpStrengthFactor = 1f;
		}

		// apply movement/idle animation based on state
		if (_movementState != MovementState.Jumping)
		{
			if (Mathf.Abs(inputDirection.X) > 0)
			{
				_movementState = MovementState.Walking;
				_animation.Animation = "1_Walk";
				if (inputDirection.X < 0)
				{
					_animation.FlipH = true;
				}
				else
				{
					_animation.FlipH = false;
				}
				_animation.Play();
			}else
			{
				_movementState = MovementState.Idle;
				_animation.Animation = "0_Idle";
				_animation.Play();
			
			}	
		}
		
		//apply movement if not preparing for a jump
		if(_movementState != MovementState.Jumping)
			Velocity += inputDirection;
		
		MoveAndSlide();
	}
	
}