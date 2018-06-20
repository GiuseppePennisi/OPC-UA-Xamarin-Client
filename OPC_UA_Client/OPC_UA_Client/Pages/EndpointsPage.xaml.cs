﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OPC_UA_Client.ViewModel;
using Rg.Plugins.Popup;
using Rg.Plugins.Popup.Extensions;
using Rg.Plugins.Popup.Pages;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace OPC_UA_Client
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EndpointsPage : ContentPage
    {

        ObservableCollection<EndpointView> endpoints = new ObservableCollection<EndpointView>();
        ClientOPC client;
        ListEndpoint storedList;
        SessionView sessionView;

        public EndpointsPage(ListEndpoint _list, ClientOPC _client)
        {
            InitializeComponent();
            BindingContext = endpoints;
            storedList = _list;
            client = _client;
            displayEndpoints();
        }

        void displayEndpoints()
        {
            endpoints.Clear();
            foreach (var endP in storedList.endpointList)
            {

                endpoints.Add(endP);

            }

            EndpointsDisplay.ItemsSource = null;
            EndpointsDisplay.SeparatorColor = Color.FromHex("#4CAF50");
            EndpointsDisplay.ItemsSource = endpoints;
        }
        async void OnSelected(object sender, ItemTappedEventArgs e)
        {


            EndpointView selected = e.Item as EndpointView;

            int i = storedList.endpointList.IndexOf(selected);
            string action = null;
            try
            {

                action = await DisplayActionSheet("Select authentication mode: ", "cancel", null, "Anonymous", "Username & Password");



                if (action.Equals("Anonymous"))
                {

                    sessionView = await client.CreateSessionChannelAsync(i);

                    if (sessionView == null)
                    {
                        await DisplayAlert("Error", "Cannot connect to an OPC UA Server!", "OK");

                    }
                    else
                    {

                        await DisplayAlert("Info", "Session created successfully!", "Ok");
                        ContentPage sessionPage = new SessionPage(client, sessionView,client.GetRootNode());
                        sessionPage.Title = "OPC Session Services";

                        await Navigation.PushAsync(sessionPage);


                    }
                }

                else if (action.Equals("Username & Password"))
                {

                    var _loginPopup = new LoginPopupPage(client, i, sessionView);

                    await Navigation.PushPopupAsync(_loginPopup);

                }


            }
            catch (NullReferenceException)
            {
             
       
            }

        }

        protected override bool OnBackButtonPressed()
        {

            // Begin an asyncronous task on the UI thread because we intend to ask the users permission.
            Device.BeginInvokeOnMainThread(async () =>
            {
                if (await DisplayAlert("Warning", "Are you sure you want to return in Homepage?", "Yes", "No"))
                {
                    base.OnBackButtonPressed();

                    await Navigation.PopToRootAsync();
                }
            });

            // Always return true because this method is not asynchronous.
            // We must handle the action ourselves: see above.
            return true;
        }
    }
}