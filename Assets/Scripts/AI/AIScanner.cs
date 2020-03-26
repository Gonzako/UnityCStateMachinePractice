using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIScanner : MonoBehaviour
{

    public delegate void AIScannerEvents();
    public AIScannerEvents onPlayerDetected;

    [Range(0.0F, 200F)]
    public float viewRadius;

    [Range(0.0F, 360F)]
    public float viewAngle;

    public LayerMask _targetMask;
    public LayerMask _obstacleMask;

    public List<Transform> _visibleTargets = new List<Transform>();

    public MeshFilter viewMeshFilter;
    Mesh viewMesh;

    [Tooltip("Amount of raycasts done to check field of view")]
    [Range(0.0F, 500F)]
    public int _meshResolution;

    public int edgeResolveIteration;
    public float edgeDistanceThreshold;

    private void Start()
    {
        viewMesh = new Mesh();
        viewMesh.name = "ViewMesh";
        viewMeshFilter.mesh = viewMesh;

        StartCoroutine(ScanForTargets(0.2F));
    }

    IEnumerator ScanForTargets(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            FindVisibleTargets();
       
        }
    }

    public void FindVisibleTargets()
    {
        _visibleTargets.Clear();
        Collider2D[] targets = Physics2D.OverlapCircleAll(transform.position, viewRadius, _targetMask);
        for (int i = 0; i < targets.Length; i++)
        {
            Transform target = targets[i].transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized;
            if (Vector2.Angle(transform.up, dirToTarget) < viewAngle / 2)
            {
                float dstToTarget = Vector2.Distance(transform.position, target.position);
                if(!Physics.Raycast(transform.position, dirToTarget, dstToTarget, _obstacleMask))
                {
                    _visibleTargets.Add(target);
                }
            }
        }
    }

    public Vector3 DirFromAngle(float angleindegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleindegrees += -transform.eulerAngles.z;
        }
        return new Vector2(
            Mathf.Sin(angleindegrees * Mathf.Deg2Rad),
            Mathf.Cos(angleindegrees * Mathf.Deg2Rad));
    }

    public Vector2 DirFromAngle2D(float angleInDegrees, bool angleIsGlobal)
    {
        //print("Before: " + angleInDegrees);
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.z;
            //print("After: " + angleInDegrees);
        }
        return new Vector2(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    private void LateUpdate()
    {
        DrawFieldOfView();
    }

    void DrawFieldOfView()
    {
        int stepCount = Mathf.RoundToInt(viewAngle * _meshResolution);
        float stepAngleSize = viewAngle / stepCount;
        List<Vector2> viewPoints = new List<Vector2>();

        ViewCastInfo oldViewCast = new ViewCastInfo();

        for (int i = 0; i <= stepCount; i++)
        {
            float angle = (-transform.eulerAngles.z - (viewAngle / 2)) + stepAngleSize * i;

            //Debug.DrawLine(transform.position, transform.position + DirFromAngle(angle, true) * viewRadius, Color.red);
            ViewCastInfo newViewCast = ViewCast(angle);
            viewPoints.Add(newViewCast.point2D);

            if (i > 0)
            {
                bool edgeDistThresholdExceeded = Mathf.Abs(oldViewCast.dist - newViewCast.dist) > edgeDistanceThreshold;
                if (oldViewCast.hit != newViewCast.hit || (oldViewCast.hit && newViewCast.hit && edgeDistThresholdExceeded))
                {
                    EdgeInfo edge = FindEdge(oldViewCast, newViewCast);

                    if (edge.pointA != Vector3.zero)
                    {
                        viewPoints.Add(edge.pointA);
                    }
                    if (edge.pointB != Vector3.zero)
                    {
                        viewPoints.Add(edge.pointB);
                    }
                }
            }

            oldViewCast = newViewCast;
        }

        int vertexCount = viewPoints.Count + 1;
        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[(vertexCount - 2) * 3];

        vertices[0] = Vector3.zero;
        for (int i = 0; i < (vertexCount - 1); i++)
        {
            vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]);

            if (i < vertexCount - 2)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
        }

        viewMesh.Clear();
        viewMesh.vertices = vertices;
        viewMesh.triangles = triangles;
        viewMesh.RecalculateNormals();
    }

    EdgeInfo FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast)
    {
        float minAngle = minViewCast.angle;
        float maxAngle = maxViewCast.angle;

        Vector2 minPoint = Vector2.zero;
        Vector2 maxPoint = Vector2.zero;

        for (int i = 0; i < edgeResolveIteration; i++)
        {
            float angle = (minAngle + maxAngle) / 2;
            ViewCastInfo newViewCast = ViewCast(angle);

            bool edgeDistThresholdExceeded = Mathf.Abs(minViewCast.dist - newViewCast.dist) > edgeDistanceThreshold;

            if (newViewCast.hit == minViewCast.hit && !edgeDistThresholdExceeded)
            {
                minAngle = angle;
                minPoint = newViewCast.point2D;
            }
            else
            {
                maxAngle = angle;
                maxPoint = newViewCast.point2D;
            }
        }

        return new EdgeInfo(minPoint, maxPoint);
    }

    ViewCastInfo ViewCast(float globalAngle)
    {
        //Vector3 dir = DirFromAngle(globalAngle, true);
        Vector3 dir = DirFromAngle(globalAngle, true);
        RaycastHit2D hit2D;

        hit2D = Physics2D.Raycast(transform.position, dir, viewRadius, _obstacleMask);

        if (hit2D)
        {
            return new ViewCastInfo(true, hit2D.point, hit2D.distance, globalAngle);
        }
        else
        {
            return new ViewCastInfo(false, transform.position + dir * viewRadius, viewRadius, globalAngle);
        }
    }

    public struct ViewCastInfo
    {
        public bool hit;
        //public Vector3 point;
        public Vector2 point2D;
        public float dist;
        public float angle;

        public ViewCastInfo(bool _hit, Vector2 _point, float _dist, float _angle)
        {
            hit = _hit;
            point2D = _point;
            dist = _dist;
            angle = _angle;
        }
    }

    public struct EdgeInfo
    {
        public Vector3 pointA;
        public Vector3 pointB;

        public EdgeInfo(Vector2 _pointA, Vector2 _pointB)
        {
            pointA = _pointA;
            pointB = _pointB;
        }
    }
}
