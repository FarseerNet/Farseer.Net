using System;
using System.Net.Sockets;
using System.Text;

namespace FS.Utils.Component.WinSocket
{
    /// <summary>
    /// </summary>
    public class StateObject
    {
        /// <summary>
        ///     发送成功的回调
        /// </summary>
        private readonly Action<StateObject> _sendCallBack;

        /// <summary>
        ///     接收的最大值
        /// </summary>
        /// <param name="bufferSize"> 接收的最大值 </param>
        /// <param name="socket"> 当前的Socket </param>
        /// <param name="sendCallBack"> 发送成功的回调 </param>
        public StateObject(int bufferSize, Socket socket = null, Action<StateObject> sendCallBack = null)
        {
            BufferSize    = bufferSize;
            Buffer        = new byte[bufferSize];
            WorkSocket    = socket;
            _sendCallBack = sendCallBack;
        }

        /// <summary>
        ///     默认为1024
        /// </summary>
        public StateObject() : this(bufferSize: 1024)
        {
        }

        /// <summary>
        ///     客户端连接的套接字
        /// </summary>
        public Socket WorkSocket { get; set; }

        /// <summary>
        ///     保存接收的数据
        /// </summary>
        public byte[] Buffer { get; set; }

        /// <summary>
        /// </summary>
        public string Msg => Encoding.ASCII.GetString(bytes: Buffer).Replace(oldValue: "\\0", newValue: "").Trim('\0');

        /// <summary>
        ///     接收的最大值
        /// </summary>
        public int BufferSize { get; set; }

        /// <summary>
        ///     字节数
        /// </summary>
        public int ByteCount { get; set; }

        /// <summary>
        ///     发送成功的回调
        /// </summary>
        public void SendCallBack()
        {
            if (_sendCallBack != null) _sendCallBack(obj: this);
        }
    }
}