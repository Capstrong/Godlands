using UnityEngine;
using System.Collections;

[RequireComponent( typeof( ActorPhysics ) )]
public class BehaviorPhysicsController : MonoBehaviour
{
	[ReadOnly]
	public Vector3 moveDirection = Vector3.zero;

	void Awake()
	{
		ActorPhysics physics = GetComponent<ActorPhysics>();
		physics.RegisterState(
			PhysicsStateType.Grounded,
			new GroundMovement( GetComponent<Actor>() ) );
		physics.RegisterState(
			PhysicsStateType.Dead,
			new Dead( GetComponent<Actor>() ) );
	}

	public class GroundMovement : PhysicsState
	{
		Actor actor;
		BehaviorPhysicsController controller;

		public GroundMovement( Actor actor )
		{
			this.actor = actor;
			controller = actor.GetComponent<BehaviorPhysicsController>();
		}

		public override void Enter() { }

		public override void Update()
		{
			actor.physics.GroundMovement( controller.moveDirection );
			actor.animator.SetFloat( "moveSpeed", actor.physics.normalizedGroundSpeed );
		}

		public override void Exit() { }
	}

	public class Dead : PhysicsState
	{
		Actor actor;

		public Dead( Actor actor )
		{
			this.actor = actor;
		}

		public override void Enter() { }

		public override void Update()
		{
			actor.physics.DeadMovement();
		}

		public override void Exit() { }
	}
}
