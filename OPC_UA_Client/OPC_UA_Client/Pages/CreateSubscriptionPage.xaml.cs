using Acr.UserDialogs;
using OPC_UA_Client.ViewModel;
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
    public partial class CreateSubscriptionPage : ContentPage
    {
        ClientOPC client;
        public CreateSubscriptionPage(ClientOPC _client)
        {
            client = _client;
            InitializeComponent();
            SubscribePage();
        }

        private async void OnCreateSubscription(object sender, EventArgs e)
        {
             Device.BeginInvokeOnMainThread(() =>
                {
                    UserDialogs.Instance.ShowLoading();
                });
            
            Double reqPubInterval;
            uint reqLifeTimeCount;
            uint reqMaxKeepAliveCount;
            uint maxNotPerPublish;
            byte priority;
            try
            {
                if (string.IsNullOrWhiteSpace(RequestedPublishingInterval.Text))
                    throw new EmptyEntryException("Empty Requested Publish Interval Entry!");
                try
                {
                    reqPubInterval = Convert.ToDouble(RequestedPublishingInterval.Text);
                }
                catch(FormatException p)
                {
                    throw new FormatException("Request Publish Format not valid!", p);
                }

                if (string.IsNullOrWhiteSpace(RequestedLifetimeCount.Text))
                    throw new EmptyEntryException("Empty Request Lifetime Count Empty!");
                try
                {
                     reqLifeTimeCount = Convert.ToUInt32(RequestedLifetimeCount.Text);
                }
                catch(FormatException p)
                {
                    throw new FormatException("Request Lifetime Count Format not valid!",p);
                }

                if (string.IsNullOrWhiteSpace(RequestedMaxKeepAliveCount.Text))
                    throw new EmptyEntryException("Empty Request Max Keep Alive Count Entry!");
                try
                {
                    reqMaxKeepAliveCount = Convert.ToUInt32(RequestedMaxKeepAliveCount.Text);

                }
                catch(FormatException p)
                {
                    throw new FormatException("Request Max Keep Alive Count Format non valid!", p);
                }

                if (string.IsNullOrWhiteSpace(MaxNotificationPerPublish.Text))
                    throw new EmptyEntryException("Empty Max Notification Per Publish Entry !");
                try { 
                 maxNotPerPublish = Convert.ToUInt32(MaxNotificationPerPublish.Text);
                }
                catch (FormatException p)
                {
                    throw new FormatException("Max Notification Per Publish Format not valid!", p);
                }

                if (string.IsNullOrWhiteSpace(Priority.Text))
                    throw new EmptyEntryException("Empty Priority Entry!");
                try
                {
                     priority = Convert.ToByte(Priority.Text);
                }
                catch(FormatException p)
                {
                    throw new FormatException("Priority Format not valid!", p);
                }

                SubscriptionView subView = await Task.Run(()=> client.CreateSub(reqPubInterval, reqLifeTimeCount, reqMaxKeepAliveCount, maxNotPerPublish, true, priority));

                Device.BeginInvokeOnMainThread(() =>
                {
                    UserDialogs.Instance.HideLoading();
                });

                if (subView.PublishingInterval != reqPubInterval || subView.KeepAliveCount != reqMaxKeepAliveCount || subView.LifeTimeCount != reqLifeTimeCount)
                {
                     await DisplayAlert("Info", "Subscription created successfully with revised parameters.", "ok");
                }
                else
                {
                    await DisplayAlert("Info", "Subscription created successfully with requested parameters.", "ok");
                }
              
                    ContentPage detailSubPage = new DetailSubscriptionPage(client, subView.SubscriptionID);
                    detailSubPage.Title = "Subscription Details";
                
                await Navigation.PushAsync(detailSubPage);
                Navigation.RemovePage(this);
            }
            catch (EmptyEntryException p)
            {
                await DisplayAlert("Error", p.Message, "Ok");
            }
            catch (FormatException p)
            {

                await DisplayAlert("Error", p.Message, "Ok");
            }
        }

        private void gotoSessionClicked(object sender, EventArgs e)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {

                {

                    base.OnBackButtonPressed();

                    await Navigation.PopAsync();

                    Navigation.RemovePage(this);
                }
            });

            // Always return true because this method is not asynchronous.
            // We must handle the action ourselves: see above.

        }

        private void SubscribePage()
        {
            MessagingCenter.Subscribe<ClientOPC>(this, "SessionClose",
                async (sender) => {

                    await Task.Run(() =>
                    {
                        Device.BeginInvokeOnMainThread(() => DisplayAlert("Error", "Session Expired!", "Ok"));
                    });


                    await Task.Run(() =>
                    {
                        Device.BeginInvokeOnMainThread(() => {
                            Page p = new MainPage();
                            Navigation.PushAsync(p);
                            foreach (var page in Navigation.NavigationStack.ToList())
                            {
                                if (page != p)
                                {
                                    Navigation.RemovePage(page);
                                }
                            }
                            MessagingCenter.Unsubscribe<ClientOPC>(this, "SessionClose");
                        });
                    });
                });
        }
    }
}