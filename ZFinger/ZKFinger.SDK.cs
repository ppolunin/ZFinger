using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;

namespace ZKFinger.SDK
{
    public static class ZFingerSDK
    {
        public static readonly int ZKFP_ERR_ALREADY_INIT = 1;
        public static readonly int ZKFP_ERR_OK = 0;
        public static readonly int ZKFP_ERR_INITLIB = -1;
        public static readonly int ZKFP_ERR_INIT = -2;
        public static readonly int ZKFP_ERR_NO_DEVICE = -3;
        public static readonly int ZKFP_ERR_INVALID_PARAM = -5;
        public static readonly int ZKFP_ERR_OPEN = -6;
        public static readonly int ZKFP_ERR_INVALID_HANDLE = -7;
        public static readonly int ZKFP_ERR_NOT_SUPPORT = -4;
        public static readonly int ZKFP_ERR_CAPTURE = -8;
        public static readonly int ZKFP_ERR_EXTRACT_FP = -9;
        public static readonly int ZKFP_ERR_ABSORT = -10;
        public static readonly int ZKFP_ERR_MEMORY_NOT_ENOUGH = -11;
        public static readonly int ZKFP_ERR_BUSY = -12;
        public static readonly int ZKFP_ERR_ADD_FINGER = -13;
        public static readonly int ZKFP_ERR_DEL_FINGER = -14;
        public static readonly int ZKFP_ERR_FAIL = -17;
        public static readonly int ZKFP_ERR_CANCEL = -18;
        public static readonly int ZKFP_ERR_VERIFY_FP = -20;
        public static readonly int ZKFP_ERR_MERGE = -22;
        public static readonly int ZKFP_ERR_NOT_OPENED = -23;
        public static readonly int ZKFP_ERR_NOT_INIT = -24;
        public static readonly int ZKFP_ERR_ALREADY_OPENED = -25;
        public static readonly int ZKFP_ERR_LOADIMAGE = -26;
        public static readonly int ZKFP_ERR_ANALYSE_IMG = -27;
        public static readonly int ZKFP_ERR_TIMEOUT = -28;

        public static readonly int MAX_TEMPLATE_SIZE = 2048;
        public static readonly int REGISTER_FINGER_COUNT = 3;
        public static readonly int MAX_CACHE_COUNT = 50000;

        private struct Pointer
        {
            private GCHandle handle;

            public IntPtr Ptr => handle.AddrOfPinnedObject();

            public void Free()
            {
                handle.Free();
            }

            public static implicit operator IntPtr(Pointer ptr) => ptr.Ptr;

            public static Pointer Alloc(object value)
            {
                return new Pointer() { handle = GCHandle.Alloc(value, GCHandleType.Pinned) };
            }
        }

        private static IntPtr __hfngrAPI;

        private static IntPtr HFingerAPI
        {
            get
            {
                if (__hfngrAPI == IntPtr.Zero)
                    __hfngrAPI = SDK.ZKFPM_CreateDBCache();
                return __hfngrAPI;
            }
        }

        public static int InitSDKAndDevices() => SDK.ZKFPM_Init();

        public static class Template
        {
            public static int MergeListInto(IEnumerable<byte[]> list, out byte[] template)
            {
                var items = list.Take(3).ToArray();
                if (items.Length != 3)
                    throw new RankException("The list parameter must has a capacity is a 3");

                Pointer[] ptr = items.Select(item => Pointer.Alloc(item)).ToArray();
                try
                {
                    template = new byte[MAX_TEMPLATE_SIZE];
                    Pointer ptemp = Pointer.Alloc(template);
                    try
                    {
                        int size = MAX_TEMPLATE_SIZE;
                        int result = SDK.ZKFPM_GenRegTemplate(HFingerAPI, ptr[0], ptr[1], ptr[2], ptemp, ref size);

                        if (result == ZKFP_ERR_OK)
                            Array.Resize(ref template, size);
                        else
                            template = null;

                        return result;
                    }
                    finally
                    {
                        ptemp.Free();
                    }
                }
                finally
                {
                    foreach (var p in ptr) p.Free();
                }
            }

