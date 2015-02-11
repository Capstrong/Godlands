using UnityEngine;
using System.Collections;

public class CuttingA : MonoBehaviour
{

	[SerializeField] LayerMask _cutableLayer = 0;
	[SerializeField] KeyCode _cuttingButton = (KeyCode) 0;
	[SerializeField] float _cuttingDistance = 0f;


	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
		if ( Input.GetKeyDown( _cuttingButton ) )
		{
			RaycastHit hitInfo;
			Physics.Raycast( new Ray( transform.position, transform.forward ), out hitInfo, _cuttingDistance, _cutableLayer );

			GameObject cutableObj = hitInfo.collider.gameObject;

			CutableA cutableComponent = cutableObj.GetComponent<CutableA>();

			if ( cutableComponent )
			{
				Cut( cutableComponent );
			}
			else
			{
				Debug.LogError( "Attach Cutable component to " + cutableObj + " at " + cutableObj.transform.position );
			}
		}
	}

	void Cut( CutableA cutableComponent )
	{

	}
}
