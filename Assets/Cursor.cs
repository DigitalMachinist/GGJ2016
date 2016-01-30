using UnityEngine;

public class Cursor : MonoBehaviour
{
    public float Speed = 100f;
    public AnimationCurve SpeedWrtDistance;

    public Node TargetNode
    {
        get
        {
            var playerTurn = GameManager.Instance.PlayerTurn;
            return ( playerTurn != null )
                ? playerTurn.SelectedNode
                : null;
        }
    }

    void Update()
    {
        // If there is no target, remain still.
        if ( TargetNode == null )
        {
            return;
        }

        // Move toward the target transform at a rate defined by the current distance
        // and independent of ingame time scaling.
        var toTarget = TargetNode.transform.position - transform.position;
        var totalSpeed = Speed * SpeedWrtDistance.Evaluate( toTarget.magnitude );
        var translation = totalSpeed * toTarget.normalized * Time.unscaledDeltaTime;
        transform.Translate( translation );
    }
}
