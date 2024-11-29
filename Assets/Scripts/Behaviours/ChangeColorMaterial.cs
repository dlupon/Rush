using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeColorMaterial : MonoBehaviour
{
    [SerializeField] private Renderer _renderer;

    public void SetMaterialColor(Material pColor)
    {
        return;
        _renderer.sharedMaterials[_renderer.materials.Length - 1] = pColor;

        Destroy(this);
    }
}