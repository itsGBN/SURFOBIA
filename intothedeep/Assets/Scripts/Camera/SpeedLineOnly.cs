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
        bool isHigh = player.GetCurrentSpeed() + speedOffset >= player.moveSpeed;

        // 2) 选定目标值
        float target = isHigh ? highSpeedTarget : lowSpeedTarget;

        foreach (Image image in images)
        {
            Material mat = image.material;
            // 3) 取当前 Shader 值
            float current = mat.GetFloat("_OpacityPower");

            // 4) 平滑推进到目标
            float next = Mathf.MoveTowards(current, target, changeSpeed * Time.deltaTime);

            // 5) 应用到材质
            mat.SetFloat("_OpacityPower", next);
        }
        
    }
}
