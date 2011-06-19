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
        private State previousState;
        private HeuristicTrunsGenerator gen = new HeuristicTrunsGenerator(new LineBasedTurnHeuristics());
        //private AI.MinMax.MinMax ai;


        public SimplePlayer(Side s)
        {
            this.Side = s;
            //ai = new AI.MinMax.MinMax(new HeuristicCostFunction(), this.gen);
        }

        public void Undo()
        {
            currentState = previousState;
        }

        public SimplePlayer(Side s, Board b)
        {
            this.Side = s;
            foreach (Turn turn in b.Turns)
            {
                if (currentState == null)
                {
                    currentState = new State(turn.position, turn.side);
                }
                else
                {                    
                    currentState = currentState.DeriveState(turn.position);
                }
            }
        }

        public Side Side { get; set; }

        public void EnemyTurn(Position pos)
        {
            previousState = currentState;
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
