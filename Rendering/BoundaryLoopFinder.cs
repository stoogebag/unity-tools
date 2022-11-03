using System.Collections.Generic;
using UnityEngine;


//todo:use this if u care
namespace stoogebag_MonuMental.stoogebag.Rendering
{
    public class BoundaryLoopFinder
    {
        // This structure will let us uniquely refer to a particular edge,
// identifying it by the (unordered) pair of vertices it joins.
        public struct EdgeID
        {
            public readonly int startIndex;
            public readonly int endIndex;

            public EdgeID(int vertexA, int vertexB)
            {
                // Put the vertices into a canonical order to de-dupe edges,
                // no matter the direction we're traversing it.
                if (vertexA < vertexB)
                {
                    startIndex = vertexA;
                    endIndex = vertexB;
                }
                else
                {
                    startIndex = vertexB;
                    endIndex = vertexA;
                }
            }
        }

// Here we'll count the number of times each edge occurs in the mesh.
        Dictionary<EdgeID, int> _edgeUseCounts = new Dictionary<EdgeID, int>();

        void IncrementEdgeUse(int vertexA, int vertexB)
        {
            var edge = new EdgeID(vertexA, vertexB);
            int count;
            _edgeUseCounts.TryGetValue(edge, out count);
            _edgeUseCounts[edge] = count + 1;
        }

        void CountEdgeUses(Mesh mesh)
        {
            _edgeUseCounts.Clear();

            var triangleIndices = mesh.triangles;

            for (int i = 0; i < triangleIndices.Length; i += 3)
            {
                IncrementEdgeUse(triangleIndices[i], triangleIndices[i + 1]);
                IncrementEdgeUse(triangleIndices[i + 1], triangleIndices[i + 2]);
                IncrementEdgeUse(triangleIndices[i + 2], triangleIndices[i]);
            }
        }

// Here, we'll extract all the edges that occur only once,
// so we know they're on the perimeter.    
        List<EdgeID> _perimeterEdges = new List<EdgeID>();

        void GatherPerimeterEdges()
        {
            _perimeterEdges.Clear();

            foreach (var pair in _edgeUseCounts)
            {
                if (pair.Value == 1)
                    _perimeterEdges.Add(pair.Key);
            }
        }

// Finally, we'll gather all these perimeter edges and organize them into ordered
// loops and arcs of vertices.
        public List<List<int>> GetPerimeterLoops(Mesh mesh)
        {

            CountEdgeUses(mesh);
            GatherPerimeterEdges();

            var loops = new List<List<int>>();

            while (_perimeterEdges.Count > 0)
            {
                var loop = new List<int>();

                int last = _perimeterEdges.Count - 1;
                var edge = _perimeterEdges[last];
                _perimeterEdges.RemoveAt(last--);

                int start = edge.startIndex;
                int next = edge.endIndex;

                loop.Add(start);
                loop.Add(next);

                do
                {
                    int i = _perimeterEdges.FindIndex(e => { return e.startIndex == next || e.endIndex == next; });
                    if (i < 0) break;

                    edge = _perimeterEdges[i];
                    _perimeterEdges[i] = _perimeterEdges[last];
                    _perimeterEdges.RemoveAt(last--);


                    next = edge.startIndex == next ? edge.endIndex : edge.startIndex;

                    loop.Add(next);
                } while (next != start);

                loops.Add(loop);
            }

            return loops;
        }
    }
}