using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace CoreCZ.AI
{
    public class SimpleCostFuntcion : MinMax.ICostFunction<GameState>
    {
        private Side winPlayer;

        public SimpleCostFuntcion(Side winPlayer)
        {
            this.winPlayer = winPlayer;
        }

        struct LineStatistics
        {
            public int cross;
            public int zero;

            public int this[Side side]
            {
                get
                {
                    return side == Side.Cross ? cross : zero;
                }
                set
                {
                    if (side == Side.Cross) cross = value;
                    else if (side == Side.Zero) zero = value;
                }
            }
        };

        public int EvaluateState(GameState state)
        {
            int max_zero = 0; //int.MinValue;
            int max_cross = 0;// int.MinValue;

            foreach (Position p in state)
            {
                PositionInfo info = state[p];
                if (info == null || info.Side != Side.Nobody)
                {
                    continue;
                }                

                LineStatistics st = EvaluatePosition(info);
                //max_cross = Math.Max(st.cross, max_cross);
                //max_zero = Math.Max(st.zero, max_zero);

                max_cross += st.cross;
                max_zero += st.zero;
            
            }

            int res = (winPlayer == Side.Zero) ? (max_zero - (int)(0.8*max_cross)) : (max_cross - (int)(0.8*max_zero));            
            
            return Math.Max(Math.Min(res, WinValue), LoseValue);
        }

        private LineStatistics EvaluatePosition(PositionInfo positionInfo)
        {
            var ls = new LineStatistics();
            ls.cross = ls.zero = 0;
            foreach (var  o in PositionInfo.Orientations)
            {

                var liPos = positionInfo[o, PositionInfo.Direction.Positive];
                var liNeg = positionInfo[o, PositionInfo.Direction.Negative];

                if (liPos.side == liNeg.side)
                {
                    ls[liPos.side] = PatternCost(liPos.amount, liPos.open, liNeg.amount, liNeg.open);
                }
                else
                {
                    ls[liPos.side] = LineCost(liPos.amount, liPos.open);
                    ls[liNeg.side] = LineCost(liNeg.amount, liNeg.open);
                }
            }
            return ls;
        }        

        private int LineCost(int size, bool isopen)
        {
            switch (size)
            {
                case 1:
                    return 1;
                case 2:
                    return isopen ? 4 : 2;
                case 3:
                    return isopen ? 300 : 3;
                case 4:
                    return isopen ? 4000 : 10;
                case 5:
                    return 100000;                        
            }
            return int.MaxValue;
        }

        private int PatternCost(int size1, bool isopen1, int size2, bool isopen2)
        {
            return LineCost(size1 + size2, isopen1 && isopen2);
        }

        public int WinValue
        {
            get {return 50000;}
        }

        public int LoseValue
        {
            get { return -50000; }
        }
    }
}
