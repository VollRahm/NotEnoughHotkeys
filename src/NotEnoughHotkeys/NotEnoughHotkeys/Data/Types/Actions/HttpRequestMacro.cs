using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Windows;
using System.Threading;

namespace NotEnoughHotkeys.Data.Types.Actions
{
    public class HttpRequestMacro : IMacroAction
    {
        public string Name { get; set; }
        public string TypeName { get => "Send Http Request"; }

        public string URL { get; set; }
        public RequestMethod Method { get; set; }
        public bool ResponseToClipboard { get; set; }

        public HttpRequestMacro(string name, string url, RequestMethod method, bool responseToClipboard)
        {
            Name = name;
            URL = url;
            Method = method;
            ResponseToClipboard = responseToClipboard;
        }

        public async Task PerformAsync()
        {
            try
            {
                HttpClient httpClient = new HttpClient();
                HttpRequestMessage request = new HttpRequestMessage(Method == RequestMethod.GET ? HttpMethod.Get : HttpMethod.Post, URL);
                var response = await httpClient.SendAsync(request);

                if (ResponseToClipboard)
                {
                    var responseStr = await response.Content.ReadAsStringAsync();
                    SetClipboardText(responseStr);
                }
            }
            catch{}
        }

        private void SetClipboardText(string str)
        {
            var clipboardThread = new Thread(() => Clipboard.SetText(str));
            clipboardThread.SetApartmentState(ApartmentState.STA);
            clipboardThread.IsBackground = false;
            clipboardThread.Start();
        }

        public enum RequestMethod
        {
            GET,
            POST
        }
    }
}
