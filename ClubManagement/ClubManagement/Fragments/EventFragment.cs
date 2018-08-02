using System;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V7.Widget;
using Android.Views;
using ClubManagement.Adapters;
using ClubManagement.Models;
using System.Linq;
using ClubManagement.Controllers;
using Android.Content;
using ClubManagement.Activities;
using Newtonsoft.Json;
using System.Collections.Generic;
using Android.Support.V4.Widget;
using ClubManagement.Fragments.Bases;
using Android.Widget;
using ClubManagement.Ultilities;

namespace ClubManagement.Fragments
{
    public class EventFragment : SwipeToRefreshDataFragment<List<UserLoginEventModel>>
    {
        private TabLayout tabLayout;

        private UserEventsController userEventsController = UserEventsController.Instance;

        private EventsController eventsController = EventsController.Instance;

        private EventsAdapter adapter = new EventsAdapter();

        private string userId = AppDataController.Instance.UserId;

        private EventDialogFragment eventDialogFragment = new EventDialogFragment();

        private FloatingActionButton fabAdd;

        protected override SwipeRefreshLayout SwipeRefreshLayout => View.FindViewById<SwipeRefreshLayout>(Resource.Id.refresher);

        public EventFragment()
        {
            data = new List<UserLoginEventModel>();

            eventDialogFragment.SaveClick += (s, e) =>
            {
                if (s is EventModel eventModel)
                {
                    var userLoginEventModel = new UserLoginEventModel(eventModel)
                    {
                        IsJoined = false
                    };

                    data.Add(userLoginEventModel);

                    adapter.Events = data;
                }
            };

            adapter.ItemClick += (s, e) =>
            {
                var intent = new Intent(Context, typeof(EventDetailActivity));

                var eventDetail = JsonConvert.SerializeObject(data[e.Position]);

                intent.PutExtra("EventDetail", eventDetail);

                StartActivity(intent);
            };
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.FragmentEvent, container, false);

            InitView(view);

            return view;
        }

        private void InitView(View view)
        {
            fabAdd = view.FindViewById<FloatingActionButton>(Resource.Id.fabAdd);
            Context.DoWithAdmin(() =>
            {
                fabAdd.Visibility = ViewStates.Visible;
            }, () => 
            {
                fabAdd.Visibility = ViewStates.Gone;
            });

            var recyclerView = view.FindViewById<RecyclerView>(Resource.Id.recyclerView1);
            recyclerView.SetLayoutManager(new LinearLayoutManager(view.Context));
            recyclerView.SetAdapter(adapter);

            tabLayout = view.FindViewById<TabLayout>(Resource.Id.tabView1);
            tabLayout.TabSelected += (s, e) => DisplayData(data);

            Context.DoWithAdmin(() =>
            {
                fabAdd.Click += AddEvent_Click;
            });
        }

        private void AddEvent_Click(object sender, System.EventArgs e)
        {
            eventDialogFragment.Show(FragmentManager, null);
        }

        public override void OnResume()
        {
            base.OnResume();
            UpdateViewData();
        }

        protected override List<UserLoginEventModel> QueryData()
        {
            try
            {
                return eventsController.Values.Count >= data.Count
                    ? eventsController.Values.Select(x =>
                    {
                        var userLoginEventModel = new UserLoginEventModel(x)
                        {
                            IsJoined = userEventsController.Values
                                .Any(y => y.EventId == x.Id && y.UserId == userId)
                        };

                        return userLoginEventModel;
                    }).ToList()
                    : data;
            }
            catch (Exception)
            {
                return new List<UserLoginEventModel>();
            }
        }

        protected override void DisplayData(List<UserLoginEventModel> data)
        {
            if (data != null)
            {
                switch (tabLayout.SelectedTabPosition)
                {
                    case 0:
                        adapter.Events = data;
                        Context.DoWithAdmin(() =>
                        {
                            fabAdd.Visibility = ViewStates.Visible;
                        });
                        break;
                    case 1:
                        adapter.Events = data.Where(x => x.Time > DateTime.Now).Where(x => !x.IsJoined).ToList();
                        fabAdd.Visibility = ViewStates.Gone;
                        break;
                    case 2:
                        adapter.Events = data.Where(x => x.IsJoined).ToList();
                        fabAdd.Visibility = ViewStates.Gone;
                        break;
                }
            }
        }
    }
}