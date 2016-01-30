using UnityEngine;
using System.Collections;

public class CenterSelectedTransform : MonoBehaviour
{
    public Transform SelectedTransform;
    public AnimationCurve SpeedWrtDistance;

    void Update()
    {
        if ( SelectedTransform == null )
        {
            return;
        }


    }
}
