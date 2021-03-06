﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionFilter : ActionBehaviour
{
    // Args:
    // 1) The nodes to be tested (could be any registered node in the game).
    // Returns: bool -- Whether or not the node should be included in the post-filter set.
    public Func<Node, bool> Test;

    public SelectionFilter( 
        string descriptor, 
        Func<Node, bool> test = null
    )
    {
        GetsFinalWord = false;
        UsesSelection = false;
        UsesProjectiles = false;

        Descriptor = descriptor;
        Test = test;
    }
}
