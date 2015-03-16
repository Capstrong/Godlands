#define USINGMOUSECONTROLS

using UnityEngine;
using System.Collections;
using System.Reflection;
using System;

public static class WadeUtils
{
	public static float SMALLNUMBER = 0.00000001f;
	public static float LARGENUMBER = 100000000f;

	public static float DUALINPUTMOD = 0.7071f;

	///////////////////////
	////	FLOATS	 /////
	//////////////////////

	public static bool AreEqual( float a, float b )
	{
		return Mathf.Abs( a - b ) < SMALLNUMBER;
	}

	public static bool IsZero( float num )
	{
		return Mathf.Abs( num ) < SMALLNUMBER;
	}

	public static bool IsNotZero( float num )
	{
		return Mathf.Abs( num ) > SMALLNUMBER;
	}

	public static bool IsPositive( float num )
	{
		return num > SMALLNUMBER;
	}

	public static bool IsNegative( float num )
	{
		return num < -SMALLNUMBER;
	}

	public static void Clamp(ref float value, float min, float max)
	{
		value = Mathf.Clamp (value, min, max);
	}
	
	public static void Lerp(ref float from, float to, float t)
	{
		from = Mathf.Lerp (from, to, t);
	}
	
	///////////////////////
	////	VECTORS	 /////
	//////////////////////

	public static bool IsZero(this Vector2 vec)
	{
		return vec.sqrMagnitude < SMALLNUMBER;
	}

	public static bool IsZero(this Vector3 vec)
	{
		return vec.sqrMagnitude < SMALLNUMBER;
	}

	public static bool IsZero(this Vector4 vec)
	{
		return vec.sqrMagnitude < SMALLNUMBER;
	}

	public static void Lerp(ref Vector2 from, Vector2 to, float t)
	{
		from = Vector2.Lerp(from, to, t);
	}
	
	public static void Lerp(ref Vector3 from, Vector3 to, float t)
	{
		from = Vector3.Lerp(from, to, t);
	}

	public static void Lerp(ref Vector4 from, Vector4 to, float t)
	{
		from = Vector4.Lerp(from, to, t);
	}

	public static float DistanceTo(this Vector2 pointA, Vector4 pointB)
	{
		return ((Vector4)pointA).DistanceTo(pointB);
	}

	public static float DistanceTo(this Vector3 pointA, Vector4 pointB)
	{
		return ((Vector4)pointA).DistanceTo(pointB);
	}
	
	public static float DistanceTo(this Vector4 pointA, Vector4 pointB)
	{
		return Vector4.Distance (pointA, pointB);
	}

	public static void Slerp(ref Vector3 from, Vector3 to, float t)
	{
		from = Vector3.Slerp(from, to, t);
	}

	////////////////////////////
	////	QUATERNIONS	////
	///////////////////////////

	public static void SetRotationX(ref Quaternion rotation, float angle)
	{
		Vector3 eulers = rotation.eulerAngles;
		eulers.x = angle;
		
		rotation.eulerAngles = eulers;
	}
	
	public static void SetRotationY(ref Quaternion rotation, float angle)
	{
		Vector3 eulers = rotation.eulerAngles;
		eulers.y = angle;
		
		rotation.eulerAngles = eulers;
	}

	public static void SetRotationZ(ref Quaternion rotation, float angle)
	{
		Vector3 eulers = rotation.eulerAngles;
		eulers.z = angle;
		
		rotation.eulerAngles = eulers;
	}

	public static void Lerp(ref Quaternion from, Quaternion to, float t)
	{
		from = Quaternion.Lerp (from, to, t);
	}

	public static void Slerp(ref Quaternion from, Quaternion to, float t)
	{
		from = Quaternion.Slerp (from, to, t);
	}
	
	////////////////////////////
	////	TRANSFORM	////
	///////////////////////////

	public static void SetPositionX(this Transform transform, float x)
	{
		transform.position = new Vector3 (x, transform.position.y, transform.position.z);
	}

	public static void SetPositionY(this Transform transform, float y)
	{
		transform.position = new Vector3 (transform.position.x, y, transform.position.z);
	}

	public static void SetPositionZ(this Transform transform, float z)
	{
		transform.position = new Vector3 (transform.position.x, transform.position.y, z);
	}

	public static void SetPosition(this Transform transform, float x, float y, float z)
	{
		transform.position = new Vector3 (x, y, z);
	}

	public static void SetPosition(this Transform transform, Vector3 vec)
	{
		transform.position = vec;
	}

