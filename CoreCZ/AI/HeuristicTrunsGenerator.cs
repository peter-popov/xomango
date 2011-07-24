using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoreCZ.AI
{

    public class HeuristicTrunsGenerator : MinMax.ITurnsGenerator<GameState>
    {
        Random r = new Random();
        static int MaxTurns = 16;
        
        public HeuristicTrunsGenerator(TurnHeuristics function)
        {
            this.function = function;
        }

        public IEnumerable<Position> GenerateTurns(GameState state)
        {
            //Use the power of LINQ!!
            var posibleTruns = from pos in state
                               where state[pos].Side == Side.Nobody
                               group pos by function.EvaluateTurn(state, pos) into g
                               orderby g.Key descending
                               select new { Cost = g.Key, Positions = randomize(g) };
            return (from g in posibleTruns from p in g.Positions select p).Take(MaxTurns);
        }
        
        private IEnumerable<T> randomize<T>(IEnumerable<T> i)
        {
            Random r = new Random();
            var groups = from el in i select new { Key = r.Next(), Value = el };
            return from g in groups
                          orderby g.Key
                          select g.Value;
        }

        private TurnHeuristics function;
    }
}
