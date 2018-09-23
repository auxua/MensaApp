using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;

using Xamarin.Forms;

using ObjCRuntime;

using System.Runtime.InteropServices;

[assembly: Dependency(typeof(MensaApp.iOS.ScreenDetector))]
namespace MensaApp.iOS
{
    public class ScreenDetector : IScreenDetection
    {
        public string GetDeviceName()
        {
            string property = "hw.machine";
            var pLen = Marshal.AllocHGlobal(sizeof(int));
            sysctlbyname(property, IntPtr.Zero, pLen, IntPtr.Zero, 0);
            var length = Marshal.ReadInt32(pLen);
            var pStr = Marshal.AllocHGlobal(length);
            sysctlbyname(property, pStr, pLen, IntPtr.Zero, 0);
            return Marshal.PtrToStringAnsi(pStr);
        }

        [DllImport(Constants.SystemLibrary)]
        internal static extern int sysctlbyname(
        [MarshalAs(UnmanagedType.LPStr)] string property,
        IntPtr output,
        IntPtr oldLen,
        IntPtr newp,
        uint newlen);
    }

    
}