using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeColorMaterial : MonoBehaviour
{
    [SerializeField] private MeshRenderer _meshRenderer;

    public void SetMaterialColor(Material pColor)
    {
        _meshRenderer.materials[_meshRenderer.materials.Length - 1] = pColor;
        Destroy(this);
    }
}