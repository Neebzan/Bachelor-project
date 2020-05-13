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

    VrPlayerServer vrPlayer = new VrPlayerServer();


    private void Start()
    {
        speed *= Time.fixedDeltaTime;
    }

    public void SetInput(bool[] _inputs)
    {
        inputs = _inputs;
    }

    public void SetHand(HandDataPacket packet, bool left = false)
    {
        if (left)
            vrPlayer.LeftHand = packet;
        else
            vrPlayer.RightHand = packet;
    }    

    public void SetHead(Vector3 pos, Quaternion rot)
    {
        vrPlayer.HeadPos = pos;
        vrPlayer.HeadRot = rot;
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
        vrPlayer.id = id;
        //position = Vector3.zero;
        //inputs = new bool[4];
    }

    public void FixedUpdate()
    {
        //Send player information
        ServerPacketSender.HeadData(vrPlayer);
        ServerPacketSender.VRRightHandData(vrPlayer);
        ServerPacketSender.VRLeftHandData(vrPlayer);


        //Vector2 _inputDirection = Vector2.zero;
        //if (inputs[0])
        //{
        //    _inputDirection.y += 1;
        //}
        //if (inputs[1])
        //{
        //    _inputDirection.y -= 1;
        //}
        //if (inputs[2])
        //{
        //    _inputDirection.x += 1;
        //}
        //if (inputs[3])
        //{
        //    _inputDirection.x -= 1;
        //}
        //Move(_inputDirection);
    }


}
