using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    private float angle;
    public float angleSpeed;
    public float moveSpeed;
    public float brakeSpeed;

    // ��ȡ������ײ���ͳ���ģ��
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
        // ��ֱ���ˮƽ��
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        // ǰ�ֽǶȣ���������
        // steerAngle:ת��Ƕȣ�����Χ������y��ת��
        // motorTorque:���ת�أ���������
        angle = angleSpeed * h;
        wcFL.steerAngle = angle;
        wcFR.steerAngle = angle;

        wcRL.motorTorque = v * moveSpeed * 20;
        wcRR.motorTorque = v * moveSpeed * 20;

        // ��������ײ��λ�ýǶȸı䣬��֮�������ģ�͵�λ�ýǶ�
        WheelsModel_Update(wmFL, wcFL);
        WheelsModel_Update(wmFR, wcFR);
        WheelsModel_Update(wmRL, wcRL);
        WheelsModel_Update(wmRR, wcRR);

        // ��ס�ո�ɲ��
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

    // ���Ƴ���ģ���ƶ� ת��
    void WheelsModel_Update(Transform t, WheelCollider wheel)
    {
        Vector3 pos = t.position;
        Quaternion rot = t.rotation;

        wheel.GetWorldPose(out pos, out rot);

        t.position = pos;
        t.rotation = rot;
    }

}
