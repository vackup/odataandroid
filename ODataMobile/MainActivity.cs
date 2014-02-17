using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace ODataMobile
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Simple.OData.Client;

    [Activity(Label = "ODataMobile", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : ListActivity
    {
        private readonly TaskScheduler uiScheduler = TaskScheduler.FromCurrentSynchronizationContext();

        private List<string> entities = new List<string>();

        private ODataClient client;

        private void StartRestRequestAsync()
        {
            Task<IEnumerable<IDictionary<string,object>>>.Factory.StartNew(() =>
                    {
                        var packages = client.FindEntries("Packages?$filter=Title eq 'Simple.OData.Client'");

                        return packages; //IEnumerable<IDictionary<string, object>>
                    }).ContinueWith(
                        t =>
                            {
                                var packages = t.Result;

                                foreach (var package in packages)
                                {
                                    entities.Add(package["Description"].ToString() + " " + package["Version"].ToString());
                                }
                                ListAdapter = new ArrayAdapter<string>(
                                    this, Android.Resource.Layout.SimpleListItem1, entities);
                            },
                        uiScheduler);
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            client = new ODataClient("http://packages.nuget.org/v1/FeedService.svc/");
            StartRestRequestAsync();
        }
    }    
}


