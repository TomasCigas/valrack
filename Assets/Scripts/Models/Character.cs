using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character
{


    public float X{
        get{
            return Mathf.Lerp(currentTile.X,destTile.X, movementPerc);
        }
    }
    public float Y{
        get{
            return Mathf.Lerp(currentTile.Y,destTile.Y, movementPerc);
        }
    }

    public int Z{
        get{
            return currentTile.Z;
        }
    }

    public Tile CurrentTile { get => currentTile; protected set => currentTile = value; }

    Action<Character> callbackCharacterChanged;

    Job job;

    float speed = 2f;

    Tile currentTile;
    Tile destTile; // No movement == currentile
    float movementPerc;

    public Character(Tile tile){
        currentTile = tile;
        destTile = tile;
    }

    public void Update(float deltaTime){
        if(job == null){
            // Take new job
            job = currentTile.Map.jobQueue.DeQueue();
            if(job != null){
                job.RegisterJobCompleteCallback(OnJobEnded);
                job.RegisterJobCancelCallback(OnJobEnded);
            }
        }

        if(job != null){
            // Move to new job
            destTile = job.jobTile;
        }

        if(currentTile == destTile){
            // Do job
            if(job != null){
                job.doWork(deltaTime);
            }
            return;
        }


        float travelDist = Mathf.Sqrt(
            Mathf.Pow(currentTile.X - destTile.X,2) + 
            Mathf.Pow(currentTile.Y - destTile.Y,2)
        );

        float distThisFrame = speed * deltaTime;

        float percThisFrame = distThisFrame/travelDist;

        movementPerc += percThisFrame;

        if(movementPerc >= 1){
            currentTile = destTile;
            movementPerc = 0;
        }

        if(callbackCharacterChanged != null){
            callbackCharacterChanged(this);
        }

    }

    public void SetDestination(Tile tile){
        if(currentTile.isNeighbour(tile,true) == false){
            Debug.Log("Character - Tile is not adjescent");
        }
        destTile = tile;
    }

    void OnJobEnded(Job j){
        // Job Completed or canceled
        if(j != job){
            Debug.LogError("Character is doing wrong job. You forgot to unregister something");
        }

        job.UnRegisterJobCancelCallback(OnJobEnded);
        job.UnRegisterJobCompleteCallback(OnJobEnded);

        job = null;

    }

    public void RegisterCharacterChangeCallback(Action<Character> callback){
        callbackCharacterChanged += callback;
    }

    public void UnRegisterCharacterChangeCallback(Action<Character> callback){
        callbackCharacterChanged -= callback;
    }


}
