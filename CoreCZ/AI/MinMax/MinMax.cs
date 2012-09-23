using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace CoreCZ.AI.MinMax
{
    public class MinMax
    {
        public MinMax(ICostFunction<GameState> costFunction, ITurnsGenerator<GameState> turnsGenerator, int level)
        {
            maxDepth = level * 2 + 1;
            this.costFunction = costFunction;
            this.turnsGenerator = turnsGenerator;
        }

        public Position FindTurn(GameState s)
        {
            Position turn = new Position(0,0);
            Debug.WriteLine(s);                         
            int turnValue = AlphaBetaSearch(s, int.MinValue, int.MaxValue, 0, ref turn);             
            Debug.WriteLine("Choose ({0},{1}) with cost {2}", turn.X, turn.Y, turnValue);
            return turn;
        }

        #region Alpha-beta pruning implementation
        private int AlphaBetaSearch(GameState s, int alpha, int beta, int deep, ref Position res_turn)
        {
            Position turn = new Position(0, 0);
            
            if (deep >= maxDepth || s.GameOver)
            {
                return costFunction.EvaluateState(s);
            }
            if (deep % 2 == 0)
            {
                foreach( Position p in turnsGenerator.GenerateTurns(s))
                {
                    GameState.ChangeSet ch = s.Advance(p, Utils.FlipSide(s.Player));
                    int score = AlphaBetaSearch(s, alpha, beta, deep + 1, ref turn) - deep * 20;
                    s.Undo(ch);

                    if (deep == 0)
                    {
                        var hscore = hf.EvaluateTurn(s, p);
                        Debug.WriteLine("Considering ({0},{1}) with ab_cost = {2}, h_cost = {3}", p.X, p.Y, score, hscore);
                    }

                    if (score > alpha)
                    {
                        res_turn = p;                        
                        alpha = score;
                    }
                    if (alpha >= beta) break;
                }
               
                return alpha;
            }
            else
            {
                foreach (Position p in turnsGenerator.GenerateTurns(s))
                {
                    GameState.ChangeSet ch = s.Advance(p, Utils.FlipSide(s.Player));
                    int score = AlphaBetaSearch(s, alpha, beta, deep + 1, ref turn) - deep * 20;
                    s.Undo(ch);

                    if (score < beta)
                    {
                        beta = score;
                        res_turn = p;
                    }
                    if (alpha >= beta) break;
                }
                return beta;
            }
        }
        #endregion

        #region MinMax implementation
        
        private string intend(int deep)
        {
            string res = "";
            for (int i = 0; i < deep; ++i)
            {
                res += ".";
            }
            return res;
        }

        private int MinMaxSearch(GameState s, int deep, ref Position turn)
        {
            int cost = costFunction.EvaluateState(s);
            if (deep == maxDepth || cost <= costFunction.LoseValue || cost >= costFunction.WinValue)
            {
                return cost;
            }
            if (deep % 2 == 0)
            {
                int max = int.MinValue;
                Position sub_turn = new Position(0, 0);
                foreach (Position p in turnsGenerator.GenerateTurns(s))
                {
                    string hash = s.ToString();                    
                    GameState.ChangeSet ch = s.Advance(p, Utils.FlipSide(s.Player));
                    int score = MinMaxSearch(s, deep + 1, ref sub_turn);
                    s.Undo(ch);
                    Debug.Assert(s.ToString() == hash);
                    if (deep == 0)
                    {
                        Debug.WriteLine(intend(deep) + "max: ({0},{1}) avaluated as {2}\n", p.X, p.Y, score);
                    }
                    if (score > max)
                    {
                        max = score;
                        turn = p;
                    }
                }
                return max;
            }
            else
            {
                int min = int.MaxValue;
                Position sub_turn = new Position(0, 0);
                foreach (Position p in turnsGenerator.GenerateTurns(s))
                {
                    string hash = s.ToString();
                    
                    GameState.ChangeSet ch = s.Advance(p, Utils.FlipSide(s.Player));
                    int score = MinMaxSearch(s, deep + 1, ref sub_turn);

                    //if (deep == 1) 
                    //    Debug.WriteLine(intend(deep) + "min: ({0},{1}) avaluated as {2}", p.X, p.Y, score);
                    
                    s.Undo(ch);
                    Debug.Assert(s.ToString() == hash);
                    

                    if (score < min)
                    {
                        min = score;
                        turn = p;
                    }
                }
                return min;
            }
        }
        #endregion
        

        #region Private fields
        ICostFunction<GameState> costFunction;
        ITurnsGenerator<GameState> turnsGenerator;
        TurnHeuristics hf = new LineBasedTurnHeuristics();
        int maxDepth = 4;
        #endregion
    }
}
