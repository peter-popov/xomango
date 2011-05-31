using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoreCZ.AI
{
    class SimpleCostFuntcion : MinMax.ICostFunction<State>
    {
        Random r = new Random(32);

        struct LineStatistics
        {
            public int cross;
            public int zero;
        };

        public int EvaluateState(State state)
        {
            int max_zero = int.MinValue;
            int max_cross = int.MinValue;

            foreach (Position p in state)
            {
                PositionInfo info = state[p];
                if (info == null)
                {
                    continue;
                }

                LineStatistics st = EvaluatePosition(info);
                max_cross = Math.Max(st.cross, max_cross);
                max_zero = Math.Max(st.zero, max_zero);
            }
            if (state.Player == Side.Cross)
            {
                return max_zero - max_cross;
            }
            else
            {
                return max_cross - max_zero;
            }
        }

        private LineStatistics EvaluatePosition(PositionInfo positionInfo)
        {
            LineStatistics ls = new LineStatistics();
            ls.cross = ls.zero = 0;
            foreach (PositionInfo.Orientation o in PositionInfo.Orientations)
            {
                foreach (PositionInfo.Direction d in PositionInfo.Directions)
                {
                    PositionInfo.LineInfo li = positionInfo[o, d];
                    if (li.side == Side.Cross)
                    {
                        ls.cross += LineCost(li.amount, li.open);
                    }
                    else
                    {
                        ls.zero += LineCost(li.amount, li.open);                    
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
                    return isopen ? 100 : 40;
                case 4:
                    return 1000;            
            }
            return 0;
        }
    }
}
