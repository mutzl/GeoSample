using Windows.UI.Xaml.Controls;
using System;

namespace GeoSample
{
	using System.Linq;

	using Windows.Devices.Geolocation;
	using Windows.Foundation;
	using Windows.Storage.Streams;
	using Windows.UI.Xaml;
	using Windows.UI.Xaml.Controls.Maps;

	using GalaSoft.MvvmLight.Messaging;

	public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

			this.DataContext = new MainViewModel();

			Messenger.Default.Register<string>(this, Zoom);
        }

		private async void Zoom(string message)
		{

			if (message != "zoom") return;
			
			var box = GeoboundingBox.TryCompute(ViewModel.PointsOfInterest.Select(poi => poi.Geopoint.Position));
			await this.map.TrySetViewBoundsAsync(box, new Thickness(10), MapAnimationKind.Linear);
		}

		public MainViewModel ViewModel => DataContext as MainViewModel;


		// not needed anymore. Solved in ViewModel
		private async void ButtonBase_OnClick(object sender, RoutedEventArgs e)
		{
			var access =  await Geolocator.RequestAccessAsync();

			switch (access)
			{
				case GeolocationAccessStatus.Allowed:

					var geolocator = new Geolocator();
					var geopositon = await geolocator.GetGeopositionAsync();
					var geopoint = geopositon.Coordinate.Point;

					map.Center = geopoint;
					map.ZoomLevel = 15;

					var mapIcon = new MapIcon
						              {
											Location = geopoint,
											Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/pin.png")),
											NormalizedAnchorPoint = new Point(0.5, 1),
						              };
					map.MapElements.Add(mapIcon);
					break;
				case GeolocationAccessStatus.Unspecified:
				case GeolocationAccessStatus.Denied:
					break;
			}
		}
    }
}
