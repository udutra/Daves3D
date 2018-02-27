using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Bridge : NetworkBehaviour
{
    public Animator animator;

    public AudioSource audioS;
    

    [SyncVar]
    public int stageBridge = 0;

    [SyncVar]
    public bool bridgeReady = false;

    private static Bridge instance;
    public static Bridge Instance { get { return instance; } }

    private void Start()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update () {

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            animator.SetTrigger("Firstmove");
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            animator.SetTrigger("Secmove");
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            animator.SetTrigger("On");
        }

    }

    public string GetTrigger(int stage)
    {
        switch (stage)
        {
            case 0:
                {
                    return "Secmove";                   
                }
            case 1:
                {
                    return "Firstmove";
                }
            case 2:
                {
                    return "On";
                }
            default:
                {
                    return "Secmove";
                }
        }
    }

    public void SendAddBridgeStage()
    {
        if (!bridgeReady)
            PlayerNetworkSetup.Instance.CmdBridgeStage(1);
    }

    public void SendRemoveBridgeStage()
    {
        if (!bridgeReady)
            PlayerNetworkSetup.Instance.CmdBridgeStage(2);
    }

    [Command]
    public void CmdBridgeStage(int option)
    {
        if (option == 1)
            stageBridge++;
        else
            stageBridge--;

        string trigger = GetTrigger(stageBridge);

        BridgeStage(trigger);
        RpcBridgeStage(trigger);
    }

    [ClientRpc]
    public void RpcBridgeStage(string trigger)
    {
        BridgeStage(trigger);
    }

    public void BridgeStage(string trigger)
    {
        animator.SetTrigger(trigger);

        if ("On".Equals(trigger))
        {
            bridgeReady = true;
            audioS.Play();
            foreach (GameObject bj in GameObject.FindGameObjectsWithTag("ButtonSwitch"))
                bj.GetComponent<ButtonSwitch>().SetBridgeReady();
        }
    }
}
