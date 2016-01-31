using System.Collections.Generic;

public class CreateNodeAction : NodeAction
{
    protected override void Awake()
    {
        name = "Create Node";
        WarmupDelay = 0f;
        Duration = 5f;
        CooldownDelay = 0f;
        Angle = 0f;
        EnergyCost = 100;
        HPCost = 0;

        CollisionDeflectBehaviours = new List<CollisionBehaviour>();
        CollisionImpactBehaviours = new List<CollisionBehaviour>();
        NodeBehaviours = new List<NodeBehaviour>()
        {
            new CreateNodeBehaviour()
        };
        SelectionFilters = new List<SelectionFilter>();
    }

    public override string ToString()
    {
        return "Create a new level 0 node at a fixed distance this one.";
    }
}
