using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CoreCZ.AI;

namespace CoreCZ
{
    public class SimplePlayer: Player
    {
        private State currentState;
        private HeuristicTrunsGenerator gen = new HeuristicTrunsGenerator(new LineBasedTurnHeuristics());
        //private AI.MinMax.MinMax ai;


        public SimplePlayer(Side s)
        {
            this.Side = s;
            //ai = new AI.MinMax.MinMax(new HeuristicCostFunction(), this.gen);
        }

        public Side Side { get; set; }

        public void EnemyTurn(Position pos)
        {
            if (currentState == null)
            {
                currentState = new State(pos, Utils.FlipSide(Side));
            }
            else
            {
                currentState = currentState.DeriveState(pos);
            }
        }

        public Position MakeNextTurn()
        {
            Position pos = new Position(4, 6);
            
            if (currentState == null)
            {
                currentState = new State(pos, Utils.FlipSide(Side));
                return pos;
            }
            else
            {
                //pos = ai.FindTurn(currentState);
                pos = gen.GetBestTurn(currentState);                
                currentState = currentState.DeriveState(pos);
            }
            return pos;
        }
    }
}
