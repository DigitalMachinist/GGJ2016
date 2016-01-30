using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DominationVictory : MonoBehaviour
{
    public Player Test( IEnumerable<Player> players, IEnumerable<Node> nodes )
    {
        foreach ( var player in players )
        {
            var hasPlayerWon = nodes.All( node => node.Player == player );
            if ( hasPlayerWon )
            {
                return player;
            }
        }

        return null;
    }
}
