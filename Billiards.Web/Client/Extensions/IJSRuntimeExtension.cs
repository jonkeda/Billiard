using Billiards.Web.Client.Models;
using Microsoft.JSInterop;

namespace Billiards.Web.Client.Extensions
{
    public static class IJSRuntimeExtension
    {
        public static async Task<WindowDimension> GetWindowDimension(this IJSRuntime jsRuntime)
        {
            return await jsRuntime.InvokeAsync<WindowDimension>("getWindowDimensions");
        }


        public static async Task<bool> OpenFullscreen(this IJSRuntime jsRuntime)
        {
            return await jsRuntime.InvokeAsync<bool>("openFullscreen");
        }

        public static async Task<bool> CloseFullscreen(this IJSRuntime jsRuntime)
        {
            return await jsRuntime.InvokeAsync<bool>("closeFullscreen");
        }
    }
}
