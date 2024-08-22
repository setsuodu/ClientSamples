using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatrixMaker : MonoBehaviour
{
    public Vector3 Position, Rotation, Scale;

    void Start()
    {
        Matrix4x4 matrix4X4 = Matrix4x4.TRS(Position, Quaternion.Euler(Rotation), Scale);
    }

    void Update()
    {
        //Graphics.DrawMeshInstanced();
    }
}