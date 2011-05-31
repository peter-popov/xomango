using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoreCZ.AI
{
    class HeuristicCostFunction : MinMax.ICostFunction<State>
    {
        TurnHeuristics hCross = new LineBasedTurnHeuristics(Side.Cross);
        TurnHeuristics hZero = new LineBasedTurnHeuristics(Side.Zero);

        HeuristicTrunsGenerator genCross;
        HeuristicTrunsGenerator genZero;


        public HeuristicCostFunction()
        {
            genCross = new HeuristicTrunsGenerator(hCross);
            genZero = new HeuristicTrunsGenerator(hZero);
        }

        Position getBest(State s, HeuristicTrunsGenerator gen)
        {
            foreach (Position p in gen.GenerateTurns(s))
            {
                return p;
            }
            System.Diagnostics.Debug.Assert(false, "Logical error");
            return new Position(0,0);
        }

        public int EvaluateState(State state)
        {           
            int res = hCross.EvaluateTurn(state, getBest(state, genCross)) - hZero.EvaluateTurn(state, getBest(state, genZero));
            
            if (state.Player == Side.Cross)
            {
                return res;
            }
            return -res;
        }

    }
}
