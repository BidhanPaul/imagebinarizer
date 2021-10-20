﻿using System.Drawing;
using System.IO;
using System.Text;
using Daenet.ImageBinarizerLib.Entities;
using LearningFoundation;
using SkiaSharp;

namespace Daenet.ImageBinarizerLib
{
    /// <summary>
    /// Main class for the Image Binarizer algorithm using Ipipeline
    /// </summary>
    public class ImageBinarizer : ImagePixelsDataHandler, IPipelineModule<double[,,], double[,,]>
    {
        #region Private members
        private BinarizerParams configuration;

        private int m_white = 1;
        private int m_black = 0;

        private Size? m_TargetSize;
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor that takes BinarizerParams as input to assign the binarizer configuration to the object.
        /// </summary>
        /// <param name="configuration">BinarizerParams object</param>
        public ImageBinarizer(BinarizerParams configuration)
        {
            this.configuration = configuration;
            if (this.configuration.Inverse)
            {
                this.m_white = 0;
                this.m_black = 1;
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// If you use the binarizer inside of the LearningApiPipeline, you should use this method.
        /// It takes the 3D array as input and start binarization base on provided arguments.
        /// </summary>
        /// <param name="data">This is the double data coming from unitest.</param>
        /// <param name="ctx">This defines the Interface IContext for Data descriptor</param>
        /// <returns>3D array of 1 bit element (0 or 1)</returns>
        public double[,,] Run(double[,,] data, IContext ctx)
        {
            this.configuration.CreateCode = false;
            return GetBinary(ResizeImageData(data));
        }

        /// <summary>
        /// Method to call Binarizer outside the LearningApiPipeline. 
        /// It receives the image as input and produce .txt file or .cs base on provided arguments from user.
        /// </summary>
        public void Run()
        {
            SKBitmap skBitmap = SKBitmap.Decode(this.configuration.InputImagePath);

            int imgWidth = skBitmap.Width;
            int imgHeight = skBitmap.Height;
            SKImageInfo info = new SKImageInfo(imgWidth, imgHeight, SKColorType.Rgba8888);
            this.m_TargetSize = GetTargetSizeFromConfigOrDefault(imgWidth, imgHeight);
            if (this.m_TargetSize != null)
            {
                info.Width = this.m_TargetSize.Value.Width;
                info.Height = this.m_TargetSize.Value.Height;
            }
            skBitmap = skBitmap.Resize(info, SKFilterQuality.High);

            double[,,] inputData = GetPixelsColors(skBitmap);

            double[,,] outputData = GetBinary(inputData);

            StringBuilder sb = CreateTextFromBinary(outputData);

            if (this.configuration.CreateCode) // check if code file need to be created
            {
                CodeCreator code = new CodeCreator(sb, this.configuration.OutputImagePath ?? ".\\LogoPrinter.cs");
                code.Create();
                return;
            }

            using (StreamWriter writer = File.CreateText(this.configuration.OutputImagePath))
            {
                writer.Write(sb.ToString());
            }
        }

        /// <summary>
        /// Method to call Binarizer outside the LearningApiPipeline. 
        /// It receives the image as input and return the binary string
        /// </summary>
        /// <returns>Binary data as string</returns>
        public string GetStringBinariy()
        {
            SKBitmap skBitmap = SKBitmap.Decode(this.configuration.InputImagePath);

            int imgWidth = skBitmap.Width;
            int imgHeight = skBitmap.Height;
            SKImageInfo info = new SKImageInfo(imgWidth, imgHeight, SKColorType.Rgba8888);
            this.m_TargetSize = GetTargetSizeFromConfigOrDefault(imgWidth, imgHeight);
            if (this.m_TargetSize != null)
            {
                info.Width = this.m_TargetSize.Value.Width;
                info.Height = this.m_TargetSize.Value.Height;
            }
            skBitmap = skBitmap.Resize(info, SKFilterQuality.High);

            double[,,] inputData = GetPixelsColors(skBitmap);

            double[,,] outputData = GetBinary(inputData);

            StringBuilder sb = CreateTextFromBinary(outputData);
            return sb.ToString();
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Resize the bimap with provide input. The method take 3D array of color data as input
        /// and assigns these to the bitmap to peform resizing process
        /// </summary>
        /// <param name="data">Data of bitmap for binarization</param>
        /// <returns>3D resized array for binarization</returns>
        private double[,,] ResizeImageData(double[,,] data)
        {
            Bitmap img = SetPixelsColors(data);

            this.m_TargetSize = GetTargetSizeFromConfigOrDefault(data.GetLength(0), data.GetLength(1));

            if (this.m_TargetSize != null)
                img = new Bitmap(img, this.m_TargetSize.Value);

            double[,,] resizedData = GetPixelsColors(img);

            return resizedData;
        }

        /// <summary>
        /// Get Binary array with input array double. The method take 3D array of color data as input
        /// and perform the binarization.
        /// </summary>
        /// <param name="data">Data of bitmap for binarization</param>
        /// <returns>3D binarized array</returns>
        private double[,,] GetBinary(double[,,] data)
        {

            // The average is calculated taking the parameters.
            // When no thresholds are given, they will be assigned automatically the average values.            
            CalcAverageRGBGrey(data);

            if (!this.configuration.GreyScale)
            {
                return RgbScaleBinarize(data);
            }

            return GreyScaleBinarize(data);
        }

        /// <summary>
        /// Average values calculation. The method take 3D array of color data as input 
        /// to set the threshold for Red, Green, Blue, and Grey automatically
        /// if these data are not provided by user.
        /// </summary>
        /// <param name="data">Data of bitmap for binarization</param>
        private void CalcAverageRGBGrey(double[,,] data)
        {
            int hg = data.GetLength(1);
            int wg = data.GetLength(0);

            const int constWidth = 4000;
            const int constHeight = 4000;

            //
            //divide the bitmap into supbitmap before calculating sum to avoid overflow
            double[,] sumR = new double[wg / constWidth + 1, hg / constHeight + 1];
            double[,] sumG = new double[wg / constWidth + 1, hg / constHeight + 1];
            double[,] sumB = new double[wg / constWidth + 1, hg / constHeight + 1];
            for (int i = 0; i < hg; i++)
            {
                for (int j = 0; j < wg; j++)
                {
                    sumR[j / constWidth, i / constHeight] += data[j, i, 0];
                    sumG[j / constWidth, i / constHeight] += data[j, i, 1];
                    sumB[j / constWidth, i / constHeight] += data[j, i, 2];
                }
            }
            double avgR = 0;
            double avgG = 0;
            double avgB = 0;
            for (int i = 0; i < hg / constHeight + 1; i++)
            {
                for (int j = 0; j < wg / constWidth + 1; j++)
                {
                    avgR += sumR[j, i] / (hg * wg);
                    avgG += sumG[j, i] / (hg * wg);
                    avgB += sumB[j, i] / (hg * wg);
                }
            }
            double avgGrey = 0.299 * avgR + 0.587 * avgG + 0.114 * avgB;//using the NTSC formula            

            if (this.configuration.RedThreshold < 0 || this.configuration.RedThreshold > 255)
                this.configuration.RedThreshold = (int)avgR;

            if (this.configuration.GreenThreshold < 0 || this.configuration.GreenThreshold > 255)
                this.configuration.GreenThreshold = (int)avgG;

            if (this.configuration.BlueThreshold < 0 || this.configuration.BlueThreshold > 255)
                this.configuration.BlueThreshold = (int)avgB;

            if (this.configuration.GreyThreshold < 0 || this.configuration.GreyThreshold > 255)
                this.configuration.GreyThreshold = (int)avgGrey;
        }

        /// <summary>
        /// Binarize using grey scale threshold. The method take 3D array of color data as input 
        /// and return the 3D binarized array as output.
        /// </summary>
        /// <param name="data">Data of bitmap for binarization</param>
        /// <returns>3D binarized array</returns>
        private double[,,] GreyScaleBinarize(double[,,] data)
        {
            int hg = data.GetLength(1);
            int wg = data.GetLength(0);
            double[,,] outArray = new double[hg, wg, 3];

            for (int i = 0; i < hg; i++)
            {
                for (int j = 0; j < wg; j++)
                {
                    //Compare value to Grey threshold for binarization  
                    outArray[i, j, 0] = ((0.299 * data[j, i, 0] + 0.587 * data[j, i, 1] +
                       0.114 * data[j, i, 2]) > this.configuration.GreyThreshold) ? this.m_white : this.m_black;
                }
            }

            return outArray;
        }

        /// <summary>
        /// Binarize using RGB threshold. The method take 3D array of color data as input and 
        /// return the 3D binarized array as output 
        /// </summary>
        /// <param name="data">Data of bitmap for binarization</param>
        /// <returns>3D binarized array</returns>
        private double[,,] RgbScaleBinarize(double[,,] data)
        {
            int hg = data.GetLength(1);
            int wg = data.GetLength(0);
            double[,,] outArray = new double[hg, wg, 3];

            for (int i = 0; i < hg; i++)
            {
                for (int j = 0; j < wg; j++)
                {
                    //Compare value to RGB threshold for binarization                    
                    outArray[i, j, 0] = (data[j, i, 0] > this.configuration.RedThreshold &&
                                         data[j, i, 1] > this.configuration.GreenThreshold &&
                                         data[j, i, 2] > this.configuration.BlueThreshold) ? this.m_white : this.m_black;
                }
            }

            return outArray;
        }

        /// <summary>
        /// Create string builder from output data
        /// </summary>
        /// <param name="outputData">Data after binarization</param>
        /// <returns>String builder</returns>
        private static StringBuilder CreateTextFromBinary(double[,,] outputData)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < outputData.GetLength(0); i++)
            {
                for (int j = 0; j < outputData.GetLength(1); j++)
                {
                    sb.Append(outputData[i, j, 0]);
                }
                sb.AppendLine();
            }

            return sb;
        }

        /// <summary>
        /// Get size of binarized image. The method takes the width and height of bitmap (image) 
        /// to calculate the aspect ratio. 
        /// Base on this ratio, if user gives only width or height as custom configuration for binarized image, 
        /// the other value will be calculated automatically.
        /// The width of the logo if not specified the customization will be 70 by default when the width is larger than 70,
        /// for fitting to console window, as user chooses to create code file. 
        /// </summary>
        /// <param name="imageOriginalWidth">Bitmap width</param>
        /// <param name="imageOriginalHeight">Bitmap Height</param>
        /// <returns>Object contains the size for resizing image, null if bitmap is null or no resizing process required</returns>
        private Size? GetTargetSizeFromConfigOrDefault(int imageOriginalWidth, int imageOriginalHeight)
        {
            if (imageOriginalWidth == 0 || imageOriginalHeight == 0)
                return null;

            if (this.configuration.ImageHeight > 0 && this.configuration.ImageWidth > 0)
                return new Size(this.configuration.ImageWidth, this.configuration.ImageHeight);

            double ratio = (double)imageOriginalHeight / imageOriginalWidth;

            if (this.configuration.ImageHeight > 0)
                return new Size((int)(this.configuration.ImageHeight / ratio), this.configuration.ImageHeight);

            if (this.configuration.ImageWidth > 0)
                return new Size(this.configuration.ImageWidth, (int)(this.configuration.ImageWidth * ratio));

            int logoWidth = 70;
            if (this.configuration.CreateCode && imageOriginalWidth > logoWidth)
                return new Size(logoWidth, (int)(logoWidth * ratio));

            return null;
        }
        #endregion
    }
}
