using UnityEngine;

public class CenterSelectedTransform : MonoBehaviour
{
    public float Speed = 1f;
    public AnimationCurve SpeedWrtDistance;

    GameManager GM;

    public Node TargetNode
    {
        get { return GM.PlayerTurn.SelectedNode; }
    }

    void Start()
    {
        GM = FindObjectOfType<GameManager>();
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
