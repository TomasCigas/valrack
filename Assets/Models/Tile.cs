using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Tile
{
    public enum TileType {Empty, // For Logic
        Grass, Stone,  // Natural tiles
        Floor          // Build tiles
    };

    public enum HoldStrength{
        None,Light,Heavy
    }

    HoldStrength tileStrength = HoldStrength.Heavy;

    TileType originType;
    TileType currentType = TileType.Empty;
    public Action<Tile> callBackTileTypeChanged;

    ItemObject itemObject;
    BuildObject buildObject;

    private Job pendingJob;

    mapInstance map;
    
    int x;
    int y;
    int z;


    public TileType OriginType { get => originType; 
        set{ 
            originType = value; 
            Type = originType;
        }
    }

    public TileType Type { 
        get{
            return currentType;
        }  
        set{
            TileType oldType = currentType;
            currentType = value;

            // TileType did not change
            if(oldType == currentType){
                return;
            }

            if(currentType == TileType.Empty){
                currentType = originType;
            }

            assignStrength();

            // Callback To OnTileChanged
            if(callBackTileTypeChanged != null){
                callBackTileTypeChanged(this);
            }
            
        }
    }
    public int X { get => x;}
    public int Y { get => y;}
    public int Z { get => z;}
    public ItemObject ItemObject { get => itemObject; set => itemObject = value; }
    public BuildObject BuildObject { get => buildObject; set => buildObject = value; }
    public mapInstance Map { get => map; protected set => map = value; }
    public HoldStrength TileStrength { get => tileStrength; set => tileStrength = value; }
    public Job PendingJob { get => pendingJob; set => pendingJob = value; }

    public Tile(mapInstance map , int x, int y, int z){
        this.map = map;
        this.x= x;
        this.y = y;
        this.z = z;
    }

    public void RegisterTileTypeChangedCallback(Action<Tile> callback){
        this.callBackTileTypeChanged += callback;
    }

    public void UnRegisterTileTypeChangedCallback(Action<Tile> callback){
        this.callBackTileTypeChanged += callback;
    }

    public bool AssignBuildObject(BuildObject buildObject){
        if(buildObject == null){
            this.buildObject = null;
            return false;
        }

        if(this.buildObject == null){
            this.buildObject = buildObject;
            return true;
        }

        Debug.LogError("Tile is already occupied.");
        return false;
    }

    void assignStrength(){
        switch (currentType)
        {
            case TileType.Empty:
                tileStrength = HoldStrength.None;
                break;
            case TileType.Grass:
                tileStrength = HoldStrength.Light;
                break;
            
            case TileType.Stone:
                tileStrength = HoldStrength.Heavy;
                break;

            case TileType.Floor:
                tileStrength = HoldStrength.Heavy;
                break;

            default:
                tileStrength = HoldStrength.None;
                Debug.Log("Type of the tile has no assigned holdStrength value");
                break;
        }
    }

    
    public bool isNeighbour(Tile tile,bool diagonals = false){
        if( 
            (X == tile.X && (Y == tile.Y +1 || Y == tile.Y - 1 )) //up and down
            ||
            (Y == tile.Y && (X == tile.X +1 || X == tile.X - 1 )) // left and right
        ){
            return true;            
        }

        if(diagonals){
            if(
            (Y == tile.Y-1 && (X == tile.X +1 || X == tile.X - 1 )) // upper right and upper left
            ||
            (Y == tile.Y+1 && (X == tile.X +1 || X == tile.X - 1 )) // lower right and lower left
            ){
                return true;
            }
        }

        return false;
    }

}
