using AnimSystem.Core;
using Core;
using MotionCore;
using UnityEngine;

public class GameLoop : MonoBehaviour
{
    public static GameLoop instance;
    [HideInInspector]
    public Rigidbody rb;
    [HideInInspector]
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
        motion = new PlayerMotion(animSetting);
        param = motion.playerParam;
        inputManager = new InputManager(inputData, "inputJson");
    }

    private void Start()
    {
        rb = this.GetComponent<Rigidbody>();
        model = this.transform;
        //animator = model.GetComponent<Animator>();
    }

    private void Update()
    {
        InputManager.instance.Update(Time.deltaTime);
        param.jumpPress = InputManager.instance.GetKeyDown("jump");
        param.runPress = InputManager.instance.GetKeyDown("shift");
        //param.inputMove = new Vector2(InputManager.instance.GetAxisValue("horizontal") ,InputManager.instance.GetAxisValue("vertical"));
    }
}