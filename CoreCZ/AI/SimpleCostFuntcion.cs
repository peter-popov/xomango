using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace CoreCZ.AI
{
    public class SimpleCostFuntcion : MinMax.ICostFunction<GameState>
    {
        struct LineStatistics
        {
            public int cross;
            public int zero;
        };

        public int EvaluateState(GameState state)
        {
            int max_zero = int.MinValue;
            int max_cross = int.MinValue;

            foreach (Position p in state)
            {
                PositionInfo info = state[p];
                if (info == null || info.Side != Side.Nobody)
                {
                    continue;
                }                

                LineStatistics st = EvaluatePosition(info);
                max_cross = Math.Max(st.cross, max_cross);
                max_zero = Math.Max(st.zero, max_zero);
            }

            int res = max_zero - max_cross;            
            
            return Math.Max(Math.Min(res, WinValue), LoseValue);
        }

        private LineStatistics EvaluatePosition(PositionInfo positionInfo)
        {
            LineStatistics ls = new LineStatistics();
            ls.cross = ls.zero = 0;
            foreach (var  o in PositionInfo.Orientations)
            {
                foreach (var d in PositionInfo.Directions)
                {
                    PositionInfo.LineInfo li = positionInfo[o, d];
                    if (li.side == Side.Cross)
                    {
                        ls.cross = Math.Max(ls.cross, LineCost(li.amount, li.open));
                    }
                    else if (li.side == Side.Zero)
                    {
                        ls.zero = Math.Max(ls.zero, LineCost(li.amount, li.open));                    
                    }
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
                    return isopen ? 200 : 4;
                case 4:
                    return isopen ? 4000 : 10;
                case 5:
                    return 100000;                        
            }
            //System.Diagnostics.Debug.Assert(false, "Length more than 5 detected!");
            return int.MaxValue;
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
