using UnityEngine;
using System.Collections;

public class TravelSounds : MonoBehaviour
{
	[Tooltip( "Audio source with settings for playing any step sound" )]
	[SerializeField] AudioSource _stepSoundSource = null;
	[SerializeField] TravelSoundPlayer _groundStepSounds = null;
	[SerializeField] TravelSoundPlayer _waterStepSounds = null;
	[SerializeField] TravelSoundPlayer _brushStepSounds = null;
	[ReadOnly]
	[SerializeField] TravelSoundPlayer _currentTravelSoundPlayer = null;

	Transform _transform = null;

	void Start()
	{
		_currentTravelSoundPlayer = _groundStepSounds;
		_transform = GetComponent<Transform>();
	}

	void OnTriggerEnter( Collider collider )
	{
		if ( collider.GetComponent<WaterVolume>() )
		{
			_currentTravelSoundPlayer = _waterStepSounds;
		}
		else if ( collider.GetComponent<BrushVolume>() )
		{
			_currentTravelSoundPlayer = _brushStepSounds;
		}
	}

	void OnTriggerExit( Collider collider )
	{
		if ( collider.GetComponent<WaterVolume>() || collider.GetComponent<BrushVolume>())
		{
			_currentTravelSoundPlayer = _groundStepSounds;
		}
	}

	// Called via the AnimationEventRedirector
	public void PlayStepSound()
	{
		_stepSoundSource.clip = _currentTravelSoundPlayer.GetRandomClip();
		SoundManager.Play3DSoundAtPosition( _stepSoundSource, _transform.position );
	}
}
