using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class ShowItens : NetworkBehaviour
{

    public GameObject pistolPrefab;
    public GameObject jetPackPrefab;
    

    public bool hasPistol;

    void Start()
    {
           
    }

    void FixedUpdate()
    {
        if (isLocalPlayer)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                CmdActiveItens(1);
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                CmdActiveItens(2);
            }
        }
    }

    [Command]
    void CmdActiveItens(int item)
    {
        ActiveItens(item);
        RpcActiveItens(item);
    }

    [ClientRpc]
    void RpcActiveItens(int item)
    {
        ActiveItens(item);
    }

    void ActiveItens(int item)
    {
        if (item == 1)
        {
            pistolPrefab.SetActive(true);
        }

        if (item == 2)
        {
            jetPackPrefab.SetActive(true);
        }
    }
}
