using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


// Queued up Jobs
public class Job 
{
    public Tile jobTile {get;}

    float jobTime;

    //NO:
    public string buildObjectType;

    Action<Job> callbackJobComplete;
    Action<Job> callbackJobCanceled;

    public Job (Tile tile,string buildObjectType,Action<Job> cbJobComplete ,float jobTime =1f){
        this.jobTile = tile;
        this.jobTime = jobTime;
        callbackJobComplete += cbJobComplete;
        this.buildObjectType = buildObjectType;

        this.jobTime = jobTime;
    }

    public void doWork(float workTime){
        jobTime -= workTime;

        if(jobTime <= 0){
            if(callbackJobComplete != null){
                callbackJobComplete(this);
            }
        }
    }

    public void cancelWork(){
        if(callbackJobCanceled != null){
            callbackJobCanceled(this);
        }
    }

    public void RegisterJobCompleteCallback(Action<Job> cb){
        callbackJobComplete += cb;

    }
    public void UnRegisterJobCompleteCallback(Action<Job> cb){

        callbackJobComplete -= cb;
    }
    public void RegisterJobCancelCallback(Action<Job> cb){
        callbackJobCanceled += cb;

    }
    public void UnRegisterJobCancelCallback(Action<Job> cb){
        callbackJobCanceled -= cb;

    }



}
