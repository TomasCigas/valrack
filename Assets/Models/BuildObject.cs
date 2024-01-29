using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public class BuildObject
{

    Tile originTile;
    Tile.HoldStrength requiredHoldStrength;

    string objectType;

    float movementCost = 1f;

    int width;
    int height;

    bool linksToNeighbour;

    Action<BuildObject> callbackOnChanged;

    public Func<Tile,BuildObject,bool> funcPositionValidation;

    public string ObjectType { get => objectType; protected set => objectType = value; }
    public Tile OriginTile { get => originTile; protected set => originTile = value; }
    public bool LinksToNeighbour { get => linksToNeighbour; protected set => linksToNeighbour = value; }
    public Tile.HoldStrength RequiredHoldStrength { get => requiredHoldStrength; protected set => requiredHoldStrength = value; }

    static public BuildObject CreatePrototype(string objectType, float movementCost,
        Tile.HoldStrength holdStrength,
        int width = 1, int height = 1,
        bool linksToNeighbour = false
        ){
        BuildObject obj = new BuildObject();
        obj.objectType = objectType;
        obj.height = height;
        obj.width = width;
        obj.linksToNeighbour = linksToNeighbour;
        obj.requiredHoldStrength = holdStrength;

        obj.funcPositionValidation = obj.strengthValidation;

        return obj;
    }

    static public BuildObject PlaceInstance( BuildObject proto,Tile originTile){

        // Check if tile has enough strength
        if( proto.funcPositionValidation(originTile,proto) ){
            Debug.LogError("Object "+proto.objectType+" cannot be placed on "+originTile.Type);
            return null;
        }

        BuildObject obj = new BuildObject();

        obj.originTile = originTile;
        obj.objectType = proto.objectType;
        obj.height = proto.height;
        obj.width = proto.width;
        obj.linksToNeighbour = proto.linksToNeighbour;
        obj.requiredHoldStrength = proto.requiredHoldStrength;

        obj.originTile = originTile;

        if(!obj.originTile.AssignBuildObject(obj)){
            return null;
        }

        if(obj.linksToNeighbour){

            int x = originTile.X;
            int y = originTile.Y;
            int z = originTile.Z;

            Tile tmpTile;
            if((tmpTile = obj.checkWorldTile(x,y+1,z)) != null){
                tmpTile.BuildObject.callbackOnChanged(tmpTile.BuildObject);
            }
            // East
            if((tmpTile = obj.checkWorldTile(x+1,y,z)) != null){
                tmpTile.BuildObject.callbackOnChanged(tmpTile.BuildObject);
            }
            // South
            if((tmpTile = obj.checkWorldTile(x,y-1,z)) != null){
                tmpTile.BuildObject.callbackOnChanged(tmpTile.BuildObject);
            }
            // West
            if((tmpTile = obj.checkWorldTile(x-1,y,z)) != null){
                tmpTile.BuildObject.callbackOnChanged(tmpTile.BuildObject);
            }
        }
        
        return obj;

    }

    Tile checkWorldTile(int x, int y, int z){
        Tile objTile = originTile.Map.getTileAt(x,y,z);

        if(objTile == null || objTile.BuildObject == null){
            return null;
        }

        if(objTile.BuildObject.ObjectType == objectType){

            return objTile;
            
        }

        return null;
    }

    public void RegisterOnChangedCallback(Action<BuildObject> callbackOnChanged){
        this.callbackOnChanged += callbackOnChanged;
    }

    public void UnRegisterOnChangedCallback(Action<BuildObject> callbackOnChanged){
        this.callbackOnChanged -= callbackOnChanged;
    }

    public bool positionIsValid(Tile t,BuildObject buildObject){
        return funcPositionValidation(t,buildObject);
    }

    bool strengthValidation(Tile tile, BuildObject buildObjectToPlace){
        if(tile.TileStrength < buildObjectToPlace.RequiredHoldStrength){
            Debug.Log("This tile ("+tile.TileStrength+") does not support weight of this object ("+buildObjectToPlace.requiredHoldStrength+")");
            return false;
        }
        return true;
    }

    bool doorValidation(Tile tile){
        return true;
    }

}
