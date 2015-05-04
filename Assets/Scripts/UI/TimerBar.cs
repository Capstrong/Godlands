using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TimerBar : MonoBehaviour 
{
	[SerializeField] RectTransform _markerStart = null;
	[SerializeField] RectTransform _markerEnd = null;
	[SerializeField] RectTransform _timeMarkerTransform = null;

	void Update()
	{
		_timeMarkerTransform.anchoredPosition = Vector3.Lerp( _markerStart.anchoredPosition, 
		                                                      _markerEnd.anchoredPosition, 
		                                                      DayCycleManager.instance.GetNormalizedCurrentTime() );
	}
}
