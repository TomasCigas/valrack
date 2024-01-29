using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class BuildObjectSpriteController : MonoBehaviour
{

     public const string WALL_PATH = "Images/BuildObjects/Walls";
    

    Dictionary<BuildObject,GameObject> buildGameDictionary;

    Dictionary<string,Sprite> buildObjectSprites;

    public mapInstance mapInstance{
        get => MapController.Instance.Map;
    }


    int currentZPosition{get => MouseController.currentZPosition;}

    // Start is called before the first frame update
    void Start()
    {
        LoadSprites();
        
        mapInstance.RegisterBuildObjectCreated(OnBuildObjectCreated);

        buildGameDictionary = new Dictionary<BuildObject, GameObject>();

    }

    void LoadSprites(){

        buildObjectSprites = new Dictionary<string,Sprite>();

        Sprite[] wallSprites = Resources.LoadAll<Sprite>(WALL_PATH);

        foreach (Sprite sprite in wallSprites)
        {
            buildObjectSprites.Add(
                sprite.name,
                sprite
            );
            Debug.Log(sprite.name);
        }
    }



    public void OnBuildObjectCreated(BuildObject buildObject){
        // Create GameObj linked to data of buildObject
        GameObject buildObj_GO = new GameObject();

        Tile tile_data = buildObject.OriginTile;

        buildGameDictionary.Add(buildObject,buildObj_GO);

        buildObj_GO.name = buildObject.ObjectType+"_"+
            tile_data.X+"_"+
            tile_data.Y+"_"+
            tile_data.Z+".";

        buildObj_GO.transform.position = new Vector3(tile_data.X,tile_data.Y,tile_data.Z);
        buildObj_GO.transform.SetParent(this.transform,true);

        // Add sprite renderer
        SpriteRenderer obj_sr = buildObj_GO.AddComponent<SpriteRenderer>();
        obj_sr.sprite = getSpriteForBuildObject(buildObject);
        obj_sr.sortingLayerID = SortingLayer.NameToID("BuildObjects");

        // Add callback
        buildObject.RegisterOnChangedCallback(OnBuildObjectChanged);
    }

    public Sprite getSpriteForBuildObject(BuildObject obj){
        if(obj.LinksToNeighbour == false){
            return buildObjectSprites[obj.ObjectType];
        }

        int x = obj.OriginTile.X;
        int y = obj.OriginTile.Y;

        //Debug.Log("Currently selecting sprite for "+obj.ObjectType+" on "+x+","+y);

        string spriteName = obj.ObjectType+"_";

        // North
        if(checkTileAt(x,y+1,obj)){
            spriteName+="N";
        }
        // East
        if(checkTileAt(x+1,y,obj)){
            spriteName+="E";
        }
        // South
        if(checkTileAt(x,y-1,obj)){
            spriteName+="S";
        }
        // West
        if(checkTileAt(x-1,y,obj)){
            spriteName+="W";
        }

        //Debug.Log(spriteName);

        if(buildObjectSprites.ContainsKey(spriteName) == false){
            Debug.LogError("Sprite dictionary does not contain sprite connected to object: "+ obj.ObjectType+"->"+spriteName );
            return null;
        }


        return buildObjectSprites[spriteName];
    }


    public Sprite getSpriteForBuildObject(string objType){
        if(buildObjectSprites.ContainsKey(objType)){
            return buildObjectSprites[objType];
        }else if(buildObjectSprites.ContainsKey(objType+"_")){
            return buildObjectSprites[objType+"_"];
        }
        
        Debug.LogError("Sprite for object "+objType+" does not exist.");
        return null;
        
    }

    bool checkTileAt(int x, int y, BuildObject obj){
        Tile objTile;
        objTile = mapInstance.getTileAt(x, y, currentZPosition);

        if(objTile == null || objTile.BuildObject == null){
            return false;
        }

        if(objTile.BuildObject.ObjectType == obj.ObjectType){


            return true;
            
        }


        return false;
    }

    void OnBuildObjectChanged(BuildObject obj){
        if(!buildGameDictionary.ContainsKey(obj)){
            Debug.LogError("OnBuildObjectChanged - "+obj.ObjectType+" is not in direcory");
            return;
        }

        GameObject gameObject = buildGameDictionary[obj];

        gameObject.GetComponent<SpriteRenderer>().sprite = getSpriteForBuildObject(obj);

    }
    
}
