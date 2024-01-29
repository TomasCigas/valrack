using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class JobQueue
{
    protected Queue<Job> jobQueue;

    Action<Job> callbackJobCreated;



    public JobQueue(){
        jobQueue = new Queue<Job>();
    }

    public void Enqueue(Job j){
        jobQueue.Enqueue(j);
        if(callbackJobCreated != null){
            callbackJobCreated(j);
        }
    }

    public Job DeQueue(){
        if(jobQueue.Count == 0){
            return null;
        }
        return jobQueue.Dequeue();
    }

    public void RegisterJobCreatedCallback(Action<Job> cb){
        callbackJobCreated += cb;
    }
    public void UnRegisterJobCreatedCallback(Action<Job> cb){
        callbackJobCreated -= cb;
    }


}
