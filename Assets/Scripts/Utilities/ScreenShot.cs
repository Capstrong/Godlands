using UnityEngine;
using System.Collections;
using System.IO;

[RequireComponent( typeof( Camera ) )]
public class ScreenShot : MonoBehaviour 
{
	[SerializeField] int resWidth = 1024;
	[SerializeField] int resHeight = 1024;

	[SerializeField] RenderTexture renderTex;

	private Camera _camera;

	void Awake()
	{
		_camera = GetComponent<Camera>();
	}

	public static string ScreenShotName(int width, int height)
	{
		return string.Format("Selfie_{0}x{1}_{2}.png", 
		                     width, height, 
		                     System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
	}
	
	public void TakeHiResShot() 
	{
	 	StartCoroutine( TakeSelfie());
	}

//	RenderTexture rt = new RenderTexture(resWidth, resHeight, 24);
//	camera.targetTexture = rt;
//	Texture2D screenShot = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false);
//	camera.Render();
//	RenderTexture.active = rt;
//	screenShot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
//	camera.targetTexture = null;
//	RenderTexture.active = null; // JC: added to avoid errors
//	Destroy(rt);
//	byte[] bytes = screenShot.EncodeToPNG();
//	string filename = ScreenShotName(resWidth, resHeight);
//	System.IO.File.WriteAllBytes(filename, bytes);
//	Debug.Log(string.Format("Took screenshot to: {0}", filename));
//	takeHiResShot = false;

	IEnumerator TakeSelfie()
	{
		_camera.enabled = true;

		yield return new WaitForEndOfFrame();

		renderTex = new RenderTexture(resWidth, resHeight, 32);
		_camera.targetTexture = renderTex;

		_camera.Render();
		RenderTexture.active = renderTex; // RenderTexture must be a static class

		Texture2D screenShot = new Texture2D(renderTex.width, renderTex.height, TextureFormat.RGB24, false);

		// this seems to read from the active RenderTexture
		screenShot.ReadPixels(new Rect(0f, 0f, renderTex.width, renderTex.height), 0, 0);
		screenShot.Apply();

		byte[] bytes = screenShot.EncodeToPNG();

		// nulling these avoids errors
		_camera.targetTexture = null;
		RenderTexture.active = null;

#if !UNITY_WEBPLAYER
	#if	UNITY_STANDALONE_OSX
			File.WriteAllBytes(Application.dataPath + "/../../" + ScreenShotName(renderTex.width, renderTex.height), bytes);
	#else
			File.WriteAllBytes(Application.dataPath + "/../" + ScreenShotName(renderTex.width, renderTex.height), bytes);
	#endif
#endif
		_camera.enabled = false;
	}
}