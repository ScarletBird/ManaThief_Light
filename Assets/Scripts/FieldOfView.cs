using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    public float viewRadius;
    [Range(0, 360)]
    public float viewAngle;

    public LayerMask targetMask;
    public LayerMask obstacle;

    public List<Transform> visibleTargets = new List<Transform>();

    public float meshResolution;

    public MeshFilter viewMeshFilter;
    Mesh viewMesh;

    public Transform user;

    public GameObject player;

    private void Start()
    {
        viewMesh = new Mesh();
        viewMesh.name = "View Mesh";
        viewMeshFilter.mesh = viewMesh;
        
        StartCoroutine("FindTargetsWithDelay", 0.05f);
        //viewMeshFilter.GetComponent<MeshRenderer>()
    }

    private void LateUpdate()
    {
        var mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,
   Input.mousePosition.y, 10));
        if(transform.tag == "Player" && !GameManager.instance.isPaused) user.LookAt(mousePosition);
        DrawFieldOfView();
    }

    IEnumerator FindTargetsWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            FindVisibleTargets();
        }
    }

    void FindVisibleTargets()
    {
        visibleTargets.Clear();
        Collider2D[] targetsInViewRadius = Physics2D.OverlapCircleAll(transform.position, viewRadius, targetMask);

        for(int i = 0; i < targetsInViewRadius.Length; i++)
        {
            Transform target = targetsInViewRadius[i].transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized;
            if(Vector3.Angle(transform.tag == "Player" ? user.forward : transform.GetComponent<EnemyController>().facingRight ? transform.right: -transform.right, dirToTarget) < viewAngle / 2)
            {
                float dstToTarget = Vector3.Distance(transform.position, target.position);

                if(!Physics2D.Raycast(transform.position, dirToTarget, dstToTarget, obstacle))
                {
                    visibleTargets.Add(target);
                }
            }
        }
    }

    void DrawFieldOfView()
    {
        int stepCount = Mathf.RoundToInt(viewAngle * meshResolution);
        float stepAngleSize = viewAngle / stepCount;
        List<Vector3> viewPoints = new List<Vector3>();

        for (int i = 0; i <= stepCount; i++)
        {
            float angle = transform.eulerAngles.y - viewAngle / 2 + stepAngleSize * i;
            if (transform.tag == "Player")
                if (player.GetComponent<PlayerPlatformerController>().facingRight)
                    angle += Vector2.Angle(Vector2.up, user.forward);
                else
                    angle -= Vector2.Angle(Vector2.up, user.forward);
            if (transform.tag == "Enemy")
                angle += 90;
            ViewCastInfo newViewCast = ViewCast(angle);
            viewPoints.Add(newViewCast.point);
        }

        int vertexCount = viewPoints.Count + 1;
        Vector3[] vertices = new Vector3[vertexCount];
        Vector2[] uv = new Vector2[vertexCount];
        int[] triangles = new int[(vertexCount - 2) * 3];

        vertices[0] = Vector3.zero;
        for (int i = 0; i < vertexCount - 1; i++)
        {
            vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]);

            uv[i] = i % 3 == 0 ? new Vector2(0, 0) : i % 3 == 1 ? new Vector2(0, 1) : new Vector2(1, 1);


            if (i < vertexCount - 2)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
        }

        viewMesh.Clear();
        viewMesh.vertices = vertices;
        viewMesh.uv = uv;
        viewMesh.triangles = triangles;
        viewMesh.RecalculateNormals();

        viewMeshFilter.GetComponent<MeshRenderer>().material.color = new Color(
            viewMeshFilter.GetComponent<MeshRenderer>().material.color.r,
            viewMeshFilter.GetComponent<MeshRenderer>().material.color.g,
            viewMeshFilter.GetComponent<MeshRenderer>().material.color.b,
            0.2f);

        List<Color> color_arr = new List<Color>();
        viewMesh.GetColors(color_arr);
        for (int c = 0; c < color_arr.Count; c++) {
            color_arr[c] = new Color(color_arr[c].r, color_arr[c].g, color_arr[c].b, 0.2f);
        }
        viewMesh.SetColors(color_arr);

    }

    ViewCastInfo ViewCast(float globalAngle)
    {
        Vector3 dir = DirFromAngle(globalAngle, true);
        //Debug.Log(dir);

        RaycastHit2D hit = new RaycastHit2D();

        hit = Physics2D.Raycast(transform.position, dir, viewRadius, obstacle);

        return hit.collider != null
            ? new ViewCastInfo(true, hit.point, hit.distance, globalAngle)
            : new ViewCastInfo(false, transform.position + dir * viewRadius, viewRadius, globalAngle);
    }

    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
            angleInDegrees += transform.eulerAngles.y;
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), Mathf.Cos(angleInDegrees * Mathf.Deg2Rad), 0 );
    }

    public struct ViewCastInfo
    {
        public bool hit;
        public Vector3 point;
        public float dst;
        public float angle;

        public ViewCastInfo(bool _hit, Vector3 _point, float _dst, float _angle)
        {
            hit = _hit;
            point = _point;
            dst = _dst;
            angle = _angle;
        }
    }
}
