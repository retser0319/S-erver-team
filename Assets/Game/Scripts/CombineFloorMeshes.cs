using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class CombineFloorMeshes : MonoBehaviour
{
    void Start()
    {
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length - 1];
        int i = 0;

        Material material = null;

        foreach (var mf in meshFilters)
        {
            if (mf.transform == transform) continue; // 자기 자신 제외

            if (material == null && mf.GetComponent<MeshRenderer>() != null)
                material = mf.GetComponent<MeshRenderer>().sharedMaterial;

            combine[i].mesh = mf.sharedMesh;
            combine[i].transform = mf.transform.localToWorldMatrix;
            i++;
        }

        Mesh combinedMesh = new Mesh();
        combinedMesh.CombineMeshes(combine);

        var meshFilter = GetComponent<MeshFilter>();
        meshFilter.sharedMesh = combinedMesh;

        var meshRenderer = GetComponent<MeshRenderer>();
        if (material != null)
            meshRenderer.sharedMaterial = material; // ✅ 머티리얼 복사!

        // 자식 오브젝트는 꺼두기 (필요하면 주석 처리 가능)
        //foreach (Transform child in transform)
            //child.gameObject.SetActive(false);
    }
}
