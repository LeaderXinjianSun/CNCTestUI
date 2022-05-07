using NModbus;
using NModbus.Serial;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CNCTestUI.Models
{	
	public class ServoModbus
    {
		#region 变量
		SerialPort serialPort = new SerialPort();
		IModbusMaster master;
		private static object _lock1 = new object();
		public event MessagePrintHandler MessagePrintEvent;
		#endregion
		#region 实例化
		private static ServoModbus _instance;
		private static object _lock = new object();
		public static ServoModbus Instance
		{
			get
			{
				if (ServoModbus._instance == null)
				{
					object @lock = ServoModbus._lock;
					lock (@lock)
					{
						if (ServoModbus._instance == null)
						{
							ServoModbus._instance = new ServoModbus();
						}
					}
				}
				return ServoModbus._instance;
			}
		}
		#endregion
		#region 功能函数
		public bool Connect(string _portName)
		{
			try
			{
				if (this.serialPort.IsOpen)
				{
                    if (MessagePrintEvent != null)
                    {
						MessagePrintEvent(this, $"伺服串口{_portName}已打开");
					}
					return false;
				}
				else
				{
					this.serialPort.PortName = _portName;
					serialPort.BaudRate = 19200;
					serialPort.DataBits = 8;
					serialPort.Parity = Parity.Even;
					serialPort.StopBits = StopBits.One;
					this.serialPort.Open();
					this.serialPort.DiscardInBuffer();
					this.serialPort.DiscardOutBuffer();
					this.serialPort.WriteTimeout = 500;
					this.serialPort.ReadTimeout = 500;
					var factory = new ModbusFactory();
					master = factory.CreateRtuMaster(serialPort);
					return true;
				}
			}
			catch (Exception ex)
			{
				if (MessagePrintEvent != null)
				{
					MessagePrintEvent(this, ex.Message);
				}
			}
			return false;
		}
		public int ReadInovance(byte station)
		{
			object @lock = ServoModbus._lock1;
			lock (@lock)
			{
				ushort[] r = master.ReadHoldingRegisters(station, 0x0B07, 2);//H0B-07 绝对位置计数器（32位）H05-30参数写6，清零
				return Convert.ToInt32( r[1].ToString("X4") + r[0].ToString("X4"),16);
			}
		}
		#endregion
	}
}
