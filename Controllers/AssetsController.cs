using Microsoft.AspNetCore.Mvc;
using AssetLayer.SDK;
using AssetLayer.SDK.Assets;

namespace CSharpProxy.Controllers
{

    [ApiController]
    [ApiRoute("asset")]
    public class AssetsController : ControllerBase
    {
        [HttpGet("info")]
        public async Task<IActionResult> GetAssets()
        {
            var props = ProxyNetworking.Utils.QueryCollectionToObject<AssetInfoProps>(Request.Query);
            var response = await AssetLayerSDK.Assets.Raw.Info(props);

            return Ok(response);
        }

        [HttpPost("mint")]
        public async Task<IActionResult> MintAssets()
        {
            Dictionary<string, string> headers = new Dictionary<string, string>() { };
            string? didtoken = ProxyNetworking.Utils.GetOptionalHeaderValue("didtoken", Request.Headers);
            var props = await ProxyNetworking.Utils.GetBodyAsObjectA<AssetMintProps>(Request.Body);
            if (!string.IsNullOrEmpty(didtoken)) {
                headers.Add("didtoken", didtoken);
            }
            else if (string.IsNullOrEmpty(props.mintTo)) {
                return BadRequest("Missing didtoken or mintTo");
            }

            var result = await AssetLayerSDK.Assets.Raw.Mint(props, headers);

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