using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoreCZ.AI.MinMax
{
    class MinMax
    {
        ICostFunction<State> costFunction;
        ITurnsGenerator<State> turnsGenerator;
        int maxDepth = 4;

        public MinMax(ICostFunction<State> costFunction, ITurnsGenerator<State> turnsGenerator)
        {
            this.costFunction = costFunction;
            this.turnsGenerator = turnsGenerator;
        }

        public Position FindTurn(State s)
        {
            int max = int.MinValue;
            Position bestPos = new Position();
            foreach (Position p in turnsGenerator.GenerateTurns(s))
            {
                int turnValue = EvaluateState(s.DeriveState(p), 1);
                if ( turnValue > max)
                {
                    max = turnValue;
                    bestPos = p;
                }
            }
            return bestPos;
        }

        private int EvaluateState(State s, int deep)
        {
            for (int i = 0; i < deep; ++i) Console.Write("  ");
            Console.WriteLine("MinMix level: {0}", deep);
            if (deep == maxDepth)
            {
                return costFunction.EvaluateState(s);
            }
            if (deep % 2 == 0)
            {
                return turnsGenerator.GenerateTurns(s).Max(x => EvaluateState(s.DeriveState(x), deep + 1));
            }
            else
            {
                return turnsGenerator.GenerateTurns(s).Min(x => EvaluateState(s.DeriveState(x), deep + 1));
            }
        }        
    }
}
