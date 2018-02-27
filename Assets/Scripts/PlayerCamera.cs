using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class PlayerCamera : MonoBehaviour
{

    public GameObject player;

    private Vector3 offset;

    Quaternion rotation;

    void Start()
    {
        offset = transform.position - player.transform.position;
        rotation = transform.rotation;
    }

    void LateUpdate()
    {
        transform.position = player.transform.position + offset;
        transform.rotation = rotation;
    }

}
