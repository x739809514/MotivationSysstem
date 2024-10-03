using System;
using System.Collections;
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
    private bool canMove = true;

    //Attack
    public float comboTimeout = 0.2f; // 连招超时时间
    public int maxComboCount = 6; // 连招最大段数

    private bool isAttacking = false; // 是否正在攻击
    private bool attackPressedDuringComboWindow = false; // 记录在等待期间是否按下攻击键

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
        /*if (InputManager.instance.GetKeyDown("jump"))
        {
            param.JumpPress = true;
        }*/

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
        if (InputManager.instance.GetKeyDown("attack"))
        {
            attackPressedDuringComboWindow = true;
            canMove = false;
            if (param.AttackLevel == 0 && !isAttacking)
            {
                StartCoroutine(ExecuteCombo());
            }
        }
    }

    private void FixedUpdate()
    {
        /*if (param.JumpPress)
        {
            param.OnGround = false;
            param.jumpHandle?.Invoke();
        }*/

        // move
        if (param.inputPress && canMove)
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

    // 执行连招攻击
    // 执行连招攻击
    // 捕捉攻击键按下
    IEnumerator ExecuteCombo()
    {
        isAttacking = true;
        param.AttackLevel = 1;

        while (param.AttackLevel <= maxComboCount)
        {
            PlayComboAnimation();

            // 等待当前动画播放完毕
            yield return new WaitForSeconds(Mixer.GetCurClipLength());  //
            canMove = true;
            
            // 如果连招计数器超过最大值，重置
            if (param.AttackLevel >= maxComboCount)
            {
                ResetCombo();
                param.attackHandle?.Invoke(param.AttackLevel); // back to idle
                break;
            }

            // 开启连招窗口
            attackPressedDuringComboWindow = false;
            float timeRemaining = comboTimeout;

            // 等待玩家输入或超时
            while (timeRemaining > 0)
            {
                if (attackPressedDuringComboWindow)
                {
                    attackPressedDuringComboWindow = false;
                    param.AttackLevel++;
                    break;
                }

                timeRemaining -= Time.deltaTime;
                yield return null;
            }

            // 如果超时未按下攻击键，重置连招
            if (timeRemaining <= 0)
            {
                ResetCombo();
                param.attackHandle?.Invoke(param.AttackLevel); // back to idle
                break;
            }
        }

        isAttacking = false;
    }

    // 播放连招动画
    void PlayComboAnimation()
    {
        param.attackHandle?.Invoke(param.AttackLevel);
    }

    // 重置连招
    void ResetCombo()
    {
        param.AttackLevel = 0;
        isAttacking = false;
        attackPressedDuringComboWindow = false; // 重置标志位
        Debug.Log("Combo reset.");
    }
}