using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoreCZ.AI
{

    public class HeuristicTrunsGenerator : MinMax.ITurnsGenerator<GameState>
    {               
        public HeuristicTrunsGenerator(TurnHeuristics function)
        {
            this.function = function;
        }

        public IEnumerable<Position> GenerateTurns(GameState state)
        {
            //Use the power of LINQ!!
            //What we do here?
            //For all possible positions generate cost-position pairs, then group them 
            //by cost, in each group shuffle results, then glue together groups sorted 
            //in descending order, and then take required amount of elements... fuh.  
            var posibleTruns = from pos in state
                               where state[pos].Side == Side.Nobody
                               group pos by function.EvaluateTurn(state, pos) into g
                               orderby g.Key descending
                               select new { Cost = g.Key, Positions = randomize(g) };
            return (from g in posibleTruns from p in g.Positions select p).Take(MaxTurns);
        }
        
        private IEnumerable<T> randomize<T>(IEnumerable<T> i)
        {
            
            var groups = from el in i select new { Key = r.Next(), Value = el };
            return from g in groups
                          orderby g.Key
                          select g.Value;
        }

        private Random r = new Random((int)DateTime.Now.Ticks);
        private static int MaxTurns = 14;       
        private TurnHeuristics function;
    }
}
