using Billiards.Web.Client.Models;
using Microsoft.JSInterop;

namespace Billiards.Web.Client.Extensions
{
    public static class IJSRuntimeExtension
    {
        public static async Task<WindowDimension> GetWindowDimension(this IJSRuntime jsRuntime)
        {
            return await jsRuntime.InvokeAsync<WindowDimension>("getWindowDimensions", null);
        }

    }
}
