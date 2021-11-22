using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Singleton managed bounds manager
public class BoundsManager : MonoBehaviour
{
    #region SINGLETON SETUP
    // Singleton setup
    private static BoundsManager _instance;
    public static BoundsManager Instance { get { return _instance; } }
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }
    #endregion

    private float smokeHeightBounds = 2f;
    private List<ObjectBounds> objectBounds = new List<ObjectBounds>();
    public IReadOnlyList<ObjectBounds> GetObjectBounds 
    { 
        get { return objectBounds; }
    }

    public void CreateBounds(Collider collider)
    {
        objectBounds.Add(new ObjectBounds(collider, BoundType.Spawnpoint));
    }    

    public void CreateBounds(Collider collider, Vector3 startPoint, Vector3 endPoint)
    {
        Vector3 heading = endPoint - startPoint;
        float distance = heading.magnitude;
        Vector3 direction = heading / distance;
        Vector3 midPoint = ((startPoint + endPoint) / 2.0f);

        collider.transform.position = midPoint;
        collider.transform.localScale = heading;

        objectBounds.Add(new ObjectBounds(collider, BoundType.Generic));
    }

    public void RemoveBounds(Collider collider)
    {
        for (int i = 0; i < objectBounds.Count; i++)
        {
            if (objectBounds[i].GetCollider == collider)
            {
                objectBounds.RemoveAt(i);
                return;
            }
        }
    }

    public void SetSmokeHeight(float height)
    {
        smokeHeightBounds = height;
    }

    public float GetSmokeHeight
    {
        get { return smokeHeightBounds; }
    }

    public void Reset()
    {
        objectBounds.Clear();
    }

}


// Container for an object's bounds
public class ObjectBounds
{
    public ObjectBounds(Collider collider, BoundType boundType)
    {
        col = collider;
        type = boundType;
    }
    private Collider col;
    private BoundType type = BoundType.Generic;
    public Collider GetCollider
    {
        get { return col; }
    }

    #region POINT IN BOUNDS
    public bool IsPointInBounds(Vector3 point) 
    {
        if (col.bounds.Contains(point))
        {
            return true;
        }
        return false;
    }
    public Vector3 GetRandomPointInBounds()
    {
        return new Vector3(
            Random.Range(col.bounds.min.x, col.bounds.max.x),
            Random.Range(col.bounds.min.y, col.bounds.max.y),
            Random.Range(col.bounds.min.z, col.bounds.max.z)
            );
    }

    // Returns a point random position on the top surface of the bounds which can fit the passed collider
    private Vector3 RandomFittingPointOnTopSide(Collider collider)
    {
        float randomX = Random.Range(collider.bounds.min.x, collider.bounds.max.x);
        float randomZ = Random.Range(collider.bounds.min.z, collider.bounds.max.z);

        if (FitsSurfaceSide(collider, ObjectSide.Top))
        {
            // If offset is negative no offset is needed so reset it to zero
            float offsetX = (randomX + (collider.bounds.size.x / 2)) - col.bounds.max.x;
            offsetX = offsetX > 0.0f ? offsetX : 0.0f;

            float offsetZ = (randomZ + (collider.bounds.size.y / 2)) - col.bounds.max.x;
            offsetZ = offsetZ > 0.0f ? offsetZ : 0.0f;

            randomX -= offsetX;
            randomZ -= offsetZ;

            return new Vector3(randomX, col.bounds.max.y, randomZ);
        }
        return new Vector3(randomX, col.bounds.max.y, randomZ);
    }

    // Returns a random point on a bound's surface that will try to fit the passed collider bounds on it.
    // You can call FitsSurfaceSide() first to ensure it will fit or you may get an overlaping point.
    public Vector3 GetRandomPointOnSide(Collider collider, ObjectSide side)
    {
        switch (side)
        {
            case ObjectSide.Top:
                return RandomFittingPointOnTopSide(collider);
            case ObjectSide.Left:
                break;
            case ObjectSide.Right:
                break;
            case ObjectSide.Front:
                break;
            case ObjectSide.Back:
                break;
            case ObjectSide.Bottom:
                break;
            default:
                return Vector3.zero;
        }
        Debug.LogError("Other sides not implemented yet!");
        return Vector3.zero;
    }
    #endregion

    #region FITS METHODS
    public bool FitsSurfaceSide(Collider collider, ObjectSide side)
    {
        switch (side)
        {
            case ObjectSide.Top:
                return FitsTopSurface(collider);
            case ObjectSide.Left:
                break;
            case ObjectSide.Right:
                break;
            case ObjectSide.Front:
                break;
            case ObjectSide.Back:
                break;
            case ObjectSide.Bottom:
                break;
            default:
                return false;
                break;
        }
        return false;
    }

    private bool FitsTopSurface(Collider collider)
    {
        if (FitsXBounds(collider) && FitsZBounds(collider))
        {
            return true;
        }
        return false;
    }

    private bool FitsXBounds(Collider collider)
    {
        if (Mathf.Abs(collider.bounds.max.x - collider.bounds.min.x) 
            < 
            Mathf.Abs(col.bounds.max.x - col.bounds.min.x))
        {
            return true;
        }
        return false;
    }

    private bool FitsYBounds(Collider collider)
    {
        if (Mathf.Abs(collider.bounds.max.y - collider.bounds.min.y)
            <
            Mathf.Abs(col.bounds.max.y - col.bounds.min.y))
        {
            return true;
        }
        return false;
    }

    private bool FitsZBounds(Collider collider)
    {
        if (Mathf.Abs(collider.bounds.max.z - collider.bounds.min.z)
            <
            Mathf.Abs(col.bounds.max.z - col.bounds.min.z))
        {
            return true;
        }
        return false;
    }
    #endregion


    public Vector3 GetClosestSurfacePoint(Vector3 point)
    {
        return col.ClosestPoint(point);
    }

}

// Side of object surface
public enum ObjectSide
{
    Top,
    Left,
    Right,
    Front,
    Back,
    Bottom
}

// Type of object and what it is bounding etc fire or and item
public enum BoundType
{
    Generic,
    Wall,
    Spawnpoint,
    Item,
    Occuluder
}