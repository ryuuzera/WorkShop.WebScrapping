using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Workshop.Services.Default
{
    internal class ScrapperDefault
    {
        protected HttpClient httpClient { get; set; }

        public ScrapperDefault()
        {
            httpClient = new HttpClient();
        }
    }
}