            public static int FP_TMP_MATCH_SCORE = 0;

            public static int Match(byte[] temp1, byte[] temp2)
            {
                Pointer[] ptr = { Pointer.Alloc(temp1), Pointer.Alloc(temp2) };
                try
                {
                    return SDK.ZKFPM_MatchFinger(HFingerAPI, ptr[0], temp1.Length, ptr[1], temp2.Length);
                }
                finally
                {
                    foreach (var p in ptr) p.Free();
                }
            }

            public static bool MatchWithList(byte[] template, IEnumerable<byte[]> list)
            {
                Pointer ptmp = Pointer.Alloc(template);
                try
                {
                    foreach (byte[] item in list)
                    {
                        Pointer pitm = Pointer.Alloc(item);
                        try
                        {
                            if (SDK.ZKFPM_MatchFinger(HFingerAPI, ptmp, template.Length, pitm, item.Length) > FP_TMP_MATCH_SCORE)
                                return true;
                        }
                        finally
                        {
                            pitm.Free();
                        }
                    }

                    return false;
                }
                finally
                {
                    ptmp.Free();
                }
            }
        }

        public static class Device
        {
            private static string GetDeviceParameter(IntPtr hDevice, int index)
            {
                int size = 256;
                byte[] buffer = new byte[size];

                Pointer ptr = Pointer.Alloc(buffer);
                try
                {
                    if (SDK.ZKFPM_GetParameters(hDevice, index, ptr, ref size) != ZKFP_ERR_OK)
                        return null;
                }
                finally
                {
                    ptr.Free();
                }

                Array.Resize(ref buffer, size);
                return System.Text.Encoding.Default.GetString(buffer);
            }

            public static int GetMetrics(IntPtr hDevice, out int width, out int height, out int DPI) =>
                SDK.ZKFPM_GetCaptureParamsEx(hDevice, out width, out height, out DPI);

            public static int AcquireFingerprint(IntPtr hDevice, ref byte[] image, out byte[] template)
            {
                template = new byte[MAX_TEMPLATE_SIZE];
                int size = MAX_TEMPLATE_SIZE;

                Pointer[] ptr = { Pointer.Alloc(template), Pointer.Alloc(image) };
                try
                {
                    int result;
                    if (ZKFP_ERR_OK == (result = SDK.ZKFPM_AcquireFingerprint(hDevice, ptr[1], image.Length, ptr[0], ref size)))
                        Array.Resize(ref template, size);
                    else
                        template = null;

                    return result;
                }
                finally
                {
                    foreach (var p in ptr) p.Free();
                }
            }

            public static int FP_IMG_BLACK_LEVEL = 16;
            public static int FP_IMG_BLACK_CAPTURE_LEVEL = 160;
            public static int FP_IMG_WHITE_LEVEL = 255;
            public static int FP_IMG_WHITE_CAPTURE_LEVEL = 224;
            public static bool FB_IMG_IS_TRANSPARENT = true;

