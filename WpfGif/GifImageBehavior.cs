using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using WpfGif.Animation;

namespace WpfGif
{
    public static class GifImageBehavior
    {
        #region DependencyProperty
        public static readonly DependencyProperty PlayGifsProperty = DependencyProperty.Register("PlayGIFs", typeof(Boolean), typeof(Image), new PropertyMetadata(true, PlayGIFsPropertyChanged));
        public static readonly DependencyProperty IsGIFInitProperty = DependencyProperty.Register("IsGIFInit", typeof(Boolean), typeof(Image), new PropertyMetadata(false));
        public static readonly DependencyProperty GIFAnimationProperty = DependencyProperty.Register("GIFAnimation", typeof(Storyboard), typeof(Image), new PropertyMetadata(null));
        public static readonly DependencyProperty GIFSourceProperty = DependencyProperty.Register("GIFSource", typeof(ImageSource), typeof(Image), new PropertyMetadata(null, GIFSourcePropertyChanged));
        #endregion

        #region DependencyProperty PropertyChanged
        private static void PlayGIFsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Image image)
            {
                Boolean newVal = (Boolean)e.NewValue;
                if (newVal)
                    StartAnimationGif(image);
                else
                    StopAnimatedGIF(image);
            }
        }

        private static void GIFSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Image image)
            {
                if (e.NewValue != null)
                {
                    string OldValue = e.OldValue == null ? "" : (string)e.OldValue.ToString();
                    string NewValue = e.NewValue == null ? "" : (string)e.NewValue.ToString();
                    if (OldValue != NewValue)
                    {
                        StopAnimatedGIF(image);
                        StartAnimationGif(image);
                    }
                }
                else
                    StopAnimatedGIF(image);
            }
        }
        #endregion

        #region Public Deklaration
        public static Boolean GetPlayGIFs(Image obj) => (Boolean)obj.GetValue(PlayGifsProperty);
        public static Boolean GetIsGIFInit(Image obj) => (Boolean)obj.GetValue(IsGIFInitProperty);
        public static void SetPlayGIFs(Image obj, Boolean value) => obj.SetValue(PlayGifsProperty, value);
        public static ImageSource GetGIFSource(Image obj) => (ImageSource)obj.GetValue(GIFSourceProperty);
        public static void SetGIFSource(Image obj, ImageSource value) => obj.SetValue(GIFSourceProperty, value);
        #endregion

        #region Init / Release
        private static void InitGIFFunctions(Image image)
        {
            if (!GetIsGIFInit(image))
            {
                image.Loaded += Image_Loaded;
                image.Unloaded += Image_Unloaded;
                image.SetValue(IsGIFInitProperty, true);
            }
        }

        private static void ReleaseGIFFunctions(Image image)
        {
            if (GetIsGIFInit(image))
            {
                image.Loaded -= Image_Loaded;
                image.Unloaded -= Image_Unloaded;
            }
        }

        private static void Image_Unloaded(object sender, RoutedEventArgs e)
        {
            if (sender is Image image)
                ReleaseGIFFunctions(image);
        }

        private static void Image_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is Image image)
                StartAnimationGif(image);
        }
        #endregion

        #region Start Stop
        /// <summary>
        /// Starts the Animation
        /// </summary>
        /// <param name="image"></param>
        private static void StartAnimationGif(Image image)
        {
            if (!GetPlayGIFs(image)) return;           

            BitmapDecoder decoder = GetDecoder(image);
            if (decoder != null && decoder is GifBitmapDecoder gifDecoder && !DesignerProperties.GetIsInDesignMode(image))
            {
                // Create the Animation of the frames
                Storyboard sb = new Storyboard();
                ObjectAnimationUsingKeyFrames animation = GifAnimationCreator.CreateAnimationFromGif(gifDecoder);

                sb.Children.Add(animation);
                Storyboard.SetTarget(animation, image);
                Storyboard.SetTargetProperty(animation, new PropertyPath("Source"));

                sb.RepeatBehavior = RepeatBehavior.Forever;
                sb.Begin();
                image.SetValue(GIFAnimationProperty, sb);
            }
            else if(decoder != null)
            {
                image.Source = GetGIFSource(image);
            }
        }

        /// <summary>
        /// Stops the Animation
        /// </summary>
        /// <param name="image"></param>
        private static void StopAnimatedGIF(Image image)
        {
            if (image.GetValue(GIFAnimationProperty) is Storyboard SB)
            {
                SB.Stop();
                SB = null;
                image.SetValue(GIFAnimationProperty, null);
            }
        }
        #endregion

        #region Decoder
        /// <summary>
        /// Get the decoder of the current <see cref="WpfGif.GifImageBehavior.GIFSourceProperty"/>
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        private static BitmapDecoder GetDecoder(Image image)
        {
            Uri SourceUri = null;

            if (GetGIFSource(image) is BitmapSource bmpSource)
            {
                if (bmpSource is BitmapImage bmpImage)
                {
                    if (bmpImage.StreamSource != null)
                    {
                        bmpImage.StreamSource.Position = 0;
                        return BitmapDecoder.Create(bmpImage.StreamSource, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                    }
                    else if (bmpImage.UriSource != null)
                    {
                        SourceUri = bmpImage.UriSource;
                        if (!SourceUri.IsAbsoluteUri)
                        {
                            throw new NotImplementedException();
                        }
                    }

                }
                else if (bmpSource is BitmapFrame bmpFrame)
                {
                    return bmpFrame.Decoder;
                }

                if (SourceUri != null && SourceUri.IsAbsoluteUri)
                {
                    return BitmapDecoder.Create(SourceUri, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                }
                else
                    return null;
            }
            else
                return null;
        }
        #endregion
    }
}
