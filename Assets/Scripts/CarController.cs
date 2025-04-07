using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    private float angle;
    public float angleSpeed;
    public float moveSpeed;
    public float brakeSpeed;

    // 获取车轮碰撞器和车轮模型
    public WheelCollider wcFL;
    public WheelCollider wcRL;
    public WheelCollider wcRR;
    public WheelCollider wcFR;

    public Transform wmFL;
    public Transform wmRL;
    public Transform wmRR;
    public Transform wmFR;

    private void Update()
    {
        WheelsControl_Update();
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

        wcRL.motorTorque = v * moveSpeed * 20;
        wcRR.motorTorque = v * moveSpeed * 20;

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

}
