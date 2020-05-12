using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int id;
    public string UserName;
    public Vector3 position;

    private float speed = 2.0f;
    private bool[] inputs;

    public bool ServerSide;

    //public Player(string _userName)
    //{
    //    UserName = _userName;
    //    position = Vector3.zero;
    //    inputs = new bool[4];


    //    inputs = new bool[5];
    //}

    private void Start()
    {
        speed *= Time.fixedDeltaTime;
    }

    public void SetInput(bool[] _inputs)
    {
        inputs = _inputs;
    }

    private void Move(Vector2 _inputDirection)
    {
        Vector3 _moveDirection = transform.right * _inputDirection.x + transform.forward * _inputDirection.y;
        _moveDirection *= speed;

        //Console.WriteLine($"new pos is: {}");
        //Console.WriteLine($"Inputdir is: {_inputDirection.x},{_inputDirection.y}");        
        //transform.position = new Vector3(transform.position.x + _inputDirection.x, transform.position.y + _inputDirection.y);
        //transform.Translate(_inputDirection * speed);
        transform.Translate(_moveDirection);
        //Console.WriteLine($"new position is: {transform.position.x},{transform.position.y}");

        //Console.WriteLine($"Movedir is: {_moveDirection.x},{_moveDirection.y}, {_moveDirection.z}");
        ServerPacketSender.PlayerPostion(this);
    }

    internal void Initialize(int id, string userName)
    {
        UserName = userName;
        this.id = id;
        //position = Vector3.zero;
        inputs = new bool[4];
    }

    public void FixedUpdate()
    {
        Vector2 _inputDirection = Vector2.zero;
        if (inputs[0])
        {
            _inputDirection.y += 1;
        }
        if (inputs[1])
        {
            _inputDirection.y -= 1;
        }
        if (inputs[2])
        {
            _inputDirection.x += 1;
        }
        if (inputs[3])
        {
            _inputDirection.x -= 1;
        }
        //Console.WriteLine($"Inputdir is: {_inputDirection.x},{_inputDirection.y}");
        Move(_inputDirection);
    }

    //private void Update()
    //{
    //    //transform.Translate(NormalSpeed * Input.GetAxis("Horizontal") * Time.deltaTime,
    //    //0f, NormalSpeed * Input.GetAxis("Vertical") * Time.deltaTime);
    //    //if (!ServerSide)
    //    //{
    //    //    if (Input.GetKey(KeyCode.D))
    //    //    {
    //    //        transform.position += Vector3.right * speed * Time.deltaTime;
    //    //    }
    //    //    if (Input.GetKey(KeyCode.A))
    //    //    {
    //    //        transform.position += Vector3.left * speed * Time.deltaTime;
    //    //    }
    //    //    if (Input.GetKey(KeyCode.W))
    //    //    {
    //    //        transform.position += Vector3.forward * speed * Time.deltaTime;
    //    //    }
    //    //    if (Input.GetKey(KeyCode.S))
    //    //    {
    //    //        transform.position += Vector3.back * speed * Time.deltaTime;
    //    //    }
    //    //}
    //}
}
