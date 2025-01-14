﻿using Billiards.Web.Client.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Billiards.Web.Client.Controllers
{
    /// <summary>
    /// Communication logic between ts/js and c#
    /// </summary>
    public class CameraStreamerController : ICameraStreamerModel
    {
        /// <summary>
        /// Path that can be used to access the static methods in the CameraStreamer typescript file
        /// </summary>
        private const string StaticInteropPath = "BlazorCameraStreamer.Scripts.CameraStreamerInterop";

        /// <summary>
        /// Runtime used to use javascript interoperability
        /// </summary>
        private readonly IJSRuntime JSRuntime;
        
        /// <summary>
        /// Typescript CameraStreamer object
        /// </summary>
        private IJSObjectReference JSObject;
        
        /// <summary>
        /// States if the object has been initialized already
        /// </summary>
        public bool IsInitialized { get; private set; } = false;

        /// <summary>
        /// Is called on each frame of the camera stream with a base64 string of the image
        /// </summary>
        private EventCallback<string> OnFrameCallback;

        /// <summary>
        /// Creates a new instance of the CameraStreamerController class
        /// </summary>
        /// <param name="runtime">Runtime used for javascript interoperability</param>
        public CameraStreamerController(IJSRuntime runtime)
        {
            JSRuntime = runtime;
        }

        /// <summary>
        /// Initializes the typescript object with the provided parameters
        /// </summary>
        /// <param name="videoReference"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="onFrameCallback"></param>
        /// <returns></returns>
        public async Task InitializeAsync(ElementReference videoReference, int width = 640, int height = 360, EventCallback<string> onFrameCallback = default)
        {
            if (IsInitialized)
            {
                await DisposeAsync();
            }

            OnFrameCallback = onFrameCallback;

            JSObject = await JSRuntime.InvokeAsync<IJSObjectReference>(StaticInteropPath + ".createInstance");

            await JSObject.InvokeVoidAsync("init", videoReference, OnFrameCallback.HasDelegate, DotNetObjectReference.Create(this), nameof(OnFrame), width, height);

            IsInitialized = true;
        }

        public async Task StartAsync(string camera = null)
        {
            // Use the first found camera if no camera is given
            await JSObject.InvokeVoidAsync("start", camera ?? (await GetCameraDevicesAsync()).FirstOrDefault()?.DeviceId);
        }

        public async Task StopAsync()
        {
            await JSObject.InvokeVoidAsync("stop");
        }

        public async Task ChangeCameraAsync(string newId)
        {
            await JSObject.InvokeVoidAsync("changeCamera", newId);
        }

        public async Task<string> GetImage()
        {
            return await JSObject.InvokeAsync<string>("getImage");
        }

        public async Task<bool> GetCameraAccessAsync()
        {
            return await JSRuntime.InvokeAsync<bool>(StaticInteropPath + ".getCameraAccess");
        }

        public async Task<MediaDeviceInfoModel[]> GetCameraDevicesAsync()
        {
            return await GetCameraDevicesAsync(JSRuntime);
        }

        public async ValueTask DisposeAsync()
        {
            if (IsInitialized)
            {
                await JSObject.InvokeVoidAsync("dispose");
            }
        }

        /// <summary>
        /// Gets all camera devices (no audio) as MediaDeviceInfo and returns them as an array
        /// </summary>
        /// <returns>An array of all cameras as MediaDeviceInfo</returns>
        public static async Task<MediaDeviceInfoModel[]> GetCameraDevicesAsync(IJSRuntime runtime)
        {
            return await runtime.InvokeAsync<MediaDeviceInfoModel[]>(StaticInteropPath + ".getCameraDeviceList");
        }

        /// <summary>
        /// Invokable method from javascript/typescript that calls the given callback method
        /// </summary>
        /// <param name="data">Base64 string of the image</param>
        [JSInvokable]
        public async Task OnFrame(string data)
        {
            if (OnFrameCallback.HasDelegate)
                await OnFrameCallback.InvokeAsync(data);
        }
    }
}
