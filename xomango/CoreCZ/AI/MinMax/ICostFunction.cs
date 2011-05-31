using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoreCZ.AI.MinMax
{
    // Generic cost function for game states
    public interface ICostFunction<S>
    {
        int EvaluateState(S state);
    }
}
