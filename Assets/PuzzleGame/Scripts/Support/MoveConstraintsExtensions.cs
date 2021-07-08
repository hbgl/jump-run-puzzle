using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.PuzzleGame.Support
{
    public static class MoveConstraintsExtensions
    {
        public static void ApplyMoveConstraints(this ref Vector3 v3, MoveConstraints moveConstraints)
        {
            if ((moveConstraints & MoveConstraints.X) != 0) v3.x = 0;
            if ((moveConstraints & MoveConstraints.Y) != 0) v3.y = 0;
            if ((moveConstraints & MoveConstraints.Z) != 0) v3.z = 0;
        }
    }
}
