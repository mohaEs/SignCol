using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Kinect;
using System.Windows;
using System.IO;

using System.Runtime.InteropServices;

namespace DbModel.Extensions
{
    public class ProcessingsAndRendering
    {


        public int color_Width = 1920;
        public int color_Height = 1080;

        int IR_Width = 512;
        int IR_Height = 424;

        int MaximumFramesNumbers_Capturing=500;
        public bool isCapturing = false;
        public int Counter_CapturingFrame;
        public int MaximumPossibleFrameNumners_AfterInitializedCaputing;


        #region Memory Allocation for capturing

        public Array[] InfraredPixels_Array;
        public Array[] DepthPixels_Array;
        public Array[] BodyIndexPixels_Array;
        public Array[] ColorBodyIndexPixels_Array;
        public string[] stringCapturingTimes_Array;
        public Array[] ColorPixels_Array;
        public string[] Bodies_Array;

        public void InitializeCapturing()
        {

            ColorPixels_Array = new Array[MaximumFramesNumbers_Capturing];
            InfraredPixels_Array = new Array[MaximumFramesNumbers_Capturing];
            DepthPixels_Array = new Array[MaximumFramesNumbers_Capturing];
            BodyIndexPixels_Array = new Array[MaximumFramesNumbers_Capturing];
            ColorBodyIndexPixels_Array = new Array[MaximumFramesNumbers_Capturing];
            stringCapturingTimes_Array = new string[MaximumFramesNumbers_Capturing];
            Bodies_Array = new string[MaximumFramesNumbers_Capturing];


            for (int i = 0; i < MaximumFramesNumbers_Capturing; ++i)
            {
                try
                {
                ColorPixels_Array[i] = new int[color_Width * color_Height];
                InfraredPixels_Array[i] = new int[IR_Width * IR_Height];
                DepthPixels_Array[i] = new int[IR_Width * IR_Height];
                BodyIndexPixels_Array[i] = new int[IR_Width * IR_Height];
                ColorBodyIndexPixels_Array[i] = new int[IR_Width * IR_Height];
                }
                catch
                {

                    MaximumPossibleFrameNumners_AfterInitializedCaputing = i - 1;
                    break;
                }
            }


        }


        #endregion

        #region Color
        /// Bitmap to display
        public WriteableBitmap colorBitmap = null;

        /// Description (width, height, etc) 
        private FrameDescription colorFrameDescription = null;

        public void InitializeColor(KinectSensor kinectSensorDevice)
        {
            // create the colorFrameDescription from the ColorFrameSource using Bgra format
            colorFrameDescription = kinectSensorDevice.ColorFrameSource.CreateFrameDescription(ColorImageFormat.Bgra);

            // create the bitmap to display
            this.colorBitmap = new WriteableBitmap(colorFrameDescription.Width, colorFrameDescription.Height, 96.0, 96.0, PixelFormats.Bgr32, null);

        }

        public void ProcessColor(ColorFrame colorFrame)
        {
            FrameDescription colorFrameDescription = colorFrame.FrameDescription;

            using (KinectBuffer colorBuffer = colorFrame.LockRawImageBuffer())
            {
                this.colorBitmap.Lock();
                colorFrame.CopyConvertedFrameDataToIntPtr(
                        this.colorBitmap.BackBuffer,
                        (uint)(color_Width * color_Height * 4),
                        ColorImageFormat.Bgra);

                this.colorBitmap.AddDirtyRect(new Int32Rect(0, 0, color_Width,
                    color_Height));

                this.colorBitmap.Unlock();

                if (isCapturing && Counter_CapturingFrame < MaximumPossibleFrameNumners_AfterInitializedCaputing)
                {
                    colorBitmap.CopyPixels(ColorPixels_Array[Counter_CapturingFrame], color_Width * 4, 0);
                }
            }

        }

        #endregion

        #region IR

        /// Maximum value (as a float) that can be returned by the InfraredFrame
        private const float InfraredSourceValueMaximum = (float)ushort.MaxValue;

        /// The value by which the infrared source data will be scaled
        private const float InfraredSourceScale = 0.75f;

        /// Smallest value to display when the infrared data is normalized
        private const float InfraredOutputValueMinimum = 0.01f;

        /// Largest value to display when the infrared data is normalized
        private const float InfraredOutputValueMaximum = 1.0f;

        /// Bitmap to display
        public WriteableBitmap infraredBitmap = null;

        /// Description (width, height, etc) 
        private FrameDescription infraredFrameDescription = null;

        public void InitializeIR(KinectSensor kinectSensorDevice)
        {
            // get FrameDescription from InfraredFrameSource
            this.infraredFrameDescription = kinectSensorDevice.InfraredFrameSource.FrameDescription;

            // create the bitmap to display
            this.infraredBitmap = new WriteableBitmap(IR_Width, IR_Height, 96.0, 96.0, PixelFormats.Gray32Float, null);
        }

        public void ProcessIR(InfraredFrame infraredFrame)
        {
            // the fastest way to process the infrared frame data is to directly access 
            // the underlying buffer
            using (Microsoft.Kinect.KinectBuffer infraredBuffer = infraredFrame.LockImageBuffer())
            {

                this.ProcessInfraredFrameData(infraredBuffer.UnderlyingBuffer, infraredBuffer.Size);

            }

        }

