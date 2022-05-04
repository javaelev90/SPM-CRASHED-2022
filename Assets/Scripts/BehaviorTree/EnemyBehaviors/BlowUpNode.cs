using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using BehaviorTree;
using Photon.Pun;

public class BlowUpNode : Node
{
    private NavMeshAgent agent;
    private AIBaseLogic ai;
    private float enemyBlowUpDistance;

    public BlowUpNode(AIBaseLogic ai, NavMeshAgent agent)
    {
        this.ai = ai;
        this.agent = agent;
        //enemyBlowUpDistance = ai.viewRadius / 2;
    }

    public override NodeStates Evaluate()
    {
        BlowUp();
        return NodeStates.SUCCESS;
    }


    void BlowUp()
    {
        //myOPhotonview.RPC(nameof(BlowUpRPC);
    }

    [PunRPC]
    private void BlowUpRPC()
    {
        //Collider[] targets = Physics.OverlapSphere(agent.transform.position, enemyBlowUpDistance, ai.targetMask);
        //if (targets.Length > 0)
        //{
        //    foreach(Collider coll in targets)
        //    {
        //        coll.gameObject.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.All, 10);
        //        //coll.GetComponent<Enemy>().TakeDamage(10);
        //    }
        //}
    }

}
