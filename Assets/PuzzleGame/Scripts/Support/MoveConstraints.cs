using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.PuzzleGame.Support
{
    [Flags]
    public enum MoveConstraints
    {
        None = 0,
        X = 1,
        Y = 2,
        Z = 4,
    }
}
