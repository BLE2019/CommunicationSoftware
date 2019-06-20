using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Text;
using System.Windows.Forms;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using System.IO;

namespace MqttClient_Lora
{
    public partial class Form1 : Form
    {
        private string CONFIG_MQTT_SERVER = "49.4.71.166";
        //  private string CONFIG_MQTT_UserName = "1";
        // private string CONFIG_MQTT_UserPass = "DHhgiTB0Fsshel1HaMJgveh-PJX2yXEs5by-E0UdE9s";
        // private string CONFIG_MQTT_UserPass = "";
        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// MQTT 客户端组件
        /// </summary>
        MqttClient client;

        /// <summary>
        /// 回复的序数
        /// </summary>
        //short AckToken = 0;
        string AckToken;

        private void Form1_Load(object sender, EventArgs e)
        {
            string topic = "application/1/node/#";
            try
            {

                // 连接至 mqtt 服务器
                client = new MqttClient(CONFIG_MQTT_SERVER);
                //client = new MqttClient("localhost");
                // client = new MqttClient("127.0.0.1:1883");
                //client = new MqttClient("49.4.71.166");
                // 注册消息接收处理事件（即消息到达时通知的方法）   

                client.MqttMsgPublishReceived += client_MqttMsgPublishReceived;

                //生成客户端ID并连接服务器  
                string clientId = Guid.NewGuid().ToString();
                client.Connect(clientId);
                //client.Connect(clientId);
            }
            catch (Exception me)
            {
                MessageBox.Show(me.Message);
                System.Environment.Exit(0);
            }

            // 订阅主题 应用下的所有终端的数据消息
            //client.Subscribe(new string[] { "application/1/node/3234364702280026/rx" }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
            client.Subscribe(new string[] { topic }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });

            //lblWd.Text = "1";
            //lblSd.Text = DateTime.Now.ToString();
            //MessageBox.Show("MqttClientService打开成功！正在接收数据！");
        }

        /// <summary>
        /// 订阅消息到达事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {

            //处理接收到的消息
            string msg = System.Text.Encoding.Default.GetString(e.Message);
            string dt = DateTime.Now.ToString();
            if (e.Topic.EndsWith("rx"))   // 上行数据消息接收
            {
                // 收到数据
                JObject message = JObject.Parse(msg);  // json 解析 
                String devEUI = message.Value<String>("devEUI");   // 数据来自终端的devEUI
                String payload = message.Value<String>("data");    // 数据部分，base64 编码
                byte[] data = Convert.FromBase64String(payload);   // 原始数据字节数组

                // 按数据格式协议解析
                AnalysisAndUpload tah = AnalysisAndUpload.Parse(data, devEUI);
                if (tah.Ret == true)
                {
                    if (tah.beacon == "蓝牙beacon数据")
                    {
                        Invoke(new MethodInvoker(delegate
                        {
                            textBox1.Text = devEUI + "-" + tah.beacon + "上传数据成功！" + dt+"\r\n"+textBox1.Text;
                        }));
                    }
                    else if (tah.sos == "sos数据")
                    {
                        Invoke(new MethodInvoker(delegate
                        {
                            textBox1.Text = devEUI + "-" + tah.sos + "上传数据成功！" + dt + "\r\n" + textBox1.Text;
                        }));
                    }
                    else if (tah.battery == "终端低电量数据")
                    {
                        Invoke(new MethodInvoker(delegate
                        {
                            textBox1.Text = devEUI + "-" + tah.battery + "上传数据成功！" + dt + "\r\n" + textBox1.Text;
                        }));
                    }
                }
                else
                {

                    if (tah.beacon == "蓝牙beacon数据")
                    {
                        Invoke(new MethodInvoker(delegate
                        {
                            textBox1.Text = devEUI + "-" + tah.beacon + "上传数据失败！" + dt + "\r\n" + textBox1.Text;
                        }));
                    }
                    else if (tah.sos == "sos数据")
                    {
                        Invoke(new MethodInvoker(delegate
                        {
                            textBox1.Text = devEUI + "-" + tah.sos + "上传数据失败！" + dt + "\r\n" + textBox1.Text;
                        }));
                    }
                    else if (tah.battery == "终端低电量数据")
                    {
                        Invoke(new MethodInvoker(delegate
                        {
                            textBox1.Text = devEUI + "-" + tah.battery + "上传数据失败！" + dt + "\r\n" + textBox1.Text;
                        }));
                    }


                }

                //if (tah != null)
                //{
                //    Invoke(new MethodInvoker(delegate
                //        {

                //            lblWd.Text = tah.Temperature.ToString();
                //            lblSd.Text = DateTime.Now.ToString();

                //      //      string data1 = lblWd.Text.TrimEnd((char[])"/n/r".ToCharArray()); 


                //            FileStream fs = new FileStream(@"D:\test.txt", FileMode.Append, FileAccess.Write);
                //            StreamWriter sw = new StreamWriter(fs);
                //            sw.WriteLine("日期：" + lblSd.Text+"     data: "+ lblWd.Text );
                //            sw.Close();
                //            fs.Close();

                //        }));
                //    // 回复序数
                //    DownlinkAckToken();

                //}
            }
        }

        /// <summary>
        /// 下发一个序号：下行 
        /// </summary>
        //private void DownlinkAckToken()
        //{
        //    if (!client.IsConnected) return;  // 未处于连接状态则忽略

        //   // AckToken++; 
        //    //byte[] buff = BitConverter.GetBytes(AckToken);

        //   // AckToken = "OK11";

        //    AckToken = huifu.Text;
        //    byte[] buff = System.Text.Encoding.Default.GetBytes(AckToken);

        //    JObject downData = new JObject();
        //    downData["reference"] = "abcd1234";
        //    downData["confirmed"] = false;
        //    downData["fPort"] = 10;
        //    downData["data"] = Convert.ToBase64String(buff);

        //    String downJson = downData.ToString(Formatting.None);
        //    // 发布至 mqtt tx 主题，即为执行下发
        //    client.Publish("application/1/node/3234364702280026/tx", Encoding.UTF8.GetBytes(downJson), MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE, false);

        //}

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            client.MqttMsgPublishReceived -= client_MqttMsgPublishReceived;
            client.Disconnect();
            System.Environment.Exit(0);
        }

        //private void button1_Click(object sender, EventArgs e)
        //{
        //    this.huifu.Visible = true;
        //    AckToken = huifu.Text;
        //    DownlinkAckToken();


        //} 
    }
}
