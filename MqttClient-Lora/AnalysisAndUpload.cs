using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace MqttClient_Lora
{
    
    public class AnalysisAndUpload
    {
        public bool Ret;
        public string beacon;    //蓝牙beacon
        public string sos;              // SOS
        public string battery;    // 终端低电量
        public static string connectionString = "server=49.4.66.231;database=testjdbc;uid=sa;pwd=admin@txsys2013";
        private static MySqlConnection mySqlConnection;
        private static string ConnectionString1 = "";

        public string Temperature;

        
        /// </summary>
        /// <param name="hexStr">以 16 进制表示的字节序列，每字节可以以空格隔开也可以没有空格</param>
        /// <returns></returns>
        public static AnalysisAndUpload Parse(string hexStr)
        {
            try
            {
                string bs = HexStringToASCII(hexStr);
                // byte[] bs = HexStringToBytes(hexStr);                
                return Parse(bs);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 解析
        /// </summary>
        /// <returns></returns>
        public static AnalysisAndUpload Parse(byte[] bs, String devEUI)
        {
            AnalysisAndUpload ret = new AnalysisAndUpload();
            receivedata rcvdata = new receivedata();
            string dt = DateTime.Now.ToString();
            try
            {
                //byte[] bt = FormatBigFlotBytes(bs, 0);
                string MsgTypeandSeqnum = BitConverter.ToString(bs, 0);
                string MsgType = MsgTypeandSeqnum.Substring(0, 1);
                string Seqnum = MsgTypeandSeqnum.Substring(1, 1);
                if (MsgType == Convert.ToString(0))
                {//蓝牙beacon扫描数据
                    mySqlConnection = new MySqlConnection(connectionString);
                    try
                    {
                        //Open DataBase
                        //打开数据库
                        //int SeqNum;
                        string beaconmsg;
                        mySqlConnection.Open();
                        beaconmsg = MsgTypeandSeqnum.Substring(3, 71);
                        //byte[] bh = FormatBigFlotBytes(bs, 4);
                        //SeqNum = BitConverter.ToInt32(bh, 0);

                        //byte[] bm = FormatBigFlotBytes1(bs, 8);
                        //beaconmsg = BitConverter.ToString(bm, 0);
                        rcvdata.devEUI = devEUI;
                        rcvdata.MsgType = MsgType;
                        rcvdata.BeaconNum = Convert.ToInt32(Seqnum);
                        rcvdata.BeaconMsg = beaconmsg;
                        rcvdata.Rcvtime = dt;

                        bool update = AddBluetooth(rcvdata);
                        ret.Ret = update;
                        ret.beacon = "蓝牙beacon数据";

                    }
                    catch (Exception ex)
                    {
                        //Can not Open DataBase
                        //打开不成功 则连接不成功
                        ret.Ret = false;
                        ret.beacon = "蓝牙beacon数据";
                    }
                    finally
                    {
                        //Close DataBase
                        //关闭数据库连接
                        mySqlConnection.Close();

                    }

                }
                else if (MsgType == Convert.ToString(1))
                {//sos数据
                    mySqlConnection = new MySqlConnection(connectionString);
                    try
                    {
                        //Open DataBase
                        //打开数据库
                        //int SeqNum;
                        //string beaconmsg;
                        mySqlConnection.Open();

                        //byte[] bh = FormatBigFlotBytes(bs, 4);
                        //SeqNum = BitConverter.ToInt32(bh, 0);
                        rcvdata.devEUI = devEUI;
                        rcvdata.MsgType = MsgType;
                        rcvdata.seqnum = Convert.ToInt32(Seqnum);
                        rcvdata.Rcvtime = dt;

                        bool update = AddSos(rcvdata);
                        ret.Ret = update;
                        ret.sos = "sos数据";
                    }
                    catch (Exception ex)
                    {
                        //Can not Open DataBase
                        //打开不成功 则连接不成功
                        ret.Ret = false;
                        ret.sos = "sos数据";
                    }
                    finally
                    {
                        //Close DataBase
                        //关闭数据库连接
                        mySqlConnection.Close();

                    }


                }
                else if (MsgType == Convert.ToString(2))
                {//终端低电量数据
                    mySqlConnection = new MySqlConnection(connectionString);
                    try
                    {
                        //Open DataBase
                        //打开数据库
                        //int SeqNum;
                        //string beaconmsg;
                        mySqlConnection.Open();

                        //byte[] bh = FormatBigFlotBytes(bs, 4);
                        //SeqNum = BitConverter.ToInt32(bh, 0);
                        rcvdata.devEUI = devEUI;
                        rcvdata.MsgType = MsgType;
                        rcvdata.seqnum = Convert.ToInt32(Seqnum);
                        rcvdata.Rcvtime = dt;

                        bool update = AddBattery(rcvdata);
                        ret.Ret = update;
                        ret.battery = "终端低电量数据";
                    }
                    catch (Exception ex)
                    {
                        //Can not Open DataBase
                        //打开不成功 则连接不成功
                        //mqttdata ret = new mqttdata();
                        ret.Ret = false;
                        ret.battery = "终端低电量数据";
                    }
                    finally
                    {
                        //Close DataBase
                        //关闭数据库连接
                        mySqlConnection.Close();

                    }

                }
                //else if (MsgType == Convert.ToString(3))
                //{//GNSS数据
                //    mySqlConnection = new MySqlConnection(connectionString);
                //    try
                //    {
                //        //Open DataBase
                //        //打开数据库
                //        mySqlConnection.Open();


                //    }
                //    catch (Exception ex)
                //    {
                //        //Can not Open DataBase
                //        //打开不成功 则连接不成功


                //    }
                //    finally
                //    {
                //        //Close DataBase
                //        //关闭数据库连接
                //        mySqlConnection.Close();

                //    }


                //}
               

            }
            catch
            {

                //mqttdata ret = new mqttdata();
                ret.Ret = false;
            }

            return ret;


            
        }

        /// <summary>
        /// 将大端模式的反转过来
        /// </summary>
        /// <param name="bs"></param>
        /// <param name="start"></param>
        /// <returns></returns>
        public static byte[] FormatBigFlotBytes(byte[] bs, int start)
        {
            //byte[] bsr = new byte[4];
            //bsr[3] = bs[start];
            //bsr[2] = bs[start + 1];
            //bsr[1] = bs[start + 2];
            //bsr[0] = bs[start + 3];
            //return bsr;

            byte tmp;
            int len = bs.Length;

            for (int i = 0; i < len / 2; i++)
            {
                tmp = bs[len - 1 - i];
                bs[len - 1 - i] = bs[i];
                bs[i] = tmp;
            }
            return bs;
        }

        public static byte[] FormatBigFlotBytes1(byte[] bs, int start)
        {
            byte[] bsr = new byte[16];
            bsr[15] = bs[start];
            bsr[14] = bs[start + 1];
            bsr[13] = bs[start + 2];
            bsr[12] = bs[start + 3];
            bsr[11] = bs[start + 4];
            bsr[10] = bs[start + 5];
            bsr[9] = bs[start + 6];
            bsr[8] = bs[start + 7];
            bsr[7] = bs[start + 8];
            bsr[6] = bs[start + 9];
            bsr[5] = bs[start + 10];
            bsr[4] = bs[start + 11];
            bsr[3] = bs[start + 12];
            bsr[2] = bs[start + 13];
            bsr[1] = bs[start + 14];
            bsr[0] = bs[start + 15];
            return bsr;
        }

        /// <summary>
        /// 16 进制字节序列转换为字节数组
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        public static byte[] HexStringToBytes(string hexString)
        {
            hexString = hexString.Replace(" ", "");
            if ((hexString.Length % 2) != 0)
                hexString += " ";
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2).Replace(" ", ""), 16);
            return returnBytes;
        }
        public static string HexStringToASCII(string hexstring)
        {
            byte[] bt = HexStringToBinary(hexstring);
            string lin = "";
            for (int i = 0; i < bt.Length; i++)
            {
                lin = lin + bt[i] + " ";
            }


            string[] ss = lin.Trim().Split(new char[] { ' ' });
            char[] c = new char[ss.Length];
            int a;
            for (int i = 0; i < c.Length; i++)
            {
                a = Convert.ToInt32(ss[i]);
                c[i] = Convert.ToChar(a);
            }

            string b = new string(c);
            return b;
        }
        /// <summary>
                /// 16进制字符串转换为二进制数组
                /// </summary>
                /// <param name="hexstring">字符串每个字节之间都应该有空格，大多数的串口通讯资料上面的16进制都是字节之间都是用空格来分割的。</param>
                /// <returns>返回一个二进制字符串</returns>
        public static byte[] HexStringToBinary(string hexstring)
        {

            string[] tmpary = hexstring.Trim().Split(' ');
            byte[] buff = new byte[tmpary.Length];
            for (int i = 0; i < buff.Length; i++)
            {
                buff[i] = Convert.ToByte(tmpary[i], 16);
            }
            return buff;
        }

        public static bool AddBluetooth(receivedata data)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into testjdbc.beacon(");
            strSql.Append("deveui,msgtype,beaconnum,beaconmsg,rcvtime)");
            strSql.Append(" values (");
            strSql.Append("@devEUI,@MsgType,@BeaconNum,@BeaconMsg,@Rcvtime)");
            strSql.Append(";select @@IDENTITY");
            MySqlParameter[] parameters = {
                    new MySqlParameter("@devEUI", MySqlDbType.VarChar,64),
                    new MySqlParameter("@MsgType", MySqlDbType.VarChar,4),
                    new MySqlParameter("@BeaconNum", MySqlDbType.Int32,4),
                    new MySqlParameter("@BeaconMsg", MySqlDbType.VarChar,255),
                    new MySqlParameter("@Rcvtime", MySqlDbType.DateTime,0) };
            parameters[0].Value = data.devEUI;
            parameters[1].Value = data.MsgType;
            parameters[2].Value = data.BeaconNum;
            parameters[3].Value = data.BeaconMsg;
            parameters[4].Value = data.Rcvtime;
            int rows = ExecuteSql(strSql.ToString(), parameters);
            if (rows > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static bool AddSos(receivedata data)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into testjdbc.sos(");
            strSql.Append("deveui,msgtype,seqnum,rcvtime)");
            strSql.Append(" values (");
            strSql.Append("@devEUI,@MsgType,@seqnum,@Rcvtime)");
            strSql.Append(";select @@IDENTITY");
            MySqlParameter[] parameters = {
                    new MySqlParameter("@devEUI", MySqlDbType.VarChar,4),
                    new MySqlParameter("@MsgType", MySqlDbType.VarChar,4),
                    new MySqlParameter("@seqnum", MySqlDbType.Int32,4),
                    new MySqlParameter("@Rcvtime", MySqlDbType.DateTime,0) };
            parameters[0].Value = data.devEUI;
            parameters[1].Value = data.MsgType;
            parameters[2].Value = data.seqnum;
            parameters[3].Value = data.Rcvtime;
            int rows = ExecuteSql(strSql.ToString(), parameters);
            if (rows > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static bool AddBattery(receivedata data)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into testjdbc.battery(");
            strSql.Append("deveui,msgtype,seqnum,rcvtime)");
            strSql.Append(" values (");
            strSql.Append("@devEUI,@MsgType,@seqnum,@Rcvtime)");
            strSql.Append(";select @@IDENTITY");
            MySqlParameter[] parameters = {
                    new MySqlParameter("@devEUI", MySqlDbType.VarChar,4),
                    new MySqlParameter("@MsgType", MySqlDbType.VarChar,4),
                    new MySqlParameter("@seqnum", MySqlDbType.Int32,4),
                    new MySqlParameter("@Rcvtime", MySqlDbType.DateTime,0) };
            parameters[0].Value = data.devEUI;
            parameters[1].Value = data.MsgType;
            parameters[2].Value = data.seqnum;
            parameters[3].Value = data.Rcvtime;
            int rows = ExecuteSql(strSql.ToString(), parameters);
            if (rows > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static int ExecuteSql(string SQLString, params MySqlParameter[] cmdParms)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    try
                    {
                        PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                        int rows = cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();
                        return rows;
                    }
                    catch (MySqlException e)
                    {
                        throw e;
                    }
                }
            }
        }

        private static void PrepareCommand(MySqlCommand cmd, MySqlConnection conn, MySqlTransaction trans, string cmdText, MySqlParameter[] cmdParms)
        {
            if (conn.State != ConnectionState.Open)
                conn.Open();
            cmd.Connection = conn;
            cmd.CommandText = cmdText;
            if (trans != null)
                cmd.Transaction = trans;
            cmd.CommandType = CommandType.Text;//cmdType;
            if (cmdParms != null)
            {


                foreach (MySqlParameter parameter in cmdParms)
                {
                    if ((parameter.Direction == ParameterDirection.InputOutput || parameter.Direction == ParameterDirection.Input) &&
                        (parameter.Value == null))
                    {
                        parameter.Value = DBNull.Value;
                    }
                    cmd.Parameters.Add(parameter);
                }
            }
        }



    }
}
