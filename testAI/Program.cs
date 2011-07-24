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

            MinMax m = new MinMax(new SimpleCostFuntcion(), new HeuristicTrunsGenerator(new LineBasedTurnHeuristics()), 2);

            Position p = m.FindTurn(gs);
            Console.WriteLine("Turn found {0},{1}", p.X, p.Y);
        }

        static void test_2()
        {
            SimplePlayer player = new SimplePlayer(Side.Zero);
            player.EnemyTurn(new Position(4, 5));
            Position p = player.MakeNextTurn();

            Console.WriteLine("Turn found {0},{1}", p.X, p.Y);
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Test 1");
            test_1();
            Console.WriteLine("_______________________________________________");
            
            Console.WriteLine("Test 2");
            test_2();
            Console.WriteLine("_______________________________________________");
        }
    }
}
