using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoreCZ.AI
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class TurnHeuristics
    {
        public struct Value 
        { 
            public int cost; 
            public int threat_level;
            public override string ToString()
            {
                return string.Format("{0}({1})", cost, threat_level);
            }
        }

        public abstract TurnHeuristics.Value EvaluateTurn(GameState s, Position pos);
    }

    /// <summary>
    /// 
    /// </summary>
    public class LineBasedTurnHeuristics: TurnHeuristics
    {        

        public LineBasedTurnHeuristics()
        {
            
        }

        public override TurnHeuristics.Value EvaluateTurn(GameState s, Position pos)
        {
            PositionInfo info = s[pos];
            if (info == null)
            {
                return new TurnHeuristics.Value();
            }
            return EvaluatePosition(info, Utils.FlipSide(s.Player));
        }

        private TurnHeuristics.Value EvaluatePosition(PositionInfo positionInfo, Side player)
        {
            var value = new TurnHeuristics.Value();
            foreach (PositionInfo.Orientation o in PositionInfo.Orientations)
            {
                value.cost += EvaluateOrientation(positionInfo[o, PositionInfo.Direction.Positive], 
                                         positionInfo[o, PositionInfo.Direction.Negative], 
                                         player);
                value.threat_level = Math.Max(EvaluateThreatLevel(positionInfo[o, PositionInfo.Direction.Positive],
                                         positionInfo[o, PositionInfo.Direction.Negative],
                                         player), value.threat_level);
            }
            return value;
        }

        private int EvaluateOrientation(PositionInfo.LineInfo positive, PositionInfo.LineInfo negative, Side player)
        {
            if (positive.amount > 0 && negative.amount > 0)
            {
                if (positive.side == negative.side)
                {
                    int n = 1 + positive.amount + negative.amount;
                    return LineCost(n, positive.open, negative.open);
                }
                else
                {
                    int npos = positive.amount + 1;
                    int nneg = negative.amount + 1;

                    return Math.Max(LineCost(npos, positive.open, false), LineCost(nneg, negative.open, false));
                }
            }

            if (positive.amount > 0)
            {
                return LineCost(positive.amount + 1, positive.open, true);
            }


            if (negative.amount > 0)
            {
                return LineCost(negative.amount + 1, negative.open, true);
            }

            return 0;
        }

        private int EvaluateThreatLevel(PositionInfo.LineInfo positive, PositionInfo.LineInfo negative, Side player)
        {
            if (positive.amount > 0 && negative.amount > 0)
            {
                if (positive.side == negative.side)
                {
                    int n = 1 + positive.amount + negative.amount;
                    return ThreatLevel(n, positive.open, negative.open, positive.side == player);
                }
                else
                {
                    int npos = positive.amount + 1;
                    int nneg = negative.amount + 1;

                    return Math.Max(ThreatLevel(npos, positive.open, false, positive.side == player), ThreatLevel(nneg, negative.open, false, negative.side == player));
                }
            }

            if (positive.amount > 0)
            {
                return ThreatLevel(positive.amount + 1, positive.open, true, positive.side == player);
            }


            if (negative.amount > 0)
            {
                return ThreatLevel(negative.amount + 1, negative.open, true, negative.side == player);
            }

            return 0;
        }

        private int LineCost(int size, bool open1, bool open2)
        {
            int lineWeight = LineSizeCoefficient(size);
            int factor = CloseOpenCoefficient(size, open1, open2);
            return (int)Math.Pow(lineWeight, factor);
        }

        private int ThreatLevel(int size, bool open1, bool open2, bool same_player)
        {
            if (size == 5 || (size == 4 && open1 && open2)) return 2;
            if (size == 4 && (open1 || open2) && same_player) return 1;
            return 0;
        }

        private int LineSizeCoefficient(int size)
        {
            int[] weights = new int[] {0, 
                                       1,
                                       2,
                                       15,
                                       100,
                                       1000, 0, 0, 0, 0};
            return weights[size];
        }

        private int CloseOpenCoefficient(int size, bool open1, bool open2)
        {
            if (size == 5) return 1;
            return 2 - (open1 ? 0 : 1) - (open2 ? 0 : 1);
        }
    }
}
