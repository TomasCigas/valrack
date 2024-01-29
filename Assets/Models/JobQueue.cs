using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JobQueue
{
    protected Queue<Job> jobQueue;

    Action<Job> callbackJobCreated;



    public JobQueue(){
        jobQueue = new Queue<Job>();
    }

    public void Enqueue(Job j){
        Debug.Log("Queued");
        jobQueue.Enqueue(j);
        if(callbackJobCreated != null){
            Debug.Log("callback");
            callbackJobCreated(j);
        }
    }

    public void RegisterJobCreatedCallback(Action<Job> cb){
        callbackJobCreated += cb;
    }
    public void UnRegisterJobCreatedCallback(Action<Job> cb){
        callbackJobCreated -= cb;
    }


}
