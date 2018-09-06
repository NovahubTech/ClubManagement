﻿using System;
using Android.Gms.Maps;
using Android.OS;
using Android.Widget;
using ClubManagement.Controllers;
using ClubManagement.Models;
using ClubManagement.Ultilities;
using Newtonsoft.Json;
using System.Linq;
using Android.Content;
using Android.Views;
using ClubManagement.Fragments;
using FragmentActivity = Android.Support.V4.App.FragmentActivity;
using Android.App;
using System.Globalization;
using System.Threading.Tasks;

namespace ClubManagement.Activities
{
    [Activity(Label = "EventDetailActivity", Theme = "@style/AppTheme")]
    public class EventDetailActivity : FragmentActivity
    {
        [InjectView(Resource.Id.tvMonth)]
        private TextView tvMonth;

        [InjectView(Resource.Id.tvDate)]
        private TextView tvDate;

        [InjectView(Resource.Id.tvTitle)]
        private TextView tvTitle;

        [InjectView(Resource.Id.tvStatus)]
        private TextView tvStatus;

        [InjectView(Resource.Id.tvUsers)]
        private TextView tvUsers;

        [InjectView(Resource.Id.tvTime)]
        private TextView tvTime;

        [InjectView(Resource.Id.tvAddress)]
        private TextView tvAddress;

        [InjectOnClick(Resource.Id.tvAddress)]
        private void AddressClick(object s, EventArgs e)
        {
            var intent = new Intent(this, typeof(MemberLocationActivity));
            intent.PutExtra("EventDetail", content);
            intent.PutExtra("NumberPeople", tvUsers.Text);

            StartActivity(intent);
        }

        [InjectView(Resource.Id.tvDescription)]
        private TextView tvDescription;

        [InjectOnClick(Resource.Id.btnBack)]
        private void Back(object s, EventArgs e)
        {
            Finish();
        }

        private UserEventsController userEventsController = UserEventsController.Instance;

        private UnjoinEventFragment unjoinEventFragment = new UnjoinEventFragment();

        private string userId = AppDataController.Instance.UserId;

        private UserLoginEventModel eventDetail;

        private bool currentIsJoined;

        private string content;

        public EventDetailActivity()
        {
            unjoinEventFragment.NotGoing += UnjoinEventFragment_NotGoing;
        }

        private void UnjoinEventFragment_NotGoing(object sender, EventArgs e)
        {
            if (sender is Dialog dialog)
            {
                UpdateUserEvents(!currentIsJoined);

                dialog.Dismiss();
            }
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.EventDetail);

            Cheeseknife.Inject(this);

            content = Intent.GetStringExtra("EventDetail");

            eventDetail = JsonConvert.DeserializeObject<UserLoginEventModel>(content);
            
            tvTitle.Text = eventDetail.Title;
            tvDescription.Text = eventDetail.Description;
            tvTime.Text = "Time";
            tvAddress.Text = eventDetail.Place;
            tvMonth.Text = eventDetail.Time.ToString("MMM", CultureInfo.InvariantCulture);
            tvDate.Text = eventDetail.Time.Day.ToString();
            tvUsers.Text = "0";

            var count = 0;

            this.DoRequest(() => count = userEventsController.Values
                    .Where(x => x.EventId == eventDetail.Id).Count()
                , () => tvUsers.Text = count.ToString());

            currentIsJoined = eventDetail.IsJoined;

            if (eventDetail.Time < DateTime.Now)
            {
                tvStatus.Text = !eventDetail.IsJoined
                    ? Resources.GetString(Resource.String.event_happened)
                    : Resources.GetString(Resource.String.you_joined);
            }
            else
            {
                tvStatus.ChangeTextViewStatus(currentIsJoined);

                tvStatus.Click += (s, e) =>
                {
                    if (!currentIsJoined)
                    {
                        UpdateUserEvents(!currentIsJoined);
                    }
                    else
                    {
                        unjoinEventFragment.Show(SupportFragmentManager, null);
                    }
                };
            }
        }

        private void UpdateUserEvents(bool isJoined)
        {
            currentIsJoined = isJoined;
            tvStatus.ChangeTextViewStatus(isJoined);

            this.DoRequest(() =>
            {
                if (isJoined)
                {
                    userEventsController.Add(new UserEventModel()
                    {
                        EventId = eventDetail.Id,
                        UserId = userId
                    });
                }
                else
                {
                    var userEvent = userEventsController.Values
                        .First(x => x.EventId == eventDetail.Id && x.UserId == userId);

                    userEventsController.Delete(userEvent);
                }
            }, () => { });
        }
    }
}