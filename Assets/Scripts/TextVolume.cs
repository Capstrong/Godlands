using UnityEngine;
using System.Collections;

public class TextVolume : MonoBehaviour
{
	[SerializeField] string _text = "";
	public string text
	{
		get { return _text; }
	}

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
		Gizmos.matrix = Matrix4x4.TRS( transform.position, transform.rotation, transform.lossyScale );

		BoxCollider collider = GetComponent<BoxCollider>();
		Gizmos.DrawWireCube( collider.center, collider.size );
	}

	public void OnTriggerEnter( Collider other )
	{
		Debug.Log( "Text volume triggered by: " + other );

		if ( !_hasBeenTriggered )
		{
			if ( _fadeOut )
			{
				_textBox.SetTextForDuration( _text, _timeUntilFadeout, _fadeoutDuration );
			}
			else
			{
				_textBox.SetText( _text );
			}

			_hasBeenTriggered = _triggerOnce;
		}
	}
}
