using UnityEngine;
using System.Collections.Generic;

public class Node : MonoBehaviour
{
    public Player Player;
    public int HP;
    public int Level;
    public NodeAction SelectedAction;
    public List<NodeAction> Actions;

    public ColorSampler Sampler { get; private set; }

    public Color Colour
    {
        get { return Sampler.SampledColor; }
    }
    public bool IsSelected
    {
        get { return ( Player == null ) ? false : ( Player.SelectedNode == this ); }
    }

    void Awake()
    {
        Sampler = GetComponent<ColorSampler>();
    }
}
