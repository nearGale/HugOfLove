using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ProjectNetWork{
    public enum NetWorkMessageIndex{
        ReqSendTryMatch_LoveCmd  			=1,
	    RetMessageMatchSuccess_LoveCmd 	 =2,
	    MessageSpawnItem_LoveCmd 		 =3,
	    ReqSendTryBuyItem_LoveCmd		 =4,
	    MessageBuyItemSuccess_LoveCmd 	 =5,
	    ReqAttack_LoveCmd 				 =6,
	    MessageBeAttacked_LoveCmd		 =7,
	    MessageSetBattleInfo_LoveCmd	 =8,
	    ReqShutScreen_LoveCmd			 =9,
	    ReqLightScreen_LoveCmd			 =10,
	    ReqCancelAttack_LoveCmd			=11,
        ReqPlayerLogintwo_LoveCmd 			=12

    }

    public enum NetWorkMessageType{
        LoveCmd = 5
    }
    public class PlayerInfo{
        public string Mail = "xxx@garena.cn";
        public int Money = 0;
        public int AttackTime = 0;
    }

    public class NetMessageVar<T>{
        public string _name;
        public T _object;
        public NetMessageVar(object obj){

        }
    }
    public class NetMessage{
        public NetWorkMessageType MessageType = NetWorkMessageType.LoveCmd;
        public NetWorkMessageIndex MessageIndex;
        public string PlayerName;
        public string PlayerMail;
        public string EnemyName;
        public string EnemyMail;
        public bool isSuccess;
        public bool isEnemySuccess;
        public string isWin;
        public int ItemID;
        public PlayerInfo Player1;
        public PlayerInfo Player2;
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
                case NetWorkMessageIndex.ReqPlayerLogintwo_LoveCmd:
                    str += "\"PlayerMail\":\""+netMessage.PlayerMail.ToString()+"\",";
                    str += "\"PlayerName\":\""+netMessage.PlayerName.ToString()+"\"";
                    break;
                case NetWorkMessageIndex.ReqSendTryMatch_LoveCmd:
                    str += "\"PlayerMail\":\""+netMessage.PlayerMail.ToString()+"\"";
                    break;
                default:break;
            }
            return str;
        }
    }

}