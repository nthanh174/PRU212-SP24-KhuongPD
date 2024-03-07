using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    PlayerController playerController;
    public GameObject respawnPoint;

    public Sprite active, passive;
    SpriteRenderer spriteRenderer;

    Collider2D coll;

    void Awake()
    {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        spriteRenderer = respawnPoint.GetComponent<SpriteRenderer>();
        coll = GetComponent<Collider2D>();
    }
    void Start()
    {
        spriteRenderer.sprite = passive;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerController.UpdateCheckPoint(respawnPoint.transform.position);
            spriteRenderer.sprite = active;
            coll.enabled = false;
        }
    }
}
