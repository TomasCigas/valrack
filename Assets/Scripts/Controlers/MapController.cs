using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class MapController : MonoBehaviour
{

    public static MapController Instance{get; protected set;}
    public Map Map{get;protected set;}

    int currentZPosition{get => CameraController.currentZPosition;}

    // Start is called before the first frame update
    void OnEnable()
    {
        // Create empty map
        Instance = this;
        this.Map = new Map(100,100,2);

        // Center Camera
        Camera.main.transform.Translate(
            new Vector3(
                Map.Width/2,
                Map.Height/2,
                Camera.main.transform.position.z
            )
        );

    }

    void Update(){

        // ADD: Controls
        Map.Update(Time.deltaTime);
    }

    public void CreateTest(){
        Map.CreateTest();
        Path_TileGraph graph = new Path_TileGraph(Map);
    }


    public Tile getTileAtMapPos(Vector3 pos){
        int x = Mathf.FloorToInt(pos.x);
        int y = Mathf.FloorToInt(pos.y);
        int z = Mathf.FloorToInt(pos.z);

        return Map.getTileAt(x,y,z);

    }
    public void OnTileTypeChange(Tile tile_data){
        // TODO fix graph on this tile
        Map.InvalidatePathGraph();
    }
    
}
