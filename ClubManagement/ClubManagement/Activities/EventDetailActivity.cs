﻿using Android.App;
using Android.OS;
using Android.Preferences;
using Android.Widget;
using ClubManagement.Controllers;
using ClubManagement.Models;
using ClubManagement.Ultilities;
using Newtonsoft.Json;
using System.Linq;

namespace ClubManagement.Activities
{
    [Activity(Label = "EventDetailActivity", Theme = "@style/AppTheme")]
    public class EventDetailActivity : Activity
    {
        [InjectView(Resource.Id.tvTitle)]
        private TextView tvTitle;

        [InjectView(Resource.Id.tvDescription)]
        private TextView tvDescription;

        [InjectView(Resource.Id.tvTime)]
        private TextView tvTime;

        [InjectView(Resource.Id.tvPlace)]
        private TextView tvPlace;

        [InjectView(Resource.Id.btnJoin)]
        private Button btnJoin;

        [InjectView(Resource.Id.tvCreatedBy)]
        private TextView tvCreatedBy;

        private UserLoginEventModel eventDetail;

        private bool currentIsJoined;

        private UserEventsController userEventsController = UserEventsController.Instance;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.EventDetail);

            Cheeseknife.Inject(this);

            var content = Intent.GetStringExtra("EventDetail");

            eventDetail = JsonConvert.DeserializeObject<UserLoginEventModel>(content);

            
            tvCreatedBy.Text = $"Created by {eventDetail.CreatedBy} in {eventDetail.CreatedTime}";
            tvTitle.Text = eventDetail.Title;
            tvDescription.Text = eventDetail.Description;
            tvTime.Text = eventDetail.Time.ToShortDateString();
            tvPlace.Text = eventDetail.Place;

            currentIsJoined = eventDetail.IsJoined;

            btnJoin.ChangeStatusButtonJoin(currentIsJoined);

            btnJoin.Click += (s, e) =>
            {
                currentIsJoined = !currentIsJoined;
                btnJoin.ChangeStatusButtonJoin(currentIsJoined);
                UpdateUserEvents(currentIsJoined);
            };
        }

        private void UpdateUserEvents(bool isJoined)
        {
            var preferences = PreferenceManager.GetDefaultSharedPreferences(Application.Context);
            var userId = preferences.GetString("UserId", string.Empty);

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
        }
    }
}