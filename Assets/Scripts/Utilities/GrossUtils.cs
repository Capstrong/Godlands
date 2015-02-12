using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class GrossUtils
{
	// Tries to load the filepath as the given type
	// Returns false and logs an error if the file could not be loaded. Returns true otherwise
	// Example use: 
	// file structure: Assets/Resources/Images/image.png 
	// function call: TryToLoad("Images/image") would return true
	public static bool TryToLoad<T>( string fullFilename, bool displayError = true )
	                               where T : UnityEngine.Object
	{
		T text = Resources.Load<T>( fullFilename );

		if ( text == null )
		{
			if ( displayError )
			{
				Debug.LogError( fullFilename + " could not be loaded as a " + typeof( T ).Name );
			}
			return false;
		}
		else
		{
			return true;
		}
	}
}

// You must create a subclass of this class with types specified for the templated parameters
// in order for it to actually serialize e.g.:
// [Serializable]
// public class fooDictionary : SerializableDictionary<int,Foo> { }
[Serializable]
public abstract class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
                                                    where TKey : new()
                                                    where TValue : new()
{
	[SerializeField]
	private List<TKey> keys = new List<TKey>();

	[SerializeField]
	private List<TValue> values = new List<TValue>();

	// save the dictionary to lists
	public void OnBeforeSerialize()
	{
		keys.Clear();
		values.Clear();
		foreach ( KeyValuePair<TKey, TValue> pair in this )
		{
			keys.Add( pair.Key );
			values.Add( pair.Value );
		}
	}

	// load dictionary from lists
	public void OnAfterDeserialize()
	{
		this.Clear();

		// Probably adding an item in the inspector
		if ( keys.Count + 1 == values.Count )
		{
			if ( keys.Count == 0 )
			{
				keys.Add( new TKey() );
			}
			else
			{
				keys.Add( keys[keys.Count - 1] ); // duplicate the last key
			}
		}

		// Probably adding an item in the inspector
		if ( values.Count + 1 == keys.Count )
		{
			if ( values.Count == 0 )
			{
				values.Add( new TValue() );
			}
			else
			{
				values.Add( values[values.Count - 1] ); // duplicate the value key
			}
		}

		if ( keys.Count != values.Count )
		{
			throw new System.Exception( string.Format( "there are {0} keys and {1} values after deserialization. Make sure that both key and value types are serializable.", keys.Count, values.Count ) );
		}

		for ( int i = 0; i < keys.Count; i++ )
		{
			this.Add( keys[i], values[i] );
		}
	}
}

// You must create a subclass of this class with types specified for the templated parameters
// in order for it to actually serialize e.g.:
// [Serializable]
// public class fooDictionary : SerializableDictionary<int,Foo> { }
[Serializable]
public abstract class SerializableHashSet<TValue> : HashSet<TValue>, ISerializationCallbackReceiver
{
	[SerializeField]
	private List<TValue> values = new List<TValue>();

	// save the dictionary to lists
	public void OnBeforeSerialize()
	{
		values.Clear();
		foreach ( TValue val in this )
		{
			values.Add( val );
		}
	}

	// load dictionary from lists
	public void OnAfterDeserialize()
	{
		this.Clear();

		foreach ( TValue val in values )
		{
			this.Add( val );
		}
	}
}