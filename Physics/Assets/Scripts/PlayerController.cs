using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f; // 移动速度
    public float rotateSpeed = 90f; // 旋转速度

    public Vector3Int pos;
    public Vector3Int rotate;
    public Vector3Int size;

    void Start()
    {

    }

    void Update()
    {
        // 移动
        float moveHorizontal = Input.GetAxis("Horizontal"); // A/D 或 左/右箭头
        float moveVertical = Input.GetAxis("Vertical"); // W/S 或 上/下箭头

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
        transform.Translate(movement * moveSpeed * Time.deltaTime, Space.World);

        // 旋转
        float rotateHorizontal = Input.GetAxis("Mouse X"); // 鼠标水平移动
        float rotateVertical = Input.GetAxis("Mouse Y"); // 鼠标垂直移动

        transform.Rotate(Vector3.up, rotateHorizontal * rotateSpeed * Time.deltaTime); // 水平旋转（Yaw）
        // 注意：垂直旋转通常用于摄像机，而不是角色本身，因为角色通常只在水平面上移动。
    }
}