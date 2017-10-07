using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace SilentSilver.Controllers
{
    public class ValuesController : ApiController
    {
        // GET api/values
        public string Get()
        {
            return $"counter is { WebApiApplication.ValueRequestCount.ToString() }";
        }

        public void Post()
        {
            // WebApiApplication.ValueRequestCount++;
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }

    [Route("api/SilverMessageLibrary/OrderAccepted")]
    public class OrderAcceptedController : ApiController
    {
        public HttpResponseMessage Post(SilverMessageLibrary.OrderAccepted messageData)
        {
            try
            {
                WebApiApplication.ValueRequestCount++;
                if (WebApiApplication.ValueRequestCount % 56 != 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK);

                }
            }
            catch (System.Exception) { }
            return Request.CreateResponse(HttpStatusCode.InternalServerError);
        }
    }

    public class ManagementController : ApiController
    {

        public void Post()
        {
            var client = new PreloadClient();

            client.Preload(new string[] { this.Request.RequestUri.AbsoluteUri.Replace("/Management", "") });

        }


    }
}