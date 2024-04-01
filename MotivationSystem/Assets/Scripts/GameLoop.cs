using System;
using UnityEngine;

public class GameLoop : MonoBehaviour
{
    public static GameLoop instance;
    [HideInInspector]
    public Rigidbody rb;
    [HideInInspector]
    public Transform model;
    public float rotateSpeed;
    public float jumpForce;
    public float walkForce;
    public float runForce;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        rb = this.GetComponent<Rigidbody>();
        model = this.transform;
    }
}