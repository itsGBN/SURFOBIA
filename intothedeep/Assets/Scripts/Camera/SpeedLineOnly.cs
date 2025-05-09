using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeedLineOnly : MonoBehaviour
{
    [Header("dependent components")]
    public PlayerController player;
    public float speedOffset = 0.1f;

    [Header("fast OpacityPower")]
    public float highSpeedTarget = 1.85f;
    [Header("slow OpacityPower")]
    public float lowSpeedTarget = 8f;

    [Header("transition speed(Greater Faster)")]
    public float changeSpeed = 10f;

    public Image image1;
    public Image image2;
    private Material mat1;
    private Material mat2;
    private Image[] images;
    void Start()
    {
        mat1 = Instantiate(image1.material);
        image1.material = mat1;
        mat2 = Instantiate(image2.material);
        image2.material = mat2;
        images = new Image[2];
        images[0] = image1;
        images[1] = image2;
    }

    void Update()
    {
        // 1) 判断当前是否超速
        bool isHigh    = player.GetCurrentSpeed() + speedOffset >= player.moveSpeed;
        // 2) 判断是否在 special spline 上
        bool isSpecial = player.isOnSpecialSpline;
        // 3) 只要超速或在 special spline 上都触发“fast”状态
        bool trigger   = isHigh || isSpecial;

        // 4) 选择目标值
        float target = trigger ? highSpeedTarget : lowSpeedTarget;

        // 5) 平滑推进并应用到两张图
        foreach (var img in images)
        {
            var mat     = img.material;
            float current = mat.GetFloat("_OpacityPower");
            float next    = Mathf.MoveTowards(current, target, changeSpeed * Time.deltaTime);
            mat.SetFloat("_OpacityPower", next);
        }
        
    }
}
