using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SilentSilver
{
    public class PreloadClient : System.Web.Hosting.IProcessHostPreloadClient
    {
        public void Preload ( string[] parameters )
        {
            WebApiApplication.ValueRequestCount = 5;
        }
    }
}