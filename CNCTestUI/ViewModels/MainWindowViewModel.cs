using CNCTestUI.Models;
using Newtonsoft.Json;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CNCTestUI.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        #region 变量
        AxisParm axisParm = default;
        Param myParam;
        string serialCOM = "COM7";
        #endregion
        #region 属性绑定
        private string title = "CNCTestUI";
        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
        }
        private string version = "1.0";
        public string Version
        {
            get { return version; }
            set { SetProperty(ref version, value); }
        }
        private bool axisCardConnectState;
        public bool AxisCardConnectState
        {
            get { return axisCardConnectState; }
            set { SetProperty(ref axisCardConnectState, value); }
        }
        private double x_Enc;
        public double X_Enc
        {
            get { return x_Enc; }
            set { SetProperty(ref x_Enc, value); }
        }
        private double y_Enc;
        public double Y_Enc
        {
            get { return y_Enc; }
            set { SetProperty(ref y_Enc, value); }
        }
        private double z_Enc;
        public double Z_Enc
        {
            get { return z_Enc; }
            set { SetProperty(ref z_Enc, value); }
        }
        private double r_Enc;
        public double R_Enc
        {
            get { return r_Enc; }
            set { SetProperty(ref r_Enc, value); }
        }
        private string messageStr = "";
        public string MessageStr
        {
            get { return messageStr; }
            set { SetProperty(ref messageStr, value); }
        }
        private string processListViewVisibility = "Visible";
        public string ProcessListViewVisibility
        {
            get { return processListViewVisibility; }
            set { SetProperty(ref processListViewVisibility, value); }
        }
        private string axisViewVisibility = "Collapsed";
        public string AxisViewVisibility
        {
            get { return axisViewVisibility; }
            set { SetProperty(ref axisViewVisibility, value); }
        }
        private ObservableCollection<GCodeItem> gCodeItems = new ObservableCollection<GCodeItem>();
        public ObservableCollection<GCodeItem> GCodeItems
        {
            get { return gCodeItems; }
            set { SetProperty(ref gCodeItems, value); }
        }
        private bool servoState;
        public bool ServoState
        {
            get { return servoState; }
            set { SetProperty(ref servoState, value); }
        }
        private bool zeroState;
        public bool ZeroState
        {
            get { return zeroState; }
            set { SetProperty(ref zeroState, value); }
        }
        private bool alarmState;
        public bool AlarmState
        {
            get { return alarmState; }
            set { SetProperty(ref alarmState, value); }
        }
        private bool runningState;
        public bool RunningState
        {
            get { return runningState; }
            set { SetProperty(ref runningState, value); }
        }
        private bool pLimitState;
        public bool PLimitState
        {
            get { return pLimitState; }
            set { SetProperty(ref pLimitState, value); }
        }
        private bool nLimitState;
        public bool NLimitState
        {
            get { return nLimitState; }
            set { SetProperty(ref nLimitState, value); }
        }
        private int motionJogSelectedIndex;
        public int MotionJogSelectedIndex
        {
            get { return motionJogSelectedIndex; }
            set { SetProperty(ref motionJogSelectedIndex, value); }
        }
        private double x1Abs;
        public double X1Abs
        {
            get { return x1Abs; }
            set { SetProperty(ref x1Abs, value); }
        }
        private double y1Abs;
        public double Y1Abs
        {
            get { return y1Abs; }
            set { SetProperty(ref y1Abs, value); }
        }
        private double z1Abs;
        public double Z1Abs
        {
            get { return z1Abs; }
            set { SetProperty(ref z1Abs, value); }
        }
        private double r1Abs;
        public double R1Abs
        {
            get { return r1Abs; }
            set { SetProperty(ref r1Abs, value); }
        }
        private double x1RunSpeed;
        public double X1RunSpeed
        {
            get { return x1RunSpeed; }
            set { SetProperty(ref x1RunSpeed, value); }
        }
        private double y1RunSpeed;
        public double Y1RunSpeed
        {
            get { return y1RunSpeed; }
            set { SetProperty(ref y1RunSpeed, value); }
        }
        private double z1RunSpeed;
        public double Z1RunSpeed
        {
            get { return z1RunSpeed; }
            set { SetProperty(ref z1RunSpeed, value); }
        }
        private double r1RunSpeed;
        public double R1RunSpeed
        {
            get { return r1RunSpeed; }
            set { SetProperty(ref r1RunSpeed, value); }
        }

        private double x1JogSpeed;
        public double X1JogSpeed
        {
            get { return x1JogSpeed; }
            set { SetProperty(ref x1JogSpeed, value); }
        }
        private double y1JogSpeed;
        public double Y1JogSpeed
        {
            get { return y1JogSpeed; }
            set { SetProperty(ref y1JogSpeed, value); }
        }
        private double z1JogSpeed;
        public double Z1JogSpeed
        {
            get { return z1JogSpeed; }
            set { SetProperty(ref z1JogSpeed, value); }
        }
        private double r1JogSpeed;
        public double R1JogSpeed
        {
            get { return r1JogSpeed; }
            set { SetProperty(ref r1JogSpeed, value); }
        }
        private ViPoint initPos;
        public ViPoint InitPos
        {
            get { return initPos; }
            set { SetProperty(ref initPos, value); }
        }
        private double z1SafePos;
        public double Z1SafePos
        {
            get { return z1SafePos; }
            set { SetProperty(ref z1SafePos, value); }
        }
        private double z1CarvePos;
        public double Z1CarvePos
        {
            get { return z1CarvePos; }
            set { SetProperty(ref z1CarvePos, value); }
        }
        private bool isAxisBusy = false;
        public bool IsAxisBusy
        {
            get { return isAxisBusy; }
            set { SetProperty(ref isAxisBusy, value); }
        }
        #endregion
        #region 方法绑定
        private DelegateCommand<object> menuCommand;
        public DelegateCommand<object> MenuCommand =>
            menuCommand ?? (menuCommand = new DelegateCommand<object>(ExecuteMenuCommand));

        private DelegateCommand<object> motionJogSelectionChangedEventCommand;
        public DelegateCommand<object> MotionJogSelectionChangedEventCommand =>
            motionJogSelectionChangedEventCommand ?? (motionJogSelectionChangedEventCommand = new DelegateCommand<object>(ExecuteMotionJogSelectionChangedEventCommand));
        private DelegateCommand axisServoOnCommand;
        public DelegateCommand AxisServoOnCommand =>
            axisServoOnCommand ?? (axisServoOnCommand = new DelegateCommand(ExecuteAxisServoOnCommand));
        private DelegateCommand axisClearAlarmCommand;
        public DelegateCommand AxisClearAlarmCommand =>
            axisClearAlarmCommand ?? (axisClearAlarmCommand = new DelegateCommand(ExecuteAxisClearAlarmCommand));
        private DelegateCommand axisServoOffCommand;
        public DelegateCommand AxisServoOffCommand =>
            axisServoOffCommand ?? (axisServoOffCommand = new DelegateCommand(ExecuteAxisServoOffCommand));
        private DelegateCommand axisHomeCommand;
        public DelegateCommand AxisHomeCommand =>
            axisHomeCommand ?? (axisHomeCommand = new DelegateCommand(ExecuteAxisHomeCommand));
        private DelegateCommand jog_MouseUpCommand;
        public DelegateCommand Jog_MouseUpCommand =>
            jog_MouseUpCommand ?? (jog_MouseUpCommand = new DelegateCommand(ExecuteJog_MouseUpCommand));
        private DelegateCommand<object> jogP_MouseDownCommand;
        public DelegateCommand<object> JogP_MouseDownCommand =>
            jogP_MouseDownCommand ?? (jogP_MouseDownCommand = new DelegateCommand<object>(ExecuteJogP_MouseDownCommand));
        private DelegateCommand<object> jogN_MouseDownCommand;
        public DelegateCommand<object> JogN_MouseDownCommand =>
            jogN_MouseDownCommand ?? (jogN_MouseDownCommand = new DelegateCommand<object>(ExecuteJogN_MouseDownCommand));
        private DelegateCommand<object> getAbsCommand;
        public DelegateCommand<object> GetAbsCommand =>
            getAbsCommand ?? (getAbsCommand = new DelegateCommand<object>(ExecuteGetAbsCommand));
        private DelegateCommand saveParamCommand;
        public DelegateCommand SaveParamCommand =>
            saveParamCommand ?? (saveParamCommand = new DelegateCommand(ExecuteSaveParamCommand));
        private DelegateCommand<object> getPositionCommand;
        public DelegateCommand<object> GetPositionCommand =>
            getPositionCommand ?? (getPositionCommand = new DelegateCommand<object>(ExecuteGetPositionCommand));
        private DelegateCommand<object> goPositionCommand;
        public DelegateCommand<object> GoPositionCommand =>
            goPositionCommand ?? (goPositionCommand = new DelegateCommand<object>(ExecuteGoPositionCommand));

        async void ExecuteGoPositionCommand(object obj)
        {
            CancellationTokenSource source = new CancellationTokenSource();
            CancellationToken token = source.Token;
            switch (obj.ToString())
            {
                case "0":
                    if (MessageBox.Show($"确认运动到\"初始点\"吗？", "确认", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                    {
                        IsAxisBusy = true;
                        await Task.Run(() => GoActionXYZR(token, InitPos), token).ContinueWith(t => IsAxisBusy = false);
                    }
                    break;
                case "1":
                    if (MessageBox.Show($"确认运动到\"Z轴安全位\"吗？", "确认", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                    {
                        IsAxisBusy = true;
                        await Task.Run(() => GoAction(token, GTSCard.Instance.Z1, Z1SafePos, Z1JogSpeed), token).ContinueWith(t => IsAxisBusy = false);
                    }
                    break;
                case "2":
                    if (MessageBox.Show($"确认运动到\"Z轴切割位\"吗？", "确认", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                    {
                        IsAxisBusy = true;
                        await Task.Run(() => GoAction(token, GTSCard.Instance.Z1, Z1CarvePos, Z1JogSpeed), token).ContinueWith(t => IsAxisBusy = false);
                    }
                    break;
                default:
                    break;
            }
        }
        private void GoAction(CancellationToken token, AxisParm axis, double Pos, double speed)
        {
            int stepnum = 0;
            while (true)
            {
                if (token.IsCancellationRequested)
                {
                    return;
                }
                switch (stepnum)
                {
                    case 0:
                        GTSCard.Instance.AxisPosMove(ref axis, Pos, speed);
                        stepnum = 1;
                        break;
                    case 1:
                        if (GTSCard.Instance.AxisCheckDone(axis))
                        {
                            return;
                        }
                        break;
                    default:
                        break;
                }
                System.Threading.Thread.Sleep(100);
            }
        }
        private void GoActionXYZR(CancellationToken token, ViPoint Pos)
        {
            AxisParm axisX, axisY, axisZ, axisR;
            double zsafe;
            double xspeed, yspeed, zspeed, rspeed;
            axisX = GTSCard.Instance.X1;
            axisY = GTSCard.Instance.Y1;
            axisZ = GTSCard.Instance.Z1;
            axisR = GTSCard.Instance.R1;
            zsafe = Z1SafePos;
            xspeed = X1JogSpeed;
            yspeed = Y1JogSpeed;
            zspeed = Z1JogSpeed;
            rspeed = R1JogSpeed;

            int stepnum = 0;
            while (true)
            {
                if (token.IsCancellationRequested)
                {
                    return;
                }
                switch (stepnum)
                {
                    case 0:
                        GTSCard.Instance.AxisPosMove(ref axisZ, zsafe, zspeed);
                        stepnum = 1;
                        break;
                    case 1:
                        if (GTSCard.Instance.AxisCheckDone(axisZ))
                        {
                            stepnum = 2;
                        }
                        break;
                    case 2:
                        GTSCard.Instance.AxisPosMove(ref axisX, Pos.X, xspeed);
                        GTSCard.Instance.AxisPosMove(ref axisY, Pos.Y, yspeed);
                        GTSCard.Instance.AxisPosMove(ref axisR, Pos.R, rspeed);
                        stepnum = 3;
                        break;
                    case 3:
                        if (GTSCard.Instance.AxisCheckDone(axisX) && GTSCard.Instance.AxisCheckDone(axisY) && GTSCard.Instance.AxisCheckDone(axisR))
                        {
                            stepnum = 4;
                        }
                        break;
                    case 4:
                        GTSCard.Instance.AxisPosMove(ref axisZ, Pos.Z, zspeed);
                        stepnum = 5;
                        break;
                    case 5:
                        if (GTSCard.Instance.AxisCheckDone(axisZ))
                        {
                            return;
                        }
                        break;
                    default:
                        break;
                }
                System.Threading.Thread.Sleep(100);
            }
        }
        void ExecuteGetPositionCommand(object obj)
        {
            switch (obj.ToString())
            {
                case "0":
                    if (MessageBox.Show($"确认设置\"初始点\"吗？", "确认", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                    {
                        InitPos.X = myParam.InitPos.X = GTSCard.Instance.GetEnc(GTSCard.Instance.X1);
                        InitPos.Y = myParam.InitPos.Y = GTSCard.Instance.GetEnc(GTSCard.Instance.Y1);
                        InitPos.Z = myParam.InitPos.Z = GTSCard.Instance.GetEnc(GTSCard.Instance.Z1);
                        InitPos.R = myParam.InitPos.R = GTSCard.Instance.GetEnc(GTSCard.Instance.R1);
                    }
                    break;
                case "1":
                    if (MessageBox.Show($"确认设置\"Z轴安全位\"吗？", "确认", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                    {
                        Z1SafePos = myParam.Z1SafePos = GTSCard.Instance.GetEnc(GTSCard.Instance.Z1);
                    }
                    break;
                case "2":
                    if (MessageBox.Show($"确认设置\"Z轴切割位\"吗？", "确认", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                    {
                        Z1CarvePos = myParam.Z1CarvePos = GTSCard.Instance.GetEnc(GTSCard.Instance.Z1);
                    }
                    break;
                default:
                    break;
            }
            string jsonString = JsonConvert.SerializeObject(myParam, Formatting.Indented);
            File.WriteAllText(System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Param.json"), jsonString);
        }
        void ExecuteSaveParamCommand()
        {
            myParam.X1RunSpeed = X1RunSpeed;
            myParam.Y1RunSpeed = Y1RunSpeed;
            myParam.Z1RunSpeed = Z1RunSpeed;
            myParam.R1RunSpeed = R1RunSpeed;

            myParam.X1JogSpeed = X1JogSpeed;
            myParam.Y1JogSpeed = Y1JogSpeed;
            myParam.Z1JogSpeed = Z1JogSpeed;
            myParam.R1JogSpeed = R1JogSpeed;

            string jsonString = JsonConvert.SerializeObject(myParam, Formatting.Indented);
            File.WriteAllText(System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Param.json"), jsonString);
            MessageBox.Show("参数保存完成", "信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        void ExecuteGetAbsCommand(object obj)
        {
            switch (obj.ToString())
            {
                case "0":
                    X1Abs = myParam.X1Abs = ServoModbus.Instance.ReadInovance(1) * GTSCard.Instance.X1.Equiv;
                    GTSCard.Instance.SigAxisPosSet(GTSCard.Instance.X1, 0);
                    GTSCard.Instance.SigAxisEncSet(GTSCard.Instance.X1, 0);
                    break;
                case "1":
                    Y1Abs = myParam.Y1Abs = ServoModbus.Instance.ReadInovance(2) * GTSCard.Instance.Y1.Equiv;
                    GTSCard.Instance.SigAxisPosSet(GTSCard.Instance.Y1, 0);
                    GTSCard.Instance.SigAxisEncSet(GTSCard.Instance.Y1, 0);
                    break;
                case "2":
                    Z1Abs = myParam.Z1Abs = ServoModbus.Instance.ReadInovance(3) * GTSCard.Instance.Z1.Equiv;
                    GTSCard.Instance.SigAxisPosSet(GTSCard.Instance.Z1, 0);
                    GTSCard.Instance.SigAxisEncSet(GTSCard.Instance.Z1, 0);
                    break;
                case "3":
                    R1Abs = myParam.R1Abs = ServoModbus.Instance.ReadInovance(4) * GTSCard.Instance.R1.Equiv;
                    GTSCard.Instance.SigAxisPosSet(GTSCard.Instance.R1, 0);
                    GTSCard.Instance.SigAxisEncSet(GTSCard.Instance.R1, 0);
                    break;
                default:
                    break;
            }
            string jsonString = JsonConvert.SerializeObject(myParam, Formatting.Indented);
            File.WriteAllText(System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Param.json"), jsonString);
            MessageBox.Show("零点设置完成", "信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        void ExecuteJogN_MouseDownCommand(object obj)
        {
            switch (obj.ToString())
            {
                case "0":
                    GTSCard.Instance.AxisJog(GTSCard.Instance.X1, 0, X1JogSpeed);
                    break;
                case "1":
                    GTSCard.Instance.AxisJog(GTSCard.Instance.Y1, 0, Y1JogSpeed);
                    break;
                case "2":
                    GTSCard.Instance.AxisJog(GTSCard.Instance.Z1, 0, Z1JogSpeed);
                    break;
                case "4":
                    GTSCard.Instance.AxisJog(GTSCard.Instance.R1, 0, R1JogSpeed);
                    break;
                default:
                    break;
            }
        }
        void ExecuteJogP_MouseDownCommand(object obj)
        {
            switch (obj.ToString())
            {
                case "0":
                    GTSCard.Instance.AxisJog(GTSCard.Instance.X1, 1, X1JogSpeed);
                    break;
                case "1":
                    GTSCard.Instance.AxisJog(GTSCard.Instance.Y1, 1, Y1JogSpeed);
                    break;
                case "2":
                    GTSCard.Instance.AxisJog(GTSCard.Instance.Z1, 1, Z1JogSpeed);
                    break;
                case "4":
                    GTSCard.Instance.AxisJog(GTSCard.Instance.R1, 1, R1JogSpeed);
                    break;
                default:
                    break;
            }
        }
        void ExecuteJog_MouseUpCommand()
        {
            GTSCard.Instance.AxisStop(GTSCard.Instance.X1, 1);
            GTSCard.Instance.AxisStop(GTSCard.Instance.Y1, 1);
            GTSCard.Instance.AxisStop(GTSCard.Instance.Z1, 1);
            GTSCard.Instance.AxisStop(GTSCard.Instance.R1, 1);
        }
        void ExecuteAxisHomeCommand()
        {

        }
        void ExecuteAxisServoOffCommand()
        {
            GTSCard.Instance.ServoOff(axisParm);
        }
        void ExecuteAxisClearAlarmCommand()
        {
            GTSCard.Instance.ClearAlm(axisParm);
        }
        void ExecuteAxisServoOnCommand()
        {
            GTSCard.Instance.ServoOn(axisParm);
            GTSCard.Instance.SigAxisPosSet(axisParm, GTSCard.Instance.GetEnc(axisParm));
        }
        void ExecuteMotionJogSelectionChangedEventCommand(object obj)
        {
            switch ((int)obj)
            {
                case 0:
                    axisParm = GTSCard.Instance.X1;
                    break;
                case 1:
                    axisParm = GTSCard.Instance.Y1;
                    break;
                case 2:
                    axisParm = GTSCard.Instance.Z1;
                    break;
                case 3:
                default:
                    axisParm = GTSCard.Instance.R1;
                    break;
            }
        }

        void ExecuteMenuCommand(object obj)
        {
            switch (obj.ToString())
            {
                case "0":
                    ProcessListViewVisibility = "Visible";
                    AxisViewVisibility = "Collapsed";
                    OpenFileDialog ofd = new OpenFileDialog();
                    ofd.Filter = "Txt文件(*.txt)|*.txt|所有文件|*.*";
                    if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        GCodeItems.Clear();
                        string[] lines = System.IO.File.ReadAllLines(ofd.FileName);
                        int id = 0;
                        int state = -1;
                        foreach (string line in lines)
                        {
                            if (line.Length > 2)
                            {
                                if (line.Length > 8 && line.Substring(0, 8) == "(* SHAPE")
                                {
                                    state = 0;
                                    continue;
                                }
                                else if (line.Contains("G1") && state == 0)
                                {
                                    state = 1;
                                }
                                else if (line.Contains("F150") && state == 1)
                                {
                                    state = -1;
                                    continue;
                                }
                                if (state >= 0)
                                {
                                    if (!line.Contains("Z") && line.Substring(0,1) == "G")
                                    {
                                        GCodeItems.Add(new GCodeItem()
                                        {
                                            Id = id++,
                                            GCode = line,
                                            Process = false
                                        });
                                    }
                                }
                            }
                        }
                    }
                    break;
                case "1":
                    ProcessListViewVisibility = "Collapsed";
                    AxisViewVisibility = "Visible";
                    break;
                default:
                    break;
            }
        }
        #endregion
        #region 构造函数
        public MainWindowViewModel()
        {
            GTSCard.Instance.StateUpdateEvent += Instance_StateUpdateEvent;
            GTSCard.Instance.MessagePrintEvent += Instance_MessagePrintEvent;
            MotionJogSelectedIndex = 0;
            axisParm = GTSCard.Instance.X1;
            GTSCard.Instance.Init();
            LoadParm();
            if (myParam.InitPos == null)
            {
                myParam.InitPos = new MPoint();
            }
            InitPos = new ViPoint() { 
                X = myParam.InitPos.X,
                Y = myParam.InitPos.Y,
                Z = myParam.InitPos.Z,
                R = myParam.InitPos.R
            };
            Z1SafePos = myParam.Z1SafePos;
            Z1CarvePos = myParam.Z1CarvePos;
            if (ServoModbus.Instance.Connect(serialCOM))
            {
                var a = ServoModbus.Instance.ReadInovance(1) * GTSCard.Instance.X1.Equiv;
                GTSCard.Instance.SigAxisPosSet(GTSCard.Instance.X1, a - myParam.X1Abs);
                GTSCard.Instance.SigAxisEncSet(GTSCard.Instance.X1, a - myParam.X1Abs);

                a = ServoModbus.Instance.ReadInovance(2) * GTSCard.Instance.Y1.Equiv;
                GTSCard.Instance.SigAxisPosSet(GTSCard.Instance.Y1, a - myParam.Y1Abs);
                GTSCard.Instance.SigAxisEncSet(GTSCard.Instance.Y1, a - myParam.Y1Abs);

                a = ServoModbus.Instance.ReadInovance(3) * GTSCard.Instance.Z1.Equiv;
                GTSCard.Instance.SigAxisPosSet(GTSCard.Instance.Z1, a - myParam.Z1Abs);
                GTSCard.Instance.SigAxisEncSet(GTSCard.Instance.Z1, a - myParam.Z1Abs);

                a = ServoModbus.Instance.ReadInovance(4) * GTSCard.Instance.R1.Equiv;
                GTSCard.Instance.SigAxisPosSet(GTSCard.Instance.R1, a - myParam.R1Abs);
                GTSCard.Instance.SigAxisEncSet(GTSCard.Instance.R1, a - myParam.R1Abs);
            }
            else
            {
                addMessage("串口打开失败");
            }
            

            UIRun();
        }
        #endregion
        #region 事件函数
        private void Instance_MessagePrintEvent(object sender, string message)
        {
            addMessage($"{sender}:{message}");
        }
        private void Instance_StateUpdateEvent(object sender, string status)
        {
            AxisCardConnectState = status == "连接";
        }
        #endregion
        #region 功能函数
        private async void UIRun()
        {
            while (true)
            {
                try
                {
                    if (GTSCard.Instance.OpenCardOk)
                    {
                        X_Enc = GTSCard.Instance.GetEnc(GTSCard.Instance.X1);
                        Y_Enc = GTSCard.Instance.GetEnc(GTSCard.Instance.Y1);
                        Z_Enc = GTSCard.Instance.GetEnc(GTSCard.Instance.Z1);
                        R_Enc = GTSCard.Instance.GetEnc(GTSCard.Instance.R1);
                        if (AxisViewVisibility == "Visible")
                        {
                            AxisStatus axisStatus = GTSCard.Instance.GetAxisStatus(axisParm);
                            ZeroState = axisStatus.FlagHome;
                            RunningState = !axisStatus.FlagMoveEnd;
                            ServoState = axisStatus.FlagServoOn;
                            PLimitState = axisStatus.FlagPosLimit;
                            NLimitState = axisStatus.FlagNeglimit;
                            AlarmState = axisStatus.FlagAlm;
                        }
                    }
                }
                catch { }
                await Task.Delay(200);
            }
        }
        private void addMessage(string str)
        {
            Console.WriteLine(str);
            string[] s = MessageStr.Split('\n');
            if (s.Length > 1000)
            {
                MessageStr = "";
            }
            if (MessageStr != "")
            {
                MessageStr += "\n";
            }
            MessageStr += str;
        }
        private void LoadParm()
        {
            try
            {
                //Json序列化，从文件读取
                string jsonString = File.ReadAllText(System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Param.json"));
                myParam = JsonConvert.DeserializeObject<Param>(jsonString);

                serialCOM = myParam.SerialCOM;

                X1Abs = myParam.X1Abs;
                Y1Abs = myParam.Y1Abs;
                Z1Abs = myParam.Z1Abs;
                R1Abs = myParam.R1Abs;

                X1RunSpeed = myParam.X1RunSpeed;
                Y1RunSpeed = myParam.Y1RunSpeed;
                Z1RunSpeed = myParam.Z1RunSpeed;
                R1RunSpeed = myParam.R1RunSpeed;

                X1JogSpeed = myParam.X1JogSpeed;
                Y1JogSpeed = myParam.Y1JogSpeed;
                Z1JogSpeed = myParam.Z1JogSpeed;
                R1JogSpeed = myParam.R1JogSpeed;


            }
            catch (Exception ex)
            {
                addMessage(ex.Message);
            }
        }
        #endregion
    }
    public class GCodeItem : BindableBase
    {
        public int Id { get; set; }
        public string GCode { get; set; }
        private bool process;
        public bool Process
        {
            get { return process; }
            set { SetProperty(ref process, value); }
        }
    }
    public class ViPoint : BindableBase
    {
        private double x;
        public double X
        {
            get { return x; }
            set { SetProperty(ref x, value); }
        }
        private double y;
        public double Y
        {
            get { return y; }
            set { SetProperty(ref y, value); }
        }
        private double z;
        public double Z
        {
            get { return z; }
            set { SetProperty(ref z, value); }
        }
        private double r;
        public double R
        {
            get { return r; }
            set { SetProperty(ref r, value); }
        }
    }
}
