using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path_TileGraph{

    public Dictionary<Tile, Path_Node<Tile>> nodes;
    // Constructs graph above map
    public Path_TileGraph(Map map){

        nodes = new Dictionary<Tile, Path_Node<Tile>>();
        // Create node graph for the whole world
        // No tiles that are empty
        // No tiles that have walls
        for(int x = 0; x < map.Width; x++){
            for(int y = 0; y < map.Height; y++){
                for(int z = 0; z < map.Depth; z++){
                    Tile tile = map.getTileAt(x,y,z);
                    // TODO: add stairs and such for Z axis
                    //if(tile.movementCost > 0){ // 0 movementCost is impassable
                        Path_Node<Tile> n = new Path_Node<Tile>();
                        n.data = tile;
                        nodes.Add(tile,n);
                    //}
                }
            }
        }

        // Create edges for nodes
        foreach(Tile tile in nodes.Keys){

            Path_Node<Tile> node = nodes[tile];
            // Get neighbour
            Tile[] neighbours = tile.getNeighbourTiles(true);

            List<Path_Edge<Tile>> edges = new List<Path_Edge<Tile>>();

            for(int i = 0; i < neighbours.Length; i++){
                if(neighbours[i] != null && neighbours[i].movementCost > 0){
                    // Neighbor is valid
                    Path_Edge<Tile> edge = new Path_Edge<Tile>();
                    edge.cost = neighbours[i].movementCost;
                    edge.pathNode = nodes[ neighbours[i] ];
                    edges.Add(edge);
                }
            }

            node.edges = edges.ToArray();
        }
    }

}
