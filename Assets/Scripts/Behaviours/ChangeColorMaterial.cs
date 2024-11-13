using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeColorMaterial : MonoBehaviour
{
    [SerializeField] private MeshRenderer _meshRenderer;

    public void SetMaterialColor(Material pColor)
    {
        print("Set Color " + name);
        _meshRenderer.materials[_meshRenderer.materials.Length - 1] = pColor;
        Destroy(this);
    }
}