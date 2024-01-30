using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JobSpriteController : MonoBehaviour
{

    Dictionary<Job,GameObject> jobGameObjectDictionary;

    BuildObjectSpriteController buildObjectSpriteController;

    // Start is called before the first frame update
    void Start()
    {
        buildObjectSpriteController = GameObject.FindObjectOfType<BuildObjectSpriteController>();

        MapController.Instance.Map.jobQueue.RegisterJobCreatedCallback(OnJobCreated);
        jobGameObjectDictionary = new Dictionary<Job, GameObject>();
    }

    void OnJobCreated(Job j){
        if(jobGameObjectDictionary.ContainsKey(j)){
            //Debug.Log("Job is already created: probably requeued");
            return;
        }

        GameObject job_GO = new GameObject();

        Sprite sprite = buildObjectSpriteController.getSpriteForBuildObject(
            j.buildObjectType
        );

        jobGameObjectDictionary.Add(j,job_GO);

        job_GO.name = "JOB_"+j.buildObjectType+"_"+
            j.jobTile.X+"_"+
            j.jobTile.Y+"_"+
            j.jobTile.Z+".";

        job_GO.transform.position = new Vector3(j.jobTile.X,j.jobTile.Y,j.jobTile.Z);
        job_GO.transform.SetParent(this.transform,true);

        // Add sprite renderer
        SpriteRenderer obj_sr = job_GO.AddComponent<SpriteRenderer>();

        obj_sr.sprite = sprite;
        obj_sr.color = new Color(4f,4f,4f,0.75f);

        obj_sr.sortingLayerID = SortingLayer.NameToID("Jobs");
        // Decides wheter job is completed or canceled

        j.RegisterJobCompleteCallback(OnJobEnded);
        j.RegisterJobCancelCallback(OnJobEnded);

        Debug.Log("Job created on "+j.jobTile.X+","+j.jobTile.Y);

    }

    void OnJobEnded(Job j){
        // Decides wheter job is completed or canceled
        // Delete sprites
        GameObject job_GO = jobGameObjectDictionary[j];

        j.UnRegisterJobCompleteCallback(OnJobCreated);
        j.UnRegisterJobCancelCallback(OnJobCreated);

        Destroy(job_GO);


    }
}
