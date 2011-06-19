using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoreCZ.AI.MinMax
{
    public interface ITurnsGenerator<S>
    {
        IEnumerable<Position> GenerateTurns(S state);
    }
}
