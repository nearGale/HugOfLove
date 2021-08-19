using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerId
{
    Left,
    Right,
}

public class DataManager : Single<DataManager>
{
    private Dictionary<PlayerId, PlayerStatus> m_ListPlayerStatus;

    public DataManager()
    {
        m_ListPlayerStatus = new Dictionary<PlayerId, PlayerStatus>
        {
            {
                PlayerId.Left, new PlayerStatus
                {
                    Money = 10000,
                    BuyCd = 0,
                    ReturnDuration = 0
                }
            },
            {
                PlayerId.Right, new PlayerStatus
                {
                    Money = 10000,
                    BuyCd = 0,
                    ReturnDuration = 0
                }
            }
        };
    }

    #region Set Func
    public void DecreaseMoney(PlayerId id, int decrease)
    {
        PlayerStatus status = GetPlayerStatus(id);
        if(status == null)
        {
            return;
        }

        status.Money -= decrease;
    }

    public void IncreaseReturnDuration(PlayerId id, int increase)
    {
        PlayerStatus status = GetPlayerStatus(id);
        if (status == null)
        {
            return;
        }

        status.ReturnDuration += increase;
    }

    public void ClearReturnDuration(PlayerId id)
    {
        PlayerStatus status = GetPlayerStatus(id);
        if (status == null)
        {
            return;
        }

        status.ReturnDuration = 0;
    }


    #endregion

    #region Get Func
    public int GetPlayerMoney(PlayerId id)
    {
        if(m_ListPlayerStatus == null)
        {
            return 0;
        }


        PlayerStatus status = GetPlayerStatus(id);
        if(status == null)
        {
            return 0;
        }

        return status.Money;
    }

    public float GetBuyCd(PlayerId id)
    {
        if (m_ListPlayerStatus == null)
        {
            return 0;
        }


        PlayerStatus status = GetPlayerStatus(id);
        if (status == null)
        {
            return 0;
        }

        return status.BuyCd;
    }

    public float GetReturnDuration(PlayerId id)
    {
        if (m_ListPlayerStatus == null)
        {
            return 0;
        }


        PlayerStatus status = GetPlayerStatus(id);
        if (status == null)
        {
            return 0;
        }

        return status.ReturnDuration;
    }


    private PlayerStatus GetPlayerStatus(PlayerId id)
    {
        if (m_ListPlayerStatus == null)
        {
            return null;
        }

        PlayerStatus status = null;
        m_ListPlayerStatus.TryGetValue(id, out status);

        return status;
    }
    #endregion


}
