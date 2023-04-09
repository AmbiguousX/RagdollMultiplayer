using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class NetworkPlayer : NetworkBehaviour
{
    [SerializeField]
    protected GameObject localPlayerRoot;
    [SerializeField]
    protected GameObject mirrorPlayerRoot;

    [SerializeField]
    private List<Transform> localPlayerSyncTransList;
    [SerializeField]
    private List<Transform> mirrorPlayerSyncTransList;

    #region  server || client
    private void SetLocalPlayer(bool isPlayer)
    {
        localPlayerRoot.SetActive(isPlayer);
        mirrorPlayerRoot.SetActive(!isPlayer);
    }
    private void Update()
    {
        if (isOwned)
        {
            SendToServer();
        }
    }
    #endregion


    #region client
    public override void OnStartClient()
    {
        base.OnStartClient();
        if (isOwned)
        {
            SetLocalPlayer(true);
        }
        else
        {
            SetLocalPlayer(false);
        }
    }

    private void SendToServer()
    {
        for (int i = 0; i < localPlayerSyncTransList.Count; i++)
        {
            CopyTrans(mirrorPlayerSyncTransList[i], localPlayerSyncTransList[i]);
        }
    }
    private void CopyTrans(Transform trans, Transform fromTrans)
    {
        trans.position = fromTrans.position;
        trans.rotation = fromTrans.rotation;
    }

    #endregion

    #region server
    public override void OnStartServer()
    {
        base.OnStartServer();
        SetLocalPlayer(false);
    }
    #endregion
}
