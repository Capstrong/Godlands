using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class EagleVision : MonoBehaviour 
{
	[SerializeField] Material _mat;

	void OnRenderImage ( RenderTexture source, RenderTexture destination )
	{	
		if( _mat )
		{
			Graphics.Blit( source, destination, _mat, 0 );
		}

//		THIS IS IN PROGRESS, DON'T BOTHER REVIEWING
//		if(CheckResources() == false) {
//			Graphics.Blit (source, destination);
//			return;
//		}
//		
//		var divider : int = resolution == Resolution.Low ? 4 : 2;
//		var widthMod : float = resolution == Resolution.Low ? 0.5f : 1.0f;
//		
//		fastBloomMaterial.SetVector ("_Parameter", Vector4 (blurSize * widthMod, 0.0f, threshhold, intensity));
//		source.filterMode = FilterMode.Bilinear;
//		
//		var rtW = source.width/divider;
//		var rtH = source.height/divider;
//		
//		// downsample
//		var rt : RenderTexture = RenderTexture.GetTemporary (rtW, rtH, 0, source.format);
//		rt.filterMode = FilterMode.Bilinear;
//		Graphics.Blit (source, rt, fastBloomMaterial, 1);
//		
//		var passOffs = blurType == BlurType.Standard ? 0 : 2;
//		
//		for(var i : int = 0; i < blurIterations; i++) {
//			fastBloomMaterial.SetVector ("_Parameter", Vector4 (blurSize * widthMod + (i*1.0f), 0.0f, threshhold, intensity));
//			
//			// vertical blur
//			var rt2 : RenderTexture = RenderTexture.GetTemporary (rtW, rtH, 0, source.format);
//			rt2.filterMode = FilterMode.Bilinear;
//			Graphics.Blit (rt, rt2, fastBloomMaterial, 2 + passOffs);
//			RenderTexture.ReleaseTemporary (rt);
//			rt = rt2;
//			
//			// horizontal blur
//			rt2 = RenderTexture.GetTemporary (rtW, rtH, 0, source.format);
//			rt2.filterMode = FilterMode.Bilinear;
//			Graphics.Blit (rt, rt2, fastBloomMaterial, 3 + passOffs);
//			RenderTexture.ReleaseTemporary (rt);
//			rt = rt2;
//		}
//		
//		fastBloomMaterial.SetTexture ("_Bloom", rt);
//		
//		Graphics.Blit (source, destination, fastBloomMaterial, 0);
//		
//		RenderTexture.ReleaseTemporary (rt);
	}	
}
