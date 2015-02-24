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
			actor.animator.SetFloat( "moveSpeed", controller.moveDirection.magnitude );
		}

		public override void Exit() { }
	}
}
