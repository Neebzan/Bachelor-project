using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmulatedFireball : MonoBehaviour
{
    private Material _material;
    private readonly string _sizeProperty = "Vector1_EE3E2A7D";
    private float _size;

    void Awake()
    {
        _material = GetComponent<MeshRenderer>().material;
    }

    public void Emulate (Vector3 position, float size) {
        transform.position = position;
        _size = size;
        _material.SetFloat(_sizeProperty, _size);
    }
}
