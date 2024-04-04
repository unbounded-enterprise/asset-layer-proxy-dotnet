using Microsoft.AspNetCore.Mvc;
using AssetLayer.SDK;
using AssetLayer.SDK.Users;

namespace CSharpProxy.Controllers
{

    [ApiController]
    [ApiRoute("user")]
    public class UsersController : ControllerBase
    {
        [HttpGet("info")]
        public async Task<IActionResult> GetUser()
        {
            Dictionary<string, string> headers = new Dictionary<string, string>() {
                { "didtoken", ProxyNetworking.Utils.GetRequiredHeaderValue("didtoken", Request.Headers) }
            };
            var response = await AssetLayerSDK.Users.Raw.GetUser(headers);

            return Ok(response);
        }

        [HttpGet("collections")]
        public async Task<IActionResult> GetUserCollections()
        {
            Dictionary<string, string> headers = new Dictionary<string, string>() {
                { "didtoken", ProxyNetworking.Utils.GetRequiredHeaderValue("didtoken", Request.Headers) }
            };
            var props = ProxyNetworking.Utils.QueryCollectionToObject<UserCollectionsProps>(Request.Query);
            var result = await AssetLayerSDK.Users.Raw.Collections(props, headers);

            if (result.Item1 != null) {
                return Ok(result.Item1);
            }
            else if (result.Item2 != null) {
                return Ok(result.Item2);
            }
            else {
                return BadRequest("Unknown Error");
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser()
        {
            Dictionary<string, string> headers = new Dictionary<string, string>() {
                { "didtoken", ProxyNetworking.Utils.GetRequiredHeaderValue("didtoken", Request.Headers) }
            };
            var props = await ProxyNetworking.Utils.GetBodyAsObjectA<RegisterUserProps>(Request.Body);
            var result = await AssetLayerSDK.Users.Raw.Register(props, headers);

            if (result.Item1 != null) {
                return Ok(result.Item1);
            }
            else if (result.Item2 != null) {
                return Ok(result.Item2);
            }
            else {
                return BadRequest("Unknown Error");
            }
        }
    }
}