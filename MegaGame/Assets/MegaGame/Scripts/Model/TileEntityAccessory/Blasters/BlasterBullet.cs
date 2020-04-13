using MegaGame;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlasterBullet : MonoBehaviourPunCallbacks, IPunObservable
{
    public Vector3 MoveDirection;
    public float MoveSpeed;
    public int Damage;

    private float lifespan = 5f;

    private Rigidbody myRB;

    // Start is called before the first frame update
    void Start()
    {
        myRB = transform.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!photonView.IsMine)
        {
            myRB.position = Vector3.MoveTowards(myRB.position, networkPosition, Time.fixedDeltaTime);
            myRB.rotation = Quaternion.RotateTowards(myRB.rotation, networkRotation, Time.fixedDeltaTime * 100.0f);
        }
        else
        {
            myRB.velocity = new Vector3(MoveDirection.x, MoveDirection.y, MoveDirection.z).normalized * MoveSpeed;
        }
    }

    private Vector3 networkPosition;
    private Quaternion networkRotation;
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(this.myRB.position);
            stream.SendNext(this.myRB.rotation);
            stream.SendNext(this.myRB.velocity);
        }
        else
        {
            networkPosition = (Vector3)stream.ReceiveNext();
            networkRotation = (Quaternion)stream.ReceiveNext();
            Vector3 v = (Vector3)stream.ReceiveNext();
            if (v != null && myRB != null)
            {
                myRB.velocity = v;

                float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));
                networkPosition -= (this.myRB.velocity * lag);
            }
        }
    }
}
