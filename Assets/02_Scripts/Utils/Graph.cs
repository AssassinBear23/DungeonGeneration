using AYellowpaper.SerializedCollections;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Dungeon.DataStructures
{
    /// <summary>  
    /// Represents a graph data structure that supports generic types.  
    /// </summary>  
    /// <typeparam name="T">The type of elements stored in the graph.</typeparam>
    [Serializable]
    public class Graph<T>
    {
        [SerializeField] private SerializedDictionary<T, List<T>> adjacencies;

        /// <summary>
        /// Creates a new instance of the <see cref="Graph{T}"/> class.
        /// </summary>
        public Graph()
        {
            adjacencies = new();
        }

        /// <summary>
        /// Clears the graph, removing all nodes and edges.
        /// </summary>
        public void Clear()
        {
            adjacencies.Clear();
        }

        /// <summary>
        /// Adds a node to the graph.
        /// </summary>
        /// <param name="node">The node to add to the graph.</param>
        public void AddNode(T node)
        {
            if (adjacencies.ContainsKey(node))
            {
                return;
            }
            adjacencies[node] = new List<T>();
        }

        /// <summary>
        /// Adds an edge between two nodes in the graph.
        /// </summary>
        /// <param name="fromNode">The starting node of the edge.</param>
        /// <param name="toNode">The ending node of the edge.</param>
        public void AddEdge(T fromNode, T toNode)
        {
            if (!adjacencies.ContainsKey(fromNode) || !adjacencies.ContainsKey(toNode))
            {
                ////debug.log("One or both nodes do not exist in graph");
                return;
            }
            adjacencies[fromNode].Add(toNode);
            adjacencies[toNode].Add(fromNode);
        }

        /// <summary>
        /// Returns the amount of nodes in the graph.
        /// </summary>
        /// <returns>The number of nodes in the graph.</returns>
        public int GetNodeCount()
        {
            return adjacencies.Count;
        }

        /// <summary>
        /// Returns a list of all nodes adjacent to the given node.
        /// </summary>
        /// <param name="node">The node whose neighbors are to be returned.</param>
        /// <returns>A list of all nodes adjacent to the given node.</returns>
        public List<T> GetNeighbours(T node)
        {
            if (!adjacencies.ContainsKey(node))
            {
                ////debug.log("Node does not exist in graph");
                return null;
            }
            return adjacencies[node];
        }

        /// <summary>
        /// Returns a list of all nodes in the graph.
        /// </summary>
        /// <returns>A list of all nodes in the graph.</returns>
        public List<T> GetNodes()
        {
            return adjacencies.Keys.ToList();
        }

        /// <summary>
        /// Tries to remove a node from the graph. If it splits the graph apart, stop the removal.
        /// </summary>
        /// <param name="nodeToRemove">The node to remove</param>
        /// <returns>True if the room was removed, false if it would split the graph.</returns>
        public bool TryRemoveNode(T nodeToRemove, T startingNode)
        {
            if (!adjacencies.ContainsKey(nodeToRemove))
            {
                return false;
            }

            if (startingNode == null)
            {
                return false;
            }

            HashSet<T> visited = new();

            RecursiveDFS(startingNode, nodeToRemove, visited);

            // Check if all nodes except the node to remove are visited
            if (visited.Count != adjacencies.Count - 1)
            {
                return false;
            }

            // Remove the node and its references
            foreach (T neighbor in adjacencies[nodeToRemove])
            {
                adjacencies[neighbor].Remove(nodeToRemove);
            }
            adjacencies.Remove(nodeToRemove);

            return true;
        }

        /// <summary>
        /// Performs a recursive depth-first search (DFS) traversal of the graph, 
        /// visiting all nodes connected to <paramref name="current"/> except for <paramref name="nodeToRemove"/>.
        /// Marks each visited node in the <paramref name="visited"/> set.
        /// </summary>
        /// <param name="current">The node currently being visited.</param>
        /// <param name="nodeToRemove">The node to exclude from traversal (as if it were removed from the graph).</param>
        /// <param name="visited">A set of nodes that have already been visited during traversal.</param>
        private void RecursiveDFS(T current, T nodeToRemove, HashSet<T> visited)
        {
            if (!visited.Add(current))
                return;

            foreach (T neighbor in adjacencies[current])
            {
                if (!neighbor.Equals(nodeToRemove) && !visited.Contains(neighbor))
                {
                    RecursiveDFS(neighbor, nodeToRemove, visited);
                }
            }
        }
    }
}