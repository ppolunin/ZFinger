using System;
using System.Linq;
using System.Windows.Forms;
using ZKFinger;

namespace TestZFinger
{
    public partial class MainForm : Form
    {
        private readonly SimpleStorage storage;

        private void OutputLine(string text)
        {
            textRes.AppendText($"{text}\n");
            textRes.SelectionStart = Int32.MaxValue;
            textRes.ScrollToCaret();
        }

        private bool CloseSessionIfChecked()
        {
            bool result = cbxCloseSession.Checked;
            if (result)
                ZKFingerSessions.CloseSession();
            return result;
        }

        private void FingerModuleStateProc(ZKSessionState state, dynamic args)
        {
            switch (state)
            {
                // ZKSessionType.Register, ZKSessionType.Identify, ZKSessionType.Unregister
                case ZKSessionState.Opened:
                    bnClose.Enabled = true;
                    cbSessionType.Enabled = false;

                    ZKSessionType type = args.Session;

                    OutputLine($"Session {type} is opened");
                    OutputLine($"Device {args.Device.VendorInfo}");

                    switch (type)
                    {
                        case ZKSessionType.Register:
                            OutputLine("Tap the finger to device a 3 times...");
                            break;

                        case ZKSessionType.Identify:
                            OutputLine("Tap the finger to device...");
                            break;
                    }
                    break;

                // ZKSessionType.Register, ZKSessionType.Identify, ZKSessionType.Unregister
                case ZKSessionState.Closed:
                    bnOpen.Enabled = true;
                    bnClose.Enabled = false;
                    cbSessionType.Enabled = true;

                    OutputLine($"Session {args.Session.ToString()} is closed");
                    OutputLine("==========================================");
                    break;


                // ZKSessionType.Register, ZKSessionType.Identify, ZKSessionType.Unregister
                case ZKSessionState.FingerAcquired:
                    picFPImg.Image = args.GetImage();
                    break;

                // ZKSessionType.Register
                case ZKSessionState.FingerAlreadyRegistered:
                    OutputLine($"Finger is already registered with the ID {args.ID}, " +
                        $"please, tap another finger to...");
                    break;

                // ZKSessionType.Register
                case ZKSessionState.FingerIsMatched:
                    OutputLine($"Finger is matched, remained is {args.Remained}");
                    break;

                // ZKSessionType.Register
                case ZKSessionState.FingerIsNotMatched:
                    OutputLine($"Finger is not matched, try again. Remained is {args.Remained}");
                    break;

                // ZKSessionType.Register
                case ZKSessionState.FingerMergeError:
                    OutputLine($"Internal sdk error {args.SDKErr} is occured, try again...");
                    CloseSessionIfChecked();
                    break;

                // ZKSessionType.Register
                case ZKSessionState.FingerRegisterSuccess:
                    OutputLine($"A finger successfully registered with the ID {args.ID}");

                    if (CloseSessionIfChecked())
                        break;

                    OutputLine("==========================================");
                    OutputLine("Tap the finger to device a 3 times...");
                    break;

                // ZKSessionType.Identify
                case ZKSessionState.FingerIdetified:
                    OutputLine($"A finger successfully identified with the ID {args.ID}");
                    CloseSessionIfChecked();
                    break;

                // ZKSessionType.Unregister
                case ZKSessionState.FingerUnregistered:
                    OutputLine($"A finger with the ID {args.ID} is successfully unregistered");
                    CloseSessionIfChecked();
                    break;

                // ZKSessionType.Identify, ZKSessionType.Unregister
                case ZKSessionState.FingerNotIdetified:
                    if (CloseSessionIfChecked())
                    {
                        OutputLine($"A finger a was not identified... sorry...");
                        break;
                    }

                    OutputLine($"A finger a was not identified or registered, try again...");
                    break;

                default:
                    break;

            }
        }

        public MainForm()
        {
            InitializeComponent();
            Icon = Properties.Resources.main;
            cbSessionType.Items.AddRange(typeof(ZKSessionType).GetEnumValues().Cast<object>().ToArray());
            cbSessionType.SelectedIndex = 0;
            storage = new SimpleStorage();
        }

