using UnityEngine;
using System.Collections;

public class GameplayBackground : MonoBehaviour
{
    public void RandomMaterial()
    {
        var number = Random.Range( 1, 22 );
        var name = "Nullify" + ( ( number < 10 ) ? "0" : "" ) + number;
        GetComponent<Renderer>().material = Resources.Load<Material>( name );
        Debug.Log( "Gameplay background changed to " + name + "." );
    }
}
