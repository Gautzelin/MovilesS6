using jcorreaS6.Models;
using Newtonsoft.Json;
using System.Collections.ObjectModel;

namespace jcorreaS6.Views;

public partial class vCrud : ContentPage
{
	private const string URL = "http://localhost:8080/api/bookings";
	private HttpClient cliente = new HttpClient();
	private ObservableCollection<Booking> bookings;
	public vCrud()
	{
		InitializeComponent();
		mostrarBooking();
	}

	public async void mostrarBooking()
	{
		var content = await cliente.GetStringAsync(URL);
		List<Booking> lista = JsonConvert.DeserializeObject<List<Booking>>(content);
		bookings = new ObservableCollection<Booking>(lista);
		lvBooking.ItemsSource = bookings;
	}
}