        private void bnClose_Click(object sender, EventArgs e)
        {
            ZKFingerSessions.CloseSession();
        }

        private void bnOpen_Click(object sender, EventArgs e)
        {
            try
            {
                ZKFingerSessions.OpenSession((ZKSessionType)cbSessionType.SelectedItem, storage, FingerModuleStateProc);

                bnOpen.Enabled = false;
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //protected override void DefWndProc(ref Message m)
        //{
        //    switch (m.Msg)
        //    {
        //        case MESSAGE_CAPTURED_OK:
        //        {
        //                //MemoryStream ms = new MemoryStream();
        //                //BitmapFormat.GetBitmap(FPBuffer, mfpWidth, mfpHeight, ref ms);
        //                //Bitmap bmp = new Bitmap(ms);
        //                this.picFPImg.Image = FromImage(mfpWidth, mfpHeight, FPBuffer);


        //                String strShow = Convert.ToBase64String(CapTmp); //zkfp2.BlobToBase64(CapTmp, CapTmp.Length);
        //                textRes.AppendText("capture template data:" + strShow + "\n");

        //                if (IsRegister)
        //                {
        //                    int ret = zkfp.ZKFP_ERR_OK;
        //                    int fid = 0, score = 0;
        //                    ret = zkfp2.DBIdentify(mDBHandle, CapTmp, ref fid, ref score);
        //                    if (zkfp.ZKFP_ERR_OK == ret)
        //                    {
        //                        textRes.AppendText("This finger was already register by " + fid + "!\n");
        //                        return;
        //                    }

        //                    if (RegisterCount > 0 && !IsMatch(CapTmp, RegTmps)) //zkfp2.DBMatch(mDBHandle, CapTmp, RegTmps[RegisterCount - 1]) <= 0)
        //                    {
        //                        textRes.AppendText("Please press the same finger 3 times for the enrollment.\n");
        //                        return;
        //                    }

        //                    //Array.Copy(CapTmp, RegTmps[RegisterCount], cbCapTmp);

        //                    RegTmps[RegisterCount] = CapTmp;

        //                    String strBase64 = zkfp2.BlobToBase64(CapTmp, CapTmp.Length);
        //                    byte[] blob = zkfp2.Base64ToBlob(strBase64);
        //                    RegisterCount++;
        //                    if (RegisterCount >= REGISTER_FINGER_COUNT)
        //                    {
        //                        RegisterCount = 0;
        //                        if (zkfp.ZKFP_ERR_OK == (ret = MergeTempaltes(mDBHandle, RegTmps, out RegTmp)) && //zkfp2.DBMerge(mDBHandle, RegTmps[0], RegTmps[1], RegTmps[2], RegTmp, ref cbRegTmp)) &&
        //                               zkfp.ZKFP_ERR_OK == (ret = InsertIntoCache(mDBHandle, iFid, RegTmp)))
        //                        {
        //                            cbRegTmp = RegTmp.Length;

        //                            iFid++;

        //                            textRes.AppendText("enroll succ\n");
        //                        }
        //                        else
        //                        {
        //                            textRes.AppendText("enroll fail, error code=" + ret + "\n");
        //                        }
        //                        IsRegister = false;
        //                        return;
        //                    }
        //                    else
        //                    {
        //                        textRes.AppendText("You need to press the " + (REGISTER_FINGER_COUNT - RegisterCount) + " times fingerprint\n");
        //                    }
        //                }
        //                else
        //                {
        //                    if (cbRegTmp <= 0)
        //                    {
        //                        textRes.AppendText("Please register your finger first!\n");
        //                        return;
        //                    }
        //                    if (bIdentify)
        //                    {
        //                        int ret = zkfp.ZKFP_ERR_OK;
        //                        int fid = 0, score = 0;
        //                        ret = zkfp2.DBIdentify(mDBHandle, CapTmp, ref fid, ref score);
        //                        if (zkfp.ZKFP_ERR_OK == ret)
        //                        {
        //                            textRes.AppendText("Identify succ, fid= " + fid + ",score=" + score + "!\n");
        //                            return;
        //                        }
        //                        else
        //                        {
        //                            textRes.AppendText("Identify fail, ret= " + ret + "\n");
        //                            return;
        //                        }
        //                    }
        //                    else
        //                    {
        //                        int ret = zkfp2.DBMatch(mDBHandle, CapTmp, RegTmp);
        //                        if (0 < ret)
        //                        {
        //                            textRes.AppendText("Match finger succ, score=" + ret + "!\n");
        //                            return;
        //                        }
        //                        else
        //                        {
        //                            textRes.AppendText("Match finger fail, ret= " + ret + "\n");
        //                            return;
        //                        }
        //                    }
        //                }
        //        }
        //        break;

        //        default:
        //            base.DefWndProc(ref m);
        //            break;
        //    }
        //}

        private void btnImport_Click(object sender, EventArgs e)
        {

        }

        private void btCaptureBmp_Click(object sender, EventArgs e)
        {
    //        SaveFileDialog saveFileDialog1 = new SaveFileDialog();
    //        saveFileDialog1.FileName = "fingertemplate.bmp";
    //        saveFileDialog1.RestoreDirectory = true;

    //        DialogResult result = saveFileDialog1.ShowDialog();
    //        if (result == DialogResult.OK)
    //        {
    //            string fileName = saveFileDialog1.FileName.ToString();
    //            if (fileName != "" && fileName != null && picFPImg.Image != null)
    //            {
    //                //http://www.wischik.com/lu/programmer/1bpp.html
    //                Bitmap bmp = new Bitmap(picFPImg.Image.Width, picFPImg.Image.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

    //                using (Graphics g = Graphics.FromImage(bmp))
    //                {
    //                    g.DrawImage(picFPImg.Image, 0, 0, bmp.Width, bmp.Height);
                        
    //                }
    //                Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
    //                System.Drawing.Imaging.BitmapData bmpData = bmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, bmp.PixelFormat);
    //                IntPtr ptr = bmpData.Scan0;
    //                int bytes = bmpData.Stride * bmpData.Height;
    //                byte[] rgbValues = new byte[bytes];
    //                System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);
    //                Rectangle rect2 = new Rectangle(0, 0, bmp.Width, bmp.Height);

    //                Bitmap bit = new Bitmap(bmp.Width, bmp.Height, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);
    //                System.Drawing.Imaging.BitmapData bmpData2 = bit.LockBits(rect2, System.Drawing.Imaging.ImageLockMode.ReadWrite, bit.PixelFormat);
    //                IntPtr ptr2 = bmpData2.Scan0;
    //                int bytes2 = bmpData2.Stride * bmpData2.Height;
    //                byte[] rgbValues2 = new byte[bytes2];
    //                System.Runtime.InteropServices.Marshal.Copy(ptr2, rgbValues2, 0, bytes2);
    //                double colorTemp = 0;
    //                for (int i = 0; i < bmpData.Height; i++)
    //                {
    //                    for (int j = 0; j < bmpData.Width * 3; j += 3)
    //                    {
    //                        colorTemp = rgbValues[i * bmpData.Stride + j + 2] * 0.299 + rgbValues[i * bmpData.Stride + j + 1] * 0.578 + rgbValues[i * bmpData.Stride + j] * 0.114;
    //                        rgbValues2[i * bmpData2.Stride + j / 3] = (byte)colorTemp;
    //                    }
    //                }
    //                System.Runtime.InteropServices.Marshal.Copy(rgbValues2, 0, ptr2, bytes2);
    //                bmp.UnlockBits(bmpData);
    //                ColorPalette tempPalette;
    //                {
    //                    using (Bitmap tempBmp = new Bitmap(1, 1, System.Drawing.Imaging.PixelFormat.Format8bppIndexed))
    //                    {
    //                        tempPalette = tempBmp.Palette;
    //                    }
    //                    for (int i = 0; i < 256; i++)
    //                    {
    //                        tempPalette.Entries[i] = Color.FromArgb(i, i, i);
    //                    }
    //                    bit.Palette = tempPalette;
    //                }
    //                bit.UnlockBits(bmpData2);

    //                bit.Save(fileName, picFPImg.Image.RawFormat);
                    
    //                bit.Dispose();
    //            }
    //        }
        }
    }
}
