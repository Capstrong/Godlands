using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class BuddyLimitsTag : MonoBehaviour
{
	void Update()
	{
#if UNITY_EDITOR
		DebugUtils.Assert( GetComponent<Collider>() is CapsuleCollider, "Buddy limits must use capsule collider." );
#endif // UNITY_EDITOR
	}
}
