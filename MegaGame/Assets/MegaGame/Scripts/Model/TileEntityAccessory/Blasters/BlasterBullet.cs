using MegaGame;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlasterBullet : MonoBehaviour
{
    public Vector3 MoveDirection;
    public float MoveSpeed;
    public int Damage;

    private Rigidbody myRB;

    public BlasterBullet(Vector3 direction, float speed, int damage)
    {
        MoveDirection = direction;
        MoveSpeed = speed;
        Damage = damage;
    }

    // Start is called before the first frame update
    void Start()
    {
        myRB = transform.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        myRB.velocity = new Vector3(MoveDirection.x, MoveDirection.y, MoveDirection.z).normalized * MoveSpeed;
    }
}
