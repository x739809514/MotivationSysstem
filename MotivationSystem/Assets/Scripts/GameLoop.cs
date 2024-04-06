using System;
using AnimSystem.Core;
using Core;
using MotionCore;
using UnityEngine;

public class GameLoop : MonoBehaviour
{
    public static GameLoop instance;
    [HideInInspector]
    public Rigidbody rb;
    public Transform model;
    public Animator animator;
    public float rotateSpeed;
    public float jumpForce;
    public float walkForce;
    public float runForce;
    public InputData inputData;
    public AnimSetting animSetting;

    private PlayerMotion motion;
    private PlayerParam param;
    private InputManager inputManager;

    private void Awake()
    {
        instance = this;
        rb = GetComponent<Rigidbody>();
        model = transform;
        
        motion = new PlayerMotion(animSetting);
        param = motion.playerParam;
        inputManager = new InputManager(inputData, "inputJson");
    }

    private void FixedUpdate()
    {
        InputManager.instance.Update(Time.deltaTime);
        if (InputManager.instance.GetKeyDown("jump"))
        {
            param.jumpPress = true;
        }
        
        param.runPress = InputManager.instance.GetKeyDown("shift");
        if (InputManager.instance.GetAxisDown("horizontal") || InputManager.instance.GetAxisDown("vertical"))
        {
            param.inputPress = true;
            // Todo: better to use Vector2.Set() to reduce allocate
            param.inputVal = new Vector2(InputManager.instance.GetAxisValue("horizontal") ,InputManager.instance.GetAxisValue("vertical"));
        }
        else
        {
            // Todo: better to use Vector2.Set() to reduce allocate
            param.inputVal = new Vector2(0f, 0f);
        }

        param.velocity = rb.velocity;
    }

    private void OnDestroy()
    {
        motion.OnDestroy();
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("ground"))
        {
            param.onGround = true;
        }
    }
}