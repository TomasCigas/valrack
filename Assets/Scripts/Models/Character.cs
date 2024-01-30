using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character
{


    public float X{
        get{
            return Mathf.Lerp(currentTile.X,nextTile.X, movementPerc);
        }
    }
    public float Y{
        get{
            return Mathf.Lerp(currentTile.Y,nextTile.Y, movementPerc);
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

    float speed = 3f;
    float lastSpeed = 3f;

    Tile currentTile;
    Tile nextTile; // Next tile in path finding 
    Path_AStar path_AStar;
    Tile destTile; // No movement == currentile
    float movementPerc;

    public Character(Tile tile){
        currentTile = tile;
        destTile = tile;
        nextTile = tile;
    }

    public void Update(float deltaTime)
    {
        Update_job(deltaTime);

        Update_movement(deltaTime);


        if (callbackCharacterChanged != null)
        {
            callbackCharacterChanged(this);
        }

    }

    public void AbandonJob(){
        nextTile = destTile = currentTile;
        currentTile.Map.jobQueue.Enqueue(job);
        job = null;
    }

    void Update_movement(float deltaTime){
        // No need to move
        if(currentTile == destTile){
            return;
        }

        if(nextTile == null || nextTile == currentTile){
            // Get next tile from pathfinder
            if(path_AStar == null || path_AStar.Length() == 0){
                // Generate new path
                path_AStar = new Path_AStar( MapController.Instance.Map,currentTile,destTile );
                if(path_AStar.Length() == 0){
                    Debug.Log("No path was found by character");
                    // Cancel job?
                    AbandonJob();
                    path_AStar = null;
                    return;
                }
            }

            nextTile = path_AStar.GetNextTile();

            if( nextTile == currentTile ){
                Debug.Log("Starting to move");
            }
        }

        float travelDist = Mathf.Sqrt(
            Mathf.Pow(currentTile.X - nextTile.X, 2) +
            Mathf.Pow(currentTile.Y - nextTile.Y, 2)
        );

        float distThisFrame = (speed - currentTile.movementCost) * deltaTime;

        float percThisFrame = distThisFrame / travelDist;

        movementPerc += percThisFrame;

        if (movementPerc >= 1)
        {
            currentTile = nextTile;
            movementPerc = 0;
        }
    }

    void Update_job(float deltaTime)
    {
        if (job == null)
        {
            // Take new job
            job = currentTile.Map.jobQueue.DeQueue();
            if (job != null)
            {
                job.RegisterJobCompleteCallback(OnJobEnded);
                job.RegisterJobCancelCallback(OnJobEnded);
            }
        }

        if (job != null)
        {
            // Move to new job
            destTile = job.jobTile;
        }

        // Do work if we are on work

        if (currentTile == destTile){
            // Do job
            if (job != null)
            {
                job.doWork(deltaTime);
            }
            return;
        }
        
        return;

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
