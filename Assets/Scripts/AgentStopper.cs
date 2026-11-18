using UnityEngine;
using UnityEngine.AI;

public static class AgentStopper
{
    public static void SetAgentEnabled(NavMeshAgent agent, bool enabled)
    {
        if (agent == null) return;

        if (enabled)
        {
            if (!agent.enabled) agent.enabled = true;

            if (!agent.isOnNavMesh &&
                NavMesh.SamplePosition(agent.transform.position, out NavMeshHit hit, 3f, NavMesh.AllAreas))
            {
                agent.Warp(hit.position);
            }

            if (agent.isOnNavMesh)
            {
                agent.isStopped = false;
                agent.ResetPath();
            }
        }
        else
        {
            if (!agent.enabled) return;

            if (agent.isOnNavMesh)
            {
                agent.isStopped = true;
                agent.ResetPath();
            }
            agent.enabled = false;
        }
    }
}
