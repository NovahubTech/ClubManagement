﻿using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ClubManagement.Models
{
    public class OutcomeModel : FirebaseModel
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("amount")]
        public long Amount { get; set; }

        [JsonProperty("date")]
        public DateTime Date { get; set; }

        [JsonProperty("items")]
        public List<OutcomeAmountItem> Items { get; set; } = new List<OutcomeAmountItem>();
    }
}