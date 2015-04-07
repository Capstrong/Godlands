using UnityEngine;
using System.Collections;

public class ShaderGlobalsManager : MonoBehaviour 
{
	abstract public class GlobalProperty
	{
		public string name = "_Property";
	}

	[System.Serializable]
	public class GlobalFloat : GlobalProperty
	{
		public float value = 0f;
	}

	[System.Serializable]
	public class GlobalTexture : GlobalProperty
	{
		public Texture2D value = null;
	}

	[System.Serializable]
	public class GlobalColor : GlobalProperty
	{
		public Color value = Color.white;
	}

	[System.Serializable]
	public class GlobalVector : GlobalProperty
	{
		public Vector4 value = Vector4.zero;
	}

	[SerializeField] GlobalFloat[] _globalFloatProperties = null;
	[SerializeField] GlobalTexture[] _globalTextureProperties = null;
	[SerializeField] GlobalColor[] _globalColorProperties = null;
	[SerializeField] GlobalVector[] _globalVectorProperties = null;

	void Awake()
	{
		Shader.WarmupAllShaders();

		UpdateProperties();
	}

	public void UpdateProperties()
	{
		foreach( GlobalFloat globalFloat in _globalFloatProperties )
		{
			Shader.SetGlobalFloat( "_" + globalFloat.name, globalFloat.value );
		}
		
		foreach( GlobalTexture globalTexture in _globalTextureProperties )
		{
			Shader.SetGlobalTexture( "_" + globalTexture.name, globalTexture.value );
		}
		
		foreach( GlobalColor globalColor in _globalColorProperties )
		{
			Shader.SetGlobalColor( "_" + globalColor.name, globalColor.value );
		}
		
		foreach( GlobalVector globalVector in _globalVectorProperties )
		{
			Shader.SetGlobalVector( "_" + globalVector.name, globalVector.value );
		}
	}
}