            public static Bitmap FingerprintImageFrom(IntPtr hDevice, byte[] image)
            {
                Bitmap result = null;

                if (GetMetrics(hDevice, out int width, out int height, out int _) != ZKFP_ERR_OK)
                    return result;

                Pointer ptr = Pointer.Alloc(image);
                try
                {
                    result = new Bitmap(width, height, ((width << 2) + 3) >> 2, System.Drawing.Imaging.PixelFormat.Format8bppIndexed, ptr);
                }
                finally
                {
                    ptr.Free();
                }

                System.Drawing.Imaging.ColorPalette palette = result.Palette;

                int index = 0;
                while (index < FP_IMG_BLACK_CAPTURE_LEVEL)
                {
                    palette.Entries[index] = Color.FromArgb(FP_IMG_BLACK_LEVEL, FP_IMG_BLACK_LEVEL, FP_IMG_BLACK_LEVEL);
                    index++;
                }


                while (index < FP_IMG_WHITE_CAPTURE_LEVEL)
                {
                    int value = FP_IMG_BLACK_CAPTURE_LEVEL + (FP_IMG_WHITE_LEVEL - FP_IMG_BLACK_CAPTURE_LEVEL) 
                        * (index - FP_IMG_BLACK_CAPTURE_LEVEL) / (FP_IMG_WHITE_CAPTURE_LEVEL - FP_IMG_BLACK_CAPTURE_LEVEL);

                    palette.Entries[index] = Color.FromArgb(value, value, value);
                    index++;
                }

                while (index < 256)
                {
                    palette.Entries[index] = Color.FromArgb(FP_IMG_WHITE_LEVEL, FP_IMG_WHITE_LEVEL, FP_IMG_WHITE_LEVEL);
                    index++;
                }

                result.Palette = palette;

                if (FB_IMG_IS_TRANSPARENT)
                    result.MakeTransparent(Color.FromArgb(FP_IMG_WHITE_LEVEL, FP_IMG_WHITE_LEVEL, FP_IMG_WHITE_LEVEL));

                return result;
            }

            public static IntPtr Open(int idx = 0) => SDK.ZKFPM_OpenDevice(idx);

            public static int Close(IntPtr hDevice) => SDK.ZKFPM_CloseDevice(hDevice);

            public static string GetSerialNumber(IntPtr hDevice) => GetDeviceParameter(hDevice, 1103);

            public static string GetVendorInfo(IntPtr hDevice) => GetDeviceParameter(hDevice, 1101);

            public static string GetProductName(IntPtr hDevice) => GetDeviceParameter(hDevice, 1102);

            public static int AllocateImage(IntPtr hDevice, out byte[] image)
            {
                int result;
                if ((result = GetMetrics(hDevice, out int width, out int height, out int _)) == ZKFP_ERR_OK)
                    image = new byte[width * height];
                else
                    image = null;

                return result;
            }
        }

        public static class Devices
        {
            public static int Initialize() => SDK.ZKFPM_Init();

            public static int Count
            {
                get
                {
                    int result = SDK.ZKFPM_GetDeviceCount();
                    return result > 0 ? --result : result;
                }
            }

            public static int Finalize() => SDK.ZKFPM_Terminate();
        }

        public static class Cache
        {
            public static int InsertTemplate(int ID, byte[] template)
            {
                Pointer ptr = Pointer.Alloc(template);
                try
                {
                    return SDK.ZKFPM_AddRegTemplateToDBCache(HFingerAPI, ID, ptr, template.Length);
                }
                finally
                {
                    ptr.Free();
                }
            }

            public static int GetCount(out int count) => SDK.ZKFPM_GetDBCacheCount(HFingerAPI, out count);

            public static int RemoveTemplate(int ID) => SDK.ZKFPM_DelRegTemplateFromDBCache(HFingerAPI, ID);

            public static int RemoveTemplate(byte[] template, out int ID)
            {
                if (IdenditfyTemplate(template, out ID, out int _) != ZKFP_ERR_OK)
                    return ZKFP_ERR_DEL_FINGER;

                return RemoveTemplate(ID);
            }

            public static int Clear() => SDK.ZKFPM_ClearDBCache(HFingerAPI);

            public static int IdenditfyTemplate(byte[] template, out int ID, out int score)
            {
                Pointer ptr = Pointer.Alloc(template);
                try
                {
                    return SDK.ZKFPM_Identify(HFingerAPI, ptr, template.Length, out ID, out score);
                }
                finally
                {
                    ptr.Free();
                }
            }

            public static int IdenditfyTemplateByID(int ID, byte[] template)
            {
                Pointer ptr = Pointer.Alloc(template);
                try
                {
                    return SDK.ZKFPM_VerifyByID(HFingerAPI, ID, ptr, template.Length);
                }
                finally
                {
                    ptr.Free();
                }
            }
        }

