using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InventoryArrow : MonoBehaviour
{
	[SerializeField] Sprite _normalSprite;
	[SerializeField] Sprite _highlightSprite;

	Image _image;

	// Use this for initialization
	void Start ()
	{
		_image = GetComponent<Image>();
	}
	
	public void Highlight()
	{
		_image.sprite = _highlightSprite;
	}

	public void Normal()
	{
		_image.sprite = _normalSprite;
	}
}
