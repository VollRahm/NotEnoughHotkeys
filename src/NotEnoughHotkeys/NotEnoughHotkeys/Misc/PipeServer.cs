using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NotEnoughHotkeys.Misc
{
    //DISCLAIMER: This is my first ever usage of named pipes, please don't blame wrong usage.

    public class PipeServer
    {
        public delegate void MessageRecievedHandler(string Reply);
        public event MessageRecievedHandler OnMessageRecieved;

        public event EventHandler OnPipeClientClosed;
        
        private string PipeName;
        private NamedPipeServerStream pipeStream;
        private CancellationTokenSource cancellationToken;

        private const int BufferSize = 255;

        public PipeServer(string pipeName)
        {
            PipeName = pipeName;
            cancellationToken = new CancellationTokenSource();
            
            var pipeSecurity = new PipeSecurity();
            pipeSecurity.AddAccessRule(new PipeAccessRule(WindowsIdentity.GetCurrent().User, PipeAccessRights.FullControl, AccessControlType.Allow));
            pipeStream = new NamedPipeServerStream(PipeName, PipeDirection.In, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous, BufferSize, BufferSize, pipeSecurity);
        }

        public async Task StartReadingAsync()
        {
            await pipeStream.WaitForConnectionAsync();
            _ = Task.Run(() => Listen());
        }

        private void Listen()
        {
            if (cancellationToken.IsCancellationRequested)
            {
                pipeStream.Close();
                return;
            }

            byte[] pBuffer = new byte[BufferSize];
            pipeStream.ReadAsync(pBuffer, 0, BufferSize, cancellationToken.Token).ContinueWith(t =>
            {
                int ReadLen = t.Result;
                if (ReadLen == 0)
                {
                    OnPipeClientClosed.Invoke(this, null);
                    cancellationToken.Cancel();
                    pipeStream.Close();
                    return;
                }

                Listen();

                OnMessageRecieved.Invoke(Encoding.UTF8.GetString(pBuffer, 0, ReadLen));
            });
        }

        public async Task StopReadingAsync()
        {
            cancellationToken.Cancel();
            await Task.Delay(0);
        }
    }
}
