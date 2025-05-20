using jcorreaS6.Models;
using Newtonsoft.Json;
using System.Text.Json;
using System.Collections.ObjectModel;
using System.Text;

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

    // Agregar un nuevo booking
    private async void Button_Clicked(object sender, EventArgs e)
    {
        // No se debe incluir el bookingId aquí porque la DB lo genera automáticamente
        Booking nuevo = new Booking
        {
            name = entryName.Text,
            lastName = entryLastName.Text,
            email = entryEmail.Text
        };

        var json = JsonConvert.SerializeObject(nuevo);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await cliente.PostAsync(URL, content);
        if (response.IsSuccessStatusCode)
        {
            await DisplayAlert("Éxito", "Booking agregado correctamente", "OK");
            mostrarBooking(); // Refrescar la lista
        }
        else
        {
            await DisplayAlert("Error", "No se pudo agregar el booking", "OK");
        }
    }

    private Booking bookingSeleccionado;

    // Cuando seleccionas un booking, llena los campos
    private void lvBooking_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        bookingSeleccionado = (Booking)e.SelectedItem;
        if (bookingSeleccionado != null)
        {
            entryName.Text = bookingSeleccionado.name;
            entryLastName.Text = bookingSeleccionado.lastName;
            entryEmail.Text = bookingSeleccionado.email;
        }
    }

    // Actualizar el booking seleccionado
    private async void Button_Clicked_1(object sender, EventArgs e)
    {
        if (bookingSeleccionado == null)
        {
            await DisplayAlert("Error", "Selecciona un booking para actualizar", "OK");
            return;
        }

        // Actualiza los datos
        bookingSeleccionado.name = entryName.Text;
        bookingSeleccionado.lastName = entryLastName.Text;
        bookingSeleccionado.email = entryEmail.Text;

        var json = JsonConvert.SerializeObject(bookingSeleccionado);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Utiliza el bookingId al hacer la solicitud PUT
        var response = await cliente.PutAsync($"{URL}/{bookingSeleccionado.bookingId}", content);
        if (response.IsSuccessStatusCode)
        {
            await DisplayAlert("Éxito", "Booking actualizado", "OK");
            mostrarBooking(); // Refrescar la lista
        }
        else
        {
            await DisplayAlert("Error", "No se pudo actualizar", "OK");
        }
    }

    // Eliminar el booking seleccionado
    private async void Button_Clicked_2(object sender, EventArgs e)
    {
        if (bookingSeleccionado == null)
        {
            await DisplayAlert("Error", "Selecciona un booking para eliminar", "OK");
            return;
        }

        bool confirm = await DisplayAlert("Confirmar", "¿Eliminar este booking?", "Sí", "No");
        if (!confirm) return;

        // Utiliza el bookingId al hacer la solicitud DELETE
        var response = await cliente.DeleteAsync($"{URL}/{bookingSeleccionado.bookingId}");
        if (response.IsSuccessStatusCode)
        {
            await DisplayAlert("Éxito", "Booking eliminado", "OK");
            mostrarBooking(); // Refrescar la lista
        }
        else
        {
            await DisplayAlert("Error", "No se pudo eliminar", "OK");
        }
    }
}