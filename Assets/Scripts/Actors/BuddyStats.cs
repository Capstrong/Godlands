using UnityEngine;
using System.Collections;

public class BuddyStats : MonoBehaviour
{
	enum StatNames
	{
		Invalid,
		Stamina,
	}

	[SerializeField] float decreaseResourcesTime = 60.0f; //seconds
	[SerializeField] float currResourceTimer = 0.0f;

	[SerializeField] int apples = 0;

	void Awake()
	{
		name = "Buddy " + GetRandomName();
		currResourceTimer = decreaseResourcesTime;
	}

	static string[] names = { "Longnose", "Jojo", "JillyJane", "Sunshine", "Moosejaw"};
	static uint uniqueID = 1;

	static string GetRandomName()
	{
		int randIndex = Random.Range(0, names.Length);

		return names[randIndex] + uniqueID++;
	}

	void Update()
	{
		currResourceTimer -= Time.deltaTime;

		if (currResourceTimer < 0.0f)
		{
			// decrease resources
			apples--;
			currResourceTimer = decreaseResourcesTime;
		}
	}

	public Material heartMaterial;

	int currentStats = 0;
	ParticleSystem _particles;

	void Awake()
	{
		_particles = GetComponentInChildren<ParticleSystem>();
	}

	public void GiveResource( ActorPhysics actorPhysics, ResourceData resourceData )
	{
		apples++;
		currentStats++;
		Emote( heartMaterial );
	}

	public void Emote( Material emoteMaterial )
	{
		_particles.Clear();
		_particles.renderer.material = emoteMaterial;
		_particles.Emit( 1 );
	}


}
