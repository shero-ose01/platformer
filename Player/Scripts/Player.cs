using Godot;
using System;

public partial class Player : CharacterBody2D
{
	private float _maxSpeed = 100;
	private float _minSpeed = 0.5f;
	private float _accelaration = 50;
	private float _jumpStrength = 200;
	private float _gravity = 5;
	private float _friction = 10;

	private AnimatedSprite2D _animation;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_animation = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		_animation.Play();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		Move(delta);
	}

	// Calculates and applies movement to keep the Process function clean
	private void Move(double delta)
	{
		Vector2 velocity = new Vector2();

		if (!IsOnFloor())
		{
			_animation.Animation = "2_Jump";
			_animation.Frame = 2;
			velocity += new Vector2(0, _gravity);
		}

		float horizontalVelocity = Mathf.Lerp(Velocity.X, 0, _friction * (float)delta);
		Velocity = new Vector2(horizontalVelocity, Velocity.Y);
		
		if (Mathf.Abs(Velocity.X) < _minSpeed)
		{
			Velocity = new Vector2(0, Velocity.Y);
		}
		
		if (Input.IsActionPressed("move_right"))
			velocity.X += _accelaration;
		if (Input.IsActionPressed("move_left"))
			velocity.X -= _accelaration;
		if (Input.IsActionJustPressed("jump")&& IsOnFloor())
			velocity.Y = 0;
		
		if (Mathf.Abs(Velocity.X) > _maxSpeed)
		{
			Velocity = new Vector2(_maxSpeed*Velocity.Normalized().X , Velocity.Y);
		}

		if (Mathf.Abs(velocity.X) > 0 && IsOnFloor())
		{
			_animation.Animation = "1_Walk";
			_animation.Play();
		}else if (Mathf.Abs(velocity.X) < 0 && IsOnFloor())
		{
			_animation.Animation = "1_Walk";
			_animation.Play();
		}
		
		Velocity += velocity;
		MoveAndSlide();
	}
	
}
