using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CarController : MonoBehaviour
{
    private float angle;
    public float angleSpeed;
    public float moveSpeed;
    public float brakeSpeed;
    // 最大允许扭矩
    public float maxMotorTorque = 3000f;
    // 最大允许转速
    public float maxRpm = 600f;
    public TextMeshProUGUI speedText;

    // 获取车轮碰撞器和车轮模型
    public WheelCollider wcFL;
    public WheelCollider wcRL;
    public WheelCollider wcRR;
    public WheelCollider wcFR;

    public Transform wmFL;
    public Transform wmRL;
    public Transform wmRR;
    public Transform wmFR;

    // 当前车速，千米/时
    public float CurrentSpeedKPH { get; private set; }

    private void Update()
    {
        WheelsControl_Update();
        // 每帧更新速度
        CurrentSpeedKPH = CalculateSpeed();
        speedText.text = $"Front Car Speed: {(int)CurrentSpeedKPH}km/h";
    }

    void WheelsControl_Update()
    {
        // 垂直轴和水平轴
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        // 前轮角度，后轮驱动
        // steerAngle:转向角度，总是围绕自身y轴转向
        // motorTorque:电机转矩，驱动车轮
        angle = angleSpeed * h;
        wcFL.steerAngle = angle;
        wcFR.steerAngle = angle;

        // 修改后代码
        float targetTorque = v * moveSpeed * 20;
        targetTorque = Mathf.Clamp(targetTorque, -maxMotorTorque, maxMotorTorque);
        wcRL.motorTorque = targetTorque;
        wcRR.motorTorque = targetTorque;

        // 当车轮碰撞器位置角度改变，随之变更车轮模型的位置角度
        WheelsModel_Update(wmFL, wcFL);
        WheelsModel_Update(wmFR, wcFR);
        WheelsModel_Update(wmRL, wcRL);
        WheelsModel_Update(wmRR, wcRR);

        // 按住空格刹车
        if (Input.GetKey(KeyCode.LeftShift))
        {
            wcFL.brakeTorque = brakeSpeed * 20;
            wcFR.brakeTorque = brakeSpeed * 20;
            wcRL.brakeTorque = brakeSpeed * 20;
            wcRR.brakeTorque = brakeSpeed * 20;

        } else
        {
            wcFL.brakeTorque = 0;
            wcFR.brakeTorque = 0;
            wcRL.brakeTorque = 0;
            wcRR.brakeTorque = 0;
        }
    }

    // 控制车轮模型移动 转向
    void WheelsModel_Update(Transform t, WheelCollider wheel)
    {
        Vector3 pos = t.position;
        Quaternion rot = t.rotation;

        wheel.GetWorldPose(out pos, out rot);

        t.position = pos;
        t.rotation = rot;
    }

    // 根据驱动轮转速计算实时速度
    float CalculateSpeed()
    {
        // 1. 获取驱动轮转速（后轮驱动）
        float rpmRL = Mathf.Clamp(wcRL.rpm, -maxRpm, maxRpm);
        float rpmRR = Mathf.Clamp(wcRR.rpm, -maxRpm, maxRpm);

        // 2. 计算平均转速并取绝对值（处理倒车负值）
        float avgRpm = (Mathf.Abs(rpmRL) + Mathf.Abs(rpmRR)) / 2f;

        // 3. 计算车轮周长（单位：米）
        float wheelCircumference = 2f * Mathf.PI * wcRL.radius;

        // 4. 转换为米/秒：rpm->转/秒 * 周长
        float speedMps = (avgRpm / 60f) * wheelCircumference;

        // 5. 转换为千米/小时并保留方向（根据前轮驱动判断方向）
        float signedSpeed = (speedMps * 3.6f) * Mathf.Sign(wcRL.motorTorque);

        return signedSpeed;
    }

}
