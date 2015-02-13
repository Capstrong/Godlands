using UnityEngine;
using System.Collections;

public enum BoneLocation
{
	Head		= 0,
	Chest		= 1,
	Waist		= 2,
	LShoulder	= 3,
	LElbow		= 4,
	LHand		= 5,
	LHip		= 6,
	LKnee		= 7,
	LFoot		= 8,
	RShoulder	= 9,
	RElbow		= 10,
	RHand		= 11,
	RHip		= 12,
	RKnee		= 13,
	RFoot		= 14
}

[RequireComponent( typeof( ActorPhysics ) )]
public class Actor : MonoBehaviour
{
	public virtual void Awake()
	{
		_actorPhysics = GetComponent<ActorPhysics>();
	}
	
	ActorPhysics _actorPhysics;
	public ActorPhysics actorPhysics
	{
		get { return _actorPhysics; }
	}

	Animator _animator;
	public Animator animator
	{
		get
		{
			if( !_animator )
			{
				_animator = GetComponentInChildren<Animator>();
			}

			return _animator;
		}
	}

	public bool isRendererOn
	{
		get { return GetComponentInChildren<Renderer>().enabled; }
	}

	public Renderer[] GetRenderers()
	{
		return GetComponentsInChildren<Renderer>();
	}

	public void SetRenderers( bool setOn )
	{
		foreach( Renderer r in GetRenderers() )
		{
			r.enabled = setOn;
		}
	}

	public Transform GetBoneAtLocation( BoneLocation boneLocation )
	{
		foreach( BoneLocationTag boneTag in GetComponentsInChildren<BoneLocationTag>() )
		{
			if( boneTag.boneLocation == boneLocation )
			{
				return boneTag.transform;
			}
		}

		return null;
	}
}
