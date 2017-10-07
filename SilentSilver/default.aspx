<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="SilentSilver._default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server" action="/api/Management">
        <div>
            WebApiApplication.ValueRequestCount =<%this.Response.Write(SilentSilver.WebApiApplication.ValueRequestCount.ToString());%><br />
            SilentSilver.PreloadClient.Log = <% this.Response.Write(SilentSilver.PreloadClient.Log.Count.ToString()); %><br />
            WebApiApplication.PreLoadCalledCount =<%this.Response.Write(SilentSilver.WebApiApplication.PreLoadCalledCount.ToString());%>
        </div>
<%if (SilentSilver.WebApiApplication.PreLoadCalledCount == 0)
    {%>
        <input type="submit" />
<%} %>
    </form>
    <table>
        <% int count = 600;
            while(SilentSilver.PreloadClient.Log.TryDequeue( out SilentSilver.SimpleLogger.LogDetails item) && count > 0)
            {
                count--;
                if (item != null)
                {%>
                    <tr> <td> <% this.Response.Write(item.ThreadId.ToString());%></td><td> <% this.Response.Write(item.Message.Details);%></td> <td><%if( item.Exception != null) this.Response.Write(item.Exception.ToString());%></td> </tr>
                <%}
            }
            %>
        
    </table>
</body>
</html>
