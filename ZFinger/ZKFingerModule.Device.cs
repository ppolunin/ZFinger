using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZKFinger
{
    using SDK;
    public static partial class ZKFingerModule
    {
        private class Device
        {
            private readonly int _width;
            private readonly int _height;
            private readonly int _dpi;
            private readonly IntPtr _handle;
            private byte[] _image;

            public Bitmap GetImage() => ZFingerSDK.Device.FingerprintImageFrom(_handle, _image);
            public string SerialNumber => ZFingerSDK.Device.GetSerialNumber(_handle);
            public string VendorInfo => ZFingerSDK.Device.GetVendorInfo(_handle);
            public string ProductName => ZFingerSDK.Device.GetProductName(_handle);
            public int Width => _width;
            public int Height => _height;
            public int DPI => _dpi;

            public bool AcquireFingerprint(out byte[] template)
            {
                return ZFingerSDK.Device.AcquireFingerprint(_handle, ref _image, out template) == ZFingerSDK.ZKFP_ERR_OK;
            }

            public void Dispose()
            {
                ZFingerSDK.Device.Close(_handle);
            }

            public Device()
            {
                if (ZFingerSDK.Devices.Count <= 0)
                    throw new InvalidOperationException("No device are connected");

                _handle = ZFingerSDK.Device.Open();
                if (_handle == IntPtr.Zero)
                    throw new InvalidOperationException("Device is busy or internal SDK error occurred");

                ZFingerSDK.Device.GetMetrics(_handle, out _width, out _height, out _dpi);
                _image = new byte[_width * _height];
            }
        }
    }
}