	public static void AddPosition(this Transform transform, float x, float y, float z)
	{
		transform.position += new Vector3(x, y, z);
	}

	public static void AddPosition(this Transform transform, Vector3 offset)
	{
		transform.position += offset;
	}

	public static void SetRotationX(this Transform transform, float x)
	{
		transform.rotation *= Quaternion.Euler(x, transform.position.y, transform.position.z);
	}

	public static void SetRotationY(this Transform transform, float y)
	{
		transform.rotation *= Quaternion.Euler(transform.position.x, y, transform.position.z);
	}

	public static void SetRotationZ(this Transform transform, float z)
	{
		transform.rotation *= Quaternion.Euler(transform.position.x, transform.position.y, z);
	}

	public static void AddRotation(this Transform transform, float x, float y, float z)
	{
		transform.rotation *= Quaternion.Euler(new Vector3(x, y, z));
	}

	public static void AddRotation(this Transform transform, Vector3 offset)
	{
		transform.rotation *= Quaternion.Euler(offset);
	}

	public static void SetScaleX(this Transform transform, float x)
	{
		transform.localScale = new Vector3(x, transform.position.y, transform.position.z);
	}

	public static void SetScaleY(this Transform transform, float y)
	{
		transform.localScale = new Vector3(transform.position.x, y, transform.position.z);
	}

	public static void SetScaleZ(this Transform transform, float z)
	{
		transform.localScale = new Vector3(transform.position.x, transform.position.y, z);
	}

	public static void SetScale(this Transform transform, float x, float y, float z)
	{
		transform.localScale = new Vector3(x, y, z);
	}

	public static void SetScale(this Transform transform, Vector3 vec)
	{
		transform.localScale = vec;
	}

	public static void AddScale(this Transform transform, float x, float y, float z)
	{
		transform.localScale += new Vector3(x, y, z);
	}

	public static void AddScale(this Transform transform, Vector3 offset)
	{
		transform.localScale += offset;
	}
	
	public static void LerpLookAt(this Transform transform, Transform target, float t)
	{
		Quaternion currentRot = transform.rotation;
		transform.LookAt (target);
		Quaternion lookRot = transform.rotation;
		
		transform.rotation = Quaternion.Lerp (currentRot, lookRot, t);
	}

	public static void LerpLookAt(this Transform transform, Vector3 target, float t)
	{
		Quaternion currentRot = transform.rotation;
		transform.LookAt (target);
		Quaternion lookRot = transform.rotation;
		
		transform.rotation = Quaternion.Lerp (currentRot, lookRot, t);
	}

	public static Vector3 SetVectorRelative(this Transform transform, Vector3 vec)
	{
		return transform.TransformDirection (vec);
	}

	public static Vector3 SetVectorRelative(this Transform transform, float x, float y, float z)
	{
		return transform.TransformDirection (x, y, z);
	}

	public static void ResetTransform(this Transform transform, bool keepScale = false)
	{
		transform.localPosition = Vector3.zero;
		transform.localRotation = Quaternion.identity;

		if(!keepScale)
		{
			transform.localScale = Vector3.one;
		}
	}
	
	///////////////////////////
	///////  PHYSICS  /////////
	///////////////////////////

	public static RaycastHit RaycastAndGetInfo(Ray ray, LayerMask layer, float dist = Mathf.Infinity)
	{
		return RaycastAndGetInfo (ray.origin, ray.direction, layer, dist);
	}
	
	public static RaycastHit RaycastAndGetInfo(Vector3 origin, Vector3 dir, LayerMask layer, float dist = Mathf.Infinity)
	{
		RaycastHit hit;
		Physics.Raycast(origin, dir, out hit, dist, layer);
		return hit;
	}

	public static RaycastHit RaycastAndGetInfo( Ray ray, float dist = Mathf.Infinity)
	{
		return RaycastAndGetInfo (ray.origin, ray.direction, dist);
	}

	public static RaycastHit RaycastAndGetInfo(Vector3 origin, Vector3 dir, float dist = Mathf.Infinity)
	{
		RaycastHit hit;
		Physics.Raycast (origin, dir, out hit, dist);
		return hit;
	}
	
	///////////////////////////////
	/////	COROUTINES	///////
	///////////////////////////////
	
	public static YieldInstruction Wait(float time)
	{
		return new WaitForSeconds (time);
	}
	
	///////////////////////////////
	/////	BIT OPERATIONS  ///////
	///////////////////////////////

