using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace CoreCZ.AI.MinMax
{
    public class MinMax
    {
        ICostFunction<GameState> costFunction;
        ITurnsGenerator<GameState> turnsGenerator;
        int maxDepth = 4;

        public MinMax(ICostFunction<GameState> costFunction, ITurnsGenerator<GameState> turnsGenerator, int level)
        {
            maxDepth = level * 2;
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

        string intend(int deep)
        {
            string res = "";
            for (int i = 0; i < deep; ++i)
            {
                res += ".";
            }
            return res;
        }

        private int minMax(GameState s, int deep, ref Position turn)
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
                foreach( Position p in turnsGenerator.GenerateTurns(s))
                {
                    GameState.ChangeSet ch = s.Advance(p, Utils.FlipSide(s.Player));
                    int score = minMax(s, deep + 1, ref sub_turn);
                    s.Undo(ch);

                    if (deep == 0) Debug.WriteLine(intend(deep) + "max: ({0},{1}) avaluated as {2}", p.X, p.Y, score);
                    
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
                    GameState.ChangeSet ch = s.Advance(p, Utils.FlipSide(s.Player));
                    int score = minMax(s, deep + 1, ref sub_turn);
                    s.Undo(ch);
                    //Debug.WriteLine(intend(deep) + "min: ({0},{1}) avaluated as {2}", p.X, p.Y, score);
                    
                    if (score < min)
                    {
                        min = score;
                        turn = p;
                    }
                }
                return min;
            }
        }


        private int AlphaBetaSearch(GameState s, int alpha, int beta, int deep, ref Position res_turn)
        {
            Position turn = new Position(0, 0);
            int cost = costFunction.EvaluateState(s);
            //State derived = s.DeriveState(p);
            if (deep >= maxDepth || cost <= costFunction.LoseValue || cost >= costFunction.WinValue)
            {
                return cost;
            }
            if (deep % 2 == 0)
            {
                //for each child
                //score = alpha-beta(other player,child,alpha,beta)
                //if score > alpha then alpha = score (we have found a better best move)
                //if alpha >= beta then return alpha (cut off)
                //return alpha (this is our best move)
                foreach( Position p in turnsGenerator.GenerateTurns(s))
                {
                    GameState.ChangeSet ch = s.Advance(p, Utils.FlipSide(s.Player));
                    int score = AlphaBetaSearch(s, alpha, beta, deep + 1, ref turn);
                    s.Undo(ch);
                    //Debug.WriteLine(intend(deep) + "max: ({0},{1}) avaluated as {2},        a = {3}, b = {4}", p.X, p.Y, score, alpha, beta);
                    
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
                //for each child
                //    score = alpha-beta(other player,child,alpha,beta)
                //    if score < beta then beta = score (opponent has found a better worse move)
                //    if alpha >= beta then return beta (cut off)
                //return beta (this is the opponent's best move)
                foreach (Position p in turnsGenerator.GenerateTurns(s))
                {
                    GameState.ChangeSet ch = s.Advance(p, Utils.FlipSide(s.Player));
                    int score = AlphaBetaSearch(s, alpha, beta, deep + 1, ref turn);
                    s.Undo(ch);
                    //Debug.WriteLine(intend(deep) + "min: ({0},{1}) avaluated as {2}         a = {3}, b = {4}", p.X, p.Y, score, alpha, beta);
                    
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
    }
}
