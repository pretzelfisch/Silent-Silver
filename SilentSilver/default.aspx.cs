using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SilentSilver
{
    public partial class _default : System.Web.UI.Page
    {
        public int currentCount=-1;
        protected void Page_Load(object sender, EventArgs e)
        {
            currentCount = WebApiApplication.ValueRequestCount;
        }
    }
}