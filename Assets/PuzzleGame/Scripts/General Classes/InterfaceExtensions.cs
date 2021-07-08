using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.PuzzleGame.Scripts.General_Classes
{
    static class InterfaceExtensions
    {
        public static bool IsOnCooldown(this IMagnetic magnetic)
        {
            return magnetic.GetCooldown() > 0.0;
        }
    }
}
