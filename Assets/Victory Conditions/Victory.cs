using System.Collections;
using UnityEngine;

public abstract class Victory : MonoBehaviour
{
    public bool IsEnabled;
    public string Name;
    [TextArea()]
    public string Description;

    public abstract Player Test( ICollection players, ICollection nodes );
}
