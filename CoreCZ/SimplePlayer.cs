using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CoreCZ.AI;

namespace CoreCZ
{
    public class SimplePlayer: Player
    {
        private GameState state = new GameState();
        private Stack<GameState.ChangeSet> undoList = new Stack<GameState.ChangeSet>();
        private HeuristicTrunsGenerator gen = new HeuristicTrunsGenerator(new LineBasedTurnHeuristics());
        private AI.MinMax.MinMax ai;


        public SimplePlayer(Side s)
        {
            this.Side = s;
            //ai = new AI.MinMax.MinMax(new HeuristicCostFunction(), this.gen);
            ai = new AI.MinMax.MinMax(new SimpleCostFuntcion(), this.gen);
        
        }

        public void Undo()
        {
            if (undoList.Count > 2)
            {
                state.Undo(undoList.Pop()); //enemy turn
                state.Undo(undoList.Pop()); //our turn
            }
        }

        public SimplePlayer(Side s, Board b):this(s)
        {
            foreach (Turn turn in b.Turns)
            {
                undoList.Push(state.Advance(turn.position, turn.side));
            }
        }

        public Side Side { get; set; }

        public void EnemyTurn(Position pos)
        {
            undoList.Push(state.Advance(pos, Utils.FlipSide(Side)));
        }

        public Position MakeNextTurn()
        {
            Position pos = new Position(4, 6);

            if (state.Count() > 0)
            {
                pos = ai.FindTurn(state);                                
            }

            undoList.Push(state.Advance(pos, Side));

            return pos;
        }
    }
}
