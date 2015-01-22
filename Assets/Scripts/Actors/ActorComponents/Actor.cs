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

[RequireComponent(typeof(ActorPhysics))]
[RequireComponent(typeof(ActorCamera))]
[RequireComponent(typeof(ActorCombat))]
public class Actor : MonoBehaviour
{
	ActorPhysics actorPhysics;
	ActorCamera actorCamera;
	ActorCombat actorCombat;
	ActorResources actorResources;

	Animator anim;

	void Awake()
	{
		actorPhysics = GetComponent<ActorPhysics>();
		actorCamera = GetComponent<ActorCamera>();
		actorCombat = GetComponent<ActorCombat>();
		actorResources = GetComponent<ActorResources>();

		actorPhysics.SetActor(this);
		actorCamera.SetActor(this);
		actorCombat.SetActor(this);
		actorResources.SetActor(this);
	}

	public ActorPhysics GetPhysics()
	{
		return actorPhysics;
	}

	public ActorCamera GetCameraScript()
	{
		return actorCamera;
	}

	public ActorCombat GetCombatScript()
	{
		return actorCombat;
	}

	public Camera GetCamera()
	{
		return actorCamera.cam;
	}

	public Transform GetModel()
	{
		return actorPhysics.model;
	}

	public Animator GetAnimator()
	{
		if(!anim)
		{
			anim = GetComponentInChildren<Animator>();
		}

		return anim;
	}

	public bool AreRenderersOn()
	{
		return GetComponentInChildren<Renderer>().enabled;
	}

	public Renderer[] GetRenderers()
	{
		return GetComponentsInChildren<Renderer>();
	}

	public void ToggleRenderers(bool setOn)
	{
		foreach(Renderer r in GetRenderers())
		{
			r.enabled = setOn;
		}
	}

	public Transform GetBoneAtLocation(BoneLocation boneLocation)
	{
		foreach(BoneLocationTag boneTag in GetComponentsInChildren<BoneLocationTag>())
		{
			if(boneTag.boneLocation == boneLocation)
			{
				return boneTag.transform;
			}
		}

		return null;
	}

	public void OnAttacked(Attack attack)
	{

	}
}
