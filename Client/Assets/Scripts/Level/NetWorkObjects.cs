using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ProjectNetWork{
    public class BasicNetWorkObject{

    }

    public class MessageMatchSuccess : BasicNetWorkObject{
        public string EnemyName = "Name";
        public string EnemyMail = "1234@garena.cn";
    }
    public class MessageSpawnItem:BasicAppObject{
        public int ItemID = 1;
    }
}
public class NetWorkObjects : MonoBehaviour
{
}
