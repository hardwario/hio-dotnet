using ZXing.Net.Maui;

namespace hio_dotnet.Demos.HardwarioManager;

public partial class QRScannerPage : ContentPage
{
	public QRScannerPage()
	{
		InitializeComponent();
	}
    protected void BarcodesDetected(object sender, BarcodeDetectionEventArgs e)
    {
        /*
        foreach (var barcode in e.Results)
            Console.WriteLine($"Barcodes: {barcode.Format} -> {barcode.Value}");
        */
        if (e.Results.Any())
        {
            var qrCode = e.Results[0].Value;
            MainDataContext.ScannedQRCode = qrCode;

            Dispatcher.Dispatch(async () =>
            {
                await Navigation.PopToRootAsync();
            });
        }
    }
}