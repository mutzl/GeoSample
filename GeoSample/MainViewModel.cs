
namespace GeoSample
{
	using System;
	using System.Collections.ObjectModel;

	using Windows.Devices.Geolocation;
	using Windows.UI.Xaml.Controls.Maps;

	using GalaSoft.MvvmLight;
	using GalaSoft.MvvmLight.Messaging;

	// INPC injected by Fody
	public class MainViewModel : ViewModelBase
	{
		private Geolocator locator;

		public MainViewModel()
		{
			locator = new Geolocator
				          {
								// define accuracy here
						  };
			//locator.PositionChanged += (sender, args) =>
			//	{
			//		args.Position;
			//	};
		}
		public Geopoint Center { get; set; } = new Geopoint(new BasicGeoposition() {Latitude = 20.0, Longitude = 10.0});

		public double Zoomlevel { get; set; } = 5;

		public double Pitch { get; set; }
		public double Orientation { get; set; }

		public MapStyle MapStyle => (MapStyle)Enum.Parse(typeof(MapStyle), MapStyleName);

		public string MapStyleName { get; set; } = "Road";

		public string Token => "ABqAfIfL8B3HIvAVMVmK~MSfiZYN7DoiZ0MlGVtjOHw~Aq0tWh2Z0wfet16va06U21q4LNDSvC4mjQSLJjhtL5MV8RMTKvWwoqDNGinI4dGn";

		public ObservableCollection<PointOfInterest> PointsOfInterest { get; private set; } = new ObservableCollection<PointOfInterest>(); 
		private Random random = new Random();
		public async void GetCurrentLocation()
		{

			var access = await Geolocator.RequestAccessAsync();

			switch (access)
			{
				case GeolocationAccessStatus.Allowed:

					var geolocator = new Geolocator();
					var geopositon = await geolocator.GetGeopositionAsync();
					var geopoint = geopositon.Coordinate.Point;

					var latitude = geopoint.Position.Latitude + random.NextDouble() - 0.5;
					var longitude = geopoint.Position.Longitude + random.NextDouble() - 0.5;

					PointsOfInterest.Add(
						new PointOfInterest
							{
								Description = DateTime.Now.ToString("T"),
								Geopoint = new Geopoint(new BasicGeoposition
															{
																Latitude = latitude,
																Longitude = longitude,
															})
							});


					ZoomToFit();
					Center = geopoint;
					Zoomlevel = 15;
					break;
				case GeolocationAccessStatus.Unspecified:
				case GeolocationAccessStatus.Denied:
					break;
			}

		}
		public void ZoomToFit()
		{
			Messenger.Default.Send("zoom");
		}
	}

	public class PointOfInterest
	{
		public string Description { get; set; }

		public Geopoint Geopoint { get; set; }

	}
}
