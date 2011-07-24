using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExpressUnitModel;
using CoreCZ;
using CoreCZ.AI;


namespace ExpressUnitGui.CoreCZTest
{

    //[TestClass]   
    class CoreCZ_HeuristicTurnsGeneratorTest
    {
        [UnitTest]
        public void HeuristicTurnsGenerator_GenerateFromTrivialState()
        {
            Position pos = new Position(3, 5);
            GameState initialState = new GameState();
            initialState.Advance(pos, Side.Cross);

            HeuristicTrunsGenerator gen = new HeuristicTrunsGenerator(new LineBasedTurnHeuristics());

            int i = 0;
            foreach (Position p in gen.GenerateTurns(initialState))
            {
                ++i;
            }

            Confirm.IsTrue(i > 0);

        }
    }
}
