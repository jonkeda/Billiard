using Billiards.Web.Client.Models;
using Microsoft.JSInterop;
using System.Text.Json;

namespace Billiards.Web.Client.Extensions
{
    public record ImageDimensions(int Width, int Height);

    public static class IJSRuntimeExtension
    {
        private static IJSObjectReference _imageModule = null!;
        public static async Task<ImageDimensions?> GetImageDimension(this IJSRuntime jsRuntime, Stream fileStream)
        {
            if (_imageModule == null!)
            {
                _imageModule = await jsRuntime.InvokeAsync<IJSObjectReference>("import", "./js/fileSize.js");
            }

            var streamRef = new DotNetStreamReference(fileStream);
            var json = await _imageModule.InvokeAsync<string>("getImageDimensions", streamRef);
            ImageDimensions? dimensions = JsonSerializer.Deserialize<ImageDimensions>(json);
            return dimensions;
        }

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
