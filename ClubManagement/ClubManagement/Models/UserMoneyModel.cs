﻿using Newtonsoft.Json;

namespace ClubManagement.Models
{
    public class UserMoneyModel : FirebaseModel
    {
        [JsonProperty("userId")]
        public string UserId { get; set; }

        [JsonProperty("moneyId")]
        public string MoneyId { get; set; }
    }
}