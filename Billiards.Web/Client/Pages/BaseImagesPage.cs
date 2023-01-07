using System.Net.Http.Headers;
using Billiards.Web.Client.Extensions;
using Billiards.Web.Client.Models;
using Billiards.Web.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using System.Net.Http.Json;

namespace Billiards.Web.Client.Pages
{
    public class BaseImagesPage : ComponentBase
    {
        [Inject] 
        protected IJSRuntime JsRuntime { get; set; } = null!;

        [Inject]
        protected HttpClient Http { get; set; } = null!;


        protected const int VideoWidth = 960;
        protected const int VideoHeight = 540;

        protected const int TableWidth = 2000;
        protected const int TableHeight = 1000;

        protected int screenWidth { get; set; }= 480;
        protected int screenHeight { get; set; } = 270;

        protected string screenWidthPx { get; set; } = "480px";
        protected string screenHeightPx { get; set; } = "270px";

        protected int finderStroke { get; set; } = 8;
        protected int finderRadius { get; set; } = 16;

        protected int ballRadius { get; set; } = 16;

        protected string cameraStyle { get; set; }

        protected ScreenOrientation screenOrientation { get; set; }

        protected BallCollection? balls { get; set; }
        protected ProblemCollection? problems { get; set; }

        protected string? tableCorners { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            await SetSize();
        }

        private async Task SetSize()
        {
            var dimension = await JsRuntime.GetWindowDimension();
            screenWidth = dimension.Width;
            if (screenWidth > 3 * 480)
            {
                screenWidth = Math.Min(screenWidth, 3 * 480);
            }
            else if (screenWidth > 2 * 480)
            {
                screenWidth = Math.Min(screenWidth, 2 * 480);
            }
            else
            {
                screenWidth = screenWidth * 9 / 10;
            }
            if (dimension.Orientation == ScreenOrientation.Portrait)
            {
                screenHeight = screenWidth * 16 / 9;
            }
            else
            {
                screenHeight = screenWidth * 9 / 16;
            }

            if (screenHeight > (dimension.Height * 9 / 10))
            {
                if (dimension.Orientation == ScreenOrientation.Portrait)
                {
                    screenWidth = screenHeight * 9 / 16;

                }
                else
                {
                    screenWidth = screenHeight * 16 / 9;
                }
            }

            finderRadius = Math.Max(screenWidth, screenHeight) / 60;
            finderStroke = Math.Max(screenWidth, screenHeight) / 120;
            ballRadius = Math.Max(screenWidth, screenHeight) / 30;

            screenHeightPx = $"{screenHeight}px";
            screenWidthPx = $"{screenWidth}px";
            screenOrientation = dimension.Orientation;

            cameraStyle = $"position: absolute; top: 0px; left: 0px; width: {screenWidthPx}; height: {screenHeightPx}; z-index: 1";
        }

        private volatile bool sending;
        protected async Task<bool> RecognizeTable(string data)
        {
            if (sending)
            {
                return false;
            }

            try
            {
                var addItem = new TableRecognitionRequest(data);
                HttpResponseMessage response = await Http.PostAsJsonAsync("Recognition/Image", addItem);
                if (!response.IsSuccessStatusCode)
                {
                    return false;
                }

                TableRecognitionResponse? result = await response.Content.ReadFromJsonAsync<TableRecognitionResponse>();

                HandleRecognition(result);
            }
            finally
            {
                sending = false;
            }

            return true;
        }

        protected async Task<bool> RecognizeTable(IBrowserFile file)
        {
            if (sending)
            {
                return false;
            }

            try
            {
                using MultipartFormDataContent content = new ();
                StreamContent fileContent = new StreamContent(file.OpenReadStream(file.Size));
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
                content.Add( fileContent, "img.jpg", "img.jpg");

                HttpResponseMessage response = await Http.PostAsync("Recognition/Stream", content);
                if (!response.IsSuccessStatusCode)
                {
                    return false;
                }

                TableRecognitionResponse? result = await response.Content.ReadFromJsonAsync<TableRecognitionResponse>();

                HandleRecognition(result);
            }
            finally
            {
                sending = false;
            }

            return true;
        }

        private void HandleRecognition(TableRecognitionResponse? result)
        {
            if (result?.Table == null)
            {
                // hide svg
            }
            else
            {
                if (result.Table.Corners.Count < 4)
                {
                    // hide table
                }
                else
                {
                    string corners = "";
                    foreach (Point point in result.Table.Corners)
                    {
                        corners += ToImageAbsolutePoint(point).ToString();
                        corners += " ";
                    }

                    tableCorners = corners;
                }

                if (result.Balls != null)
                {
                    foreach (Ball ball in result.Balls)
                    {
                        if (ball.ImagePoint != null)
                        {
                            ball.ImageAbsolutePoint = ToImageAbsolutePoint(ball.ImagePoint);
                        }

                        if (ball.TablePoint != null)
                        {
                            ball.TableAbsolutePoint = ToTableAbsolutePoint(ball.TablePoint);
                        }
                    }

                    balls = result.Balls;
                }
            }
        }

        private Point ToTableAbsolutePoint(Point p)
        {
            return new Point(p.X * TableWidth, p.Y * TableHeight);
        }

        private Point ToImageAbsolutePoint(Point p)
        {
            return new Point(p.X * screenWidth, p.Y * screenHeight);
        }

        protected async Task<bool> MakePrediction()
        {
            if (balls == null
                || balls.Count < 3)
            {
                return false;
            }
            var request = new PredictionRequest(balls);
            HttpResponseMessage response = await Http.PostAsJsonAsync("Prediction", request);
            if (!response.IsSuccessStatusCode)
            {
                return false;
            }
            PredictionResponse? result = await response.Content.ReadFromJsonAsync<PredictionResponse>();
            problems = result?.Problems;
            return true;
        }
    }
}
