using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.EventSystems;

public class BuildModeController : MonoBehaviour
{


    public MapController mapControllerInstance{get => MapController.Instance; }

    bool buildModeIsObject = false;
    Tile.TileType tileMode;
    string buildObjectType;

    public static int currentZPosition = 0;



    // Start is called before the first frame update
    void Start()
    {
    }


    public void SetMode_BuildObject(string buildObjectType){
        buildModeIsObject = true;
        this.buildObjectType = buildObjectType;
    }

    public void SetMode_Floor(){
        tileMode = Tile.TileType.Floor;
        buildModeIsObject = false;
    }

    public void SetMode_Delete(){
        tileMode = Tile.TileType.Empty;
        buildModeIsObject = false;
    }


    void onBuildObjectComplete(string objectType, Tile t){
        mapControllerInstance.Map.PlaceBuildObject(objectType, t);
        t.PendingJob = null;
    }

    public void doBuild(Tile t){

        if (buildModeIsObject)
        {
            // Create installed object
            //mapControllerInstance.Map.PlaceBuildObject(buildObjectType, t);

            string buildObjectForJob = buildObjectType;

            // Check if tile is available
            if(
                mapControllerInstance.Map.buildObjectPlacementValidation(buildObjectForJob,t) == false
            ){
                return;
            }

            if(t.PendingJob != null){
                Debug.Log("There already is a pending job on this tile");
                return;
            }
                                

            // Create new job
            Job j = new Job( t, buildObjectForJob,(todoJob) => onBuildObjectComplete(buildObjectForJob,t) );
            t.PendingJob = j;

            mapControllerInstance.Map.jobQueue.Enqueue(j);
        }
        else
        {
            t.Type = tileMode;
        }
    }

}
