using System.Collections.Generic;
using System.Web.Http;
using Swashbuckle.NetFx.HideApi;

namespace SwaggerDemo_WebApi_Net45.Controllers
{
    /// <summary>
    /// </summary>
    [HiddenApi]
    public class Values2Controller : ApiController
    {
        /// <summary>
        ///     Get
        /// </summary>
        /// <returns></returns>
        // GET api/values
        public IEnumerable<string> Get()
        {
            return new[] { "value1", "value2" };
        }

        /// <summary>
        ///     getbyid
        /// </summary>
        /// <param name="id">id </param>
        /// <returns></returns>
        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }

        /// <summary>
        ///     post
        /// </summary>
        /// <param name="value">value </param>
        // POST api/values
        public void Post([FromBody] string value)
        {
        }

        /// <summary>
        ///     put
        /// </summary>
        /// <param name="id">id</param>
        /// <param name="value"> value</param>
        // PUT api/values/5
        public void Put(int id, [FromBody] string value)
        {
        }

        /// <summary>
        ///     delete by id
        /// </summary>
        /// <param name="id">id</param>
        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
