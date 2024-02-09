using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class TileSpriteController : MonoBehaviour
{

    
    public Sprite floorSprite;
    public Sprite grassSprite;
    public Sprite stoneSprite;

    Dictionary<Tile,GameObject> tileDictionary;

    public Map mapInstance{
        get => MapController.Instance.Map;
    }


    int currentZPosition{get => CameraController.currentZPosition;}

    // Start is called before the first frame update
    void Start()
    {

        tileDictionary = new Dictionary<Tile, GameObject>();

        // GameObject for every tile - visual
        for(int x = 0; x < mapInstance.Width; x++){
            for(int y = 0; y < mapInstance.Height; y++){
                for(int z = 0; z < mapInstance.Depth; z++){
                    GameObject tile_GO = new GameObject();

                    Tile tile_data = mapInstance.getTileAt(x, y, z);

                    tileDictionary.Add(tile_data, tile_GO);

                    tile_GO.name = "Tile_"+ x + "_" + y + "_" + z;
                    tile_GO.transform.position = new Vector3(tile_data.X, tile_data.Y, tile_data.Z);
                    tile_GO.transform.SetParent(this.transform);

                    // Add sprite renderer
                    SpriteRenderer tile_sr = tile_GO.AddComponent<SpriteRenderer>();

                    // Add callback
                    tile_data.RegisterTileTypeChangedCallback(OnTileTypeChange);
                    tile_data.RegisterTileTypeChangedCallback(MapController.Instance.OnTileTypeChange);
                    
                }
            }
        }

        // FIX: this should be called on Map controller when SpriteController is done
        // mapInstance.RandomizeTiles();
        mapInstance.FillTiles(Tile.TileType.Grass);
    }

    void DestroyAllUnseenTiles(){
        while(tileDictionary.Count > 0){
            Tile tile_data = tileDictionary.Keys.First();
            GameObject tile_GO = tileDictionary[tile_data];

            tileDictionary.Remove(tile_data);

            tile_data.UnRegisterTileTypeChangedCallback( OnTileTypeChange );

            Destroy(tile_GO);
        }
    }


    void OnTileTypeChange(Tile tile_data){

        if(tileDictionary.ContainsKey(tile_data) == false){
            Debug.LogError("Tile"+tile_data.ToString()+" is not in dictionary.");
            return;
        }

        GameObject tile_OB = tileDictionary[tile_data];

        if(tile_OB == null){
            Debug.LogError("Tile"+tile_data.ToString()+" has no object in dictionary.");
            return;
        }

        if(tile_data.Type == Tile.TileType.Empty){
            tile_OB.GetComponent<SpriteRenderer>().sprite = null;
        }else if(tile_data.Type == Tile.TileType.Grass) {
            tile_OB.GetComponent<SpriteRenderer>().sprite = grassSprite;
        }else if(tile_data.Type == Tile.TileType.Stone){
            tile_OB.GetComponent<SpriteRenderer>().sprite = stoneSprite;
        }else if(tile_data.Type == Tile.TileType.Floor){
            tile_OB.GetComponent<SpriteRenderer>().sprite = floorSprite;
        }else{
            Debug.LogError("Tile type is set to unrecognized value");
        }
    }



    // Obsolete?
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

}
