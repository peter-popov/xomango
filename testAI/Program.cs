using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CoreCZ;
using CoreCZ.AI;
using CoreCZ.AI.MinMax;

namespace testAI
{
    class Program
    {

        static void test_1()
        {
            GameState gs = new GameState();

            Position pos0 = new Position(3, 3);
            Position pos1 = new Position(4, 4);

            gs.Advance(pos0, Side.Cross);
            gs.Advance(pos1, Side.Zero);

            Console.WriteLine(gs);

            MinMax m = new MinMax(new SimpleCostFuntcion(Side.Zero), new HeuristicTrunsGenerator(new LineBasedTurnHeuristics()), 2);

            Position p = m.FindTurn(gs);
            Console.WriteLine("Turn found {0},{1}", p.X, p.Y);
        }

        static void test_2()
        {
            SimplePlayer player = new SimplePlayer(Side.Zero, 2);
            player.EnemyTurn(new Position(4, 5));
            Position p = player.MakeNextTurn();

            Console.WriteLine("Turn found {0},{1}", p.X, p.Y);
        }

        static void test_2x2_final()
        {
            GameState gs = new GameState();
            MinMax m = new MinMax(new SimpleCostFuntcion(Side.Zero), new HeuristicTrunsGenerator(new LineBasedTurnHeuristics()), 2);

            gs.Advance(new Position(3, 3), Side.Cross);
                gs.Advance(new Position(4, 3), Side.Zero);            
            gs.Advance(new Position(4, 4), Side.Cross);
                gs.Advance(new Position(3, 4), Side.Zero);
            gs.Advance(new Position(6, 6), Side.Cross);
                gs.Advance(new Position(2, 5), Side.Zero);
            gs.Advance(new Position(7, 7), Side.Cross);

            Position p = m.FindTurn(gs);
            Console.WriteLine("Turn found {0},{1}", p.X, p.Y);
        }


        static void test_performance(int runs = 10)
        {


            double[] times = new double[runs];

            for (int i = 0; i < runs; ++i)
            {
                Board board = new Board();
                SimplePlayer cross = new SimplePlayer(Side.Cross, 2);
                SimplePlayer zero = new SimplePlayer(Side.Zero, 2);

                int counter = 0;
                Console.Write("{0:D2}", i);
                var start_time = DateTime.Now;
                while (counter < 12)
                {

                    Position p = cross.MakeNextTurn();
                    board[p] = Side.Cross;
                    counter++;
                    Console.Write(".");
                    if (board.Winner) break;
                    zero.EnemyTurn(p);
                    
                    
                    p = zero.MakeNextTurn();
                    board[p] = Side.Zero;
                    counter++;
                    Console.Write(".");
                    if (board.Winner) break;                    
                    cross.EnemyTurn(p);                   
                }
                Console.WriteLine();
                times[i] = (DateTime.Now - start_time).TotalMilliseconds;                
            }

            var max_time = times.Max() / 1000;
            var min_time = times.Min() / 1000;
            var avg_time = times.Average() / 1000;


            Console.WriteLine("Average time: {0:F3}", avg_time);
            Console.WriteLine("    max time: {0:F3}", max_time);
            Console.WriteLine("    min time: {0:F3}", min_time);
        
        }


        static void Main(string[] args)
        {
            //Console.WriteLine("Test 1");
            //test_1();
            //Console.WriteLine("_______________________________________________");
            
            //Console.WriteLine("Test 2");
            //test_2();
            //Console.WriteLine("_______________________________________________");

            //Console.WriteLine("Final turn xx_xx combination");            
            //test_2x2_final();
            //Console.WriteLine("_______________________________________________");

            test_performance();
        }
    }
}
