using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Priority_Queue;
using Unity.VisualScripting;
using System.Linq;

public class Path_AStar
{
    Queue<Tile> path;


    public Path_AStar(mapInstance map, Tile tileStart, Tile tileEnd){

        if(map.pathFindingGraph == null){
            map.pathFindingGraph = new Path_TileGraph(map);
        }

        Dictionary<Tile,Path_Node<Tile>> nodes = map.pathFindingGraph.nodes;

        // Validating tile graph

        if(nodes.ContainsKey(tileStart) == false){
            Debug.LogError("A* starting at nonexistent node");
            return;
        }

        if(nodes.ContainsKey(tileEnd) == false){
            Debug.LogError("A* ending at nonexistent node");
            return;
        }



        // =============================
        // Begin A*
        // =============================

        // Define start and end nodes
        Path_Node<Tile> start = nodes[tileStart];
        Path_Node<Tile> goal = nodes[tileEnd];

        List<Path_Node<Tile>> closedSet = new List<Path_Node<Tile>>();
        
        //List<Path_Node<Tile>> openSet = new List<Path_Node<Tile>>();

        //openSet.Add(map.pathFindingGraph.nodes[tileStart]);

        SimplePriorityQueue<Path_Node<Tile>> openSet = new SimplePriorityQueue<Path_Node<Tile>>();

        openSet.Enqueue(start, 0);

        // Navigated nodes
        Dictionary<Path_Node<Tile>,Path_Node<Tile>> cameFrom = new Dictionary<Path_Node<Tile>, Path_Node<Tile>>();

        Dictionary<Path_Node<Tile>,float> g_score = new Dictionary<Path_Node<Tile>, float>();

        // Initialize g_score for all nodes (Current movement cost)
        foreach(Path_Node<Tile> n in nodes.Values){
            g_score[n] = Mathf.Infinity;
        }

        // Starting G score is 0, since we are here
        g_score[ start ] = 0;


        Dictionary<Path_Node<Tile>,float> f_score = new Dictionary<Path_Node<Tile>, float>();

        // Initialize f_score heuristic for all nodes (Air distance from start to finish)
        foreach(Path_Node<Tile> n in nodes.Values){
            f_score[n] = Mathf.Infinity;
        }
        f_score[ start ] = heuristicCostEstimate(start, goal);

        // Start Search
        while(openSet.Count > 0){
            Path_Node<Tile> current = openSet.Dequeue();
            
            // Goal found
            if(current == goal ){
                // reconstruct path
                reconstructPath(cameFrom,goal);
                return;
            }

            closedSet.Add(current);

            if(current == null){
                Debug.LogError("Current tile node: "+current.data.X+","+current.data.Y+" is null");
            }
            if(current.edges == null){
                Debug.LogError("Current tile node's : "+current.data.X+","+current.data.Y+" edges are null");
            }

            // Check neighbours
            foreach(Path_Edge<Tile> neighbor in current.edges){
                //neighbor.pathNode.data.Type = Tile.TileType.Floor;
                if( closedSet.Contains(neighbor.pathNode) == true ){ // Already searched
                    continue;
                }

                float cumulativeGScore = g_score[current] + distanceBetween(current,neighbor.pathNode);

                if( openSet.Contains(neighbor.pathNode) && cumulativeGScore >= g_score[neighbor.pathNode]){
                    continue;
                }

                cameFrom[neighbor.pathNode] = current;
                g_score[neighbor.pathNode] = cumulativeGScore;
                f_score[neighbor.pathNode] = g_score[neighbor.pathNode] + heuristicCostEstimate(neighbor.pathNode, goal);

                if(openSet.Contains(neighbor.pathNode) == false){
                    openSet.Enqueue(neighbor.pathNode,f_score[neighbor.pathNode]);
                }
            }

        }

        // There is no Path !
        Debug.Log("A* goal cannot be reached");
        return;

    }
    
    float distanceBetween( Path_Node<Tile> a, Path_Node<Tile> b ){
        float baseDistance = heuristicCostEstimate(a,b);

        return baseDistance * b.data.movementCost;

    }
    float heuristicCostEstimate( Path_Node<Tile> a, Path_Node<Tile> b ){
        return Mathf.Sqrt(
            Mathf.Pow(a.data.X - b.data.X, 2) +
            Mathf.Pow(a.data.Y - b.data.Y, 2)
        );
    }

    void reconstructPath(Dictionary<Path_Node<Tile>,Path_Node<Tile>> cameFrom, Path_Node<Tile> current){
        
        Queue<Tile> reversedPath = new Queue<Tile>();

        if(current == null){
            Debug.LogError("No current");
        }

        if(current.data == null){
            Debug.LogError("No current data");
        }

        reversedPath .Enqueue(current.data);

        while( cameFrom.ContainsKey(current) ){
            current = cameFrom[current];
            reversedPath.Enqueue(current.data);
        }
        
        path = new Queue<Tile>(reversedPath.Reverse());
    }


    public Tile GetNextTile(){
        return path.Dequeue();
    }

    public int Length(){
        if(path == null){
            return 0;
        }
        return path.Count;
    }

}
