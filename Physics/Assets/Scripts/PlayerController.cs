using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f; // �ƶ��ٶ�
    public float rotateSpeed = 90f; // ��ת�ٶ�

    public Vector3Int pos;
    public Vector3Int rotate;
    public Vector3Int size;

    void Start()
    {

    }

    void Update()
    {
        // �ƶ�
        float moveHorizontal = Input.GetAxis("Horizontal"); // A/D �� ��/�Ҽ�ͷ
        float moveVertical = Input.GetAxis("Vertical"); // W/S �� ��/�¼�ͷ

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
        transform.Translate(movement * moveSpeed * Time.deltaTime, Space.World);

        // ��ת
        float rotateHorizontal = Input.GetAxis("Mouse X"); // ���ˮƽ�ƶ�
        float rotateVertical = Input.GetAxis("Mouse Y"); // ��괹ֱ�ƶ�

        transform.Rotate(Vector3.up, rotateHorizontal * rotateSpeed * Time.deltaTime); // ˮƽ��ת��Yaw��
        // ע�⣺��ֱ��תͨ������������������ǽ�ɫ������Ϊ��ɫͨ��ֻ��ˮƽ�����ƶ���
    }
}