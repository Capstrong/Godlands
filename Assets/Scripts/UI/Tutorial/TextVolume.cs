using UnityEngine;
using System.Collections;

[RequireComponent( typeof( BoxCollider ) )]
public abstract class TextVolume : MonoBehaviour
{
	[ReadOnly]
	[SerializeField] bool _hasBeenTriggered = false;

	private TextBox _textBox = null;
	Collider _collider = null;

	void Awake()
	{
		_textBox = FindObjectOfType<TextBox>();
		_collider = GetComponent<Collider>();
	}

	void OnDrawGizmos()
	{
		Gizmos.color = Color.blue;
		Transform transform = GetComponent<Transform>();
		Gizmos.matrix = transform.localToWorldMatrix;

		BoxCollider collider = GetComponent<BoxCollider>();
		Gizmos.DrawWireCube( collider.center, collider.size );
	}

	public abstract void TriggerText( PlayerActor player );

	public void OnTriggerStay( Collider other )
	{
		TriggerText( other.gameObject.GetComponent<PlayerActor>() );
	}

	public void DisplayText( string text )
	{
		if ( !_hasBeenTriggered )
		{
			_textBox.SetText( text );
			_hasBeenTriggered = true;
			_collider.enabled = false;
		}
	}

	public void Reactivate()
	{
		_hasBeenTriggered = false;
		_collider.enabled = true;
	}
}
