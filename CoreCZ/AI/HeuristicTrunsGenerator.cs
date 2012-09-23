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

        struct WeightedPoistion: IComparable<WeightedPoistion>
        { 
            public Position pos; 
            public int cost;

            public int CompareTo(WeightedPoistion other)
            {
                return other.cost-cost;
            }
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

            var turns = new List<WeightedPoistion>();
            turns.Capacity = 2*MaxTurns;
            var threats = new List<Position>();
            threats.Capacity = 2;
            var semi_threats = new List<Position>();
            semi_threats.Capacity = 6;

            foreach (var p in state)
            {
                if (p.Info.Side == Side.Nobody)
                {
                    var v = function.EvaluatePosition(p.Info, Utils.FlipSide(state.Player));

                    if (v.threat_level == 2)
                    {
                        threats.Add(p.Pos);
                        continue;
                    }
                    if (v.threat_level == 1)
                    {
                        semi_threats.Add(p.Pos);                        
                    }
                    turns.Add(new WeightedPoistion { pos = p.Pos, cost = v.cost });
                }
            }

            if (threats.Count > 0) return threats.Concat(semi_threats);

            turns.Sort();
            return (from t in turns select t.pos).Take(MaxTurns);
            
            //var posibleTruns = from t in turns                               
            //                   group t by t.cost into g
            //                   orderby g.Key descending
            //                   select new { Cost = g.Key, Positions = g/*randomize(g)*/ };

            //return (from g in posibleTruns from p in g.Positions select p.pos).Take(MaxTurns);                        
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
