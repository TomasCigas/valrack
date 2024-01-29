using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.EventSystems;

public class MouseController : MonoBehaviour
{

    public MapController mapControllerInstance{get => MapController.Instance;}

    Vector3 lastFramePos;

    public GameObject circleCursorPrefab;

    Vector3 currFramePosition;
    Vector3 positionStartDrag;
    List<GameObject> dragPreviewList;

    public static int currentZPosition = 0;

    BuildModeController buildModeController;



    // Start is called before the first frame update
    void Start()
    {
        dragPreviewList = new List<GameObject>();
        buildModeController = GameObject.FindAnyObjectByType<BuildModeController>();
    }

    // Update is called once per frame
    void Update()
    {

        currFramePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        currFramePosition.z = 0;

        UpdateDragging();
        UpdateCameraMovement();

    }



    void UpdateDragging()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        // Start drag
        if (Input.GetMouseButtonDown(0))
        {
            positionStartDrag = currFramePosition;
        }

        // Display Drag
        while (dragPreviewList.Count > 0)
        {
            GameObject dragPreview_GO = dragPreviewList[0];
            dragPreviewList.Remove(dragPreview_GO);
            SimplePool.Despawn(dragPreview_GO);
        }

        if (Input.GetMouseButton(0))
        {
            int start_x = Mathf.FloorToInt(positionStartDrag.x);
            int end_x = Mathf.FloorToInt(currFramePosition.x);


            if (end_x < start_x)
            {
                int tmp_x = start_x;
                start_x = end_x;
                end_x = tmp_x;
            }

            int start_y = Mathf.FloorToInt(positionStartDrag.y);
            int end_y = Mathf.FloorToInt(currFramePosition.y);

            if (end_y < start_y)
            {
                int tmp_y = start_y;
                start_y = end_y;
                end_y = tmp_y;
            }

            for (int x = start_x; x <= end_x; x++)
            {
                for (int y = start_y; y <= end_y; y++)
                {
                    Tile t = mapControllerInstance.Map.getTileAt(x, y, currentZPosition);
                    if (t != null)
                    {
                        GameObject dragPreview_GO = SimplePool.Spawn(circleCursorPrefab, new Vector3(x, y, currentZPosition), Quaternion.identity);
                        dragPreviewList.Add(dragPreview_GO);
                    }
                }
            }

        }

        // End drag
        endDrag();
    }

    private void endDrag()
    {
        if (Input.GetMouseButtonUp(0))
        {
            int start_x = Mathf.FloorToInt(positionStartDrag.x);
            int end_x = Mathf.FloorToInt(currFramePosition.x);


            if (end_x < start_x)
            {
                int tmp_x = start_x;
                start_x = end_x;
                end_x = tmp_x;
            }

            int start_y = Mathf.FloorToInt(positionStartDrag.y);
            int end_y = Mathf.FloorToInt(currFramePosition.y);

            if (end_y < start_y)
            {
                int tmp_y = start_y;
                start_y = end_y;
                end_y = tmp_y;
            }

            for (int x = start_x; x <= end_x; x++)
            {
                for (int y = start_y; y <= end_y; y++)
                {


                    Tile t = mapControllerInstance.Map.getTileAt(x, y, currentZPosition);
                    if (t != null){
                        // Tell BuildModeController to build
                        buildModeController.doBuild(t);
                    }
                }
            }

        }
    }

    void UpdateCameraMovement(){
        // --Camera Control--
        if(Input.GetMouseButton(1) || Input.GetMouseButton(2)){
            Vector3 diff = lastFramePos - currFramePosition;
            Camera.main.transform.Translate(diff);
        }else if(Input.GetAxis("Mouse ScrollWheel") < 0f && Camera.main.orthographicSize < 25){ 
            Camera.main.orthographicSize++;
        }else if(Input.GetAxis("Mouse ScrollWheel") > 0f && Camera.main.orthographicSize > 2){ 
            Camera.main.orthographicSize--;
        }

        lastFramePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        lastFramePos.z = 0;
    }

}
