using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InventoryScrollBar : MonoBehaviour 
{
	[SerializeField] Image prevItemIcon;
	[SerializeField] Image currentItemIcon;
	[SerializeField] Image nextItemIcon;

	int currentIndex = 0;

	public void UpdateInventoryBar(int currentIndex, ResourceData[] resourceData)
	{
		Debug.Log(currentIndex);
		SetIcon(currentItemIcon, resourceData[currentIndex].icon);
		// tell player to hold item

		int prevIndex = currentIndex - 1;
		if(prevIndex < 0)
		{
			prevIndex = resourceData.Length - 1;
		}
		SetIcon(prevItemIcon, resourceData[prevIndex].icon);

		int nextIndex = currentIndex + 1;
		if(nextIndex > resourceData.Length - 1)
		{
			nextIndex = 0;
		}
		SetIcon(nextItemIcon, resourceData[nextIndex].icon);
	}

	void SetIcon(Image image, Sprite icon)
	{
		if(icon)
		{
			image.sprite = icon;
			image.color = Color.white;	
		}
		else
		{
			image.color = Color.white * 0f;
		}
	}
}
