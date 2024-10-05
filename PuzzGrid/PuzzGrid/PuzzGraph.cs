using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PuzzGraph<TPuzzGraph, TPuzzNode, TPuzzEdge, TPuzzEntity> : MonoBehaviour 
    where TPuzzGraph : PuzzGraph<TPuzzGraph, TPuzzNode, TPuzzEdge, TPuzzEntity>
    where TPuzzNode : IPuzzNode<TPuzzGraph, TPuzzNode, TPuzzEdge, TPuzzEntity> 
    where TPuzzEdge : IPuzzEdge<TPuzzGraph, TPuzzNode, TPuzzEdge, TPuzzEntity>
    where TPuzzEntity : PuzzEntity<TPuzzGraph, TPuzzNode, TPuzzEdge, TPuzzEntity>
{
    public Dictionary<string,TPuzzNode> Nodes;

    private void OnDrawGizmosSelected()
    {
        if (Nodes == null) return;
        foreach (var node in Nodes.Values)
        {
            Gizmos.DrawSphere(node.Position, 0.1f);
            foreach (var edge in node.OutEdges.Values)
            {
                Gizmos.DrawLine(edge.Start.Position, edge.End.Position);
            }
        }
    }
    
}

public interface IPuzzNode<TPuzzGraph, TPuzzNode, TPuzzEdge, TPuzzEntity> 
    where TPuzzGraph : PuzzGraph<TPuzzGraph, TPuzzNode, TPuzzEdge, TPuzzEntity>
    where TPuzzNode : IPuzzNode<TPuzzGraph, TPuzzNode, TPuzzEdge, TPuzzEntity> 
    where TPuzzEdge : IPuzzEdge<TPuzzGraph, TPuzzNode, TPuzzEdge, TPuzzEntity>
    where TPuzzEntity : PuzzEntity<TPuzzGraph, TPuzzNode, TPuzzEdge, TPuzzEntity>
{
    //public List<TPuzzNode> Neighbours {get;}
    public Dictionary<string, TPuzzEdge> OutEdges {get;}
    public IPuzzNodeType Type { get; }

    public Vector3 Position { get; }
    public List<TPuzzEntity> Entities { get; }
    public string NodeID { get; }
}

public interface IPuzzEdge<TPuzzGraph, TPuzzNode, TPuzzEdge, TPuzzEntity> 
    where TPuzzGraph : PuzzGraph<TPuzzGraph, TPuzzNode, TPuzzEdge, TPuzzEntity>
    where TPuzzNode : IPuzzNode<TPuzzGraph, TPuzzNode, TPuzzEdge, TPuzzEntity> 
    where TPuzzEdge : IPuzzEdge<TPuzzGraph, TPuzzNode, TPuzzEdge, TPuzzEntity>
    where TPuzzEntity : PuzzEntity<TPuzzGraph, TPuzzNode, TPuzzEdge, TPuzzEntity>
{
    public TPuzzNode Start { get; }
    public TPuzzNode End { get; }

    public Vector3 GetPosition(float t)
    {
        return Start.Position + t*(End.Position - Start.Position);
    }
    
    public (float, float, Vector3) GetNearestPoint(Vector3 point)
    {
        var start = Start.Position;
        var end = End.Position;
        var dir = (end - start).normalized;
        var length = (end - start).magnitude;
        var t = Vector3.Dot(dir, point - start);
        var clamped= Mathf.Clamp(t, 0, length);
        return (t, clamped, start + t * dir);
    }
}

public interface IPuzzNodeType
{
    
}

public abstract class PuzzEntity<TPuzzGraph, TPuzzNode, TPuzzEdge, TPuzzEntity> :MonoBehaviour 
    where TPuzzGraph : PuzzGraph<TPuzzGraph, TPuzzNode, TPuzzEdge, TPuzzEntity>
    where TPuzzNode : IPuzzNode<TPuzzGraph, TPuzzNode, TPuzzEdge, TPuzzEntity> 
    where TPuzzEdge : IPuzzEdge<TPuzzGraph, TPuzzNode, TPuzzEdge, TPuzzEntity>
    where TPuzzEntity : PuzzEntity<TPuzzGraph, TPuzzNode, TPuzzEdge, TPuzzEntity>
{
    public TPuzzNode Node;
    public TPuzzGraph Grid;

    //GUID Guid { get; }

    // public abstract PuzzEntityState<TPuzzGraph, TPuzzNode, TPuzzEdge, TPuzzEntity> GetState();
    // public abstract void  RestoreState(PuzzEntityState<TPuzzGraph, TPuzzNode, TPuzzEdge, TPuzzEntity> state);

}
//
// public abstract class PuzzEntityState< TPuzzGraph, TPuzzNode, TPuzzEdge, TPuzzEntity> 
//     where TPuzzGraph : PuzzGraph<TPuzzGraph, TPuzzNode, TPuzzEdge, TPuzzEntity>
//     where TPuzzNode : IPuzzNode<TPuzzGraph, TPuzzNode, TPuzzEdge, TPuzzEntity> 
//     where TPuzzEdge : IPuzzEdge<TPuzzGraph, TPuzzNode, TPuzzEdge, TPuzzEntity>
//     where TPuzzEntity : PuzzEntity<TPuzzGraph, TPuzzNode, TPuzzEdge, TPuzzEntity>
// {
//     public TPuzzEntity Entity;
// }