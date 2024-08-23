using System.Collections.Generic;
using UnityEngine;

public class Instancer : MonoBehaviour
{
    public int Instancers;

    public Mesh Mesh;

    public Material[] Materials;

    public List<List<Matrix4x4>> Batches = new List<List<Matrix4x4>>();

    void Start()
    {
        int AddMatrix = 0;

        for (int i = 0; i < Instancers; i++)
        {
            if (AddMatrix < 1000 && Batches.Count != 0)
            {
                Batches[Batches.Count - 1].Add(Matrix4x4.TRS(new Vector3(Random.Range(0, 50), Random.Range(0, 50), Random.Range(0, 50)), Quaternion.identity, Vector3.one));
                AddMatrix++;
            }
            else
            {
                Batches.Add(new List<Matrix4x4>());
                AddMatrix = 0;
            }
        }
    }

    void Update()
    {
        RenderBatches();
    }

    private void RenderBatches()
    {
        foreach (var Batch in Batches)
        {
            for(int i = 0; i < Mesh.subMeshCount; i++)
            {
                Graphics.DrawMeshInstanced(Mesh, i, Materials[i], Batch);
            }
        }
    }
}
