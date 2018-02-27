using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class GameOverPanel : NetworkBehaviour
{
    public GridLayoutGroup Grid;

    private void Start()
    {
        Grid = GameObject.Find("Panel").GetComponent<GridLayoutGroup>();
        gameObject.transform.SetParent(Grid.transform);
        gameObject.transform.localScale = new Vector3(1, 1, 1);
        gameObject.transform.localPosition = Vector3.zero;

        if(!isServer)
            PlayerNetworkSetup.Instance.CmdRefreshScorePanel();
    }

}
