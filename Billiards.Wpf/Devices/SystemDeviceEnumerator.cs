using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace Billiards.Wpf.Devices
{
    public class VideoDeviceCollection : Collection<VideoDevice>
    {
    }

    public class VideoDevice
    {
        public VideoDevice(int index, string name)
        {
            Index = index;
            Name = name;
        }

        public int Index { get; }

        public string Name { get; }

        public override string ToString()
        {
            return Name;
        }
    }

    /// <summary>
    /// A system device enumerator.
    /// </summary>
    public class SystemDevices : IDisposable
    {
        private bool disposed;
        private ICreateDevEnum _systemDeviceEnumerator;

        /// <summary>
        /// Initializes a new instance of the <see cref="SystemDevices"/> class.
        /// </summary>
        public SystemDevices()
        {
            var comType = Type.GetTypeFromCLSID(new Guid("62BE5D10-60EB-11D0-BD3B-00A0C911CE86"));
            _systemDeviceEnumerator = (ICreateDevEnum)Activator.CreateInstance(comType);
        }

        /// <summary>
        /// Lists the video input devices connected to the system.
        /// </summary>
        /// <returns>A dictionary with the id and name of the device.</returns>
        public IReadOnlyList<VideoDevice> VideoDevices()
        {
            var videoInputDeviceClass = new Guid("{860BB310-5D01-11D0-BD3B-00A0C911CE86}");

            var hresult = _systemDeviceEnumerator.CreateClassEnumerator(ref videoInputDeviceClass, out var enumMoniker, 0);
            if (hresult != 0)
            {
                throw new ApplicationException("No devices of the category");
            }

            var moniker = new IMoniker[1];
            var list = new VideoDeviceCollection();

            while (true)
            {
                hresult = enumMoniker.Next(1, moniker, IntPtr.Zero);
                if (hresult != 0 || moniker[0] == null)
                {
                    break;
                }

                var device = new VideoInputDevice(moniker[0]);
                list.Add(new VideoDevice(list.Count, device.Name));

                // Release COM object
                Marshal.ReleaseComObject(moniker[0]);
                moniker[0] = null;
            }

            return list.AsReadOnly();
        }

        /// <summary>
        /// rees, releases, or resets unmanaged resources.
        /// </summary>
        /// <param name="disposing"><c>false</c> if invoked by the finalizer because the object is being garbage collected; otherwise, <c>true</c></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    if (!(_systemDeviceEnumerator is null))
                    {
                        Marshal.ReleaseComObject(_systemDeviceEnumerator);
                        _systemDeviceEnumerator = null;
                    }
                }

                disposed = true;
            }
        }

        /// <summary>
        /// Frees, releases, or resets unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