	public static bool CheckBit(int bit, int shouldContainBit)
	{
		return (bit & shouldContainBit) == bit;
	}

	///////////////////////////////
	/////	GAMEOBJECTS  	///////
	///////////////////////////////

	public static GameObject Instantiate(GameObject obj)
	{
		return (GameObject)MonoBehaviour.Instantiate (obj);
	}
	
	public static GameObject Instantiate(GameObject obj, Vector3 pos, Quaternion rot)
	{
		return (GameObject)MonoBehaviour.Instantiate (obj, pos, rot);
	}

	public static GameObject TempInstantiate(GameObject obj, float time)
	{
		GameObject go = (GameObject)MonoBehaviour.Instantiate (obj);
		MonoBehaviour.Destroy(go, time);
		return go;
	}

	public static GameObject TempInstantiate(GameObject obj, Vector3 pos, Quaternion rot, float time)
	{
		GameObject go = (GameObject)MonoBehaviour.Instantiate (obj, pos, rot);
		MonoBehaviour.Destroy(go, time);
		return go;
	}

	public static void MakeCopyOf<T>(this Component comp, T other) where T : Component
	{
		Type type = comp.GetType();
		if (type != other.GetType()) 
		{
			return;
		}
		
		BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Default | BindingFlags.DeclaredOnly;
		PropertyInfo[] pinfos = type.GetProperties(flags);
		int counter = 0;
		foreach (PropertyInfo pinfo in pinfos) 
		{
			if (pinfo.CanWrite && 
			    (type != typeof(AudioSource) || counter < 21)) // horrible hacked in solution to unsuppressible error caused by unity's shitty source
			{
				pinfo.SetValue(comp, pinfo.GetValue(other, null), null);
				counter++;
			}
		}
		
		FieldInfo[] finfos = type.GetFields(flags);
		foreach (FieldInfo finfo in finfos) 
		{
			finfo.SetValue(comp, finfo.GetValue(other));
		}
	}

	public static T GetCopyOf<T>(this Component comp, T other) where T : Component
	{
		Type type = comp.GetType();
		if (type != other.GetType()) 
		{
			return null;
		}

		BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Default | BindingFlags.DeclaredOnly;
		PropertyInfo[] pinfos = type.GetProperties(flags);
		int counter = 0;
		foreach (PropertyInfo pinfo in pinfos) 
		{
			if (pinfo.CanWrite && 
			    (type != typeof(AudioSource) || counter < 21)) // horrible hacked in solution to unsuppressible error caused by unity's shitty source
			{
				pinfo.SetValue(comp, pinfo.GetValue(other, null), null);
				counter++;
			}
		}

		FieldInfo[] finfos = type.GetFields(flags);
		foreach (FieldInfo finfo in finfos) 
		{
			finfo.SetValue(comp, finfo.GetValue(other));
		}

		return comp as T;
	}

	/////////////////////////////////
	//////  COLORS           ////////
	/////////////////////////////////

	public static HSVColor RGBToHSV( Color color )
	{
		HSVColor ret = new HSVColor( 0f, 0f, 0f, color.a );
		
		float r = color.r;
		float g = color.g;
		float v = color.b;
		
		float max = Mathf.Max( r, Mathf.Max(g, v) );
		
		if ( max <= 0f )
		{
			return ret;
		}
		
		float min = Mathf.Min( r, Mathf.Min(g, v) );
		float dif = max - min;
		
		if ( max > min )
		{
			if ( WadeUtils.AreEqual( g, max ) )
			{
				ret.h = (v - r) / dif * 60f + 120f;
			}
			else if ( WadeUtils.AreEqual( v, max ) )
			{
				ret.h = (r - g) / dif * 60f + 240f;
			}
			else if ( v > g )
			{
				ret.h = (g - v) / dif * 60f + 360f;
			}
			else
			{
				ret.h = (g - v) / dif * 60f;
			}
			
			if ( WadeUtils.IsNegative( ret.h ) )
			{
				ret.h = ret.h + 360f;
			}
		}
		else
		{
			ret.h = 0f;
		}
		
		ret.h *= 1f / 360f;
		ret.s = (dif / max) * 1f;
		ret.v = max;
		
		return ret;
	}

