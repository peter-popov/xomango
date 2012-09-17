using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CoreCZ.AI;
using CoreCZ;

namespace testAI
{
    class Scenario
    {
        private List<Position> turns = new List<Position>();

        public Scenario(string repr)
        { 
            loadFromString(repr);
        }

        public GameState Play()
        {
            GameState gs = new GameState();

            var side = Side.Cross;
            foreach(var t in turns)
            {
                gs.Advance(t, side);
                side = Utils.FlipSide(side);
            }

            return gs;
        }



        private void loadFromString(string representation)
        {
            int offset = 5;

            var turnsX = new List<Position>();
            var turnsO = new List<Position>();

            var lines = representation.Split('\n');
            for (int li = 0; li < lines.Length; li++)
            {
                var line = lines[li];
                for (int ci = 0; ci < line.Length; ci++)
                {
                    if ( line[ci] == 'x') turnsX.Add(new Position(offset + li, offset + ci));
                    if ( line[ci] == 'o') turnsO.Add(new Position(offset + li, offset + ci));
                }
            }

            if (turnsX.Count != turnsO.Count + 1) return;

            turns.Clear();
            turns.Add(turnsX[0]);
            for (int i = 0; i < turnsO.Count; i++)
            {
                turns.Add(turnsO[i]);
                turns.Add(turnsX[i+1]);
            }            
        }
    }
}
