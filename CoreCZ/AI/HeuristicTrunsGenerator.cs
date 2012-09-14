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
            if (state.Counter < 8)
            {
                MaxTurns = 8;
            }
            else if (state.Counter < 20)
            {
                MaxTurns = 15;
            }
            else
            {
                MaxTurns = 20;
            }


            var turns = from pos in state
                        where state[pos].Side == Side.Nobody
                        select new { Value = function.EvaluateTurn(state, pos), pos };

            var threats = from t in turns where t.Value.threat_level == 2 select t.pos;
            var semi_threats = from t in turns where t.Value.threat_level == 1 select t.pos;
            
            if (threats.Count() > 0) return threats.Concat(semi_threats);

            var posibleTruns = from t in turns                               
                               group t by t.Value.cost into g
                               orderby g.Key descending
                               select new { Cost = g.Key, Positions = randomize(g) };

            return (from g in posibleTruns from p in g.Positions select p.pos).Take(MaxTurns);                        
        }
        
        private IEnumerable<T> randomize<T>(IEnumerable<T> i)
        {
            
            var groups = from el in i select new { Key = r.Next(), Value = el };
            return from g in groups
                          orderby g.Key
                          select g.Value;
        }

        private Random r = new Random((int)DateTime.Now.Ticks);
        private int MaxTurns = 20;       
        private TurnHeuristics function;
    }
}