        private static class SDK
        {
            [DllImport("libzkfp.dll")]
            public static extern int ZKFPM_GetCaptureParamsEx(IntPtr handle, out int width, out int height, out int dpi);

            [DllImport("libzkfp.dll")]
            public static extern int ZKFPM_Init();

            [DllImport("libzkfp.dll")]
            public static extern int ZKFPM_Terminate();

            [DllImport("libzkfp.dll")]
            public static extern int ZKFPM_GetDeviceCount();

            [DllImport("libzkfp.dll")]
            public static extern IntPtr ZKFPM_OpenDevice(int index);

            [DllImport("libzkfp.dll")]
            public static extern int ZKFPM_CloseDevice(IntPtr handle);

            [DllImport("libzkfp.dll")]
            public static extern int ZKFPM_SetParameters(IntPtr handle, int nParamCode, IntPtr paramValue, int cbParamValue);

            [DllImport("libzkfp.dll")]
            public static extern int ZKFPM_GetParameters(IntPtr handle, int nParamCode, IntPtr paramValue, ref int cbParamValue);

            [DllImport("libzkfp.dll")]
            public static extern int ZKFPM_AcquireFingerprint(IntPtr handle, IntPtr fpImage, int cbFPImage, IntPtr fpTemplate, ref int cbTemplate);

            [DllImport("libzkfp.dll")]
            public static extern IntPtr ZKFPM_CreateDBCache();

            [DllImport("libzkfp.dll")]
            public static extern int ZKFPM_CloseDBCache(IntPtr hDBCache);

            [DllImport("libzkfp.dll")]
            public static extern int ZKFPM_GenRegTemplate(IntPtr hDBCache, IntPtr temp1, IntPtr temp2, IntPtr temp3, IntPtr regTemp, ref int cbRegTemp);

            [DllImport("libzkfp.dll")]
            public static extern int ZKFPM_AddRegTemplateToDBCache(IntPtr hDBCache, int fid, IntPtr fpTemplate, int cbTemplate);

            [DllImport("libzkfp.dll")]
            public static extern int ZKFPM_DelRegTemplateFromDBCache(IntPtr hDBCache, int fid);

            [DllImport("libzkfp.dll")]
            public static extern int ZKFPM_ClearDBCache(IntPtr hDBCache);

            [DllImport("libzkfp.dll")]
            public static extern int ZKFPM_GetDBCacheCount(IntPtr hDBCache, out int count);

            [DllImport("libzkfp.dll")]
            public static extern int ZKFPM_Identify(IntPtr hDBCache, IntPtr fpTemplate, int cbTemplate, out int fid, out int score);

            [DllImport("libzkfp.dll")]
            public static extern int ZKFPM_VerifyByID(IntPtr hDBCache, int fid, IntPtr fpTemplate, int cbTemplate);

            [DllImport("libzkfp.dll")]
            public static extern int ZKFPM_MatchFinger(IntPtr hDBCache, IntPtr fpTemplate1, int cbTemplate1, IntPtr fpTemplate2, int cbTemplate2);

            [DllImport("libzkfp.dll")]
            public static extern int ZKFPM_ExtractFromImage(IntPtr hDBCache, string fileName, int dpi, IntPtr fpTemplate, ref int cbTemplate);

            [DllImport("libzkfp.dll")]
            public static extern int ZKFPM_AcquireFingerprintImage(IntPtr hDBCache, IntPtr fpImage, int cbFPImage);

            [DllImport("libzkfp.dll")]
            public static extern int ZKFPM_DBSetParameter(IntPtr handle, int nParamCode, int paramValue);

            [DllImport("libzkfp.dll")]
            public static extern int ZKFPM_DBGetParameter(IntPtr handle, int nParamCode, out int paramValue);

            [DllImport("libzkfp.dll")]
            public static extern int ZKFPM_GetTemplateQuality(IntPtr handle, IntPtr fpTemplate, int cbTemplate);
        }
    }
}