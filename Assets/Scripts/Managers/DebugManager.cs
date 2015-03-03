using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DebugManager : SingletonBehaviour<DebugManager> 
{
	public class DebugGizmo
	{
		public Color color;
		public string registryName;
	}

	public class ShapeGizmo : DebugGizmo
	{
		public ShapeGizmo(string inRegistryName, ShapeType inShapeType, Vector3 inPosition, float inSize, Color inColor)
		{
			registryName = inRegistryName;
			shapeType = inShapeType;
			position = inPosition;
			size = inSize;
			color = inColor;
		}

		public enum ShapeType
		{
			cube,
			wirecube,
			sphere,
			wiresphere
		}

		public ShapeType shapeType;
		public Vector3 position;
		public float size;
	}

	public class LineGizmo : DebugGizmo
	{
		public LineGizmo(string inRegistryName, Vector3 inFrom, Vector3 inTo, Color inColor)
		{
			registryName = inRegistryName;
			from = inFrom;
			to = inTo;
			color = inColor;
		}

		public Vector3 from;
		public Vector3 to;
	}

	List<DebugGizmo> activeDebugGizmos = new List<DebugGizmo>();

	public void RegisterDebugDraw(string debugName, ShapeGizmo.ShapeType shapeType, Vector3 position, float size, Color color)
	{
		foreach(DebugGizmo dg in activeDebugGizmos)
		{
			if(dg.registryName == debugName)
			{
				Debug.Log("Duplicate Debug Draw: " + debugName);
				return;
			}
		}

		activeDebugGizmos.Add(new ShapeGizmo(debugName, shapeType, position, size, color));
	}

	public void RegisterDebugDraw(string debugName, Vector3 from, Vector3 to, Color color)
	{
		foreach(DebugGizmo dg in activeDebugGizmos)
		{
			if(dg.registryName == debugName)
			{
				Debug.Log("Duplicate Debug Draw: " + debugName);
				return;
			}
		}

		activeDebugGizmos.Add(new LineGizmo(debugName, from, to, color));
	}

	public void DeregisterDebugDraw(string registryName)
	{
		foreach(DebugGizmo dg in activeDebugGizmos)
		{
			if(dg.registryName == registryName)
			{
				activeDebugGizmos.Remove(dg);
				return;
			}
		}
	}

	void Update()
	{

	}

	void OnDrawGizmos()
	{
		// look through all the registered gizmos of each type and draw them
		foreach(DebugGizmo dg in activeDebugGizmos)
		{
			Gizmos.color = dg.color;

			if(dg.GetType() == typeof(ShapeGizmo))
			{
				ShapeGizmo sg = (ShapeGizmo)dg;

				switch(sg.shapeType)
				{
				case ShapeGizmo.ShapeType.cube:
					Gizmos.DrawCube(sg.position, Vector3.one * sg.size);
					break;
				case ShapeGizmo.ShapeType.wirecube:
					Gizmos.DrawWireCube(sg.position, Vector3.one * sg.size);
					break;
				case ShapeGizmo.ShapeType.sphere:
					Gizmos.DrawSphere(sg.position, sg.size);
					break;
				case ShapeGizmo.ShapeType.wiresphere:
					Gizmos.DrawWireSphere(sg.position, sg.size);
					break;
				}
			}

			if(dg.GetType() == typeof(LineGizmo))
			{
				LineGizmo lg = (LineGizmo)dg;
				Gizmos.color = lg.color;
				Gizmos.DrawLine(lg.from, lg.to);
			}
		}
	}
}
