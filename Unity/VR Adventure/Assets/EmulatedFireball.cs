using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmulatedFireball : MonoBehaviour {
    private Material _material;
    private readonly string _sizeProperty = "Vector1_EE3E2A7D";
    private float _size;
    public int ID { get; private set; }

    void Awake () {
        _material = GetComponent<MeshRenderer>().material;
    }

    public void Init (int id, float size) {
        ID = id;
        _size = size;
    }

    public void Emulate (Vector3 position, float size) {
        transform.position = position;
        _size = size;
        _material.SetFloat(_sizeProperty, _size);
    }
}