        /// <summary>
        /// Directly accesses the underlying image buffer of the InfraredFrame to 
        /// create a displayable bitmap.
        /// This function requires the /unsafe compiler option as we make use of direct
        /// access to the native memory pointed to by the infraredFrameData pointer.
        /// </summary>
        /// <param name="infraredFrameData">Pointer to the InfraredFrame image data</param>
        /// <param name="infraredFrameDataSize">Size of the InfraredFrame image data</param>
        private unsafe void ProcessInfraredFrameData(IntPtr infraredFrameData, uint infraredFrameDataSize)
        {
            // infrared frame data is a 16 bit value
            ushort* frameData = (ushort*)infraredFrameData;

            // lock the target bitmap
            this.infraredBitmap.Lock();

            // get the pointer to the bitmap's back buffer
            float* backBuffer = (float*)this.infraredBitmap.BackBuffer;

            // process the infrared data
            for (int i = 0; i < (int)(infraredFrameDataSize / this.infraredFrameDescription.BytesPerPixel); ++i)
            {
                // since we are displaying the image as a normalized grey scale image, we need to convert from
                // the ushort data (as provided by the InfraredFrame) to a value from [InfraredOutputValueMinimum, InfraredOutputValueMaximum]
                backBuffer[i] = Math.Min(InfraredOutputValueMaximum, (((float)frameData[i] / InfraredSourceValueMaximum * InfraredSourceScale) * (1.0f - InfraredOutputValueMinimum)) + InfraredOutputValueMinimum);
            }

            // mark the entire bitmap as needing to be drawn
            this.infraredBitmap.AddDirtyRect(new Int32Rect(0, 0, IR_Width, IR_Height));

            // unlock the bitmap 
            this.infraredBitmap.Unlock();

            if (isCapturing && Counter_CapturingFrame < MaximumPossibleFrameNumners_AfterInitializedCaputing)
            {
                infraredBitmap.CopyPixels(InfraredPixels_Array[Counter_CapturingFrame], IR_Width * 4, 0);
            }

        }


        #endregion

        #region coordinateMapping

        /// Coordinate mapper to map one type of point to another
        public CoordinateMapper coordinateMapper = null;

        public void initializeCoordinateMapper(KinectSensor kinectSensorDevice)
        {
            // get the coordinate mapper
            coordinateMapper = kinectSensorDevice.CoordinateMapper;
        }

        /// Moha: this section is inspired from the LightBuzz.Vitruvius source codes
        /// Kinect DPI.
        public static readonly double DPI = 96.0;

        /// Default format.
        public static readonly PixelFormat FORMAT = PixelFormats.Bgr32;

        /// Bytes per pixel.
        public static readonly int BYTES_PER_PIXEL = (FORMAT.BitsPerPixel + 7) / 8;

        /// The depth values.
        ushort[] _depthData = null;

        /// The body index values.
        byte[] _bodyData = null;

        /// The RGB pixel values.
        byte[] _colorData = null;

        /// The RGB pixel values used for the background removal (green-screen) effect.
        byte[] _displayPixels_onDepthSpace = null;
        byte[] _displayPixels_onColorSpace = null;

        /// The points used for the background removal (green-screen) effect.
        ColorSpacePoint[] _colorPoints = null;
        DepthSpacePoint[] _depthPoints = null;

        /// Returns the actual bitmap.
        public WriteableBitmap imageSourceColorBodies_onDepthSpace { get; protected set; }
        public WriteableBitmap imageSourceColorBodies_onColorSpace { get; protected set; }


