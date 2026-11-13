using UnityEngine;
using UnityEngine.AI;

public static class AgentStopper
{
    public static void SetAgentState(NavMeshAgent agent, bool active = false)
    {
        if (active)
        {
            if (agent != null)
            {
                agent.enabled = true;
                if (agent.isOnNavMesh)
                {
                    agent.isStopped = false;
                    agent.ResetPath();
                }
            }
        }
        else
        {
            if (agent != null && agent.enabled)
            {
                if (agent.isOnNavMesh)
                {
                    agent.isStopped = true;
                    agent.ResetPath();
                }
                agent.enabled = false;
            }
        }
    }
}
