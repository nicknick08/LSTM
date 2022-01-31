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
        //Lstm
        this.rBody.angularVelocity = Vector3.zero;
        this.rBody.velocity= Vector3.zero;
        this.transform.localPosition = new Vector3(0.0f,0.5f,-5.5f);
        this.transform.rotation = Quaternion.Euler(0f,0f,0f);

        this.boardId= Random.Range(0,2);
        this.boards[0].SetActive(boardId==0);
        this.boards[1].SetActive(boardId==1);
        if (Random.Range(0,2)==0){
            this.targets[0].localPosition = new Vector3(-3f,0.5f,5f);
            this.targets[1].localPosition = new Vector3(3f,0.5f,5f);

        }
        else{
            this.targets[0].localPosition = new Vector3(3f, 0.5f, 5f);
            this.targets[1].localPosition = new Vector3(-3f,0.5f,5f);

        }




        // // curiosity
        // this.lastCheckPoint = 0;
        // this.checkPointCount = 0;




        // // RaycastAgentが床から落下している時
        // if (this.transform.localPosition.y < 0)
        // {
        //     // RaycastAgentの位置と速度をリセット
        //     this.rBody.angularVelocity = Vector3.zero;
        //     this.rBody.velocity = Vector3.zero;
        //     this.transform.localPosition = new Vector3(0.0f, 0.5f, 0.0f);
        // }

        // // Targetの位置のリセット
        // target.localPosition = new Vector3(
        //     Random.value*8-4, 0.5f, Random.value*8-4);
    }

    // public override void CollectObservations(VectorSensor sensor){
    //     sensor.AddObservation(rBody.velocity.x); // RollerAgentのX速度
    //     sensor.AddObservation(rBody.velocity.z); // RollerAgentのZ速度
    // }

    // 行動実行時に呼ばれる
    public override void OnActionReceived(float[] vectorAction)
    {
        // Lstm
        Vector3 dirToGo = Vector3.zero;
        Vector3 rotateDir = Vector3.zero;
        int action = (int)vectorAction[0];
        if (action == 1) dirToGo = transform.forward;
        if(action == 2) dirToGo= transform.forward * -1.0f;
        if(action == 3) rotateDir = transform.up * -1.0f;
        if(action == 4) rotateDir = transform.up;
        this.transform.Rotate(rotateDir, Time.deltaTime * 200f);
        this.rBody.AddForce(dirToGo * 0.4f, ForceMode.VelocityChange);

        for(int i = 0 ; i < 2 ; i++){
            float distanceToTarget = Vector3.Distance(
                this.transform.localPosition, targets[i].localPosition);
                if(distanceToTarget<1.42f){
                    if(i == boardId){
                        AddReward(1.0f);

                    }
                    EndEpisode();
                }
            
        }
        AddReward(-0.0005f);


    //     // RaycastAgentに力を加える
    //     Vector3 dirToGo = Vector3.zero;
    //     Vector3 rotateDir = Vector3.zero;
    //     int action = (int)vectorAction[0];
    
    //     if(action == 1) dirToGo = transform.forward;
    //     if(action == 2) dirToGo = transform.forward *-1.0f;
    //     if(action == 3) rotateDir = transform.up *-1.0f;
    //     if(action == 4) rotateDir = transform.up;
    //     this.transform.Rotate(rotateDir, Time.deltaTime*200f);
    //     // this.transform.position+=dirToGo*Time.deltaTime*2.0f;
    //     this.rBody.AddForce(dirToGo * 0.4f, ForceMode.VelocityChange);


    //     // // RaycastAgentがTargetの位置に到着した時
    //     // float distanceToTarget = Vector3.Distance(
    //     //     this.transform.localPosition, target.localPosition);

    //   AddReward(-0.001f);

        // if (distanceToTarget < 1.42f){
        //     AddReward(1.0f);
        //     EndEpisode();
        // }
    }


                //  curiosity

    // public void EnterCheckPoint(int checkPoint){
    //     if(checkPoint == (this.lastCheckPoint+1)%4){
    //         this.checkPointCount++;

    //         if(this.checkPointCount >= 4){
    //             AddReward(2.0f);
    //             EndEpisode();
    //         }
    //     }else if(checkPoint == (this.lastCheckPoint - 1 +4 )%4){
    //         this.checkPointCount--;
    //     }


    //     this.lastCheckPoint = checkPoint;
    // }
    // ヒューリスティックモードの行動決定時に呼ばれる
    public override void Heuristic(float[] actionsOut)
    {
        actionsOut[0]=0;
        if(Input.GetKey(KeyCode.UpArrow)) actionsOut[0]=1;
        if(Input.GetKey(KeyCode.DownArrow)) actionsOut[0]=2;
        if(Input.GetKey(KeyCode.LeftArrow)) actionsOut[0]=3;
        if(Input.GetKey(KeyCode.RightArrow)) actionsOut[0]=4;
    }
}