        /// Moha: this function is inspired from the LightBuzz.Vitruvius source codes
        /// <summary>
        /// Updates the bitmap with new frame data.
        /// </summary>
        /// <param name="depthFrame">The specified depth frame.</param>
        /// <param name="colorFrame">The specified color frame.</param>
        /// <param name="bodyIndexFrame">The specified body index frame.</param>
        public void ProcessCoordinatingBodies(ColorFrame colorFrame, DepthFrame depthFrame, BodyIndexFrame bodyIndexFrame)
        {
            int colorWidth = colorFrame.FrameDescription.Width;
            int colorHeight = colorFrame.FrameDescription.Height;

            int depthWidth = depthFrame.FrameDescription.Width;
            int depthHeight = depthFrame.FrameDescription.Height;

            int bodyIndexWidth = bodyIndexFrame.FrameDescription.Width;
            int bodyIndexHeight = bodyIndexFrame.FrameDescription.Height;

            if (_displayPixels_onDepthSpace == null)
            {
                _depthData = new ushort[depthWidth * depthHeight];
                _bodyData = new byte[depthWidth * depthHeight];
                _colorData = new byte[colorWidth * colorHeight * BYTES_PER_PIXEL];

                _displayPixels_onDepthSpace = new byte[depthWidth * depthHeight * BYTES_PER_PIXEL];
                _displayPixels_onColorSpace = new byte[colorWidth * colorHeight * BYTES_PER_PIXEL];

                _colorPoints = new ColorSpacePoint[depthWidth * depthHeight];
                _depthPoints = new DepthSpacePoint[colorWidth * colorHeight];

                imageSourceColorBodies_onDepthSpace = new WriteableBitmap(depthWidth, depthHeight, DPI, DPI, FORMAT, null);
                imageSourceColorBodies_onColorSpace = new WriteableBitmap(colorWidth, colorHeight, DPI, DPI, FORMAT, null);
            }

            ///  ---------------
            depthFrame.CopyFrameDataToArray(_depthData);
            if (colorFrame.RawColorImageFormat == ColorImageFormat.Bgra)
            {
                colorFrame.CopyRawFrameDataToArray(_colorData);
            }
            else
            {
                colorFrame.CopyConvertedFrameDataToArray(_colorData, ColorImageFormat.Bgra);
            }
            bodyIndexFrame.CopyFrameDataToArray(_bodyData);

            /// --------------

            coordinateMapper.MapDepthFrameToColorSpace(_depthData, _colorPoints);

            /// Moha: Since sweeping on the following for loop is so massive, 
            /// this makes the program to work slowly with lower frame rates
            /// we decide to just save color frames and use a simple background (which can be easily removed from bodies later)
            /// in our gesture capturing if we want to have Mega pixel resolution RGB information
            /// 
            /// If you want to use it, just uncomment the lines labeld by AA:
            /// AA:
            //  coordinateMapper.MapColorFrameToDepthSpace(_depthData, _depthPoints);


            /// -------------

            Array.Clear(_displayPixels_onDepthSpace, 0, _displayPixels_onDepthSpace.Length);
            /// AA:
            //Array.Clear(_displayPixels_onColorSpace, 0, _displayPixels_onColorSpace.Length);

            for (int y = 0; y < depthHeight; ++y)
            {
                for (int x = 0; x < depthWidth; ++x)
                {
                    int depthIndex = (y * depthWidth) + x;

                    byte player = _bodyData[depthIndex];

                    if (player != 0xff)
                    {
                        ColorSpacePoint colorPoint = _colorPoints[depthIndex];

                        int colorX = (int)Math.Floor(colorPoint.X + 0.5);
                        int colorY = (int)Math.Floor(colorPoint.Y + 0.5);

                        if ((colorX >= 0) && (colorX < colorWidth) && (colorY >= 0) && (colorY < colorHeight))
                        {
                            int colorIndex = ((colorY * colorWidth) + colorX) * BYTES_PER_PIXEL;
                            int displayIndex = depthIndex * BYTES_PER_PIXEL;

                            _displayPixels_onDepthSpace[displayIndex + 0] = _colorData[colorIndex];
                            _displayPixels_onDepthSpace[displayIndex + 1] = _colorData[colorIndex + 1];
                            _displayPixels_onDepthSpace[displayIndex + 2] = _colorData[colorIndex + 2];
                            _displayPixels_onDepthSpace[displayIndex + 3] = 0xff;
                        }
                    }
                }
            }

            /// Moha: Since sweeping on the following for loop is so massive, 
            /// this makes the program to work slowly with lower frame rates
            /// we decide to just save color frames and use a simple background (which can be easily removed from bodies later)
            /// in our gesture capturing if we want to have Mega pixel resolution RGB information
            /// AA:
            //for (int colorIndex = 0; colorIndex < _depthPoints.Length; ++colorIndex)
            //{
            //DepthSpacePoint depthPoint = _depthPoints[colorIndex];

            //if (!float.IsNegativeInfinity(depthPoint.X) && !float.IsNegativeInfinity(depthPoint.Y))
            //{
            //    int depthX = (int)(depthPoint.X + 0.5f);
            //    int depthY = (int)(depthPoint.Y + 0.5f);

            //    if ((depthX >= 0) && (depthX < depthWidth) && (depthY >= 0) && (depthY < depthHeight))
            //    {
            //        int depthIndex = (depthY * depthWidth) + depthX;
            //        byte player = _bodyData[depthIndex];

            //        // Identify whether the point belongs to a player
            //        if (player != 0xff)
            //        {
            //            int sourceIndex = colorIndex * BYTES_PER_PIXEL;

            //            _displayPixels_onColorSpace[sourceIndex] = _colorData[sourceIndex++];    // B
            //            _displayPixels_onColorSpace[sourceIndex] = _colorData[sourceIndex++];    // G
            //            _displayPixels_onColorSpace[sourceIndex] = _colorData[sourceIndex++];    // R
            //            _displayPixels_onColorSpace[sourceIndex] = 0xff;                         // A
            //        }
            //    }
            //}
            //}

            imageSourceColorBodies_onDepthSpace.Lock();
            /// AA:
            //imageSourceColorBodies_onColorSpace.Lock();

            Marshal.Copy(_displayPixels_onDepthSpace, 0, imageSourceColorBodies_onDepthSpace.BackBuffer, _displayPixels_onDepthSpace.Length);
            /// AA:
            // Marshal.Copy(_displayPixels_onColorSpace, 0, imageSourceColorBodies_onColorSpace.BackBuffer, _displayPixels_onColorSpace.Length);

            imageSourceColorBodies_onDepthSpace.AddDirtyRect(new Int32Rect(0, 0, depthWidth, depthHeight));
            /// AA:
            //imageSourceColorBodies_onColorSpace.AddDirtyRect(new Int32Rect(0, 0, colorWidth, colorHeight));

            imageSourceColorBodies_onDepthSpace.Unlock();
            /// AA:
            //imageSourceColorBodies_onColorSpace.Unlock();

            if (isCapturing && Counter_CapturingFrame < MaximumPossibleFrameNumners_AfterInitializedCaputing)
            {
                imageSourceColorBodies_onDepthSpace.CopyPixels(ColorBodyIndexPixels_Array[Counter_CapturingFrame], IR_Width * 4, 0);
            }


        }

        #endregion





        #region Depth 
        /// Bitmap to display
        public WriteableBitmap depthBitmap = null;
        /// Description (width, height, etc) 
        private FrameDescription depthFrameDescription = null;
        /// Intermediate storage for frame data converted 
        private byte[] depthPixels = null;
        /// Map depth range to byte range
        private const int MapDepthToByte = 8000 / 256;


        public void InitializeDepth(KinectSensor kinectSensorDevice)
        {
            // create the FrameDescriptions 
            depthFrameDescription = kinectSensorDevice.DepthFrameSource.FrameDescription;
            // allocate space to put the pixels being received and converted
            this.depthPixels = new byte[this.depthFrameDescription.Width * this.depthFrameDescription.Height];
            this.depthBitmap = new WriteableBitmap(this.depthFrameDescription.Width, this.depthFrameDescription.Height, 96.0, 96.0, PixelFormats.Gray8, null);

        }

