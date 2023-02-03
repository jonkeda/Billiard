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

        protected BallColor CueBall { get; set; } = BallColor.White;

        protected int VideoWidth { get; set; } = 960;
        protected int VideoHeight { get; set; } = 540;

        protected const int TableWidth = 2000;
        protected const int TableHeight = 1000;

        protected int ScreenWidth { get; set; } = 480;
        protected int ScreenHeight { get; set; } = 270;

        protected string Transform { get; set; }
        protected string Viewbox { get; set; } = "0 0 2000 1000";

        protected string ScreenWidthPx { get; set; } = "480px";
        protected string ScreenHeightPx { get; set; } = "270px";

        protected int FinderStroke { get; set; } = 8;
        protected int FinderRadius { get; set; } = 16;

        protected int BallRadius { get; set; } = 31;

        protected string CameraStyle { get; set; }

        protected ScreenOrientation ScreenOrientation { get; set; }

        protected BallCollection? Balls { get; set; }
        protected ProblemCollection? Problems { get; set; }

        protected string? TableCorners { get; set; }

        protected LogCollection Log { get; } = new ();

        private bool busy;
        protected bool Busy
        {
            get { return busy; }
            set
            {
                busy = value;
                StateHasChanged();
            }
        }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            await SetSize();
            await JsRuntime.OpenFullscreen();
        }

        protected async Task SetSize()
        {
            var dimension = await JsRuntime.GetWindowDimension();
            ScreenWidth = dimension.Width;
            if (dimension.Orientation == ScreenOrientation.Portrait)
            {
                ScreenHeight = ScreenWidth * 16 / 9;
            }
            else
            {
                ScreenHeight = ScreenWidth * 9 / 16;
            }

            if (ScreenHeight > dimension.Height)
            {
                ScreenHeight = dimension.Height;
                if (dimension.Orientation == ScreenOrientation.Portrait)
                {
                    ScreenWidth = ScreenHeight * 9 / 16;
                }
                else
                {
                    ScreenWidth = ScreenHeight * 16 / 9;
                }
            }

            if (dimension.Orientation == ScreenOrientation.Portrait)
            {
                Viewbox = "0 0 1000 2000";
                Transform = "rotate(90, 500, 1000) translate(-500, 500)";
            }
            else
            {
                Viewbox = "0 0 2000 1000";
                Transform = "";
            }

            FinderRadius = Math.Max(ScreenWidth, ScreenHeight) / 60;
            FinderStroke = Math.Max(ScreenWidth, ScreenHeight) / 120;

            ScreenHeightPx = $"{ScreenHeight}px";
            ScreenWidthPx = $"{ScreenWidth}px";
            ScreenOrientation = dimension.Orientation;

            //CameraStyle = $"width: {ScreenWidthPx}; height: {ScreenHeightPx}";
            CameraStyle = $"position: absolute; top: 0px; left: 0px; width: {ScreenWidthPx}; height: {ScreenHeightPx}; z-index: 1";
        }


        protected async Task RecognizeAndPredict(string data)
        {
            bool recognized = await RecognizeTable(data);
            Log.Add("Recognize");
            if (recognized)
            {
                bool predicted = await MakePrediction();
            }
            Log.Add("Predict");
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
                sending = true;
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
                sending = true;
                using MultipartFormDataContent content = new();
                StreamContent fileContent = new StreamContent(file.OpenReadStream(file.Size));
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
                content.Add(fileContent, "img.jpg", "img.jpg");

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
            Log.Add(result?.Log);

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

                    TableCorners = corners;
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

                    Balls = result.Balls;
                }
            }
        }

        private Point ToTableAbsolutePoint(Point p)
        {
            return new Point(p.X * TableWidth, p.Y * TableHeight);
        }

        private Point ToImageAbsolutePoint(Point p)
        {
            return new Point(p.X * ScreenWidth, p.Y * ScreenHeight);
        }

        protected async Task<bool> MakePrediction()
        {
            if (sending)
            {
                return false;
            }
            try
            {
                if (Balls == null
                    || Balls.Count < 3)
                {
                    return false;
                }
                sending = true;
                var request = new PredictionRequest(Balls, CueBall);
                HttpResponseMessage response = await Http.PostAsJsonAsync("Prediction", request);
                if (!response.IsSuccessStatusCode)
                {
                    return false;
                }
                PredictionResponse? result = await response.Content.ReadFromJsonAsync<PredictionResponse>();

                Log.Add(result?.Log);

                Problems = result?.Problems;
            }
            finally
            {
                sending = false;
            }
            return true;
        }

        protected async Task ClickWhite()
        {
            CueBall = BallColor.White;

            await MakePrediction();
        }

        protected async Task ClickYellow()
        {
            CueBall = BallColor.Yellow;
            await MakePrediction();
        }
    }
}
