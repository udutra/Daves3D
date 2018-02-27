using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class ButtonSwitch : NetworkBehaviour
{
    public Animator animator;
    public int idButton;
    public bool bridgeReady = false;

    public Material red;
    public Material green;

    public AudioSource audioS;

    //private static ButtonSwitch instance;
    //public static ButtonSwitch Instance { get { return instance; } }

    private void Start()
    {
        //instance = this;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("Player") && !bridgeReady)
        {
            audioS.Play();
            PlayerNetworkSetup.Instance.CmdButton(true, idButton, gameObject);
            Bridge.Instance.SendAddBridgeStage();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag.Equals("Player") && !bridgeReady)
        {
            PlayerNetworkSetup.Instance.CmdButton(false, idButton, gameObject);
            Bridge.Instance.SendRemoveBridgeStage();
        }
    }

    public void SetBridgeReady()
    {
        animator.SetBool("Press", true);
        bridgeReady = true;
    }

    [Command]
    public void CmdButton(bool trigger, int idButton)
    {
        SetTrigger(trigger, idButton);
        RpcButton(trigger, idButton);
    }

    [ClientRpc]
    public void RpcButton(bool trigger, int idButton)
    {
        SetTrigger(trigger, idButton);
    }

    public void SetTrigger(bool trigger, int idButton)
    {      
        foreach (GameObject bj in GameObject.FindGameObjectsWithTag("ButtonSwitch"))
        {         
            if (bj.GetComponent<ButtonSwitch>().idButton == idButton)
            {
                bj.GetComponent<ButtonSwitch>().animator.SetBool("Press", trigger);

                if (trigger)
                    bj.transform.Find("Cube").GetComponent<Renderer>().material = bj.GetComponent<ButtonSwitch>().green;
                else
                    bj.transform.Find("Cube").GetComponent<Renderer>().material = bj.GetComponent<ButtonSwitch>().red;
            }            
        }
            
    }

}