        public void ProcessDepth(DepthFrame RecievedDepthFrame)
        {

            if (RecievedDepthFrame != null)
            {
                // the fastest way to process the body index data is to directly access 
                // the underlying buffer
                using (Microsoft.Kinect.KinectBuffer depthBuffer = RecievedDepthFrame.LockImageBuffer())
                {
                    // verify data and write the color data to the display bitmap
                    if (((this.depthFrameDescription.Width * this.depthFrameDescription.Height) == (depthBuffer.Size / this.depthFrameDescription.BytesPerPixel)) &&
                        (this.depthFrameDescription.Width == this.depthBitmap.PixelWidth) && (this.depthFrameDescription.Height == this.depthBitmap.PixelHeight))
                    {
                        // Note: In order to see the full range of depth (including the less reliable far field depth)
                        // we are setting maxDepth to the extreme potential depth threshold
                        ushort maxDepth = ushort.MaxValue;

                        // If you wish to filter by reliable depth distance, uncomment the following line:
                        //// maxDepth = depthFrame.DepthMaxReliableDistance

                        this.ProcessDepthFrameData(depthBuffer.UnderlyingBuffer, depthBuffer.Size, RecievedDepthFrame.DepthMinReliableDistance, maxDepth);

                    }
                }
                this.RenderDepthPixels();
                //ImageBoxDepth.Source = depthBitmap;
            } //end if Depth

        }

        /// <summary>
        /// Directly accesses the underlying image buffer of the DepthFrame to 
        /// create a displayable bitmap.
        /// This function requires the /unsafe compiler option as we make use of direct
        /// access to the native memory pointed to by the depthFrameData pointer.
        /// </summary>
        /// <param name="depthFrameData">Pointer to the DepthFrame image data</param>
        /// <param name="depthFrameDataSize">Size of the DepthFrame image data</param>
        /// <param name="minDepth">The minimum reliable depth value for the frame</param>
        /// <param name="maxDepth">The maximum reliable depth value for the frame</param>
        public unsafe void ProcessDepthFrameData(IntPtr depthFrameData, uint depthFrameDataSize, ushort minDepth, ushort maxDepth)
        {
            // depth frame data is a 16 bit value
            ushort* frameData = (ushort*)depthFrameData;

            // convert depth to a visual representation
            for (int i = 0; i < (int)(depthFrameDataSize / this.depthFrameDescription.BytesPerPixel); ++i)
            {
                // Get the depth for this pixel
                ushort depth = frameData[i];

                // To convert to a byte, we're mapping the depth value to the byte range.
                // Values outside the reliable depth range are mapped to 0 (black).
                this.depthPixels[i] = (byte)(depth >= minDepth && depth <= maxDepth ? (depth / MapDepthToByte) : 0);
            }
        }

        /// Renders color pixels into the writeableBitmap.
        public void RenderDepthPixels()
        {
            this.depthBitmap.WritePixels(
                new Int32Rect(0, 0, this.depthBitmap.PixelWidth, this.depthBitmap.PixelHeight),
                this.depthPixels,
                this.depthBitmap.PixelWidth,
                0);


            if (isCapturing && Counter_CapturingFrame < MaximumPossibleFrameNumners_AfterInitializedCaputing)
            {
                depthBitmap.CopyPixels(DepthPixels_Array[Counter_CapturingFrame], IR_Width * 4, 0);
            }

        }



        #endregion

        #region bodyIndex
        private const int BytesPerPixel = 4;
        /// Bitmap to display 
        public WriteableBitmap bodyIndexBitmap = null;

        /// Description (width, height, etc) 
        private FrameDescription bodyIndexFrameDescription = null;

        /// Intermediate storage for frame data converted to color
        private uint[] bodyIndexPixels = null;

        /// Collection of colors to be used to display the BodyIndexFrame data.
        private static readonly uint[] BodyColor =
        {
            0x0000FF00,
            0x00FF0000,
            0xFFFF4000,
            0x40FFFF00,
            0xFF40FF00,
            0xFF808000,
        };


        public void InitializeBodyIndex(KinectSensor kinectSensorDevice)
        {

            // create the FrameDescriptions 
            bodyIndexFrameDescription = kinectSensorDevice.BodyIndexFrameSource.FrameDescription;
            // allocate space to put the pixels being received and converted
            this.bodyIndexPixels = new uint[this.bodyIndexFrameDescription.Width * this.bodyIndexFrameDescription.Height];
            this.bodyIndexBitmap = new WriteableBitmap(this.bodyIndexFrameDescription.Width, this.bodyIndexFrameDescription.Height, 96.0, 96.0, PixelFormats.Bgr32, null);
        }

        public void ProcessBodyIndex(BodyIndexFrame RecievedBodyIndexFrame)
        {

            if (RecievedBodyIndexFrame != null)
            {
                // the fastest way to process the body index data is to directly access 
                // the underlying buffer
                using (Microsoft.Kinect.KinectBuffer bodyIndexBuffer = RecievedBodyIndexFrame.LockImageBuffer())
                {
                    // verify data and write the color data to the display bitmap
                    if (((this.bodyIndexFrameDescription.Width * this.bodyIndexFrameDescription.Height) == bodyIndexBuffer.Size) &&
                        (this.bodyIndexFrameDescription.Width == this.bodyIndexBitmap.PixelWidth) && (this.bodyIndexFrameDescription.Height == this.bodyIndexBitmap.PixelHeight))
                    {
                        this.ProcessBodyIndexFrameData(bodyIndexBuffer.UnderlyingBuffer, bodyIndexBuffer.Size);
                    }
                }
                this.RenderBodyIndexPixels();
                // ImageBoxBodyIndex.Source = bodyIndexBitmap;
            } // end if body index

        }

