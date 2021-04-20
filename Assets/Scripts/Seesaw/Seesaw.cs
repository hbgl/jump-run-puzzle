using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JumpRunPuzzle.Seesaw
{
    public class Seesaw : MonoBehaviour
    {
        public ISeesawAgent AgentA;

        public ISeesawAgent AgentB;

        public float JumpHeightA = 0f;

        public float JumpHeightB = 0f;

        public float JumpHeightReduction = 1f;

        public float JumpHeightMin = 0.5f;

        public void SetAgent(ISeesawAgent agent, int index)
        {
            if (index == 0)
            {
                AgentA = agent;
            }
            else
            {
                AgentB = agent;
            }
        }

        public bool HasOtherAgent(ISeesawAgent agent)
        {
            return GetOtherAgent(agent) != null;
        }

        public ISeesawAgent GetOtherAgent(ISeesawAgent agent)
        {
            if (AgentA != null && AgentA != agent)
            {
                return AgentA;
            }
            if (AgentB != null && AgentB != agent)
            {
                return AgentB;
            }
            return null;
        }

        public void RemoveAgent(ISeesawAgent agent)
        {
            if (AgentA == agent)
            {
                AgentA = null;
            }
            if (AgentB == agent)
            {
                AgentB = null;
            }
        }
    }
}