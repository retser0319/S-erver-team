using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Nav_AI : MonoBehaviour
{
    [SerializeField] float size;
    [SerializeField] float moveSpeed = 2f; // 이동 속도

    public float distance;

    Tile_Manager tileManager;
    List<Node> path;

    int currentNodeIndex = 0; // 현재 목표 노드 인덱스

    private void Awake()
    {
        tileManager = GameObject.Find("Map").GetComponent<Tile_Manager>();
    }
    private void Start()
    {
        Find();
    }

    private void FixedUpdate()
    {
        if (path == null || path.Count == 0) return;
        MoveAlongPath();
        GetRemainingDistance();
    }
    public void Find()
    {
        Vector2 start = transform.position;
        Vector2 end = new Vector2(0, start.y);

        path = FindPathOptimized(start, end);

        if (path == null)
        {
            Debug.Log("경로 없음");
            return;
        }
    }
    void MoveAlongPath()
    {
        if (currentNodeIndex >= path.Count) return;

        Vector2 targetPos = path[currentNodeIndex].pos;

        // 이동
        Vector2 currentPos = transform.position;
        Vector2 newPos = Vector2.MoveTowards(currentPos, targetPos, moveSpeed * Time.fixedDeltaTime);
        transform.position = newPos;

        // 목표 노드에 거의 도착하면 다음 노드로
        if (Vector2.Distance(newPos, targetPos) < 0.05f)
        {
            currentNodeIndex++;
        }
    }
    private void GetRemainingDistance()
    {
        if (path == null || path.Count == 0) return;
        if (currentNodeIndex >= path.Count) return;
        distance = 0;
        // 1. 현재 위치 → 다음 목표 노드까지의 거리
        Vector2 currentPos = transform.position;
        Vector2 targetPos = path[currentNodeIndex].pos;
        distance += Vector2.Distance(currentPos, targetPos);

        // 2. 다음 노드부터 마지막 노드까지의 거리
        for (int i = currentNodeIndex; i < path.Count - 1; i++)
        {
            distance += Vector2.Distance(path[i].pos, path[i + 1].pos);
        }
    }
    public List<Node> FindPathOptimized(Vector2 start, Vector2 end)
    {
        currentNodeIndex = 0;
        Queue<Node> q = new Queue<Node>();
        Dictionary<Vector2, Node> visited = new Dictionary<Vector2, Node>();

        Node startNode = new Node(start);
        q.Enqueue(startNode);
        visited[start] = startNode;

        Vector2[] dirs = {
        new Vector2(0.2f, 0),
        new Vector2(-0.2f, 0),
        new Vector2(0, 0.2f),
        new Vector2(0, -0.2f)
        };

        while (q.Count > 0)
        {
            Node current = q.Dequeue();

            // 목적지 가까우면 경로 추적
            if (Vector2.Distance(current.pos, end) < 0.5f)
                return BuildPath(current);

            foreach (var dir in dirs)
            {
                Vector2 next = current.pos + dir;

                if (!IsWalkable(next)) continue;

                // 이미 방문한 노드면 스킵
                if (visited.ContainsKey(next))
                    continue;

                Node newNode = new Node(next);

                // 여기서 "부모를 최적화" 한다!
                // current.parent → grandparent 라인 체크
                if (current.parent != null)
                {
                    // current.parent 와 next 사이가 직선이면 grandparent로 설정
                    if (!HasWallBetween(current.parent.pos, next))
                        newNode.parent = current.parent;
                    else
                        newNode.parent = current;
                }
                else
                {
                    newNode.parent = current;
                }

                visited[next] = newNode;
                q.Enqueue(newNode);
            }
        }

        Debug.Log("경로 없음");
        return null;
    }
    private List<Node> BuildPath(Node endNode)
    {
        List<Node> path = new List<Node>();
        Node cur = endNode;

        while (cur != null)
        {
            path.Add(cur);
            cur = cur.parent;
        }

        path.Reverse();
        return path;
    }
    public bool IsWalkable(Vector2 pos)
    {
        // 배열 범위 체크
        if (pos.x < 0 || pos.x >= tileManager.xSize) return false;
        if (pos.y < 0 || pos.y >= tileManager.ySize) return false;

        RaycastHit2D hit = Physics2D.CircleCast(pos, size, Vector2.zero, 0f, LayerMask.GetMask("Wall"));
        return hit.collider == null;
    }
    bool HasWallBetween(Vector2 a, Vector2 b)
    {
        Vector2 dir = (b - a).normalized;
        float distance = Vector2.Distance(a, b);

        // CircleCast로 장애물 체크, radius = size
        RaycastHit2D hit = Physics2D.CircleCast(a, size, dir, distance, LayerMask.GetMask("Wall"));
        return hit.collider != null;
    }

    void OnDrawGizmos()
    {
        if (path == null || path.Count == 0) return;

        Gizmos.color = Color.green;

        for (int i = 0; i < path.Count; i++)
        {
            // 경로 노드 위치 표시 (타일 중심으로 +0.5f)
            Vector3 nodePos = new Vector3(path[i].pos.x, path[i].pos.y, 0);
            Gizmos.DrawSphere(nodePos, 0.2f);

            // 이전 노드와 연결 선 그리기
            if (i > 0)
            {
                Vector3 from = new Vector3(path[i - 1].pos.x, path[i - 1].pos.y, 0);
                Vector3 to = nodePos;
                Gizmos.DrawLine(from, to);
            }
        }
    }
}
public class Node
{
    public Vector2 pos;
    public Node parent;

    public Node(Vector2 p, Node par = null)
    {
        pos = p;
        parent = par;
    }
}