        /// <summary>
        /// Directly accesses the underlying image buffer of the BodyIndexFrame to 
        /// create a displayable bitmap.
        /// This function requires the /unsafe compiler option as we make use of direct
        /// access to the native memory pointed to by the bodyIndexFrameData pointer.
        /// </summary>
        /// <param name="bodyIndexFrameData">Pointer to the BodyIndexFrame image data</param>
        /// <param name="bodyIndexFrameDataSize">Size of the BodyIndexFrame image data</param>
        public unsafe void ProcessBodyIndexFrameData(IntPtr bodyIndexFrameData, uint bodyIndexFrameDataSize)
        {
            byte* frameData = (byte*)bodyIndexFrameData;

            // convert body index to a visual representation
            for (int i = 0; i < (int)bodyIndexFrameDataSize; ++i)
            {
                // the BodyColor array has been sized to match
                // BodyFrameSource.BodyCount
                if (frameData[i] < BodyColor.Length)
                {
                    // this pixel is part of a player,
                    // display the appropriate color
                    this.bodyIndexPixels[i] = BodyColor[frameData[i]];
                    /// Moha: mapping the pixel to color space
                    /// 

                }
                else
                {

                    // this pixel is not part of a player
                    // display black
                    this.bodyIndexPixels[i] = 0x00000000;
                }
            }
        }

        public void RenderBodyIndexPixels()
        {
            this.bodyIndexBitmap.WritePixels(
                new Int32Rect(0, 0, this.bodyIndexBitmap.PixelWidth, this.bodyIndexBitmap.PixelHeight),
                this.bodyIndexPixels,
                this.bodyIndexBitmap.PixelWidth * (int)BytesPerPixel,
                0);

            if (isCapturing && Counter_CapturingFrame < MaximumPossibleFrameNumners_AfterInitializedCaputing)
            {
                bodyIndexBitmap.CopyPixels(BodyIndexPixels_Array[Counter_CapturingFrame], IR_Width * 4, 0);
            }

        }


        #endregion



        #region Body

        /// Radius of drawn hand circles
        private const double HandSize = 30;

        /// Thickness of drawn joint lines
        private const double JointThickness = 3;

        /// Thickness of clip edge rectangles
        private const double ClipBoundsThickness = 10;

        /// Constant for clamping Z values of camera space points from being negative
        private const float InferredZPositionClamp = 0.1f;

        /// Brush used for drawing hands that are currently tracked as closed
        private readonly Brush handClosedBrush = new SolidColorBrush(Color.FromArgb(128, 255, 0, 0));

        /// Brush used for drawing hands that are currently tracked as opened
        private readonly Brush handOpenBrush = new SolidColorBrush(Color.FromArgb(128, 0, 255, 0));

        /// Brush used for drawing hands that are currently tracked as in lasso (pointer) position
        private readonly Brush handLassoBrush = new SolidColorBrush(Color.FromArgb(128, 0, 0, 255));

        /// Brush used for drawing joints that are currently tracked
        private readonly Brush trackedJointBrush = new SolidColorBrush(Color.FromArgb(255, 68, 192, 68));

        /// Brush used for drawing joints that are currently inferred
        private readonly Brush inferredJointBrush = Brushes.Yellow;

        /// Pen used for drawing bones that are currently inferred
        private readonly Pen inferredBonePen = new Pen(Brushes.Gray, 1);

        /// Drawing group for body rendering output on depth space
        private DrawingGroup drawingGroup_onDepth;

        /// Drawing image that we will display on depth space
        public DrawingImage imageSourceBody_onDepth;

        /// Drawing group for body rendering output on color space
        private DrawingGroup drawingGroup_onColor;

        /// Drawing image that we will display on color space
        public DrawingImage imageSourceBody_onColor;

        /// Array for the bodies
        private Body[] bodies = null;

        /// definition of bones
        private List<Tuple<JointType, JointType>> bones;

        /// Width of display (depth space)
        private int displayWidth_depth;

        /// Height of display (depth space)
        private int displayHeight_depth;

        /// Width of display (color space)
        private int displayWidth_color;

        /// Height of display (color space)
        private int displayHeight_color;

        /// List of colors for each body tracked
        private List<Pen> bodyColors;



