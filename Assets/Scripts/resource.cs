using UnityEngine;
using System.Collections;

public enum ResourceType
{
    Invalid,
    Apple,
    Joy,
    Creativity,
}

public class resource : MonoBehaviour {

    public ResourceType type;
    public int amount = 1;
}
