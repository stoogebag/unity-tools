using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using stoogebag.Extensions;
using stoogebag.PuzzGrid;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

[SelectionBase]
public abstract class GridEntity : MonoBehaviour
{
    public List<GridAction> PendingMoves = new List<GridAction>();

    private void Start()
    {
        _components = GetComponents<IGridEntityComponent>().ToList();
        foreach (var entityComponent in _components)
        {
            entityComponent.BindGridEntity(this);
        }

        // foreach (var nodeEntity in NodeEnts)
        // {
        //     nodeEntity.GridEntity = this;
        //     nodeEntity.PuzzGrid = PuzzGrid;
        // }
    }

    protected List<IGridEntityComponent> _components;

    protected bool Equals(GridEntity other)
    {
        return base.Equals(other) && ID == other.ID;
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((GridEntity)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(base.GetHashCode(), ID);
    }


    public PuzzGrid PuzzGrid { get; set; }

    public string ID { get; set; }

    //returns a tuple: (ray origin, buffer distance)
    //numRays MUST be at least 2 or we'll get a divide by zero error.
    public IEnumerable<(Vector3, float)> GetFrontierRayOrigins(Vector3 dir, float gridSpacing, int numRays = 16)
    {
        //todo: improve.
        //bc: do we just bake these? or try to be clever? or both?
        //lets be super lazy lol.

        var boxes = gameObject.GetComponentsInChildren<BoxCollider>();

        Physics.SyncTransforms();

        var buffer = 1f; //todo:careful if the object is mega small. maybe use bounds.size or stg.

        foreach (var box in boxes)
        {
            var bounds = box.GetBounds();
            foreach (var point in GetGridPoints(bounds, dir, numRays))
            {
                var vec = point;
                var reverseDir = (-dir).normalized;
                Vector3 exitPoint = Vector3.zero;

                if (box.Raycast(new Ray(vec + dir.normalized * 100, reverseDir), out RaycastHit hit, 120))
                {
                    exitPoint = hit.point;
                    yield return (exitPoint - dir.normalized * buffer, buffer);
                }
            }
        }
    }


    public static IEnumerable<Vector3> GetGridPoints(Bounds bounds, Vector3 Dir, int numPoints)
    {
        // Ensure Dir is a unit vector (normalized)
        Dir = Dir.normalized;

        // Get collider size and center
        Vector3 size = bounds.size * 0.99f;
        Vector3 center = bounds.center;


        // Calculate spacing between points on the perpendicular axis
        float spacing = size.x / (numPoints);

        // Loop through each point on the perpendicular axis
        for (int i = 0; i < numPoints; i++)
        {
            for (int j = 0; j < numPoints; j++)
            {
                // Calculate offset based on spacing and index
                float offset1 = (i + .5f - numPoints * .5f) * spacing / 1;
                float offset2 = (j + .5f - numPoints * .5f) * spacing / 1;

                Vector3 offset;
                if (Dir == Vector3.forward || Dir == -Vector3.forward) offset = new Vector3(offset1, offset2, 0);
                else if (Dir == Vector3.right || Dir == -Vector3.right) offset = new Vector3(0, offset1, offset2);
                else if (Dir == Vector3.up || Dir == -Vector3.up) offset = new Vector3(offset2, 0, offset1);
                else throw new Exception("direction is not an axis. what is happening.");

                // Calculate point position relative to center based on Dir and offset
                Vector3 point = center +
                                offset;
                yield return point;
                // Add point to the grid
            }
        }
    }


    public abstract GridActionSet GetSideEffectMoves(IEnumerable<GridAction> set);
    public abstract GridActionSet GetSettlementMoves(GridActionSummary actionSummary);
    public abstract GridActionSetGroup GetGravityMoves();


    public static Direction GetDirection(Vector3 dir)
    {
        Direction dirEnum = Direction.up;
        if (dir == Vector3.left) dirEnum = Direction.left;
        if (dir == Vector3.right) dirEnum = Direction.right;
        if (dir == Vector3.up) dirEnum = Direction.up;
        if (dir == Vector3.down) dirEnum = Direction.down;
        if (dir == Vector3.forward) dirEnum = Direction.north;
        if (dir == Vector3.back) dirEnum = Direction.south;
        return dirEnum;
    }

    public static Vector3 GetDirectionVec(Direction dir)
    {
        Vector3 dirVector = Vector3.up;
        if (dir == Direction.left) dirVector = Vector3.left;
        if (dir == Direction.right) dirVector = Vector3.right;
        if (dir == Direction.up) dirVector = Vector3.up;
        if (dir == Direction.down) dirVector = Vector3.down;
        if (dir == Direction.north) dirVector = Vector3.forward;
        if (dir == Direction.south) dirVector = Vector3.back;
        return dirVector;
    }

    public List<PuzzGridRaycastResult> GetNeighbours(Vector3 direction, int numRays = 2)
    {
        var hits = new List<PuzzGridRaycastResult>();

        foreach ((var rayOrigin, var buffer) in GetFrontierRayOrigins(direction, numRays)
                     .ToList())
        {
            var rayDir = direction + direction.normalized * buffer;

            Debug.DrawRay(rayOrigin, rayDir, Color.red, 1f);


            //cast.
            if (Physics.Raycast(rayOrigin, rayDir, out var hit, PuzzGrid.GridSpacing() * 100,
                    LayerMask.GetMask("Default")))
            {
                var hitDistance = hit.distance - buffer;
                if (hitDistance >= direction.magnitude) continue;

                //portal related stuff.
                Vector3 portalOutDirection = Vector3.zero;
                float prePortalDistance = 0;
                float postPortalDistance = 0;
                float portalMultiplier = 1;


                var hitEnt = hit.collider.gameObject.GetComponentInAncestor<GridEntity>();
                if (hitEnt == null) continue;
                if (hitEnt == this) continue;


                var res = new PuzzGridRaycastResult()
                {
                    HitEnt = hitEnt,
                    PrePortalDistance = prePortalDistance,
                    PostPortalDistance = postPortalDistance,
                    PortalMultiplier = portalMultiplier,
                    PortalOutDirection = portalOutDirection,
                    HitDistance = hitDistance,
                    ThroughPortal = postPortalDistance > 0,
                };
                hits.Add(res);
            }
        }


        if (!hits.Any()) return null;
        return hits;
    }


    public virtual IEnumerable<GridAction> FilterSideEffects(List<GridAction> effects)
    {
        return effects;
    }

    public virtual UniTask Hide(float time)
    {
        foreach (var c in GetComponentsInChildren<Renderer>())
        {
            c.enabled = false;
        }

        return UniTask.CompletedTask;
    }

    public virtual UniTask Show(float time)
    {
        foreach (var c in GetComponentsInChildren<Renderer>())
        {
            c.enabled = true;
        }

        return UniTask.CompletedTask;
    }

    public abstract GridActionConsequences GetConsequences(GridAction action);
}

public class GridActionConsequences
{
    public GridAction Dependency; //if this fails, the original action fails
    public GridAction SideEffect; //this can fail without affecting the original action.

    public Approvals Approval;

    public static GridActionConsequences ActionApproved => new GridActionConsequences
    {
        Approval = Approvals.Approved,
    };

    public static GridActionConsequences ActionFailed => new GridActionConsequences
    {
        Approval = Approvals.Failed,
    };
}

public enum Approvals
{
    Approved,
    Partial,
    Failed,
    Unevaluated,
}