        public void InitializeBody(KinectSensor kinectSensorDevice)
        {

            // get the depth (display) extents
            FrameDescription frameDescription_depth = kinectSensorDevice.DepthFrameSource.FrameDescription;

            // get size of joint space
            displayWidth_depth = frameDescription_depth.Width;
            displayHeight_depth = frameDescription_depth.Height;

            // get the color (display) extents
            FrameDescription frameDescription_color = kinectSensorDevice.ColorFrameSource.FrameDescription;

            // get size of joint space
            displayWidth_color = frameDescription_color.Width;
            displayHeight_color = frameDescription_color.Height;


            // a bone defined as a line between two joints
            this.bones = new List<Tuple<JointType, JointType>>();

            // Torso
            this.bones.Add(new Tuple<JointType, JointType>(JointType.Head, JointType.Neck));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.Neck, JointType.SpineShoulder));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.SpineShoulder, JointType.SpineMid));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.SpineMid, JointType.SpineBase));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.SpineShoulder, JointType.ShoulderRight));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.SpineShoulder, JointType.ShoulderLeft));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.SpineBase, JointType.HipRight));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.SpineBase, JointType.HipLeft));

            // Right Arm
            this.bones.Add(new Tuple<JointType, JointType>(JointType.ShoulderRight, JointType.ElbowRight));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.ElbowRight, JointType.WristRight));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.WristRight, JointType.HandRight));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.HandRight, JointType.HandTipRight));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.WristRight, JointType.ThumbRight));

            // Left Arm
            this.bones.Add(new Tuple<JointType, JointType>(JointType.ShoulderLeft, JointType.ElbowLeft));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.ElbowLeft, JointType.WristLeft));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.WristLeft, JointType.HandLeft));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.HandLeft, JointType.HandTipLeft));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.WristLeft, JointType.ThumbLeft));

            // Right Leg
            this.bones.Add(new Tuple<JointType, JointType>(JointType.HipRight, JointType.KneeRight));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.KneeRight, JointType.AnkleRight));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.AnkleRight, JointType.FootRight));

            // Left Leg
            this.bones.Add(new Tuple<JointType, JointType>(JointType.HipLeft, JointType.KneeLeft));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.KneeLeft, JointType.AnkleLeft));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.AnkleLeft, JointType.FootLeft));

            // populate body colors, one for each BodyIndex
            this.bodyColors = new List<Pen>();

            this.bodyColors.Add(new Pen(Brushes.Red, 6));
            this.bodyColors.Add(new Pen(Brushes.Orange, 6));
            this.bodyColors.Add(new Pen(Brushes.Green, 6));
            this.bodyColors.Add(new Pen(Brushes.Blue, 6));
            this.bodyColors.Add(new Pen(Brushes.Indigo, 6));
            this.bodyColors.Add(new Pen(Brushes.Violet, 6));

            // Create the drawing group we'll use for drawing on depth space
            this.drawingGroup_onDepth = new DrawingGroup();

            // Create an image source that we can use in our image control on depth space
            this.imageSourceBody_onDepth = new DrawingImage(this.drawingGroup_onDepth);

            // Create the drawing group we'll use for drawing on color space
            this.drawingGroup_onColor = new DrawingGroup();

            // Create an image source that we can use in our image control on color space
            this.imageSourceBody_onColor = new DrawingImage(this.drawingGroup_onColor);


        }



        public void ProcessBody(BodyFrame bodyFrame)
        {

            if (this.bodies == null)
            {
                this.bodies = new Body[bodyFrame.BodyCount];
            }
            // The first time GetAndRefreshBodyData is called, Kinect will allocate each Body in the array.
            // As long as those body objects are not disposed and not set to null in the array,
            // those body objects will be re-used.
            bodyFrame.GetAndRefreshBodyData(this.bodies);



            using (DrawingContext dc = this.drawingGroup_onDepth.Open())
            using (DrawingContext dc2 = this.drawingGroup_onColor.Open())
            {
                // Draw a transparent background to set the render size
                dc.DrawRectangle(Brushes.Black, null, new Rect(0.0, 0.0, this.displayWidth_depth, this.displayHeight_depth));
                //dc2.DrawRectangle(Brushes.Black, null, new Rect(0.0, 0.0, this.displayWidth_color, this.displayHeight_color));
                dc2.DrawImage(colorBitmap, new Rect(0.0, 0.0, this.displayWidth_color, this.displayHeight_color));
                int penIndex = 0;
                foreach (Body body in this.bodies)
                {
                    Pen drawPen = this.bodyColors[penIndex++];

                    if (body.IsTracked)
                    {
                        this.DrawClippedEdges(body, dc);
                        this.DrawClippedEdges(body, dc2);

                        IReadOnlyDictionary<JointType, Joint> joints = body.Joints;

                        // Moha: position should be saved in csv file (3d joints info)

                        // convert the joint points to depth (display) space
                        Dictionary<JointType, Point> jointPoints_Mapped2Depth = new Dictionary<JointType, Point>();
                        Dictionary<JointType, Point> jointPoints_Mapped2Color = new Dictionary<JointType, Point>();


                        foreach (JointType jointType in joints.Keys)
                        {
                            // sometimes the depth(Z) of an inferred joint may show as negative
                            // clamp down to 0.1f to prevent coordinatemapper from returning (-Infinity, -Infinity)
                            CameraSpacePoint position = joints[jointType].Position;
                            if (position.Z < 0)
                            {
                                position.Z = InferredZPositionClamp;
                            }
                            DepthSpacePoint depthSpacePoint = this.coordinateMapper.MapCameraPointToDepthSpace(position);
                            ColorSpacePoint colorSpacePoint = this.coordinateMapper.MapCameraPointToColorSpace(position);
                            jointPoints_Mapped2Depth[jointType] = new Point(depthSpacePoint.X, depthSpacePoint.Y);
                            jointPoints_Mapped2Color[jointType] = new Point(colorSpacePoint.X, colorSpacePoint.Y);

                            if (isCapturing && Counter_CapturingFrame < MaximumPossibleFrameNumners_AfterInitializedCaputing)
                            {
                                Bodies_Array[Counter_CapturingFrame] = Bodies_Array[Counter_CapturingFrame]
                                    + "JointType:," + jointType.ToString()
                                    + ", CameraSpacePoint:,"
                                    + " X: " + position.X.ToString() + " Y: " + position.Y.ToString() + " Z: " + position.Z.ToString()
                                    + ", DepthSpacePoint:,"
                                    + " X: " + depthSpacePoint.X.ToString() + " Y: " + depthSpacePoint.Y.ToString()
                                    + ", ColorSpacePoint:,"
                                    + " X: " + colorSpacePoint.X.ToString() + " Y: " + colorSpacePoint.Y.ToString()
                                    + "\n";
                            }

                        }
                        // Moha: jointPoints should be saved in csv file (2d skel infor)
                        // Moha: draw body on depth space
                        this.DrawBody(joints, jointPoints_Mapped2Depth, dc, drawPen);

                        this.DrawHand(body.HandLeftState, jointPoints_Mapped2Depth[JointType.HandLeft], dc);
                        this.DrawHand(body.HandRightState, jointPoints_Mapped2Depth[JointType.HandRight], dc);

                        // Moha: draw body on color space
                        this.DrawBody(joints, jointPoints_Mapped2Color, dc2, drawPen);
                        this.DrawHand(body.HandLeftState, jointPoints_Mapped2Color[JointType.HandLeft], dc2);
                        this.DrawHand(body.HandRightState, jointPoints_Mapped2Color[JointType.HandRight], dc2);

                        if (isCapturing && Counter_CapturingFrame < MaximumPossibleFrameNumners_AfterInitializedCaputing)
                        {
                            Bodies_Array[Counter_CapturingFrame] = Bodies_Array[Counter_CapturingFrame] + "\n" + "---" + "\n";
                        }

                    }


                }

                // prevent drawing outside of our render area
                this.drawingGroup_onDepth.ClipGeometry = new RectangleGeometry(new Rect(0.0, 0.0, this.displayWidth_depth, this.displayHeight_depth));
                this.drawingGroup_onColor.ClipGeometry = new RectangleGeometry(new Rect(0.0, 0.0, this.displayWidth_color, this.displayHeight_color));
            }

        }

        /// <summary>
        /// Draws a body
        /// </summary>
        /// <param name="joints">joints to draw</param>
        /// <param name="jointPoints">translated positions of joints to draw</param>
        /// <param name="drawingContext">drawing context to draw to</param>
        /// <param name="drawingPen">specifies color to draw a specific body</param>
        private void DrawBody(IReadOnlyDictionary<JointType, Joint> joints, IDictionary<JointType, Point> jointPoints, DrawingContext drawingContext, Pen drawingPen)
        {
            // Draw the bones
            foreach (var bone in this.bones)
            {
                this.DrawBone(joints, jointPoints, bone.Item1, bone.Item2, drawingContext, drawingPen);
            }

            // Draw the joints
            foreach (JointType jointType in joints.Keys)
            {
                Brush drawBrush = null;

                TrackingState trackingState = joints[jointType].TrackingState;

                if (trackingState == TrackingState.Tracked)
                {
                    drawBrush = this.trackedJointBrush;
                }
                else if (trackingState == TrackingState.Inferred)
                {
                    drawBrush = this.inferredJointBrush;
                }

                if (drawBrush != null)
                {
                    drawingContext.DrawEllipse(drawBrush, null, jointPoints[jointType], JointThickness, JointThickness);
                }
            }
        }

        /// <summary>
        /// Draws one bone of a body (joint to joint)
        /// </summary>
        /// <param name="joints">joints to draw</param>
        /// <param name="jointPoints">translated positions of joints to draw</param>
        /// <param name="jointType0">first joint of bone to draw</param>
        /// <param name="jointType1">second joint of bone to draw</param>
        /// <param name="drawingContext">drawing context to draw to</param>
        /// /// <param name="drawingPen">specifies color to draw a specific bone</param>
        private void DrawBone(IReadOnlyDictionary<JointType, Joint> joints, IDictionary<JointType, Point> jointPoints, JointType jointType0, JointType jointType1, DrawingContext drawingContext, Pen drawingPen)
        {
            Joint joint0 = joints[jointType0];
            Joint joint1 = joints[jointType1];

            // If we can't find either of these joints, exit
            if (joint0.TrackingState == TrackingState.NotTracked ||
                joint1.TrackingState == TrackingState.NotTracked)
            {
                return;
            }

            // We assume all drawn bones are inferred unless BOTH joints are tracked
            Pen drawPen = this.inferredBonePen;
            if ((joint0.TrackingState == TrackingState.Tracked) && (joint1.TrackingState == TrackingState.Tracked))
            {
                drawPen = drawingPen;
            }

            drawingContext.DrawLine(drawPen, jointPoints[jointType0], jointPoints[jointType1]);
        }

        /// <summary>
        /// Draws a hand symbol if the hand is tracked: red circle = closed, green circle = opened; blue circle = lasso
        /// </summary>
        /// <param name="handState">state of the hand</param>
        /// <param name="handPosition">position of the hand</param>
        /// <param name="drawingContext">drawing context to draw to</param>
        private void DrawHand(HandState handState, Point handPosition, DrawingContext drawingContext)
        {
            switch (handState)
            {
                case HandState.Closed:
                    drawingContext.DrawEllipse(this.handClosedBrush, null, handPosition, HandSize, HandSize);
                    break;

                case HandState.Open:
                    drawingContext.DrawEllipse(this.handOpenBrush, null, handPosition, HandSize, HandSize);
                    break;

                case HandState.Lasso:
                    drawingContext.DrawEllipse(this.handLassoBrush, null, handPosition, HandSize, HandSize);
                    break;
            }
        }

        /// <summary>
        /// Draws indicators to show which edges are clipping body data
        /// </summary>
        /// <param name="body">body to draw clipping information for</param>
        /// <param name="drawingContext">drawing context to draw to</param>
        private void DrawClippedEdges(Body body, DrawingContext drawingContext)
        {
            FrameEdges clippedEdges = body.ClippedEdges;

            if (clippedEdges.HasFlag(FrameEdges.Bottom))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0, this.displayHeight_depth - ClipBoundsThickness, this.displayWidth_depth, ClipBoundsThickness));
            }

            if (clippedEdges.HasFlag(FrameEdges.Top))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0, 0, this.displayWidth_depth, ClipBoundsThickness));
            }

            if (clippedEdges.HasFlag(FrameEdges.Left))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0, 0, ClipBoundsThickness, this.displayHeight_depth));
            }

            if (clippedEdges.HasFlag(FrameEdges.Right))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(this.displayWidth_depth - ClipBoundsThickness, 0, ClipBoundsThickness, this.displayHeight_depth));
            }
        }


        #endregion



        public void SaveImages_ArrayPixels(Array Ar_Px, int i, string Type, string SavingPath)
        {
            if (Ar_Px != null)
            {

                string path = "";
                string filename = "";


                if (i < 10)
                {
                    filename = "000" + i.ToString();
                }
                if (i >= 10 & i < 100)
                {
                    filename = "00" + i.ToString();
                }

                if (i >= 100 & i < 999)
                {
                    filename = "0" + i.ToString();
                }
                if (i >= 1000)
                {
                    filename = i.ToString();
                }

                WriteableBitmap wb = null;

                switch (Type)
                {
                    case "color":
                        path = SavingPath + "\\02 Color Frames\\" + filename + ".png";
                        wb = new WriteableBitmap(color_Width, color_Height, 96.0, 96.0, PixelFormats.Bgr32, null);
                        wb.WritePixels(new Int32Rect(0, 0, color_Width, color_Height)
                                    , Ar_Px,
                                    color_Width * 4, 0);
                        break;
                    case "infrared":
                        path = SavingPath + "\\03 Infrared Frames\\" + filename + ".png";
                        wb = new WriteableBitmap(IR_Width, IR_Height, 96.0, 96.0, PixelFormats.Gray32Float, null);
                        wb.WritePixels(new Int32Rect(0, 0, IR_Width, IR_Height)
                                    , Ar_Px,
                                    IR_Width * 4, 0);
                        break;
                    case "depth":
                        path = SavingPath + "\\04 Depth Frames\\" + filename + ".png";
                        wb = new WriteableBitmap(IR_Width, IR_Height, 96.0, 96.0, PixelFormats.Gray8, null);
                        wb.WritePixels(new Int32Rect(0, 0, IR_Width, IR_Height)
                                    , Ar_Px,
                                    IR_Width * 4, 0);
                        break;
                    case "bodyindex":
                        path = SavingPath + "\\05 BodyIndex Frames\\" + filename + ".png";
                        wb = new WriteableBitmap(IR_Width, IR_Height, 96.0, 96.0, PixelFormats.Bgr32, null);
                        wb.WritePixels(new Int32Rect(0, 0, IR_Width, IR_Height)
                                    , Ar_Px,
                                    IR_Width * 4, 0);
                        break;
                    case "colorbody":
                        path = SavingPath + "\\07 Color Body Frames\\" + filename + ".png";
                        wb = new WriteableBitmap(IR_Width, IR_Height, 96.0, 96.0, PixelFormats.Bgr32, null);
                        wb.WritePixels(new Int32Rect(0, 0, IR_Width, IR_Height)
                                    , Ar_Px,
                                    IR_Width * 4, 0);
                        break;
                    default:
                        break;
                }



                // create a png bitmap encoder which knows how to save a .png file
                BitmapEncoder encoder = new PngBitmapEncoder();

                // create frame from the writable bitmap and add to encoder
                encoder.Frames.Add(BitmapFrame.Create(wb));


                // write the new file to disk
                try
                {
                    // FileStream is IDisposable
                    using (FileStream fs = new FileStream(path, FileMode.Create))
                    {
                        encoder.Save(fs);
                        fs.Dispose();
                    }

                }
                catch (IOException)
                {

                }
            }


        }


        public void SaveCSVs(Array Ar_Da, int i, string Type, string SavingPath)
        {
            if (Ar_Da != null)
            {

                string path = "";
                string filename = "";


                if (i < 10)
                {
                    filename = "000" + i.ToString();
                }
                if (i >= 10 & i < 100)
                {
                    filename = "00" + i.ToString();
                }

                if (i >= 100 & i < 999)
                {
                    filename = "0" + i.ToString();
                }
                if (i >= 1000)
                {
                    filename = i.ToString();
                }


                switch (Type)
                {
                    case "time":
                        path = SavingPath + "\\01 Times\\" + filename + ".csv";
                        using (System.IO.StreamWriter file = new System.IO.StreamWriter(filename))
                        {
                            file.Write(string.Join(",", stringCapturingTimes_Array));
                        }
                        break;


                    default:
                        break;
                }


            }

        }

        public void SaveTimes(Array Ar_Da, string SavingPath)
        {
            if (Ar_Da != null)
            {

                string path = "";
                string filename = "Times";


                path = SavingPath + "\\01 Times\\" + filename + ".csv";
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(path))
                {
                    file.Write(string.Join("\n", stringCapturingTimes_Array));
                }



            }

        }

        public void SaveBodies(string BodyText, int i, string SavingPath)
        {
            string path = "";
            string filename = "";


            if (i < 10)
            {
                filename = "000" + i.ToString();
            }
            if (i >= 10 & i < 100)
            {
                filename = "00" + i.ToString();
            }

            if (i >= 100 & i < 999)
            {
                filename = "0" + i.ToString();
            }
            if (i >= 1000)
            {
                filename = i.ToString();
            }

            path = SavingPath + "\\06 Body Skels data\\" + filename + ".csv";

            System.IO.File.WriteAllText(path, BodyText);

        }


    } // class
}// end namespace
