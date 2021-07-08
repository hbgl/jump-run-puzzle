using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JumpRunPuzzle.Seesaw
{
    public interface ISeesawAgent
    {
        void SetVelocity(float velocity);

        float GetMass();
    }
}
