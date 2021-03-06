﻿using System;
using System.Collections.Generic;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using ClubManagement.Models;

namespace ClubManagement.CustomAdapters
{
    public class MoneyAdminListAdapter : RecyclerView.Adapter
    {
        public MoneyAdminListAdapter(string moneyId)
        {
            MoneyId = moneyId;
        }

        private List<MoneyAdminState> moneyAdminStates = new List<MoneyAdminState>();

        public event EventHandler ItemPayClick;

        public string MoneyId { get; set; }

        public List<MoneyAdminState> MoneyAdminStates
        {
            get => moneyAdminStates;
            set
            {
                moneyAdminStates = value;
                NotifyDataSetChanged();
            }
        }

        private readonly TextView tvState;

        private readonly string moneyId;

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            if (holder is MoneyAdminViewHolder viewHolder)
            {
                viewHolder.MoneyAdminState = moneyAdminStates[position];
                viewHolder.MoneyId = MoneyId;
                viewHolder.PayClick += ItemPayClick;
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var itemView = LayoutInflater.From(parent.Context)
                .Inflate(Resource.Layout.recyclerview_money_admin_item, parent, false);
            return new MoneyAdminViewHolder(itemView);
        }

        public override int ItemCount => moneyAdminStates.Count;
    }
}