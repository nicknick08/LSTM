using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;



// RaycastAgent
public class RaycastAgent : Agent
{

    Rigidbody rBody;
    public Transform[] targets;
    public GameObject[] boards;
    int boardId;
    


    

    // 初期化時に呼ばれる
    public override void Initialize()
    {
        this.rBody = GetComponent<Rigidbody>();
    }

    // エピソード開始時に呼ばれる
    public override void OnEpisodeBegin()
    {
        //Agentの位置と速度のリセット
        this.rBody.angularVelocity = Vector3.zero;
        this.rBody.velocity= Vector3.zero;
        this.transform.localPosition = new Vector3(0.0f,0.5f,-5.5f);
        this.transform.rotation = Quaternion.Euler(0f,0f,0f);

        // Boardのリセットとランダム化
        this.boardId= Random.Range(0,2);
        this.boards[0].SetActive(boardId==0);
        this.boards[1].SetActive(boardId==1);

        // ターゲットの位置をランダム化
        if (Random.Range(0,2)==0){ 
            this.targets[0].localPosition = new Vector3(-3f,0.5f,5f); //赤が右
            this.targets[1].localPosition = new Vector3(3f,0.5f,5f);

        }
        else{
            this.targets[0].localPosition = new Vector3(3f, 0.5f, 5f);
            this.targets[1].localPosition = new Vector3(-3f,0.5f,5f);

        }

    }

   

    // 行動実行時に呼ばれる
    public override void OnActionReceived(float[] vectorAction)
    {
        // Agentsの動き
        Vector3 dirToGo = Vector3.zero;
        Vector3 rotateDir = Vector3.zero;
        int action = (int)vectorAction[0];
        if (action == 1) dirToGo = transform.forward;
        if(action == 2) dirToGo= transform.forward * -1.0f;
        if(action == 3) rotateDir = transform.up * -1.0f; //rotate
        if(action == 4) rotateDir = transform.up;
        this.transform.Rotate(rotateDir, Time.deltaTime * 200f);
        this.rBody.AddForce(dirToGo * 0.4f, ForceMode.VelocityChange);


        for(int i = 0 ; i < 2 ; i++){
            float distanceToTarget = Vector3.Distance(                      //Agentsとターゲットの距離を測る
                this.transform.localPosition, targets[i].localPosition);
                if(distanceToTarget<1.42f){　//ターゲットと接触時
                    if(i == boardId){　//正しいターゲットの場合+
                        AddReward(1.0f);

                    }
                    EndEpisode();
                }
            
        }
        AddReward(-0.0005f);
        

    }


    public override void Heuristic(float[] actionsOut)
    {
        actionsOut[0]=0;
        if(Input.GetKey(KeyCode.UpArrow)) actionsOut[0]=1;
        if(Input.GetKey(KeyCode.DownArrow)) actionsOut[0]=2;
        if(Input.GetKey(KeyCode.LeftArrow)) actionsOut[0]=3;
        if(Input.GetKey(KeyCode.RightArrow)) actionsOut[0]=4;
    }
}