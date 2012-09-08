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
            maxDepth = level * 2;
            this.costFunction = costFunction;
            this.turnsGenerator = turnsGenerator;
        }

        public Position FindTurn(GameState s)
        {
            Position turn = new Position(0,0);
            Debug.WriteLine(s);
            //int turnValue = MinMaxSearch(s, 0, ref turn);                         
            int turnValue = AlphaBetaSearch(s, int.MinValue, int.MaxValue, 0, ref turn);  
            
            Debug.WriteLine("Choose ({0},{1}) with cost {2}", turn.X, turn.Y, turnValue);
            return turn;
        }

        #region Alpha-beta pruning implementation
        private int AlphaBetaSearch(GameState s, int alpha, int beta, int deep, ref Position res_turn)
        {
            Position turn = new Position(0, 0);
            int cost = costFunction.EvaluateState(s);
            
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
                    int score = AlphaBetaSearch(s, alpha, beta, deep + 1, ref turn) - deep * 20;
                    s.Undo(ch);
                    s[p].Weight = score;
                        
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
                //for each child
                //    score = alpha-beta(other player,child,alpha,beta)
                //    if score < beta then beta = score (opponent has found a better worse move)
                //    if alpha >= beta then return beta (cut off)
                //return beta (this is the opponent's best move)
                foreach (Position p in turnsGenerator.GenerateTurns(s))
                {
                    GameState.ChangeSet ch = s.Advance(p, Utils.FlipSide(s.Player));
                    int score = AlphaBetaSearch(s, alpha, beta, deep + 1, ref turn) - deep * 20;
                    s.Undo(ch);

                    //if (deep == 1)
                    //{
                    //    var hscore = hf.EvaluateTurn(s, p);
                    //    Debug.WriteLine("\tEnemy turn ({0},{1}) with ab_cost = {2}, h_cost = {3}", p.X, p.Y, score, hscore);
                    //}

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

                    if (deep == 1) Debug.WriteLine(intend(deep) + "min: ({0},{1}) avaluated as {2}", p.X, p.Y, score);
                    
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

        #region Alpha-beta pruning with memory
        class AlphaBetaNode
        {
            public AlphaBetaNode()
            {
                this.lower_bound = int.MinValue;
                this.upper_bound = int.MaxValue;
                this.best_turn = new Position(0, 0);
            }
            public int lower_bound;
            public int upper_bound;
            public Position best_turn;
            public bool valid() { return this.lower_bound > int.MinValue || this.upper_bound < int.MaxValue; }
        }

        IDictionary<GameState, AlphaBetaNode> memory = new Dictionary<GameState, AlphaBetaNode>();

        private bool retrieve(GameState s, out AlphaBetaNode node)
        {
            memory.TryGetValue(s, out node);
            return node != null && node.valid();
        }

        private void store(GameState s, AlphaBetaNode node)
        {
            memory[s] = node;
        }

        private int AlphaBetaSearchWithMemory(GameState s, int alpha, int beta, int deep, ref Position res_turn)
        {
            AlphaBetaNode n = null;

            if (retrieve(s, out n))
            {
                if (n.lower_bound >= beta)
                {
                    res_turn = n.best_turn;
                    return n.lower_bound;
                }
                if (n.upper_bound <= alpha)
                {
                    res_turn = n.best_turn;
                    return n.upper_bound;
                }
                alpha = Math.Max(alpha, n.lower_bound);
                beta = Math.Min(beta, n.upper_bound);
            }
            else
            {
                n = new AlphaBetaNode();
            }


            var turn = new Position(0, 0);
            var g = int.MinValue; 

            if (deep >= maxDepth /*|| g <= costFunction.LoseValue || g >= costFunction.WinValue*/)
            {
                g = costFunction.EvaluateState(s);
            }
            else if (deep % 2 == 0)
            {
                g = int.MinValue;
                var a = alpha;  

                foreach (Position p in turnsGenerator.GenerateTurns(s))
                {
                    GameState.ChangeSet ch = s.Advance(p, Utils.FlipSide(s.Player));
                    var score = AlphaBetaSearchWithMemory(s, a, beta, deep + 1, ref turn);
                    var hscore = costFunction.EvaluateState(s);
                    s.Undo(ch);
                    s[p].Weight = score;

                    if (deep == 0)
                    {
                        
                        Debug.WriteLine("Considering ({0},{1}) with ab_cost = {2}, h_cost = {3}", p.X, p.Y, score, hscore);            
                    }

                    if (score > g)
                    {
                        res_turn = p;
                        g = score;
                    }
                    a = Math.Max(a, g);
                    if (g >= beta) break;
                }
            }
            else
            {
                g = int.MaxValue;
                var b = beta;

                foreach (Position p in turnsGenerator.GenerateTurns(s))
                {
                    GameState.ChangeSet ch = s.Advance(p, Utils.FlipSide(s.Player));
                    int score = AlphaBetaSearchWithMemory(s, alpha, b, deep + 1, ref turn);
                    s.Undo(ch);

                    if (score < g)
                    {
                        g = score;
                        res_turn = p;
                    }
                    b = Math.Min(g, b);
                    if (alpha >= g) break;
                }
            }

            if (g <= alpha) 
            {
                n.upper_bound = g;
                n.best_turn = res_turn;
                //store n.upperbound;
            }
            /* Found an accurate minimax value - will not occur if called with zero window */ 
            if (g > alpha && g < beta )
            {              
                n.lower_bound = g; 
                n.upper_bound = g;
                n.best_turn = res_turn;
                //store n.lowerbound, n.upperbound;
            }
            /* Fail high result implies a lower bound */
            if (g >= beta)
            {
                n.lower_bound = g;
                n.best_turn = res_turn;
                //store n.lowerbound;
            }
            store(s, n);
            return g;



            /*
            Position turn = new Position(0, 0);
            int cost = costFunction.EvaluateState(s);

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
                foreach (Position p in turnsGenerator.GenerateTurns(s))
                {
                    GameState.ChangeSet ch = s.Advance(p, Utils.FlipSide(s.Player));
                    int score = AlphaBetaSearch(s, alpha, beta, deep + 1, ref turn);
                    s.Undo(ch);
                    s[p].Weight = score;

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

                    if (score < beta)
                    {
                        beta = score;
                        res_turn = p;
                    }
                    if (alpha >= beta) break;
                }
                return beta;
            }*/
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
