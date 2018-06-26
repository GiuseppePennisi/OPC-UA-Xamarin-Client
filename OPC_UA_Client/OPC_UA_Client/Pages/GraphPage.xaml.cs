using OPC_UA_Client.ViewModel;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot.Xamarin.Forms;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace OPC_UA_Client.Pages
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class GraphPage : ContentPage
	{
        public PlotModel LinePlot { get; set; }
        string message;
        ClientOPC  client;
        Double i = 0;

        public GraphPage (ClientOPC _client,string _message)
		{
            LinePlot = new PlotModel();
            LinePlot.Title ="Curve Of Monitoring"; 
            
            
            LinePlot.Series.Add(new LineSeries());
            
            client = _client;
            message = _message;
            
            MessagingCenter.Subscribe<ClientOPC, DataChangeView>(this, message, (client, view) => {


                Device.BeginInvokeOnMainThread(() => {
                    (LinePlot.Series[0] as LineSeries).Color = OxyColors.Orange;
                    (LinePlot.Series[0] as LineSeries).Points.Add(new DataPoint( i, Convert.ToDouble(view.Value)));
                    
                    LinePlot.InvalidatePlot(true);
                    i++;
                

                });

            });
           
            InitializeComponent ();
		}
        protected override void OnAppearing()
        {
            Content = new PlotView
            {
                Model = LinePlot
            };
        }
        protected override bool OnBackButtonPressed()
        {
            // Begin an asyncronous task on the UI thread because we intend to ask the users permission.
            Device.BeginInvokeOnMainThread(async () =>
            {
                    LinePlot.InvalidatePlot(false);
                    MessagingCenter.Unsubscribe<ClientOPC, DataChangeView>(this, message);
                    base.OnBackButtonPressed();
                    await Navigation.PopAsync();
                    Navigation.RemovePage(this);
                
            });

            // Always return true because this method is not asynchronous.
            // We must handle the action ourselves: see above.
            return true;
        }
    }
}