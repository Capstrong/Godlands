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

	public Material heartMaterial;

	uint ID = 0;
	ParticleSystem _particles;

	void Awake()
	{
		ID = GetID(); // Grab the current unique ID
		name = "Buddy " + GetRandomName( ID );
		currResourceTimer = decreaseResourcesTime;
		_particles = GetComponentInChildren<ParticleSystem>();
	}

	static string[] names = { "Longnose", "Jojo", "JillyJane", "Sunshine", "Moosejaw"};
	static uint uniqueID = 1;

	static uint GetID()
	{
		return uniqueID++;
	}

	static string GetRandomName( uint ID )
	{
		int randIndex = Random.Range( 0, names.Length );

		return names[randIndex] + ID;
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

	public void GiveResource( PlayerActorStats actorStats, ResourceData resourceData )
	{
		apples++;

		actorStats.IncrementMaxStamina();

		Emote( heartMaterial );
	}

	public void Emote( Material emoteMaterial )
	{
		_particles.Clear();
		_particles.renderer.material = emoteMaterial;
		_particles.Emit( 1 );
	}
}
