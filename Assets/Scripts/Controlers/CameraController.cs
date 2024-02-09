using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.EventSystems;

public class CameraController : MonoBehaviour
{
    public static int MAX_ORTOGRAPHIC_SIZE = 25;
    public static int MIN_ORTOGRAPHIC_SIZE = 2;

    public MapController mapControllerInstance{get => MapController.Instance;}

    Vector3 lastFramePos;

    Vector3 currFramePosition;

    public static int currentZPosition = 0;

    public float cameraMovementSpeed;

    // Start is called before the first frame update
    void Start()
    {
        cameraMovementSpeed = 0.1f;
    }

    // Update is called once per frame
    void Update()
    {

        currFramePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        currFramePosition.z = 0;

        UpdateCameraMouseMovement();
        UpdateCameraKeyBoardMovement();

    }

    void UpdateCameraKeyBoardMovement(){

        if(Input.GetKey(KeyCode.LeftShift)){
            cameraMovementSpeed = 0.5f;
        }else{
            cameraMovementSpeed = 0.1f;
        }

        Vector3 cameraMov = new Vector3();
        // UP
        if(Input.GetKey(KeyCode.W)&& currFramePosition.y < mapControllerInstance.Map.Height){
            cameraMov = cameraMov + new Vector3(0,cameraMovementSpeed,0);
        }
        // Down
        if(Input.GetKey(KeyCode.S) && currFramePosition.y > 0 ){
            cameraMov = cameraMov + new Vector3(0,-cameraMovementSpeed,0);
        }
        // Right
        if(Input.GetKey(KeyCode.D) && currFramePosition.x < mapControllerInstance.Map.Width){
            cameraMov = cameraMov + new Vector3(cameraMovementSpeed,0,0);
        }
        // Left
        if(Input.GetKey(KeyCode.A) && currFramePosition.x > 0){
            cameraMov = cameraMov + new Vector3(-cameraMovementSpeed,0,0);
        }

        Camera.main.transform.Translate(cameraMov);

        // Zoom
        if(Input.GetKey(KeyCode.E) && Camera.main.orthographicSize < MAX_ORTOGRAPHIC_SIZE){
            Camera.main.orthographicSize++;
        }else if(Input.GetKey(KeyCode.Q) && Camera.main.orthographicSize > MIN_ORTOGRAPHIC_SIZE){
            Camera.main.orthographicSize--;
        }

        // Z level
        if(Input.GetKeyDown(KeyCode.R) && currentZPosition < mapControllerInstance.Map.Depth){
            currentZPosition++;
        }else if(Input.GetKeyDown(KeyCode.F) && currentZPosition < mapControllerInstance.Map.Depth){
            currentZPosition--;
        }

    }

    void UpdateCameraMouseMovement(){
        // --Camera Control--
        if(Input.GetMouseButton(1) || Input.GetMouseButton(2)){
            Vector3 diff = lastFramePos - currFramePosition;

            Camera.main.transform.Translate(diff);

        }else if(Input.GetAxis("Mouse ScrollWheel") < 0f && Camera.main.orthographicSize < MAX_ORTOGRAPHIC_SIZE){ 
            Camera.main.orthographicSize++;
        }else if(Input.GetAxis("Mouse ScrollWheel") > 0f && Camera.main.orthographicSize > MIN_ORTOGRAPHIC_SIZE){ 
            Camera.main.orthographicSize--;
        }

        lastFramePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        lastFramePos.z = 0;
    }

    bool CheckCameraPosition(){
        if( Camera.main.transform.position.x < 0 ||Camera.main.transform.position.y < 0){
            return false;
        }
        if( Camera.main.transform.position.x > mapControllerInstance.Map.Width || Camera.main.transform.position.y > mapControllerInstance.Map.Height){
            return false;
        }
        return true;
    }

}
