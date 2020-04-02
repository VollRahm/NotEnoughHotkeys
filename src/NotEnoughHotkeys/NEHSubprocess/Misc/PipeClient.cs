using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEHSubprocess.Misc
{
    //DISCLAIMER: This is my first ever usage of named pipes, please don't blame wrong usage.

    public class PipeClient
    {
        private NamedPipeClientStream pipeStream;

        public PipeClient(string PipeName)
        {
            pipeStream = new NamedPipeClientStream(".", PipeName, PipeDirection.Out, PipeOptions.Asynchronous);
            pipeStream.Connect();
        }

        public async Task<bool> Send(string Message)
        {
            if (pipeStream.IsConnected)
            {
                byte[] buffer = Encoding.UTF8.GetBytes(Message);
                await pipeStream.WriteAsync(buffer, 0, buffer.Length);
                await pipeStream.FlushAsync();
                return true;
            }
            else
                return false;            
        }
    }
}
