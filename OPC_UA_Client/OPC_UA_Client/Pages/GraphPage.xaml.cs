using OPC_UA_Client.ViewModel;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Xamarin.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
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
        ClientOPC client;
        
        Double i = 0;

        public GraphPage (ClientOPC _client,string _message)
		{
            LinePlot = new PlotModel();
            LinePlot.Series.Add(new LineSeries());
            client = _client;
            message = _message;
            MessagingCenter.Subscribe<ClientOPC, DataChangeView>(this, message, (client, view) => {


                Device.BeginInvokeOnMainThread(() => {
                    (LinePlot.Series[0] as LineSeries).Points.Add(new DataPoint(Convert.ToDouble(view.Value), i));
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

    }
}