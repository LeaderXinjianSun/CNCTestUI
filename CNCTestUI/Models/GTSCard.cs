using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CNCTestUI.Models
{
    public delegate void StatusUpdateHandler(object sender, string status);
    public delegate void MessagePrintHandler(object sender, string message);
    public class GTSCard
    {
        #region 变量
        public AxisParm X1;//横移轴
        public AxisParm Y1;//直线电机1（左）
        public AxisParm Z1;//Z轴1（左）
        public AxisParm R1;//R轴1（左）
        #endregion
        #region 属性
        private short sRtn;
        public short SRtn
        {
            get
            {
                return sRtn;
            }
            set
            {
                sRtn = value;
                if (sRtn != 0)
                {
                    if (MessagePrintEvent != null)
                    {
                        MessagePrintEvent(this, GetCardReturnValueMessage(sRtn));
                    }
                }
            }
        }
        private bool openCardOk = false;
        public bool OpenCardOk
        {
            get { return openCardOk; }
            set
            {
                openCardOk = value;
                if (StateUpdateEvent != null)
                {
                    StateUpdateEvent(this, openCardOk ? "连接" : "断开");
                }
            }
        }
        #endregion
        #region 事件
        public event StatusUpdateHandler StateUpdateEvent;
        public event MessagePrintHandler MessagePrintEvent;
        #endregion
        #region 实例化
        private static GTSCard _instance;
        private static object _lock = new object();
        public static GTSCard Instance
        {
            get
            {
                if (GTSCard._instance == null)
                {
                    object @lock = GTSCard._lock;
                    lock (@lock)
                    {
                        if (GTSCard._instance == null)
                        {
                            GTSCard._instance = new GTSCard();
                        }
                    }
                }
                return GTSCard._instance;
            }
        }
        #endregion
        #region 功能函数
        public void Init()
        {
            LoadAxisParm();
            OpenCardOk = OpenCard();
        }
        public void LoadAxisParm()
        {
            List<AxisParm> axisParmList;
            try
            {
                string jsonString = File.ReadAllText(System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "AxisParm.json"));
                axisParmList = JsonConvert.DeserializeObject<List<AxisParm>>(jsonString);
            }
            catch (Exception ex)
            {
                if (MessagePrintEvent != null)
                {
                    MessagePrintEvent(this, ex.Message);
                    return;
                }
                axisParmList = new List<AxisParm>();
                for (int i = 0; i < 4; i++)
                {
                    axisParmList.Add(default(AxisParm));
                }
                string jsonString = JsonConvert.SerializeObject(axisParmList, Formatting.Indented);
                File.WriteAllText(System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "AxisParm.json"), jsonString);
            }
            X1 = axisParmList[0];
            Y1 = axisParmList[1];
            Z1 = axisParmList[2];
            R1 = axisParmList[3];

        }
        public bool OpenCard()
        {
            try
            {
                SRtn = gts.mc.GT_Open(0, 0, 1);
                if (SRtn != 0)
                {
                    return false;                    
                }
                SRtn = gts.mc.GT_Reset(0);
                if (SRtn != 0)
                {
                    return false;
                }
                SRtn = gts.mc.GT_LoadConfig(0, "GTS800.cfg");
                if (SRtn != 0)
                {
                    return false;
                }
                SRtn = gts.mc.GT_ClrSts(0, 1, 4);
                if (SRtn != 0)
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                if (MessagePrintEvent != null)
                {
                    MessagePrintEvent(this, ex.Message);
                }
                return false;
            }
        }
        string GetCardReturnValueMessage(short srtn)
        {
            string rString = "";
            switch (srtn)
            {
                case 0:
                    rString = "指令执行成功";
                    break;
                case 1:
                    rString = "指令执行错误";
                    break;
                case 2:
                    rString = "license不支持";
                    break;
                case 7:
                    rString = "指令参数错误";
                    break;
                case -1:
                    rString = "主机和运动控制器通讯失败";
                    break;
                case -6:
                    rString = "打开控制器失败";
                    break;
                case -7:
                    rString = "运动控制器没有响应";
                    break;
                default:
                    rString = "未知错误 " + srtn.ToString();
                    break;
            }
            return rString;
        }
        #endregion
        #region 轴卡功能
        public AxisStatus GetAxisStatus(AxisParm axisParm)
        {
            int AxisStatus = 0;
            uint temp_pClock = 0;
            gts.mc.GT_GetSts(axisParm.CardNo, axisParm.AxisId, out AxisStatus, 1, out temp_pClock);
            int pValue;
            AxisStatus result = default(AxisStatus);
            result.FlagServoOn = (AxisStatus & 0x200) == 0x200;
            result.FlagAlm = (AxisStatus & 0x2) == 0x2;
            result.FlagPosLimit = (AxisStatus & 0x20) == 0x20;
            result.FlagNeglimit = (AxisStatus & 0x40) == 0x40;
            result.FlagMoveEnd = (AxisStatus & 0x400) != 0x400;
            result.FlagEmgStop = (AxisStatus & 0x100) == 0x100;
            result.FlagSmoothStop = (AxisStatus & 0x80) == 0x80;

            gts.mc.GT_GetDi(axisParm.CardNo, gts.mc.MC_HOME, out pValue);
            result.FlagHome = (pValue & (1 << axisParm.AxisId - 1)) == 0;
            double pValue1;
            uint pClock;
            gts.mc.GT_GetPrfPos(axisParm.CardNo, axisParm.AxisId, out pValue1, 1,out pClock);
            result.PrfPos = pValue1 * axisParm.Equiv;
            gts.mc.GT_GetEncPos(axisParm.CardNo, axisParm.AxisId, out pValue1, 1, out pClock);
            result.EncPos = pValue1 * axisParm.Equiv;
            return result;
        }
        public bool GetAxisAlarm(AxisParm axisParm)
        {
            int AxisStatus = 0;
            uint temp_pClock = 0;
            gts.mc.GT_GetSts(axisParm.CardNo, axisParm.AxisId, out AxisStatus, 1, out temp_pClock);
            return (AxisStatus & 0x2) == 0x2;
        }
        public double GetPos(AxisParm axisParm)
        {
            double pValue1;
            uint pClock;
            gts.mc.GT_GetPrfPos(axisParm.CardNo, axisParm.AxisId, out pValue1, 1, out pClock);
            return pValue1 * axisParm.Equiv;
        }
        public double GetEnc(AxisParm axisParm)
        {
            double pValue1;
            uint pClock;
            gts.mc.GT_GetEncPos(axisParm.CardNo, axisParm.AxisId, out pValue1, 1, out pClock);
            return pValue1 * axisParm.Equiv;
        }
        public void ServoOn(AxisParm axisParm)
        {
            gts.mc.GT_AxisOn(axisParm.CardNo, axisParm.AxisId);
        }
        public void ServoOff(AxisParm axisParm)
        {
            gts.mc.GT_AxisOff(axisParm.CardNo, axisParm.AxisId);
        }
        public void ClearAlm(AxisParm axisParm)
        {            
            gts.mc.GT_SetDoBit(axisParm.CardNo, gts.mc.MC_CLEAR, axisParm.AxisId, 0);
            System.Threading.Thread.Sleep(1000);
            gts.mc.GT_SetDoBit(axisParm.CardNo, gts.mc.MC_CLEAR, axisParm.AxisId, 1);
            gts.mc.GT_ClrSts(axisParm.CardNo, axisParm.AxisId, 1);
        }
        public bool GetDi(ushort bitno)
        {
            int pValue;
            gts.mc.GT_GetDi(0, gts.mc.MC_GPI, out pValue);
            return (pValue & (1 << bitno)) == 0;
        }
        public void SetDo(short outbit, short value)
        {
            gts.mc.GT_SetDoBit(0, gts.mc.MC_GPO, (short)(outbit + 1), value);
        }
        public int GetDiPort1()
        {
            int pValue;
            gts.mc.GT_GetDi(0, gts.mc.MC_GPI, out pValue);
            return pValue;
        }
        public int GetDoPort1()
        {
            int pValue;
            gts.mc.GT_GetDo(0, gts.mc.MC_GPO, out pValue);
            return pValue;
        }
        public void SigAxisHomeMove(AxisParm _axisParam)
        {
            gts.mc.GT_ZeroPos(_axisParam.CardNo, _axisParam.AxisId, 1);
            gts.mc.THomePrm tHomePrm;
            SRtn = gts.mc.GT_GetHomePrm(_axisParam.CardNo, _axisParam.AxisId, out tHomePrm);
            tHomePrm.mode = _axisParam.HomeMode;//gts.mc.HOME_MODE_LIMIT_HOME
            tHomePrm.moveDir = _axisParam.HomeDir;
            tHomePrm.indexDir = (short)(_axisParam.HomeDir * -1);
            tHomePrm.edge = 0;
            tHomePrm.triggerIndex = -1;
            tHomePrm.velHigh = _axisParam.HomeHSpd / _axisParam.Equiv / 1000;
            tHomePrm.velLow = _axisParam.HomeLSpd / _axisParam.Equiv / 1000;
            tHomePrm.acc = _axisParam.Acc / 10;
            tHomePrm.dec = _axisParam.Acc / 10;
            tHomePrm.searchHomeDistance = 1000000;
            tHomePrm.searchIndexDistance = 0;
            tHomePrm.escapeStep = 100;
            sRtn = gts.mc.GT_GoHome(_axisParam.CardNo, _axisParam.AxisId, ref tHomePrm);//启动 Smart Home 回原点
        }
        public bool SigAxisHomeCheckDone(AxisParm _axisParam)
        {
            gts.mc.THomeStatus tHomeSts;
            gts.mc.GT_GetHomeStatus(_axisParam.CardNo, _axisParam.AxisId, out tHomeSts);
            if (tHomeSts.run == 0)
            {
                if (tHomeSts.stage == gts.mc.HOME_STAGE_END)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        public void SigAxisHomeZeroSet(AxisParm _axisParam)
        {
            gts.mc.GT_ZeroPos(_axisParam.CardNo, _axisParam.AxisId, 1);//清零规划位置和实际位置，并进行零漂补偿。
        }
        public void SigAxisSetBand(AxisParm _axisParam, int band, int time)
        {
            gts.mc.GT_SetAxisBand(_axisParam.CardNo, _axisParam.AxisId, band, time);
        }
        public void SigAxisPosSet(AxisParm _axisParam, double value)
        {
            gts.mc.GT_SetPrfPos(_axisParam.CardNo, _axisParam.AxisId, (int)Math.Round(value / _axisParam.Equiv, 0));
        }
        public void SigAxisEncSet(AxisParm _axisParam, double value)
        {
            gts.mc.GT_SetEncPos(_axisParam.CardNo, _axisParam.AxisId, (int)Math.Round(value / _axisParam.Equiv, 0));
        }
        public void AxisPosMove(ref AxisParm _AxisParam, double givePos, double speed = 0.0)
        {
            _AxisParam.Target = givePos / _AxisParam.Equiv;
            gts.mc.TTrapPrm ATrapPrm = new gts.mc.TTrapPrm();
            gts.mc.GT_PrfTrap(_AxisParam.CardNo, _AxisParam.AxisId);
            gts.mc.GT_GetTrapPrm(_AxisParam.CardNo, _AxisParam.AxisId, out ATrapPrm); //读取点位模式运动参数
            ATrapPrm.acc = _AxisParam.Acc;
            ATrapPrm.dec = _AxisParam.Acc;
            ATrapPrm.smoothTime = (short)25;
            gts.mc.GT_SetTrapPrm(_AxisParam.CardNo, _AxisParam.AxisId, ref ATrapPrm); //设置点位模式运动参数
            gts.mc.GT_SetPos(_AxisParam.CardNo, _AxisParam.AxisId, (int)Math.Round(givePos / _AxisParam.Equiv, 0)); //设置目标位置
            double max_Vel;
            if (speed == 0.0)
            {
                max_Vel = _AxisParam.MaxWorkSpd / _AxisParam.Equiv / 1000;
            }
            else
            {
                max_Vel = speed / _AxisParam.Equiv / 1000;
            }
            gts.mc.GT_SetVel(_AxisParam.CardNo, _AxisParam.AxisId, max_Vel); //设置目标速度
            gts.mc.GT_Update(_AxisParam.CardNo, 1 << (_AxisParam.AxisId - 1)); //启动当前轴运动
        }
        /// <summary>
        /// PT运动
        /// </summary>
        /// <param name="_AxisParam"></param>
        /// <param name="data">脉冲、毫秒</param>
        /// <param name="fifo"></param>
        public void AxisPTMove(ref AxisParm _AxisParam, PTData[] data,short fifo)
        {
            double pos = 0;
            int time = 0;
            gts.mc.GT_PrfPt(_AxisParam.CardNo, _AxisParam.AxisId, gts.mc.PT_MODE_STATIC);//设置静态PT运动模式
            gts.mc.GT_PtClear(_AxisParam.CardNo, _AxisParam.AxisId, fifo);//清空FIFO0
            for (int i = 0; i < data.Length; i++)
            {
                pos += (int)Math.Round(data[i].pos / _AxisParam.Equiv, 0);
                time += data[i].time;
                short type1 = gts.mc.PT_SEGMENT_NORMAL;
                if (i == data.Length - 1)
                {
                    type1 = gts.mc.PT_SEGMENT_STOP;
                }
                gts.mc.GT_PtData(_AxisParam.CardNo, _AxisParam.AxisId, pos, time, type1, fifo);
            }
            double pValue1;
            uint pClock;
            gts.mc.GT_GetEncPos(_AxisParam.CardNo, _AxisParam.AxisId, out pValue1, 1, out pClock);
            _AxisParam.Target = pValue1 + pos;
            int option = fifo;
            gts.mc.GT_PtStart(_AxisParam.CardNo, 1 << (_AxisParam.AxisId - 1), option << (_AxisParam.AxisId - 1));//启动FIFO的PT运动
        }
        /// <summary>
        /// 设置坐标系
        /// </summary>
        /// <param name="offsetx"></param>
        /// <param name="offsety"></param>
        /// <returns></returns>
        public bool SetCrd(double offsetx,double offsety)
        {
            gts.mc.TCrdPrm crdPrm;

            crdPrm.dimension = 2;                        // 建立二维的坐标系
            crdPrm.synVelMax = 1000;                      // 坐标系的最大合成速度是: 500 pulse/ms
            crdPrm.synAccMax = 20;                        // 坐标系的最大合成加速度是: 2 pulse/ms^2
            crdPrm.evenTime = 0;                         // 坐标系的最小匀速时间为0
            crdPrm.profile1 = 1;                       // 规划器1对应到X轴                       
            crdPrm.profile2 = 2;                       // 规划器2对应到Y轴
            crdPrm.profile3 = 0;                       
            crdPrm.profile4 = 0;
            crdPrm.profile5 = 0;
            crdPrm.profile6 = 0;
            crdPrm.profile7 = 0;
            crdPrm.profile8 = 0;
            crdPrm.setOriginFlag = 1;                    // 需要设置加工坐标系原点位置
            crdPrm.originPos1 = (int)(offsetx / X1.Equiv);                     // 加工坐标系原点位置在(0,0,0)，即与机床坐标系原点重合
            crdPrm.originPos2 = (int)(offsety / Y1.Equiv);
            crdPrm.originPos3 = 0;
            crdPrm.originPos4 = 0;
            crdPrm.originPos5 = 0;
            crdPrm.originPos6 = 0;
            crdPrm.originPos7 = 0;
            crdPrm.originPos8 = 0;

            SRtn = gts.mc.GT_SetCrdPrm(0, 1, ref crdPrm);
            if (SRtn != 0)
            {
                return false;
            }
            return true;
        }
        public void AxisLnXYMove(double targetx,double targety,double speed)
        {
            gts.mc.GT_CrdClear(0, 1, 0);
            gts.mc.GT_LnXY(0, 1, (int)(targetx / X1.Equiv), (int)(targety / Y1.Equiv), speed / X1.Equiv / 1000, 10, 0, 0);
            gts.mc.GT_CrdStart(0, 1, 0);
        }
        public void AxisArcMove(double startx, double starty, double xCenter, double yCenter, short circleDir, double speed)
        {
            
            // 即将把数据存入坐标系1的FIFO0中，所以要首先清除此缓存区中的数据
            gts.mc.GT_CrdClear(0, 1, 0);

            gts.mc.GT_BufGear(0, 1, 4, 360000, 0);
            // 向缓存区写入第一段插补数据，该段数据是以圆心描述方法描述了一个整圆
            gts.mc.GT_ArcXYC(0,
                1,					// 坐标系是坐标系1
                (int)(startx / X1.Equiv), (int)(starty / Y1.Equiv),			// 该圆弧的终点坐标(0, 0)用户设置的起点坐标和终点坐标重合时，则表示将要进行一个整圆的运动
                xCenter / X1.Equiv, yCenter / Y1.Equiv,			// 圆弧插补的圆心相对于起点位置的偏移量(-100000, 0)
                circleDir,					// 该圆弧是顺时针圆弧
                speed / X1.Equiv / 1000,					// 该插补段的目标速度：100pulse/ms
                10,					// 该插补段的加速度：0.1pulse/ms^2
                0,					// 终点速度为0
                0);                 // 向坐标系1的FIFO0缓存区传递该直线插补数据
                                                                                      // 启动坐标系1的FIFO0的插补运动
            gts.mc.GT_CrdStart(0, 1, 0);
        }
        public bool AxisCheckCrdDone()
        {
            short run;
            int seg;
            gts.mc.GT_CrdStatus(0, 1, out run, out seg, 0);
            return run != 1;
        }
        public bool AxisCheckDone(AxisParm _AxisParam)
        {
            int AxisStatus = 0;
            uint temp_pClock = 0;
            gts.mc.GT_GetSts(_AxisParam.CardNo, _AxisParam.AxisId, out AxisStatus, 1, out temp_pClock);
            if ((AxisStatus & 0x400) != 0x400)
            {
                double pValue1;
                uint pClock;
                gts.mc.GT_GetEncPos(_AxisParam.CardNo, _AxisParam.AxisId, out pValue1, 1, out pClock);
                if (Math.Abs(pValue1 - _AxisParam.Target) < 100)
                {
                    return true;
                }
            }
            return false;
        }
        public void AxisJog(AxisParm _AxisParam, ushort dir, double spd = 0.0)
        {
            double Vel_JSpeed = 0;
            if (dir == 0)//反向
            {
                Vel_JSpeed = spd / _AxisParam.Equiv / 1000 * -1;
            }
            else
            {
                Vel_JSpeed = spd / _AxisParam.Equiv / 1000;
            }
            gts.mc.TJogPrm JogPrm = new gts.mc.TJogPrm();
            gts.mc.GT_PrfJog(_AxisParam.CardNo, _AxisParam.AxisId);
            gts.mc.GT_GetJogPrm(_AxisParam.CardNo, _AxisParam.AxisId, out JogPrm);
            JogPrm.acc = 1;
            JogPrm.dec = 1;
            gts.mc.GT_SetJogPrm(_AxisParam.CardNo, _AxisParam.AxisId, ref JogPrm);
            gts.mc.GT_SetVel(_AxisParam.CardNo, _AxisParam.AxisId, Vel_JSpeed); //设置当前轴的目标速度
            gts.mc.GT_Update(_AxisParam.CardNo, (1 << (_AxisParam.AxisId - 1))); //启动当前轴运动
        }
        public void AxisStop(AxisParm _AxisParam, int type)//type:1:紧急停止0:平滑停止
        {
            gts.mc.GT_Stop(_AxisParam.CardNo, 1 << (_AxisParam.AxisId - 1), type << (_AxisParam.AxisId - 1));
        }
        public double GetAdc(short adc)
        {
            double pValue; uint pClock;
            gts.mc.GT_GetAdc(1, adc, out pValue, 1, out pClock);
            return pValue;
        }
        #endregion
    }
    #region 数据类型
    public struct AxisStatus
    {
        public bool FlagHome;
        public bool FlagAlm;
        public bool FlagMError;
        public bool FlagPosLimit;
        public bool FlagNeglimit;
        public bool FlagPosSoftLim;
        public bool FlagNegSoftLim;
        public bool FlagEmgStop;
        public bool FlagSmoothStop;
        public bool FlagAbruptStop;
        public bool FlagServoOn;
        public bool FlagMoveEnd;
        public double PrfPos;
        public double EncPos;
        public ushort MoveMode;
    }
    public struct AxisParm
    {
        public short CardNo;
        public short AxisId;
        public double Equiv;//换算成mm或°的齿轮比
        public short HomeDir;
        public short HomeMode;
        public double HomeHSpd;
        public double HomeLSpd;
        public double HomeOffset;
        public double Acc;
        public double MaxWorkSpd;
        public double PosLimit;
        public double NegLimit;
        public bool HomeFinish;
        public AxisStatus AxisSts;
        public double SafePos;
        public double Target;
    }
    public struct PTData
    {
        public double pos;
        public int time;
    }
    #endregion
}
