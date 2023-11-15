namespace OpenVoice
{
    internal class Helper
    {
        // Replaces all occurences of Key with Value
        // IF KEY = #ffffff00 it will be treated as any
        public static unsafe Bitmap ModulateBitmap(Bitmap bitmap, Color Key, Color Value)
        {
            if (bitmap.PixelFormat != System.Drawing.Imaging.PixelFormat.Format32bppArgb) { return null; }
            var bitmapSize = bitmap.Width * bitmap.Height;
            var bitmapRect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            var bitmapData = bitmap.LockBits(bitmapRect, System.Drawing.Imaging.ImageLockMode.ReadWrite, bitmap.PixelFormat);
            int* ptr = (int*)bitmapData.Scan0;
            for (int i = 0; i < bitmapSize; i++)
            { if (Key.A > 0 && ptr[i] == Key.ToArgb()) ptr[i] = Value.ToArgb(); else if (Key.A == 0) ptr[i] = Value.ToArgb(); }
            bitmap.UnlockBits(bitmapData);
            return bitmap;
        }
    }
}
