using CNCTestUI.Models;
using CsvHelper;
using Newtonsoft.Json;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
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
        CancellationTokenSource source;
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
        private string processListViewVisibility = "Collapsed";
        public string ProcessListViewVisibility
        {
            get { return processListViewVisibility; }
            set { SetProperty(ref processListViewVisibility, value); }
        }
        private string axisViewVisibility = "Visible";
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
        private ViPoint toolPoint;
        public ViPoint ToolPoint
        {
            get { return toolPoint; }
            set { SetProperty(ref toolPoint, value); }
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
        private double x_Tool;
        public double X_Tool
        {
            get { return x_Tool; }
            set { SetProperty(ref x_Tool, value); }
        }
        private double y_Tool;
        public double Y_Tool
        {
            get { return y_Tool; }
            set { SetProperty(ref y_Tool, value); }
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
        private DelegateCommand appLoadedEventCommand;
        public DelegateCommand AppLoadedEventCommand =>
            appLoadedEventCommand ?? (appLoadedEventCommand = new DelegateCommand(ExecuteAppLoadedEventCommand));
        private DelegateCommand appClosedEventCommand;
        public DelegateCommand AppClosedEventCommand =>
            appClosedEventCommand ?? (appClosedEventCommand = new DelegateCommand(ExecuteAppClosedEventCommand));
        private DelegateCommand<object> operateButtonCommand;
        public DelegateCommand<object> OperateButtonCommand =>
            operateButtonCommand ?? (operateButtonCommand = new DelegateCommand<object>(ExecuteOperateButtonCommand));
        private DelegateCommand grabTriggerCommand;
        public DelegateCommand GrabTriggerCommand =>
            grabTriggerCommand ?? (grabTriggerCommand = new DelegateCommand(ExecuteGrabTriggerCommand));

        void ExecuteGrabTriggerCommand()
        {
            GTSCard.Instance.ComparePulseTrigger();
        }
        async void ExecuteOperateButtonCommand(object obj)
        {
            switch (obj.ToString())
            {
                case "0":
                    {
                        source = new CancellationTokenSource();
                        CancellationToken token = source.Token;
                        GTSCard.Instance.ServoOn(GTSCard.Instance.X1);
                        GTSCard.Instance.ServoOn(GTSCard.Instance.Y1);
                        GTSCard.Instance.ServoOn(GTSCard.Instance.Z1);
                        GTSCard.Instance.ServoOn(GTSCard.Instance.R1);
                        IsAxisBusy = true;
                        await Task.Delay(200);
                        List<M1Point> pickPoints,pastePoints;
                        using (var reader = new StreamReader(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "PickPoints.csv")))
                        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                        {
                            pickPoints = csv.GetRecords<M1Point>().ToList();
                        }
                        using (var reader = new StreamReader(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "PastePoints.csv")))
                        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                        {
                            pastePoints = csv.GetRecords<M1Point>().ToList();
                        }
                        if (pickPoints.Count > 0 && pastePoints.Count > 0)
                        {
                            await Task.Run(() => ALineMotion(token, pickPoints, pastePoints), token).ContinueWith(t => IsAxisBusy = false);
                        }
                        else
                        {
                            MessageBox.Show("未加载到点", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            IsAxisBusy = false;
                        }
                    }
                    break;
                case "1":
                    if (source != null)
                    {
                        source.Cancel();
                    }
                    GTSCard.Instance.AxisStop(0, 1);
                    IsAxisBusy = false;
                    break;
                default:
                    break;
            }
        }
        void ExecuteAppClosedEventCommand()
        {
            try
            {
                if (source != null)
                {
                    source.Cancel();
                }
                ResetAction();
            }
            catch { }
        }
        void ExecuteAppLoadedEventCommand()
        {

        }
        async void ExecuteGoPositionCommand(object obj)
        {
            source = new CancellationTokenSource();
            CancellationToken token = source.Token;
            GTSCard.Instance.ServoOn(GTSCard.Instance.X1);
            GTSCard.Instance.ServoOn(GTSCard.Instance.Y1);
            GTSCard.Instance.ServoOn(GTSCard.Instance.Z1);
            GTSCard.Instance.ServoOn(GTSCard.Instance.R1);
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
        private void ALineMotion(CancellationToken token, List<M1Point> pickPoints, List<M1Point> pastePoints)
        {
            int stepnum = 0;
            Stopwatch sw = new Stopwatch();
            while (true)
            {
                if (token.IsCancellationRequested)
                {
                    return;
                }
                switch (stepnum)
                {
                    //case 0:
                    //    GTSCard.Instance.AxisPosMove(ref GTSCard.Instance.Z1, myParam.Z1SafePos, myParam.Z1RunSpeed);
                    //    stepnum = 1;
                    //    break;
                    //case 1:
                    //    if (GTSCard.Instance.AxisCheckDone(GTSCard.Instance.Z1))
                    //    {
                    //        stepnum = 2;
                    //    }
                    //    break;
                    //case 0:
                    //    GTSCard.Instance.AxisPosMove(ref GTSCard.Instance.X1, myParam.ToolPoint.X, myParam.X1RunSpeed);
                    //    GTSCard.Instance.AxisPosMove(ref GTSCard.Instance.Y1, myParam.ToolPoint.Y, myParam.X1RunSpeed);
                    //    GTSCard.Instance.AxisPosMove(ref GTSCard.Instance.Z1, myParam.ToolPoint.Z, myParam.Z1RunSpeed);
                    //    stepnum = 3;
                    //    break;
                    case 0:
                        {
                            var r = GTSCard.Instance.SetCrd(myParam.ToolPoint.X, myParam.ToolPoint.Y, myParam.ToolPoint.Z);
                            if (!r)
                            {
                                addMessage("坐标系初始化:失败");
                            }
                            else
                            {
                                addMessage("初始化坐标系");
                            }
                            stepnum = 4;
                        }
                        break;
                    case 4:
                        GTSCard.Instance.AxisLnXYZMove(pickPoints);
                        stepnum = 5;
                        break;
                    case 5:
                        if (GTSCard.Instance.AxisCheckCrdDone())
                        {
                            //addMessage(DateTime.Now.ToString("HH:mm:ss:fff"));
                            stepnum = 6;
                        }
                        break;
                    case 6:
                        GTSCard.Instance.AxisLnXYZMove(pastePoints);
                        stepnum = 7;
                        break;
                    case 7:
                        if (GTSCard.Instance.AxisCheckCrdDone())
                        {
                            addMessage(DateTime.Now.ToString("HH:mm:ss:fff"));
                            stepnum = 4;
                        }
                        break;
                    default:
                        break;
                }
                System.Threading.Thread.Sleep(10);
            }
        }
        private void ARCMotion(CancellationToken token, Queue<GCodeItem1> gCodeItem1s)
        {
            int stepnum = 0;
            Stopwatch sw = new Stopwatch();
            GCodeItem1 gCodeItem1 = new GCodeItem1();
            double targetX = 0, targetY = 0, targetI = 0, targetJ = 0;
            double origX = 0, origY = 0;
            double targetA = 0; double finalA = 0;
            double offsetX = 0, offsetY = 0;
            while (true)
            {
                if (token.IsCancellationRequested)
                {
                    return;
                }
                switch (stepnum)
                {
                    case 0:
                        GTSCard.Instance.AxisPosMove(ref GTSCard.Instance.Z1, myParam.Z1SafePos, myParam.Z1RunSpeed);
                        stepnum = 1;
                        break;
                    case 1:
                        if (GTSCard.Instance.AxisCheckDone(GTSCard.Instance.Z1))
                        {
                            stepnum = 2;
                        }
                        break;
                    case 2:
                        GTSCard.Instance.AxisPosMove(ref GTSCard.Instance.X1, myParam.ToolPoint.X, myParam.X1RunSpeed);
                        GTSCard.Instance.AxisPosMove(ref GTSCard.Instance.Y1, myParam.ToolPoint.Y, myParam.X1RunSpeed);
                        stepnum = 3;
                        break;
                    case 3:
                        if (GTSCard.Instance.AxisCheckDone(GTSCard.Instance.X1) && GTSCard.Instance.AxisCheckDone(GTSCard.Instance.Y1))
                        {
                            var r = GTSCard.Instance.SetCrd(myParam.ToolPoint.X, myParam.ToolPoint.Y);
                            if (!r)
                            {
                                addMessage("坐标系初始化:失败");
                            }
                            else
                            {
                                addMessage("初始化坐标系");
                            }
                            stepnum = 4;
                        }
                        break;
                    case 4:
                        GTSCard.Instance.AxisPosMove(ref GTSCard.Instance.R1, 0, myParam.R1RunSpeed);
                        stepnum = 5;
                        break;
                    case 5:
                        if (GTSCard.Instance.AxisCheckDone(GTSCard.Instance.R1))
                        {
                            stepnum = 6;
                        }
                        break;



                    case 6:
                        if (gCodeItem1s.Count == 0)
                        {
                            stepnum = 500;
                            //处理完了
                        }
                        else
                        {
                            gCodeItem1 = gCodeItem1s.Dequeue();
                            //显示当前执行行
                            System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
                            {
                                var curr = GCodeItems.FirstOrDefault(t => t.Process == true);
                                if (curr != null)
                                {
                                    curr.Process = false;
                                }
                                var next = GCodeItems.FirstOrDefault(t => t.Id == gCodeItem1.Id);
                                if (next != null)
                                {
                                    next.Process = true;
                                }
                            }));
                            stepnum = 7;
                        }
                        break;
                    case 7:
                        switch (gCodeItem1.GCode.Substring(0, 2))
                        {
                            case "G0":
                                stepnum = 100;
                                break;
                            case "G1":
                                stepnum = 200;
                                break;
                            case "G2":
                                stepnum = 300;
                                break;
                            case "G3":
                                stepnum = 400;
                                break;
                            default://其他指令？
                                stepnum = 6;
                                break;
                        }
                        break;
                    case 100:
                        GTSCard.Instance.AxisPosMove(ref GTSCard.Instance.Z1, myParam.Z1SafePos, myParam.Z1RunSpeed);
                        stepnum = 101;
                        break;
                    case 101:
                        if (GTSCard.Instance.AxisCheckDone(GTSCard.Instance.Z1))
                        {
                            stepnum = 102;
                        }
                        break;
                    case 102:
                        {
                            //分离出目标位置的X和Y坐标
                            string gcode = gCodeItem1.GCode.Replace(" ", "");
                            int xstart = gcode.IndexOf('X');
                            int ystart = gcode.IndexOf('Y');
                            targetX = double.Parse(gcode.Substring(xstart + 1, ystart - xstart - 1));
                            targetY = double.Parse(gcode.Substring(ystart + 1));
                            GTSCard.Instance.AxisLnXYMove(targetX, targetY, myParam.X1RunSpeed);
                            stepnum = 103;
                        }
                        break;
                    case 103:
                        if (GTSCard.Instance.AxisCheckCrdDone())
                        {
                            stepnum = 6;
                        }
                        break;
                    case 200:
                        GTSCard.Instance.AxisPosMove(ref GTSCard.Instance.Z1, myParam.Z1CarvePos + 3, myParam.Z1RunSpeed);
                        stepnum = 201;
                        break;
                    case 201:
                        if (GTSCard.Instance.AxisCheckDone(GTSCard.Instance.Z1))
                        {
                            stepnum = 202;
                        }
                        break;
                    case 202:
                        {
                            //分离出目标位置的X和Y坐标
                            string gcode = gCodeItem1.GCode.Replace(" ", "");
                            int xstart = gcode.IndexOf('X');
                            int ystart = gcode.IndexOf('Y');
                            targetX = double.Parse(gcode.Substring(xstart + 1, ystart - xstart - 1));
                            targetY = double.Parse(gcode.Substring(ystart + 1));
                            origX = GTSCard.Instance.GetEnc(GTSCard.Instance.X1) - myParam.ToolPoint.X;
                            origY = GTSCard.Instance.GetEnc(GTSCard.Instance.Y1) - myParam.ToolPoint.Y;
                            targetA = getAngleBetweenPoints(origX, origY, targetX, targetY) - 90;
                            GTSCard.Instance.AxisPosMove(ref GTSCard.Instance.R1, targetA, myParam.R1RunSpeed);
                            stepnum = 203;
                        }
                        break;
                    case 203:
                        if (GTSCard.Instance.AxisCheckDone(GTSCard.Instance.R1))
                        {
                            stepnum = 204;
                        }
                        break;
                    case 204:
                        GTSCard.Instance.AxisPosMove(ref GTSCard.Instance.Z1, myParam.Z1CarvePos, myParam.Z1RunSpeed);
                        stepnum = 205;
                        break;
                    case 205:
                        if (GTSCard.Instance.AxisCheckDone(GTSCard.Instance.Z1))
                        {
                            GTSCard.Instance.AxisLnXYMove(targetX, targetY, myParam.X1RunSpeed);
                            stepnum = 206;
                        }
                        break;
                    case 206:
                        if (GTSCard.Instance.AxisCheckCrdDone())
                        {
                            stepnum = 6;
                        }
                        break;

                    case 300:
                        GTSCard.Instance.AxisPosMove(ref GTSCard.Instance.Z1, myParam.Z1CarvePos + 3, myParam.Z1RunSpeed);
                        stepnum = 301;
                        break;
                    case 301:
                        if (GTSCard.Instance.AxisCheckDone(GTSCard.Instance.Z1))
                        {
                            stepnum = 302;
                        }
                        break;
                    case 302:
                        {
                            //分离出目标位置的X和Y坐标
                            string gcode = gCodeItem1.GCode.Replace(" ", "");
                            int xstart = gcode.IndexOf('X');
                            int ystart = gcode.IndexOf('Y');
                            int istart = gcode.IndexOf('I');
                            int jstart = gcode.IndexOf('J');
                            targetX = double.Parse(gcode.Substring(xstart + 1, ystart - xstart - 1));
                            targetY = double.Parse(gcode.Substring(ystart + 1, istart - ystart - 1));
                            origX = GTSCard.Instance.GetEnc(GTSCard.Instance.X1) - myParam.ToolPoint.X;
                            origY = GTSCard.Instance.GetEnc(GTSCard.Instance.Y1) - myParam.ToolPoint.Y;
                            targetI = double.Parse(gcode.Substring(istart + 1, jstart - istart - 1));
                            targetJ = double.Parse(gcode.Substring(jstart + 1));
                            double cirCenterX = origX + targetI;
                            double cirCenterY = origY + targetJ;

                            targetA = getAngleBetweenPoints(cirCenterX, cirCenterY, origX, origY) - 180;
                            finalA = getAngleBetweenPoints(cirCenterX, cirCenterY, targetX, targetY) - 180;
                            GTSCard.Instance.AxisPosMove(ref GTSCard.Instance.R1, targetA, myParam.R1RunSpeed);
                            stepnum = 303;
                        }
                        break;

                    case 303:
                        if (GTSCard.Instance.AxisCheckDone(GTSCard.Instance.R1))
                        {
                            stepnum = 304;
                        }
                        break;
                    case 304:
                        GTSCard.Instance.AxisPosMove(ref GTSCard.Instance.Z1, myParam.Z1CarvePos, myParam.Z1RunSpeed);
                        stepnum = 305;
                        break;
                    case 305:
                        if (GTSCard.Instance.AxisCheckDone(GTSCard.Instance.Z1))
                        {
                            double speed = 0;
                            double radius = Math.Sqrt(Math.Pow(targetI, 2) + Math.Pow(targetJ, 2));
                            if (radius > 100)
                            {
                                speed = myParam.X1RunSpeed;
                            }
                            else
                            {
                                speed = myParam.X1RunSpeed / 100 * radius;
                            }
                            GTSCard.Instance.AxisArcMove(targetX, targetY, targetI, targetJ, finalA - targetA, 0, speed);
                            stepnum = 306;
                        }
                        break;
                    case 306:
                        if (GTSCard.Instance.AxisCheckCrdDone())
                        {
                            stepnum = 6;
                        }
                        break;

                    case 400:
                        GTSCard.Instance.AxisPosMove(ref GTSCard.Instance.Z1, myParam.Z1CarvePos + 3, myParam.Z1RunSpeed);
                        stepnum = 401;
                        break;
                    case 401:
                        if (GTSCard.Instance.AxisCheckDone(GTSCard.Instance.Z1))
                        {
                            stepnum = 402;
                        }
                        break;
                    case 402:
                        {
                            //分离出目标位置的X和Y坐标
                            string gcode = gCodeItem1.GCode.Replace(" ", "");
                            int xstart = gcode.IndexOf('X');
                            int ystart = gcode.IndexOf('Y');
                            int istart = gcode.IndexOf('I');
                            int jstart = gcode.IndexOf('J');
                            targetX = double.Parse(gcode.Substring(xstart + 1, ystart - xstart - 1));
                            targetY = double.Parse(gcode.Substring(ystart + 1, istart - ystart - 1));
                            origX = GTSCard.Instance.GetEnc(GTSCard.Instance.X1) - myParam.ToolPoint.X;
                            origY = GTSCard.Instance.GetEnc(GTSCard.Instance.Y1) - myParam.ToolPoint.Y;
                            targetI = double.Parse(gcode.Substring(istart + 1, jstart - istart - 1));
                            targetJ = double.Parse(gcode.Substring(jstart + 1));
                            double cirCenterX = origX + targetI;
                            double cirCenterY = origY + targetJ;

                            targetA = getAngleBetweenPoints(cirCenterX, cirCenterY, origX, origY);
                            finalA = getAngleBetweenPoints(cirCenterX, cirCenterY, targetX, targetY);
                            GTSCard.Instance.AxisPosMove(ref GTSCard.Instance.R1, targetA, myParam.R1RunSpeed);
                            stepnum = 403;
                        }
                        break;

                    case 403:
                        if (GTSCard.Instance.AxisCheckDone(GTSCard.Instance.R1))
                        {
                            stepnum = 404;
                        }
                        break;
                    case 404:
                        GTSCard.Instance.AxisPosMove(ref GTSCard.Instance.Z1, myParam.Z1CarvePos, myParam.Z1RunSpeed);
                        stepnum = 405;
                        break;
                    case 405:
                        if (GTSCard.Instance.AxisCheckDone(GTSCard.Instance.Z1))
                        {
                            double speed = 0;
                            double radius = Math.Sqrt(Math.Pow(targetI, 2) + Math.Pow(targetJ, 2));
                            if (radius > 100)
                            {
                                speed = myParam.X1RunSpeed;
                            }
                            else
                            {
                                speed = myParam.X1RunSpeed / 100 * radius;
                            }
                            GTSCard.Instance.AxisArcMove(targetX, targetY, targetI, targetJ, finalA - targetA, 1, speed);
                            stepnum = 406;
                        }
                        break;
                    case 406:
                        if (GTSCard.Instance.AxisCheckCrdDone())
                        {
                            stepnum = 6;
                        }
                        break;


                    case 500:
                        GTSCard.Instance.AxisPosMove(ref GTSCard.Instance.Z1, myParam.Z1SafePos, myParam.Z1RunSpeed);
                        stepnum = 501;
                        break;
                    case 501:
                        if (GTSCard.Instance.AxisCheckDone(GTSCard.Instance.Z1))
                        {
                            stepnum = 502;
                        }
                        break;
                    case 502:
                        {
                            GTSCard.Instance.AxisLnXYMove(0, 0, myParam.X1RunSpeed);
                            stepnum = 503;
                        }
                        break;
                    case 503:
                        if (GTSCard.Instance.AxisCheckCrdDone())
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
                case "3":
                    if (MessageBox.Show($"确认设置\"对刀\"吗？", "确认", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                    {
                        ToolPoint.X = myParam.ToolPoint.X = GTSCard.Instance.GetEnc(GTSCard.Instance.X1);
                        ToolPoint.Y = myParam.ToolPoint.Y = GTSCard.Instance.GetEnc(GTSCard.Instance.Y1);
                        ToolPoint.Z = myParam.ToolPoint.Z = GTSCard.Instance.GetEnc(GTSCard.Instance.Z1);
                        ToolPoint.R = myParam.ToolPoint.R = GTSCard.Instance.GetEnc(GTSCard.Instance.R1);

                        var r = GTSCard.Instance.SetCrd(myParam.ToolPoint.X, myParam.ToolPoint.Y);
                        if (!r)
                        {
                            MessageBox.Show("对刀失败", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
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

            myParam.InitPos.X = InitPos.X;
            myParam.InitPos.Y = InitPos.Y;
            myParam.InitPos.Z = InitPos.Z;
            myParam.InitPos.R = InitPos.R;

            myParam.ToolPoint.X = ToolPoint.X;
            myParam.ToolPoint.Y = ToolPoint.Y;
            myParam.ToolPoint.Z = ToolPoint.Z;
            myParam.ToolPoint.R = ToolPoint.R;

            myParam.Z1SafePos = Z1SafePos;
            myParam.Z1CarvePos = Z1CarvePos;

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
            GTSCard.Instance.Init();
            LoadParm();
            axisParm = GTSCard.Instance.X1;
            GTSCard.Instance.SetDo(0, 0);
            GTSCard.Instance.SetDo(1, 0);
            GTSCard.Instance.ServoOn(GTSCard.Instance.Z1);
            System.Threading.Thread.Sleep(1000);
            if (myParam.InitPos == null)
            {
                myParam.InitPos = new MPoint();
            }
            InitPos = new ViPoint()
            {
                X = myParam.InitPos.X,
                Y = myParam.InitPos.Y,
                Z = myParam.InitPos.Z,
                R = myParam.InitPos.R
            };
            if (myParam.ToolPoint == null)
            {
                myParam.ToolPoint = new MPoint();
            }
            ToolPoint = new ViPoint()
            {
                X = myParam.ToolPoint.X,
                Y = myParam.ToolPoint.Y,
                Z = myParam.ToolPoint.Z,
                R = myParam.ToolPoint.R
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
            bool _xAlarm = false, _yAlarm = false, _zAlarm = false;
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

                        X_Tool = X_Enc - myParam.ToolPoint.X;
                        Y_Tool = Y_Enc - myParam.ToolPoint.Y;
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
                        var xAlarm = GTSCard.Instance.GetAxisAlarm(GTSCard.Instance.X1);
                        var yAlarm = GTSCard.Instance.GetAxisAlarm(GTSCard.Instance.Y1);
                        var zAlarm = GTSCard.Instance.GetAxisAlarm(GTSCard.Instance.Z1);
                        if (_xAlarm != xAlarm || _yAlarm != yAlarm || _zAlarm != zAlarm)
                        {
                            if (xAlarm || yAlarm || zAlarm)
                            {
                                if (source != null)
                                {
                                    source.Cancel();
                                }
                                GTSCard.Instance.AxisStop(GTSCard.Instance.X1, 1);
                                GTSCard.Instance.AxisStop(GTSCard.Instance.Y1, 1);
                                GTSCard.Instance.AxisStop(GTSCard.Instance.Z1, 1);
                                GTSCard.Instance.AxisStop(GTSCard.Instance.R1, 1);
                                IsAxisBusy = false;
                            }
                            if (xAlarm)
                            {
                                addMessage("X轴报警");
                            }
                            if (yAlarm)
                            {
                                addMessage("Y轴报警");
                            }
                            if (zAlarm)
                            {
                                addMessage("Z轴报警");
                            }
                            _xAlarm = xAlarm;
                            _yAlarm = yAlarm;
                            _zAlarm = zAlarm;
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
        private void ResetAction()
        {
            GTSCard.Instance.AxisStop(GTSCard.Instance.X1, 1);
            GTSCard.Instance.AxisStop(GTSCard.Instance.Y1, 1);
            GTSCard.Instance.AxisStop(GTSCard.Instance.Z1, 1);
            GTSCard.Instance.AxisStop(GTSCard.Instance.R1, 1);
            GTSCard.Instance.ServoOff(GTSCard.Instance.X1);
            GTSCard.Instance.ServoOff(GTSCard.Instance.Y1);
            GTSCard.Instance.ServoOff(GTSCard.Instance.Z1);
            GTSCard.Instance.ServoOff(GTSCard.Instance.R1);
        }
        private double getAngleBetweenPoints(double x_orig, double y_orig, double x_landmark, double y_landmark)
        {
            double deltaY = y_landmark - y_orig;
            double deltaX = x_landmark - x_orig;
            return angle_trunc(Math.Atan2(deltaY, deltaX)) / Math.PI * 180;
        }
        private double angle_trunc(double a)
        {
            while (a < 0)
            {
                a += Math.PI * 2;
            }
            return a;
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
    public class GCodeItem1
    {
        public int Id { get; set; }
        public string GCode { get; set; }
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
