using UnityEngine;
using System.Collections;

public abstract class TextVolume : MonoBehaviour
{
	[SerializeField] bool _fadeOut = true;
	[SerializeField] float _timeUntilFadeout = 7.0f;
	[SerializeField] float _fadeoutDuration = 2.0f;

	[SerializeField] bool _triggerOnce = true;

	[ReadOnly]
	[SerializeField] bool _hasBeenTriggered = false;

	private TextBox _textBox;

	void Awake()
	{
		_textBox = FindObjectOfType<TextBox>();
	}

	void OnDrawGizmos()
	{
		Gizmos.color = Color.blue;
		Transform transform = GetComponent<Transform>();
		Gizmos.matrix = transform.localToWorldMatrix;

		BoxCollider collider = GetComponent<BoxCollider>();
		Gizmos.DrawWireCube( collider.center, collider.size );
	}

	public abstract void TriggerText();

	public void OnTriggerStay( Collider other )
	{
		TriggerText();
	}

	public void DisplayText( string text )
	{
		if ( !_hasBeenTriggered )
		{
			if ( _fadeOut )
			{
				_textBox.SetTextForDuration( text, _timeUntilFadeout, _fadeoutDuration );
			}
			else
			{
				_textBox.SetText( text );
			}

			_hasBeenTriggered = _triggerOnce;
		}
	}
}
