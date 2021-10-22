using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ProjectNetWork{
    public enum NetWorkMessageIndex{
        ReqSendTryMatch_LoveCmd  	=1,
	RetMessageMatchSuccess_LoveCmd 	 =2,
	RetPlayerLogin_LoveCmd			=3,
	ReqSendTryBuyItem_LoveCmd		 =4,
	RetMessageSpawnItem_LoveCmd		=5,
	ReqAttack_LoveCmd 				 =6,
	RetLookBackSuccess_LoveCmd		 =7,
	RetMessageSetBattleInfo_LoveCmd	 =8,
	ReqShutScreen_LoveCmd			 =9,
	ReqLightScreen_LoveCmd			 =10,
	ReqCancelAttack_LoveCmd			=11,
	ReqPlayerLogin_LoveCmd			=12,
	RetMessageBuyItemSuccess_LoveCmd =13,
	RetShutScreenReceived_LoveCmd =14,
	RetLightScreenReceived_LoveCmd =15,
	RetAttackReceived_LoveCmd		=16,
	RetCancelAttackReceived_LoveCmd	=17,
	ReqHeartBag_LoveCmd				=18,
	RetSetPlayerInfo_LoveCmd		=19,
	RetBlockSuccess_LoveCmd			=20,
    RetAuction_LoveCmd				=22




    }

    public enum NetWorkMessageType{
        LoveCmd = 5
    }
    [System.Serializable]
    public class PlayerInfo{
        public string PlayerMail = "xxx@garena.cn";
        public int Money = 0;
        public int LookBackTime = 0;//在原来基础上×100000
    }
    [System.Serializable]
    public class NetMessageVar<T>{
        public string _name;
        public T _object;
        public NetMessageVar(object obj){

        }
    }
    [System.Serializable]
    public class NetMessage{
        public NetWorkMessageType MessageType = NetWorkMessageType.LoveCmd;
        public NetWorkMessageIndex MessageIndex;
        public string PlayerName;
        public string PlayerMail;
        public string EnemyName;
        public string EnemyMail;
        public bool IsSuccess;
        public bool IsEnemySuccess;
        public string isWin;
        public int ItemID;
        public PlayerInfo PlayerOne;
        public PlayerInfo PlayerTwo;
        public string templetStr = "{{\"MessageType\":{0},\"MessageIndex\":{1}{2}}}";
    }

    public static class NetWorkUtility{
        public static string toNetStr(NetMessage netMessage){
            return string.Format(netMessage.templetStr,((uint)netMessage.MessageType),((uint)netMessage.MessageIndex),toStrObject(netMessage));
        }

        public static string toStrObject(NetMessage netMessage){
            string str = ",";
            switch (netMessage.MessageIndex)
            {
                case NetWorkMessageIndex.ReqPlayerLogin_LoveCmd:
                    str += "\"PlayerMail\":\""+netMessage.PlayerMail.ToString()+"\",";
                    str += "\"PlayerName\":\""+netMessage.PlayerName.ToString()+"\"";
                    break;
                case NetWorkMessageIndex.ReqSendTryMatch_LoveCmd:
                    str += "\"PlayerMail\":\""+netMessage.PlayerMail.ToString()+"\"";
                    break;
                case NetWorkMessageIndex.ReqSendTryBuyItem_LoveCmd:
                    str += "\"PlayerMail\":\""+netMessage.PlayerMail.ToString()+"\",";
                    str += "\"ItemID\":"+netMessage.ItemID.ToString()+"";
                    break;
                case NetWorkMessageIndex.ReqAttack_LoveCmd:
                    str += "\"PlayerMail\":\""+netMessage.PlayerMail.ToString()+"\"";
                    break;
                case NetWorkMessageIndex.ReqShutScreen_LoveCmd:
                    str += "\"PlayerMail\":\""+netMessage.PlayerMail.ToString()+"\"";
                    break;
                case NetWorkMessageIndex.ReqLightScreen_LoveCmd:
                    str += "\"PlayerMail\":\""+netMessage.PlayerMail.ToString()+"\"";
                    break;
                case NetWorkMessageIndex.ReqCancelAttack_LoveCmd:
                    str += "\"PlayerMail\":\""+netMessage.PlayerMail.ToString()+"\"";
                    break;
                case NetWorkMessageIndex.ReqHeartBag_LoveCmd:
                    str += "\"PlayerMail\":\""+netMessage.PlayerMail.ToString()+"\"";
                    break;
                default:
                    str = "";
                    break;
            }
            return str;
        }
    }

}