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
            var possibleTurns = GetPossibleTurns(state);
            return enumarateTurns(possibleTurns).Take(MaxTurns);
        }

        private IEnumerable<Position> enumarateTurns(IDictionary<int, List<Position>> turns)
        {
            foreach (var list in turns.OrderByDescending(k => k.Key).Select(e => e.Value))
            {                
                foreach( var p in list.Select( x => new KeyValuePair<int, Position>(r.Next(), x)).OrderByDescending(e => e.Key))
                {
                    yield return p.Value;
                }               
            }          
        }

        private IDictionary<int, List<Position>> GetPossibleTurns(GameState state)
        {
            var possibleTurns = new Dictionary<int, List<Position>>();
            foreach (Position p in state)
            {
                if (state[p].Side == Side.Nobody)
                {
                    int cost = function.EvaluateTurn(state, p);
                    if (possibleTurns.ContainsKey(cost))
                    {
                        possibleTurns[cost].Add(p);
                    }
                    else
                    {
                        var list = new List<Position>();
                        list.Add(p);
                        possibleTurns[cost] = list;
                    }
                }
            }

            return possibleTurns;            
        }

        /* Cool bu SLOW linq version. Investigate why
        private IEnumerable<T> randomize<T>(IEnumerable<T> i)
        {
            Random r = new Random();
            var groups = from el in i select new { Key = r.Next(), Value = el };
            return from g in groups
                          orderby g.Key
                          select g.Value;
        }

       
        public IEnumerable<Position> selectbest(GameState state)
        {
            var posibleTruns = from pos in state
                               where state[pos].Side == Side.Nobody
                               group pos by function.EvaluateTurn(state, pos) into g
                               orderby g.Key descending
                               select new { Cost = g.Key, Positions = randomize(g) };
            return (from g in posibleTruns from p in g.Positions select p).Take(MaxTurns);
        }*/

        private TurnHeuristics function;
    }
}
