using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class PlayerSyncRotation : NetworkBehaviour {

    [SyncVar] private Quaternion syncPlayerRotation;

    [SerializeField]
    private Transform playerTransform;

    [SerializeField]
    private float lerpRate = 20;

    private Quaternion lastPlayerRot;
    private float threshold = 5;


	
	void Start () {
	
	}
	
	
	void Update ()
    {
        TransmitRotation();
        LerpRotations();
	}

    void LerpRotations()
    {
        if(!isLocalPlayer)
        {
            playerTransform.rotation = Quaternion.Lerp(playerTransform.rotation, syncPlayerRotation, Time.deltaTime * lerpRate);
        }        
    }

    [Command]
    void CmdProvideRotationToServer(Quaternion playerRot)
    {
        syncPlayerRotation = playerRot;
    }

    [Client]
    void TransmitRotation()
    {
        if (isLocalPlayer && Quaternion.Angle(playerTransform.rotation, lastPlayerRot) > threshold)
        {
            CmdProvideRotationToServer(playerTransform.rotation);
            lastPlayerRot = playerTransform.rotation;
        }
    }
}
