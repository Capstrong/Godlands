using UnityEngine;
using System.Collections;

public class MidnightVolume : MonoBehaviour
{
	void OnTriggerEnter()
	{
		DayCycleManager.TriggerMidnight();
	}
}