	public static Color HSVToRGB( HSVColor hsbColor )
	{
		float r = hsbColor.v;
		float g = hsbColor.v;
		float b = hsbColor.v;
		
		if ( WadeUtils.IsNotZero( hsbColor.s ) )
		{
			float max = hsbColor.v;
			float dif = hsbColor.v * hsbColor.s;
			float min = hsbColor.v - dif;
			
			float h = hsbColor.h * 360f;
			
			if ( h < 60f )
			{
				r = max;
				g = h * dif / 60f + min;
				b = min;
			}
			else if ( h < 120f )
			{
				r = -(h - 120f) * dif / 60f + min;
				g = max;
				b = min;
			}
			else if ( h < 180f )
			{
				r = min;
				g = max;
				b = (h - 120f) * dif / 60f + min;
			}
			else if ( h < 240f )
			{
				r = min;
				g = -( h - 240f ) * dif / 60f + min;
				b = max;
			}
			else if ( h < 300f )
			{
				r = (h - 240f) * dif / 60f + min;
				g = min;
				b = max;
			}
			else if ( h <= 360f )
			{
				r = max;
				g = min;
				b = -(h - 360f) * dif / 60f + min;
			}
			else
			{
				r = 0f;
				g = 0f;
				b = 0f;
			}
		}
		
		return new Color( Mathf.Clamp01(r), Mathf.Clamp01(g), Mathf.Clamp01(b), hsbColor.a );
	}

	/////////////////////////////////
	//////  FRAME CHECKING   ////////
	/////////////////////////////////

	public static bool IsWithinFrame(float currentFrame, int targetFrame) // this seems so jank
	{
		return Mathf.Abs(currentFrame - targetFrame) < 1f;
	}
}

[System.Serializable]
public struct MinMaxF
{
	public MinMaxF(float _min, float _max)
	{
		min = _min;
		max = _max;
	}

	public float min;
	public float max;

	public float Range
	{
		get { return max - min; }
	}

	public void Clamp( ref float v )
	{
		v = Mathf.Clamp( v, min, max );
	}

	public float Random
	{
		get { return UnityEngine.Random.Range( min, max ); }
	}

	public bool IsOutside( float v )
	{
		return ( v < min || max < v );
	}
}

[System.Serializable]
public struct MinMaxI
{
	public MinMaxI(int _min, int _max)
	{
		min = _min;
		max = _max;
	}
	
	public int min;
	public int max;

	public int Range
	{
		get { return max - min; }
	}

	public void Clamp( ref int v )
	{
		v = Mathf.Clamp( v, min, max );
	}

	public int Random
	{
		get { return UnityEngine.Random.Range( min, max ); }
	}

	public bool IsOutside( int v )
	{
		return ( v < min || max < v );
	}
}

// Credit to Unity Wiki for original version of this
[System.Serializable]
public struct HSVColor
{
	public float h;
	public float s;
	public float v;
	public float a;
	
	public HSVColor( float h, float s, float v, float a = 1f)
	{
		this.h = h;
		this.s = s;
		this.v = v;
		this.a = a;
	}
	
	public HSVColor( Color col )
	{
		HSVColor temp = WadeUtils.RGBToHSV( col );
		h = temp.h;
		s = temp.s;
		v = temp.v;
		a = temp.a;
	}
	
	public Color HSVToRGB()
	{
		return WadeUtils.HSVToRGB(this);
	}
	
	public override string ToString()
	{
		return "H:" + h + " S:" + s + " V:" + v;
	}
	
	public static HSVColor Lerp( HSVColor a, HSVColor b, float t )
	{
		float h,s;
		
		//check special case black (color.v==0): interpolate neither hue nor saturation!
		//check special case grey (color.s==0): don't interpolate hue!
		if( WadeUtils.AreEqual( a.v, 0f ) )
		{
			h = b.h;
			s = b.s;
		}
		else if( WadeUtils.AreEqual( b.v, 0f ))
		{
			h = a.h;
			s = a.s;
		}
		else
		{
			if( WadeUtils.AreEqual( a.s, 0f ) )
			{
				h = b.h;
			}
			else if( WadeUtils.AreEqual( b.s, 0f ) )
			{
				h = a.h;
			}
			else
			{
				// works around bug with LerpAngle
				float angle = Mathf.LerpAngle( a.h * 360f, b.h * 360f, t );
				while (angle < 0f)
				{
					angle += 360f;
				}
				while (angle > 360f)
				{
					angle -= 360f;
				}

				h = angle/360f;
			}

			s = Mathf.Lerp( a.s, b.s, t );
		}

		return new HSVColor( h, s, Mathf.Lerp( a.v, b.v, t ), Mathf.Lerp( a.a, b.a, t ) );
	}
}