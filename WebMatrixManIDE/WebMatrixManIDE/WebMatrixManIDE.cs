using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Microsoft.VisualStudio.Text.Editor;

namespace WebMatrixManIDE
{
	/// <summary>
	/// Adornment class that draws a square box in the top right hand corner of the viewport
	/// </summary>
	class WebMatrixManIDE
	{
		private Image _image;
		private BitmapImage _bitmap;
		private IWpfTextView _view;
		private IAdornmentLayer _adornmentLayer;
		private string[] _imagePathCollection;
		private DoubleAnimation _fadeOutAnimation = new DoubleAnimation
		{
			From = 0.45,
			To = 0
		};
		private DoubleAnimation _fadeInAnimation = new DoubleAnimation
		{
			From = 0,
			To = 0.45
		};

		/// <summary>
		/// Creates a square image and attaches an event handler to the layout changed event that
		/// adds the the square in the upper right-hand corner of the TextView via the adornment layer
		/// </summary>
		/// <param name="view">The <see cref="IWpfTextView"/> upon which the adornment will be drawn</param>
		public WebMatrixManIDE(IWpfTextView view)
		{
			_view = view;

			try
			{
				_view = view;
				_image = new Image();

				_image.Opacity = 0.45;

				var assemblylocation = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
				_imagePathCollection =
					System.IO.Directory.GetFiles(Path.Combine(string.IsNullOrEmpty(assemblylocation) ? "" : assemblylocation, "assets"),
					                                          "*.png");
				_image.Stretch = Stretch.Uniform;
				_image.Height = 500;
				_image.Width = 500;
				Task.Run(delegate
				         {
							 var r = new Random();
							 while (true)
							 {
								 _image.Dispatcher.InvokeAsync(() =>
								                                {
																	_bitmap = new BitmapImage(new Uri(_imagePathCollection[r.Next(0, _imagePathCollection.Length - 1)], UriKind.Absolute));
									                                _image.BeginAnimation(Image.OpacityProperty, _fadeOutAnimation);
																	_image.Source = _bitmap;
																	_image.BeginAnimation(Image.OpacityProperty, _fadeInAnimation);
																});
								 Thread.Sleep(TimeSpan.FromSeconds(10));
							 }
						 });
			}
			catch (Exception)
			{
			}

			//Grab a reference to the adornment layer that this adornment should be added to
			_adornmentLayer = view.GetAdornmentLayer("WebMatrixManIDE");

			_view.ViewportHeightChanged += delegate { this.onSizeChange(); };
			_view.ViewportWidthChanged += delegate { this.onSizeChange(); };
		}

		public void onSizeChange()
		{
			//clear the adornment layer of previous adornments
			_adornmentLayer.RemoveAllAdornments();

			//Place the image in the top right hand corner of the Viewport
			Canvas.SetLeft(this._image, this._view.ViewportRight - (double)_image.Width);
			Canvas.SetTop(this._image, this._view.ViewportBottom - (double)_image.Height);

			//add the image to the adornment layer and make it relative to the viewport
			_adornmentLayer.AddAdornment(AdornmentPositioningBehavior.ViewportRelative, null, null, _image, null);
		}
	}
}
