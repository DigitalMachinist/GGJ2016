using System.Collections.Generic;

public class GrowAction : NodeAction
{
    protected override void Awake()
    {
        name = "Grow";
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
            new GrowBehaviour()
        };
        SelectionFilters = new List<SelectionFilter>();
    }

    public override string ToString()
    {
        return "Increase max HP, regen rate, action rate, energy rate, and gain access to another action.";
    }
}
