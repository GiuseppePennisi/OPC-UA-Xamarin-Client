using Acr.UserDialogs;
using OPC_UA_Client.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace OPC_UA_Client.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SubscriptionsPage : ContentPage
    {
        ClientOPC client;
        ObservableCollection<SubscriptionView> subscriptionsView = new ObservableCollection<SubscriptionView>();
        List<SubscriptionView> storedList;

        public SubscriptionsPage(ClientOPC _client)
        {
            client = _client;
            storedList = _client.GetSubscriptionViews();
            InitializeComponent();
            BindingContext = subscriptionsView;
            displaySubscriptions();
            SubscribePage();
           // SubscriberOnSubDelete();
        }

        private void displaySubscriptions()
        {
            subscriptionsView.Clear();
            foreach (var sub in storedList)
            {
                subscriptionsView.Add(sub);
            }
            if (subscriptionsView.Count == 0)
            {
                NoSubsLabel.IsVisible = true;
            }
            SubscriptionsDisplay.ItemsSource = null;
            SubscriptionsDisplay.SeparatorColor = Color.FromHex("#4CAF50");
            SubscriptionsDisplay.ItemsSource = subscriptionsView;
        }

        private async void OnTapped(object sender, ItemTappedEventArgs e)
        {
            var item = e.Item as SubscriptionView;
            ContentPage detailSubPage = new DetailSubscriptionPage(client, item.SubscriptionID);
            detailSubPage.Title = "Subscription Details";
            await Navigation.PushAsync(detailSubPage);
            
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

        private async void OnSubscriptionDelete(object sender, EventArgs e)
        {
            if (await DisplayAlert("Warning", "Do you want to cancel the selected subscription and its monitored items?", "yes", "no"))
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                 UserDialogs.Instance.ShowLoading();
                 });

                var button = sender as Button;
                var subView = button.BindingContext as SubscriptionView;
                await Task.Run(() => client.CloseSubscription(subView.SubscriptionID));
                storedList = client.GetSubscriptionViews();
                displaySubscriptions();

                Device.BeginInvokeOnMainThread(() =>
                 {
                     UserDialogs.Instance.HideLoading();
                 });
            }
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
                            p.Title = "Client OPC";
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
       /* 
        private void SubscriberOnSubDelete()
        {
            MessagingCenter.Subscribe<ClientOPC>(this, "SubDelete",
               async (sender) => {

                   await Task.Run(() =>
                   {
                       Device.BeginInvokeOnMainThread(() => DisplayAlert("Error", "One Subscription is deleted!", "Ok"));
                   });


                   await Task.Run(() =>
                   {
                       Device.BeginInvokeOnMainThread(() => {

                           MessagingCenter.Unsubscribe<ClientOPC>(this, "SubDelete");
                       });
                   });
               });
        }*/
    }
}