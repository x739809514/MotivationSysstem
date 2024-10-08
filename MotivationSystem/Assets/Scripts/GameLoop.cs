using System;
using System.Collections;
using AnimSystem.Core;
using Core;
using MotionCore;
using Tool;
using UnityEngine;
using UnityEngine.Serialization;

public class GameLoop : MonoBehaviour
{
    public static GameLoop instance;
    [HideInInspector] public Rigidbody rb;

    // setting
    public Transform model;
    public Animator animator;
    public float rollForce;
    public float walkForce;
    public float runForce;
    public GameObject weapon;
    public InputData inputData;
    public AnimSetting riotSetting;
    public AnimSetting swordSetting;

    // fields
    private PlayerMotion motion;
    private PlayerParam param;
    private InputManager inputManager;
    private float executionTimeout;
    private bool inExecutionWindow;

    // IK
    public Transform b_leftHandPosition;
    public Transform b_rightHandPosition;
    public Transform n_leftHandPosition;
    public Transform n_rightHandPosition;

    //Attack
    public float comboTimeout = 0.2f; // 连招超时时间
    public int maxComboCount = 6; // 连招最大段数
    private Coroutine comboCoroutine;
    private bool isAttacking = false; // 是否正在攻击
    private bool attackPressedDuringComboWindow = false; // 记录在等待期间是否按下攻击键

    private void Awake()
    {
        instance = this;
        rb = GetComponent<Rigidbody>();
        model = transform;
        motion = new PlayerMotion(riotSetting, swordSetting);
        param = motion.playerParam;
        inputManager = new InputManager(inputData, "inputJson");
    }

    private void Start()
    {
        motion.LoadSwordAttack();
        param.canMove = true;
    }

    private void Update()
    {
        if (executionTimeout > 0)
        {
            executionTimeout -= Time.deltaTime;
        }
        else
        {
            inExecutionWindow = false;
        }

        InputManager.instance.Update(Time.deltaTime);
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
        if (InputManager.instance.GetKeyDown("attack") && motion.GetCurAttackType() != AttackType.Null)
        {
            param.canMove = false;
            if (inExecutionWindow)
            {
                StartCoroutine(Execution());
            }
            else
            {
                attackPressedDuringComboWindow = true;
                if (param.AttackLevel == 0 && !isAttacking)
                {
                    comboCoroutine = StartCoroutine(ExecuteCombo());
                }
            }
        }

        // block
        if (InputManager.instance.GetKeyDown("block"))
        {
            if (param.isExecution == false)
            {
                EnterBlock();
            }
        }
        else
        {
            QuitBlock();
        }

        // roit
        if (InputManager.instance.GetKeyDown("riot") && param.isRolling == false && param.isBlocking == false)
        {
            weapon.SetActive(false);
            param.canMove = true;
            motion.LoadRiotAttack();
        }

        // sword
        if (InputManager.instance.GetKeyDown("sword") && param.isRolling == false && param.isBlocking == false)
        {
            weapon.SetActive(true);
            param.canMove = true;
            motion.LoadSwordAttack();
        }

        // roll
        if (InputManager.instance.GetKeyDown("roll"))
        {
            param.rollPress = true;
        }

        // test
        if (Input.GetKeyDown(KeyCode.R))
        {
            executionTimeout = 0.8f;
            param.canMove = true;
            inExecutionWindow = true;
            param.isBlocking = false;
        }
    }

    private void FixedUpdate()
    {
        if (param.inputPress)
        {
            param.moveHandle?.Invoke(param.InputVal);
        }

        param.velocity = rb.velocity;

        if (param.rollPress && param.isRolling == false)
        {
            StartCoroutine(ExecuteRoll());
        }
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

    private void OnAnimatorIK(int layerIndex)
    {
        var curAttackType = motion.GetCurAttackType();
        if (curAttackType == AttackType.Sword)
        {
            if (param.blockPress)
            {
                animator.SetIKPosition(AvatarIKGoal.LeftHand, b_leftHandPosition.position);
                animator.SetIKPosition(AvatarIKGoal.RightHand, b_rightHandPosition.position);
            }
            else
            {
                animator.SetIKPosition(AvatarIKGoal.LeftHand, n_leftHandPosition.position);
                animator.SetIKPosition(AvatarIKGoal.RightHand, n_rightHandPosition.position);
            }
        }

        animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1f);
        animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1f);
        animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1f);
        animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1f);
    }


#region Methods

    // block
    private void EnterBlock()
    {
        if (comboCoroutine != null)
        {
            StopCoroutine(comboCoroutine);
        }

        isAttacking = false;
        param.blockPress = true;
        param.blockHandle?.Invoke();
    }

    private void QuitBlock()
    {
        if (param.isBlocking == false) return;
        param.idleHandle?.Invoke();
    }

    IEnumerator Execution()
    {
        param.executionHandle?.Invoke();
        yield return new WaitForSeconds(1.2f);
        param.idleHandle?.Invoke();
    }

    // roll
    IEnumerator ExecuteRoll()
    {
        param.rollHandle?.Invoke();
        yield return new WaitForSeconds(1.2f);
        param.idleHandle?.Invoke();
    }

#endregion


#region combo

    // 执行连招攻击
    IEnumerator ExecuteCombo()
    {
        isAttacking = true;
        param.AttackLevel = 1;

        while (param.AttackLevel <= maxComboCount)
        {
            PlayComboAnimation();

            // 等待当前动画播放完毕
            yield return new WaitForSeconds(0.8f); //Mixer.GetCurClipLength()
            param.canMove = true;

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

#endregion
}