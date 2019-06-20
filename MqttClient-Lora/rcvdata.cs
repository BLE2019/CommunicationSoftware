using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MqttClient_Lora
{
    public partial class receivedata
    {
        public string _devEUI;
        public string _MsgType;
        public int _BeaconNum;
        public int _seqnum;
        public string _BeaconMsg;
        public string _Rcvtime;


        public string devEUI
        {
            set { _devEUI = value; }
            get { return _devEUI; }
        }
        public string MsgType
        {
            set { _MsgType = value; }
            get { return _MsgType; }
        }
        public int BeaconNum
        {
            set { _BeaconNum = value; }
            get { return _BeaconNum; }
        }
        public int seqnum
        {
            set { _seqnum = value; }
            get { return _seqnum; }
        }
        public string BeaconMsg
        {
            set { _BeaconMsg = value; }
            get { return _BeaconMsg; }
        }
        public string Rcvtime
        {
            set { _Rcvtime = value; }
            get { return _Rcvtime; }
        }
    }
}
