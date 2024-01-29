using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character
{

    float speed = 2f;

    public float X{
        get{
            return Mathf.Lerp(currentTile.X,destTile.X, movementPerc);
        }
    }
    public float Y{
        get{
            return Mathf.Lerp(currentTile.Y,destTile.Y, movementPerc);
        }
    }

    public Tile CurrentTile { get => currentTile; protected set => currentTile = value; }

    int z;

    Tile currentTile;
    Tile destTile; // No movement == currentile
    float movementPerc;

    public Character(Tile tile){
        currentTile = tile;
        destTile = tile;
    }

    public void Update(float deltaTime){
        if(currentTile == destTile){
            return;
        }

        float travelDist = Mathf.Sqrt(
            Mathf.Pow(currentTile.X - destTile.X,2) + 
            Mathf.Pow(currentTile.Y - destTile.Y,2)
        );

        float distThisFrame = speed * deltaTime;

        float percThisFrame = distThisFrame/travelDist;

        movementPerc += percThisFrame;

        if(movementPerc >= 1){
            currentTile = destTile;
            movementPerc = 0;
        }

    }





    public void SetDestination(Tile tile){
        if(currentTile.isNeighbour(tile,true) == false){
            Debug.Log("Character - Tile is not adjescent");
        }
        destTile = tile;
    }

}
