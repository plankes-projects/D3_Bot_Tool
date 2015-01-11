using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;

namespace D3_Bot_Tool
{
    class LockedFastImage
    {
        private Bitmap image;
        private byte[] rgbValues;
        private System.Drawing.Imaging.BitmapData bmpData;

        private IntPtr ptr;
        private int bytes;

        public LockedFastImage(Bitmap image)
        {
            this.image = image;
            Rectangle rect = new Rectangle(0, 0, image.Width, image.Height);
            bmpData = image.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, image.PixelFormat);

            ptr = bmpData.Scan0;
            bytes = Math.Abs(bmpData.Stride) * image.Height;
            rgbValues = new byte[bytes];
            System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);
        }

        ~LockedFastImage()
        {
            Dispose();
        }

        public void Dispose()
        {
            // Try to unlock the bits. Who cares if it dont work...
            try
            {
                image.UnlockBits(bmpData);
                image.Dispose();
            }
            catch { }
        }

        /// <summary>
        /// Returns or sets a pixel of the image. 
        /// </summary>
        /// <param name="x">x parameter of the pixel</param>
        /// <param name="y">y parameter of the pixel</param>
        public Color this[int x, int y]
        {
            get 
            {
                int index = (x + (y * image.Width)) * 4;
                return Color.FromArgb(rgbValues[index + 3], rgbValues[index + 2], rgbValues[index + 1], rgbValues[index]);
            }
            
            set 
            {
                int index = (x + (y * image.Width)) * 4;
                rgbValues[index]     = value.B;
                rgbValues[index + 1] = value.G;
                rgbValues[index + 2] = value.R;
                rgbValues[index + 3] = value.A;
            }
        }

        public Color this[Point p]
        {
            get
            {
                return this[p.X, p.Y];
            }

            set
            {
                this[p.X, p.Y] = value; 
            }
        }

        /// <summary>
        /// Width of the image. 
        /// </summary>
        public int Width
        {
            get
            {
                return image.Width;
            }
        }

        /// <summary>
        /// Height of the image. 
        /// </summary>
        public int Height
        {
            get
            {
                return image.Height;
            }
        }

        /// <summary>
        /// Returns the modified Bitmap. 
        /// </summary>
        public Bitmap asBitmap()
        {
            System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, ptr, bytes);
            return image;
        }
    }

    class ImageChecker
    {

        private LockedFastImage big_image;
        private LockedFastImage small_image; 
        /// <summary>
        /// The time needed for last operation.
        /// </summary>
        public TimeSpan time_needed = new TimeSpan();

        /// <summary>
        /// Constructor of the ImageChecker
        /// </summary>
        /// <param name="big_image">The image containing the small image.</param>
        /// <param name="small_image">The image located in the big image.</param>
        public ImageChecker(Bitmap big_image, Bitmap small_image)
        {
            this.big_image = new LockedFastImage(big_image);
            this.small_image = new LockedFastImage(small_image);
        }

        /// <summary>
        /// Constructor of the ImageChecker
        /// </summary>
        /// <param name="big_image">The image containing the small image.</param>
        /// <param name="small_image">The image located in the big image.</param>
        public ImageChecker(LockedFastImage big_image, Bitmap small_image)
        {
            this.big_image = big_image;
            this.small_image = new LockedFastImage(small_image);
        }

        /// <summary>
        /// Returns the location of the small image in the big image. Returns CHECKFAILED if not found.
        /// </summary>
        /// <param name="x_speedUp">speeding up at x achsis.</param>
        /// <param name="y_speedUp">speeding up at y achsis.</param>
        /// <param name="begin_percent_x">Reduces the search rect. 0 - 100</param>
        /// <param name="end_percent_x">Reduces the search rect. 0 - 100</param>
        /// <param name="begin_percent_x">Reduces the search rect. 0 - 100</param>
        /// <param name="end_percent_y">Reduces the search rect. 0 - 100</param>
        public List<Point> bigContainsSmall(int number_of_results = 1, int x_speedUp = 1, int y_speedUp = 1, int begin_percent_x = 0, int end_percent_x = 100, int begin_percent_y = 0, int end_percent_y = 100, int variance = 0)
        {
            /*
             * SPEEDUP PARAMETER
             * It might be enough to check each second or third pixel in the small picture.
             * However... In most cases it would be enough to check 4 pixels of the small image for diablo porposes.
             * */

            /*
             * BEGIN, END PARAMETER
             * In most cases we know where the image is located, for this we have the begin and end paramenters.
             * */

            DateTime begin = DateTime.Now;

            List<Point> ret = new List<Point>();

            if (variance < 0) variance = 0;

            if (x_speedUp < 1) x_speedUp = 1;
            if (y_speedUp < 1) y_speedUp = 1;
            if (begin_percent_x < 0 || begin_percent_x > 100) begin_percent_x = 0;
            if (begin_percent_y < 0 || begin_percent_y > 100) begin_percent_y = 0;
            if (end_percent_x   < 0 || end_percent_x   > 100) end_percent_x   = 100;
            if (end_percent_y   < 0 || end_percent_y   > 100) end_percent_y   = 100;

            int x_start = (int)((double)big_image.Width  * ((double)begin_percent_x / 100.0));
            int x_end   = (int)((double)big_image.Width  * ((double)end_percent_x   / 100.0));
            int y_start = (int)((double)big_image.Height * ((double)begin_percent_y / 100.0));
            int y_end   = (int)((double)big_image.Height * ((double)end_percent_y   / 100.0));

            /*
             * We cant speed up the big picture, because then we have to check pixels in the small picture equal to the speeded up size 
             * for each pixel in the big picture.
             * Would give no speed improvement.
             * */

            //+ 1 because first pixel is in picture. - small because image have to be fully in the other image
            for (int x = x_start; x < x_end - small_image.Width + 1; x++)
                for (int y = y_start; y < y_end - small_image.Height + 1; y++)
                {
                    //now we check if all pixels matches
                    for (int sx = 0; sx < small_image.Width; sx += x_speedUp)
                        for (int sy = 0; sy < small_image.Height; sy += y_speedUp)
                        {
                            if (variance == 0)
                            {
                                if (small_image[sx, sy] != big_image[x + sx, y + sy])
                                    goto CheckFailed;
                            }
                            else
                            {
                                if (!equalsWithFail(small_image[sx, sy], big_image[x + sx, y + sy], variance))
                                    goto CheckFailed;
                            }
                        }

                    //check ok
                    ret.Add(new Point(x, y));
                    if (ret.Count >= number_of_results)
                    {
                        time_needed = DateTime.Now - begin;
                        return ret;
                    }
                        
                    CheckFailed: ;
                }

            time_needed = DateTime.Now - begin;
            return ret;
        }

        static public bool equalsWithFail(Color a, Color b, int variance)
        {
            if (a.G - variance < b.G && b.G < a.G + variance)
                if (a.R - variance < b.R && b.R < a.R + variance)
                    if (a.B - variance < b.B && b.B < a.B + variance)
                        if (a.A - variance < b.A && b.A < a.A + variance)
                            return true;

            return false;
        }
    }
}
    