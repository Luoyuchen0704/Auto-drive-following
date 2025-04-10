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
    // �������Ť��
    public float maxMotorTorque = 3000f;
    // �������ת��
    public float maxRpm = 600f;
    public TextMeshProUGUI speedText;

    // ��ȡ������ײ���ͳ���ģ��
    public WheelCollider wcFL;
    public WheelCollider wcRL;
    public WheelCollider wcRR;
    public WheelCollider wcFR;

    public Transform wmFL;
    public Transform wmRL;
    public Transform wmRR;
    public Transform wmFR;

    // ��ǰ���٣�ǧ��/ʱ
    public float CurrentSpeedKPH { get; private set; }

    private void Update()
    {
        WheelsControl_Update();
        // ÿ֡�����ٶ�
        CurrentSpeedKPH = CalculateSpeed();
        speedText.text = $"Front Car Speed: {(int)CurrentSpeedKPH}km/h";
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

        // �޸ĺ����
        float targetTorque = v * moveSpeed * 20;
        targetTorque = Mathf.Clamp(targetTorque, -maxMotorTorque, maxMotorTorque);
        wcRL.motorTorque = targetTorque;
        wcRR.motorTorque = targetTorque;

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

    // ����������ת�ټ���ʵʱ�ٶ�
    float CalculateSpeed()
    {
        // 1. ��ȡ������ת�٣�����������
        float rpmRL = Mathf.Clamp(wcRL.rpm, -maxRpm, maxRpm);
        float rpmRR = Mathf.Clamp(wcRR.rpm, -maxRpm, maxRpm);

        // 2. ����ƽ��ת�ٲ�ȡ����ֵ����������ֵ��
        float avgRpm = (Mathf.Abs(rpmRL) + Mathf.Abs(rpmRR)) / 2f;

        // 3. ���㳵���ܳ�����λ���ף�
        float wheelCircumference = 2f * Mathf.PI * wcRL.radius;

        // 4. ת��Ϊ��/�룺rpm->ת/�� * �ܳ�
        float speedMps = (avgRpm / 60f) * wheelCircumference;

        // 5. ת��Ϊǧ��/Сʱ���������򣨸���ǰ�������жϷ���
        float signedSpeed = (speedMps * 3.6f) * Mathf.Sign(wcRL.motorTorque);

        return signedSpeed;
    }

}
