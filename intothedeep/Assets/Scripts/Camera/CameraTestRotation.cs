using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

public class CameraTestRotation : MonoBehaviour
{
    public Transform target;
    public PlayerController player;
    private Rigidbody rb;
    private Vector3 targetRotation;
    
    [Header("connected VirtualCameraController")]
    public VirtualCameraController virtualCameraController;

    public CinemachineVirtualCamera vcam;

    [Header("Current Velocity State")]
    // 这里用 MovementState 来代表你当前要测试的状态（例如 Sprint）
    public VelocityState testState;

    public bool velocityStateFixed = false;

    public bool alwaysRunning = false;
    
    private float timer = 0f;

    public bool start = false;

    private CinemachineComposer composer;

    private float targetScreenX;

    private float smoothVelocity;
    [Header("X rotation test")]    
    public float defaultTargetScreenX=0.5f;
    public float targetScreenXOffset=0.1f;
    public float _yawSen;
    private float cachedHorizontalInput; // 【改动】新增：缓存水平输入值

    public VirtualCameraConfig screenXrotationConfigTest;
    // Start is called before the first frame update
    void Start()
    {
        composer = vcam.GetCinemachineComponent<CinemachineComposer>();
        rb=player.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        

        //【改动】更新screenX，同时缓存水平输入
        cachedHorizontalInput = Input.GetAxisRaw("Horizontal"); // 【改动】使用缓存水平输入
        if (cachedHorizontalInput > 0)
        {
            targetScreenX = defaultTargetScreenX - targetScreenXOffset;
        }
        else if (cachedHorizontalInput < 0)
        {
            targetScreenX = defaultTargetScreenX + targetScreenXOffset;
        }
        else
        {
            targetScreenX = defaultTargetScreenX;
        }
        
        //先手动更改testConfig来进行测试。
        VirtualCameraConfig testConfig = screenXrotationConfigTest; // 【改动】构造测试配置
        
        ChangeRotationAndPYRDampingWhenRotate(testConfig, Time.deltaTime); // 【改动】调用新的方法
        
        // 计算距离差，决定不同过渡时间
        float distance = Mathf.Abs(targetScreenX - composer.m_ScreenX);
        
        // 根据距离调整 smoothTime，让远距离切换更慢
        float smoothTime = Mathf.Lerp(0.3f, 0.8f, distance / 0.4f);  // 0.4 是最大可能的变动范围
        
        // 限制 smoothTime 防止过长或过短
        smoothTime = Mathf.Clamp(smoothTime, 0.3f, 0.6f);
        composer.m_ScreenX = Mathf.SmoothDamp(composer.m_ScreenX, targetScreenX, ref smoothVelocity, smoothTime);

        if (velocityStateFixed)
        {
            return;
        }
        //targetRotation = target.eulerAngles;
        //Debug.Log("target rotation is:"+targetRotation+"\n is grounded = "+player.Grounded);
        if (player.isGrounded && rb.velocity.magnitude <= 30f)
        {
            testState = VelocityState.Idle;
        }else if (player.isGrounded && rb.velocity.magnitude is > 30f and < 48f)
        {
            testState = VelocityState.Landed;
        }else if (player.isGrounded && rb.velocity.magnitude >= 48f)
        {
            testState = VelocityState.RegularMaxSpeed;
        }
        else
        {
            testState = VelocityState.InAir;
        }
    }
    
    void LateUpdate()
    {
        if (virtualCameraController == null)
        {
            Debug.LogWarning("没有关联 VirtualCameraController！");
            return;
        }
        


        if (start != true) return;
        // 每帧调用 ApplySettings，传入测试状态和 Time.deltaTime
        virtualCameraController.ApplySettings(testState, Time.deltaTime);

        timer += Time.deltaTime;


        // 你可以在这里增加更多 Debug.Log 语句来监控 Transposer 或 Composer 的其他参数，
        // 例如：transposer damping、composer 参数等

        
    }
    
    // 【改动】新增方法：将 ChangeRotationAndPYRDampingWhenRotate 移植到此脚本中，用于测试 damping 调整功能，不受 start boolean 影响
    public void ChangeRotationAndPYRDampingWhenRotate(VirtualCameraConfig baseConfig, float deltaTime)
    {
        // 读取输入轴，水平控制 Yaw，垂直控制 Pitch；Roll 可根据需要扩展 【改动】
        float horizontalInput = Mathf.Abs(cachedHorizontalInput);
        float verticalInput = Input.GetAxis("Vertical");     // 【改动】

        // 定义输入敏感度（可根据实际情况调整） 【改动】
        //yaw是y rotation，横向旋转
        //yawInputOffset计算方式是1*yawSen
        float yawSensitivity = _yawSen;   // 【改动】
        float pitchSensitivity = 1.0f; // 【改动】
        float rollSensitivity = 1.0f;  // 【改动】（如果需要 Roll 调整）

        float yawInputOffset = horizontalInput * yawSensitivity;   
        float pitchInputOffset = 0f; 
        float rollInputOffset = 0f; 

        // 获取当前虚拟相机的 Transposer 组件 【改动】
        CinemachineTransposer transposer = vcam.GetCinemachineComponent<CinemachineTransposer>(); // 【改动】
        if (transposer == null)
        {
            Debug.LogWarning("未找到 CinemachineTransposer 组件！"); // 【改动】
            return;
        }

        // 将基础 damping 值与输入偏移叠加后赋值 【改动】
        transposer.m_YawDamping = baseConfig.yawDamping + yawInputOffset;       // 【改动】
        transposer.m_PitchDamping = baseConfig.pitchDamping + pitchInputOffset;   // 【改动】
        transposer.m_RollDamping = baseConfig.rollDamping + rollInputOffset;      // 【改动】

        Debug.Log("调整后的 Damping 值：Yaw=" + transposer.m_YawDamping +
                  " Pitch=" + transposer.m_PitchDamping +
                  " Roll=" + transposer.m_RollDamping); // 【改动】输出调试信息
    }

}
