using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �������
public class CameraFllower : MonoBehaviour
{
    public Transform obj;
    public Vector3 offset;

    // ����Ϳ����ٶ�
    public float fSpeed = 10;
    public float lSpeed = 10;

    public void LookAtTrager()
    {
        Vector3 rot = obj.position - this.transform.position;
        Quaternion _rot = Quaternion.LookRotation(rot, Vector3.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, _rot, lSpeed * Time.deltaTime);


    }

    public void MoveToTrager()
    {
        Vector3 pos = obj.position +
            obj.forward * offset.z +
            obj.right * offset.x +
            obj.up * offset.y;
        transform.position = Vector3.Lerp(transform.position, pos, fSpeed*Time.deltaTime);
    }

    public void FixedUpdate()
    {
        LookAtTrager();
        MoveToTrager();
    }
}
