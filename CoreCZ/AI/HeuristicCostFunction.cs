using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoreCZ.AI
{
    class HeuristicCostFunction : MinMax.ICostFunction<GameState>
    {
        TurnHeuristics hCross = new LineBasedTurnHeuristics();
        TurnHeuristics hZero = new LineBasedTurnHeuristics();

        HeuristicTrunsGenerator genCross;
        HeuristicTrunsGenerator genZero;


        public HeuristicCostFunction()
        {
            genCross = new HeuristicTrunsGenerator(hCross);
            genZero = new HeuristicTrunsGenerator(hZero);
        }

        Position getBest(GameState s, HeuristicTrunsGenerator gen)
        {
            foreach (Position p in gen.GenerateTurns(s))
            {
                return p;
            }
            System.Diagnostics.Debug.Assert(false, "Logical error");
            return new Position(0,0);
        }

        public int EvaluateState(GameState state)
        {           
            int res = hCross.EvaluateTurn(state, getBest(state)) - hZero.EvaluateTurn(state, getBest(state));
            
            if (state.Player == Side.Cross)
            {
                return -res;
            }
            return res;
        }


        public int WinValue
        {
            get { return 50000; }
        }

        public int LoseValue
        {
            get { return -50000; }
        }
    }
}
