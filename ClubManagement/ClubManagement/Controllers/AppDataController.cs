﻿using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Preferences;
using ClubManagement.Models;

namespace ClubManagement.Controllers
{
    public class AppDataController
    {
        public static AppDataController Instance = new AppDataController();

        private readonly string userId;

        private AppDataController()
        {
            userId = PreferenceManager.GetDefaultSharedPreferences(Application.Context)
                .GetString("UserId", string.Empty);
        }

        public int NumberOfUnpaidBudgets
        {
            get
            {
                var moneyList = MoneysController.Instance.Values ?? new List<MoneyModel>();
                var userMoneyList = UserMoneysController.Instance.Values ?? new List<UserMoneyModel>();
                return moneyList.Count - userMoneyList.Count(x => x.UserId == userId);
            }
        }

        public List <EventModel> UpcomingEvents 
        {
            get
            {
                var joinedEvents = (UserEventsController.Instance.Values ??
                                    new List<UserEventModel>()).Where(x => x.UserId == userId).ToList();

                return joinedEvents.Join(EventsController.Instance.Values ?? new List<EventModel>(),
                        j => j.EventId, e => e.Id, (j, e) => e)
                    .Where(e => e.Time > DateTime.Now).ToList();
            }
        }

        public List<MoneyState> GetListMoneyState()
        {
            var moneyStates = new List<MoneyState>();
            MoneysController.Instance.Values.ForEach(x => moneyStates.Add(new MoneyState
            {
                IsPaid = true,
                MoneyModel = x
            }));
            return moneyStates;
        }
    }
}