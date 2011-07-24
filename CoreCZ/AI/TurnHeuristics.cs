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
        public abstract int EvaluateTurn(GameState s, Position pos);
    }

    /// <summary>
    /// 
    /// </summary>
    public class LineBasedTurnHeuristics: TurnHeuristics
    {
        private Side? basePlayer;
        Func<PositionInfo.LineInfo, PositionInfo.LineInfo, Side, int> evalOrientation;

        public LineBasedTurnHeuristics()
        {
            evalOrientation = EvaluateOrientation;
        }

        public LineBasedTurnHeuristics(Side ?player)
        {
            this.basePlayer = player;
            evalOrientation = EvaluateOrientationF;        
        }

        public override int EvaluateTurn(GameState s, Position pos)
        {
            PositionInfo info = s[pos];
            if (info == null)
            {
                return 0;
            }
            return EvaluatePosition(info, Utils.FlipSide(s.Player));
        }

        private int EvaluatePosition(PositionInfo positionInfo, Side player)
        {
            int value = 0;
            foreach (PositionInfo.Orientation o in PositionInfo.Orientations)
            {
                value += evalOrientation(positionInfo[o, PositionInfo.Direction.Positive], 
                                         positionInfo[o, PositionInfo.Direction.Negative], 
                                         player);
            }
            return value;
        }

        private int EvaluateOrientationF(PositionInfo.LineInfo positive, PositionInfo.LineInfo negative, Side player)
        {
            if (positive.amount > 0 && negative.amount > 0)
            {
                if (positive.side == negative.side && positive.side == basePlayer)
                {
                    int n = 1 + positive.amount + negative.amount;
                    return LineCost(n, positive.open, negative.open);
                }
            }

            if (positive.amount > 0 && positive.side == basePlayer)
            {
                return LineCost(positive.amount + (player == positive.side ? 1 : 0), positive.open, true);
            }


            if (negative.amount > 0 && positive.side == basePlayer)
            {
                return LineCost(negative.amount + (player == negative.side ? 1 : 0), negative.open, true);
            }
            return 0;        
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
                    int npos = positive.amount + (player == positive.side ? 1 : 0);
                    int nneg = negative.amount + (player == negative.side ? 1 : 0);

                    return Math.Max(LineCost(npos, positive.open, false), LineCost(nneg, negative.open, false));
                }
            }

            if (positive.amount > 0)
            {
                return LineCost(positive.amount + (player == positive.side ? 1 : 0), positive.open, true);
            }


            if (negative.amount > 0)
            {
                return LineCost(negative.amount + (player == negative.side ? 1 : 0), negative.open, true);
            }

            return 0;
        }

        private int LineCost(int size, bool open1, bool open2)
        {
            int lineWeight = LineSizeCoefficient(size);
            int factor = CloseOpenCoefficient(size, open1, open2);
            return (int)Math.Pow(lineWeight, factor);
        }

        private int LineSizeCoefficient(int size)
        {
            int[] weights = new int[] {0, 
                                       1,
                                       2,
                                       10,
                                       1000,
                                       100000, 0, 0, 0, 0};
            return weights[size];
        }

        private int CloseOpenCoefficient(int size, bool open1, bool open2)
        {
            if (size == 5) return 1;
            return 2 - (open1 ? 0 : 1) - (open2 ? 0 : 1);
        }
    }
}
