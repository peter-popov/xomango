using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoreCZ
{
    /// <summary>
    /// 
    /// </summary>
    class PlayerException : Exception
    {
    }

    /// <summary>
    /// 
    /// </summary>
    public interface Player
    {
        void EnemyTurn(Position pos);

        Position MakeNextTurn();
    }
}
