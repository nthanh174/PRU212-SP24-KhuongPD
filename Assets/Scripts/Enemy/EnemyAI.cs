using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemyAI : MonoBehaviour
{
    private Path path;
    private Coroutine coroutine;
    [SerializeField]
    private Seeker seeker;
    [SerializeField]
    private bool roaming = true;
    private bool reachDestination = false;
    [SerializeField]
    private bool updateContinuesPath;
    [SerializeField]
    private float moveSpeed = 3f;
    [SerializeField]
    private float nextWPDistance = 0.3f;
    [SerializeField]
    private float maxDistanceToPlayer = 15f; // Khoảng cách tối đa giữa EnemyAI và Player để bắt đầu di chuyển về phía Player


    void Start()
    {
        InvokeRepeating("CalculatePath", 0f, 0.5f);
    }

    void CalculatePath()
    {
        Vector3 playerPos = FindObjectOfType<PlayerController>().transform.position;
        float distanceToPlayer = Vector3.Distance(transform.position, playerPos); // Tính khoảng cách đến Player

        if (distanceToPlayer <= maxDistanceToPlayer) // Kiểm tra nếu khoảng cách nhỏ hơn hoặc bằng khoảng cách tối đa
        {
            Vector2 target = FindTarget();
            if (seeker.IsDone() && (updateContinuesPath || reachDestination))
            {
                seeker.StartPath(transform.position, target, OnPathComplete);
            }
        }
    }

    Vector2 FindTarget()
    {
        Vector3 playerPos = FindObjectOfType<PlayerController>().transform.position;
        if (roaming)
        {
            // Tính toán hướng từ quái vật đến người chơi
            Vector2 directionToPlayer = ((Vector2)playerPos - (Vector2)transform.position).normalized;

            // Đặt khoảng cách từ quái vật đến người chơi
            float distanceToPlayer = 50f; // Khoảng cách tùy chỉnh bạn muốn

            // Độ cao mong muốn
            float desiredHeight = 0f; // Độ cao tùy chỉnh bạn muốn

            // Tính toán vị trí mục tiêu dựa trên hướng và khoảng cách, cũng như độ cao mong muốn
            Vector2 target = (Vector2)transform.position + directionToPlayer * distanceToPlayer;
            target += Vector2.up * desiredHeight;

            return target;
        }
        else
        {
            return (Vector2)playerPos;
        }
    }

    void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            MoveToTarget();
        }
    }

    void MoveToTarget()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }
        coroutine = StartCoroutine(MoveToTargetCoroutine());
    }

    IEnumerator MoveToTargetCoroutine()
    {
        int currentWayPoint = 0;
        reachDestination = false;
        while (currentWayPoint < path.vectorPath.Count)
        {
            Vector2 direction = ((Vector2)path.vectorPath[currentWayPoint] - (Vector2)transform.position).normalized;
            Vector2 force = direction * moveSpeed * Time.deltaTime;
            transform.position += (Vector3)force;

            float distance = Vector2.Distance(transform.position, path.vectorPath[currentWayPoint]);
            if (distance < nextWPDistance)
            {
                currentWayPoint++;
            }
            yield return null;
        }
        reachDestination = true;
    }
}
