using System;
using AnimSystem.Core;
using Core;
using MotionCore;
using UnityEngine;

public class GameLoop : MonoBehaviour
{
    public static GameLoop instance;
    [HideInInspector] public Rigidbody rb;
    public Transform model;
    public Animator animator;
    public float jumpForce;
    public float walkForce;
    public float runForce;
    public InputData inputData;
    public AnimSetting animSetting;

    private PlayerMotion motion;
    private PlayerParam param;
    private InputManager inputManager;

    //Attack
    private bool acceptAttackLevel2;
    private float attackInterval = 0.8f;
    private float realInterval = 0f;

    private void Awake()
    {
        instance = this;
        rb = GetComponent<Rigidbody>();
        model = transform;

        motion = new PlayerMotion(animSetting);
        param = motion.playerParam;
        inputManager = new InputManager(inputData, "inputJson");
    }

    private void Update()
    {
        InputManager.instance.Update(Time.deltaTime);
        // jump
        if (InputManager.instance.GetKeyDown("jump"))
        {
            param.JumpPress = true;
        }

        // move
        param.runPress = InputManager.instance.GetKeyDown("shift");
        if (InputManager.instance.GetAxisDown("horizontal") || InputManager.instance.GetAxisDown("vertical"))
        {
            param.inputPress = true;
            // Todo: better to use Vector2.Set() to reduce allocate
            param.InputVal = new Vector2(InputManager.instance.GetAxisValue("horizontal"),
                InputManager.instance.GetAxisValue("vertical"));
        }
        else
        {
            // Todo: better to use Vector2.Set() to reduce allocate
            param.InputVal = new Vector2(0f, 0f);
        }

        // attack
        if (acceptAttackLevel2)
        {
            realInterval += Time.deltaTime;
            if (realInterval > attackInterval)
            {
                param.AttackLevel = 0;
                realInterval = 0;
                acceptAttackLevel2 = false;
            }
            else
            {
                if (InputManager.instance.GetKeyDown("attack"))
                {
                    param.AttackLevel = 2;
                    realInterval = 0;
                    acceptAttackLevel2 = false;
                }
            }
        }
        else
        {
            if (InputManager.instance.GetKeyDown("attack"))
            {
                param.AttackLevel = 1;
                acceptAttackLevel2 = true;
                realInterval = 0f;
            }
        }
    }

    private void FixedUpdate()
    {
        if (param.JumpPress)
        {
            param.OnGround = false;
            param.jumpHandle?.Invoke();
        }

        // move
        if (param.inputPress && param.AttackLevel == 0)
        {
            param.moveHandle?.Invoke(param.InputVal);
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
            param.OnGround = true;
        }
    }
}