using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoreCZ.AI
{
    public class HeuristicTrunsGenerator : MinMax.ITurnsGenerator<GameState>
    {
        Random r = new Random();
        static int MaxTurns = 12;

        public HeuristicTrunsGenerator(TurnHeuristics function)
        {
            this.function = function;
        }

        public IEnumerable<Position> GenerateTurns(GameState state)
        {
            List<KeyValuePair<int, Position>> possibleTurns = GetPossibleTurns(state);
            int k = 0;
            for (int i = 0; i < Math.Min(possibleTurns.Count, MaxTurns); ++i)
            {
                ++k;
                yield return possibleTurns[i].Value;
            }            
            //todo: generate several random turns
        }

        public Position GetBestTurn(GameState s)
        {
            List<KeyValuePair<int, Position>> possibleTurns = GetPossibleTurns(s);
            List<Position> bestTurns = new List<Position>();
            int bestValue = possibleTurns[0].Key;
            bestTurns.Add(possibleTurns[0].Value);
            for (int i = 1; i < possibleTurns.Count; ++i)
            {
                if (possibleTurns[i].Key != bestValue) break;
                bestTurns.Add(possibleTurns[i].Value);
            }

            return bestTurns[r.Next(bestTurns.Count)];
        }

        private List<KeyValuePair<int, Position>> GetPossibleTurns(GameState state)
        {
            List<KeyValuePair<int, Position>> possibleTurns = new List<KeyValuePair<int, Position>>();
            foreach (Position p in state)
            {
                if (state[p].Side == Side.Nobody)
                {
                    possibleTurns.Add(new KeyValuePair<int, Position>(function.EvaluateTurn(state, p), p));
                }
            }

            return possibleTurns.OrderByDescending(x => x.Key).ToList();            
        }
     
        private TurnHeuristics function;
    }
}
