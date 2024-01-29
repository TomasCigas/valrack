using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.AI;
using System;

public class mapInstance
{
    Tile [,,] tiles;
    int width;
    int height;
    int depth;

    List<Character> characters;

    public JobQueue jobQueue;

    Dictionary<string,BuildObject> stringBuildObjectDictionary;

    Action<BuildObject> callbackBuildObjectCreated;
    Action<Character> callbackCharacterCreated;


    public mapInstance(int height = 100, int width = 100, int depth = 1){
        // TODO: expand JobQueue class
        jobQueue = new JobQueue();

        this.width = width;
        this.height = height;
        this.depth = depth;
        tiles = new Tile[width,height,depth];
        for(int x = 0; x < width; x++){
            for (int y = 0 ; y < height; y++){
                for(int z=0; z < depth;z++){
                    tiles[x,y,z] = new Tile(this,x,y,z);
                }
                
            }
        }


        Debug.Log("World created with parameters: "+width+","+height+","+depth+".");

        CreateBuildObjectPrototypes();

        characters = new List<Character>();


    }


    public void Update(float deltaTime){
        foreach(Character c in characters){
            c.Update(deltaTime);
        }
    }

    public Character CreateCharacter(Tile tile){

        //tiles[ width/2,height/2,0 ]
        Character c = new Character(tile);
        characters.Add(c);

        if(callbackCharacterCreated != null){
            callbackCharacterCreated(c);
        }
        return c;
    }

    void CreateBuildObjectPrototypes(){
        stringBuildObjectDictionary = new Dictionary<string, BuildObject>();

        BuildObject wallPrototype = BuildObject.CreatePrototype(
            "Wall",0,Tile.HoldStrength.Heavy,1,1,true
        );

        stringBuildObjectDictionary.Add("Wall",wallPrototype);
        Debug.Log("Added Wall to BuildObjectDictionary.");
    }
    
    public void PlaceBuildObject(string objectType, Tile t){

        if( !stringBuildObjectDictionary.ContainsKey(objectType) ){
            Debug.LogError("Object "+objectType+" does not exist in dictionary.");
            return;
        }

        BuildObject obj = BuildObject.PlaceInstance(stringBuildObjectDictionary[objectType],t);
        if(obj == null){
            return;
        }

        if(callbackBuildObjectCreated != null){
            callbackBuildObjectCreated(obj);
        }

    }
    /*
    void InstantiateBuildObjectPrototype(){

    }
    */

    public int Width { get => width;}
    public int Height { get => height;}
    public int Depth { get => depth;}

    public void RandomizeTiles(){
        for(int x = 0; x < width; x++){
            for (int y = 0 ; y < height; y++){
                for(int z=0; z < depth;z++){
                    if(UnityEngine.Random.Range(0,2) == 0){
                        tiles[x,y,z].OriginType = Tile.TileType.Grass;
                    }else{
                        tiles[x,y,z].OriginType = Tile.TileType.Stone;
                    }
                }
            }
        }
    }

    public void FillTiles(Tile.TileType tileType){

        for(int x = 0; x < width; x++){
            for (int y = 0 ; y < height; y++){
                for(int z=0; z < depth;z++){
                        tiles[x,y,z].OriginType = tileType;
                }
            }
        }
    }


    public Tile getTileAt(int x, int y, int z){
        if(x < 0 || x > Width){
            return null;
        }

        if(y < 0 || y > Width){
            return null;
        }

        if(z < 0 || z > Width){
            return null;
        }

        return tiles[x,y,z];
    }

    public bool buildObjectPlacementValidation(string buildObjectPrototype, Tile t){
        BuildObject buildObject = stringBuildObjectDictionary[buildObjectPrototype];

        if(t.BuildObject != null){
            return false;
        }

        return buildObject.
            funcPositionValidation(
                t,
                buildObject
            );
    }

    public BuildObject GetBuildObject(string objectType){
        if( ! stringBuildObjectDictionary.ContainsKey(objectType)){
            Debug.LogError("Prototype "+objectType+" does not exist.");
            return null;
        }
        return stringBuildObjectDictionary[objectType];
    }

    public void RegisterBuildObjectCreated(Action<BuildObject> callback){
        callbackBuildObjectCreated += callback;
    }

    public void UnRegisterBuildObjectCreated(Action<BuildObject> callback){
        callbackBuildObjectCreated -= callback;
    }

    public void RegisterCharacterCreated(Action<Character> callback){
        callbackCharacterCreated += callback;
    }

    public void UnRegisterCharacterCreated(Action<Character> callback){
        callbackCharacterCreated -= callback;
    }
}
