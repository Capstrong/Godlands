﻿using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;

public class CheckpointLifter : MonoBehaviour 
{
	[SerializeField] float _moveSpeed = 5f;

	[ReadOnly]
	[SerializeField] Vector3 _initPos = Vector3.zero;
	[SerializeField] float _maxHeightOffset = 25f;

	[SerializeField] bool _isActive = false;
	public bool isActive
	{
		get { return _isActive; }
	}

	[ReadOnly]
	[SerializeField] bool _isRising = false;

	Renderer _renderer = null;
	Transform _transform = null;

	void Awake()
	{
		_transform = GetComponent<Transform>();

		_initPos = _transform.position;

		// Start the lifter at the top of its range
		_transform.position = _transform.position.SetY( _initPos.y + _maxHeightOffset );

		_renderer = GetComponentInChildren<Renderer>();
		_renderer.material.SetColor( "_EmissionColor", Color.black);
	}

	void FixedUpdate()
	{
		if( _isActive )
		{
			if( _isRising )
			{
				// If below maximum height
				if( _transform.position.y < _initPos.y + _maxHeightOffset )
				{
					_transform.position += Vector3.up * _moveSpeed * Time.deltaTime;
				}
				else
				{
					_transform.position = _initPos + Vector3.up * _maxHeightOffset;
				}
			}
			else
			{
				// If above minimum height
				if( _transform.position.y > _initPos.y )
				{
					_transform.position -= Vector3.up * _moveSpeed * Time.deltaTime;
				}
				else
				{
					_transform.position = _initPos;
				}
			}
		}
	}

	public void Activate()
	{
		_isActive = true;
		_renderer.material.SetColor( "_EmissionColor", Color.white );
	}

	public void Stay()
	{
		_isRising = true;
	}

	public void Exit()
	{
		_isRising = false;
	}

	void OnDrawGizmos()
	{
		// Need to set _initPos while out of playmode to see gizmo
		if( !Application.isPlaying )
		{
			_initPos = transform.position;
		}

		// Calculate gizmo transform
		Vector3 matScale = transform.localScale * 2f;
		matScale.y = 1f;

		Vector3 matRot = Vector3.zero;
		matRot.y = transform.eulerAngles.y;

		Gizmos.matrix = Matrix4x4.TRS( _initPos, Quaternion.Euler( matRot ) , matScale );

		// Average of bottom and top positions
		Vector3 halfwayPoint = (Vector3.up * _maxHeightOffset)/2f;

		Gizmos.color = Color.white - new Color( 0f, 0f, 0f, 0.7f );
		Gizmos.DrawCube( halfwayPoint, new Vector3( 1f, _maxHeightOffset, 1f ) );

		Gizmos.color = Color.white - new Color( 0f, 0f, 0f, 0.3f );
		Gizmos.DrawWireCube( halfwayPoint, new Vector3( 1f, _maxHeightOffset, 1f ) );
	}
}
