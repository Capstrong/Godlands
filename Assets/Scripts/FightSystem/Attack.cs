using UnityEngine;
using System.Collections;

public enum ColliderType
{
	Box,
	Sphere
}

public enum DamageType
{
	Normal
}

[System.Serializable]
public class FrameWindow
{
	public int start;
	public int end;

	public int Length
	{
		get { return end - start; }
	}

	public bool CheckInBounds(float num)
	{
		return num > start && num < end;
	}

	public bool IsStartTime(float currentFrame)
	{
		return Mathf.Abs(currentFrame - start) <= 1;
	}

	public bool IsEndTime(float currentFrame)
	{
		return Mathf.Abs(currentFrame - end) <= 1;
	}
}

public class Attack : ScriptableObject 
{
	public float playerSpeedMod = 0.7f;
	public float vulnerableTime = 1f;

	public AnimationClip attackAnim;

	void Awake()
	{
	}

	void Update()
	{

	}
}
