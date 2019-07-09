using System;
using Newtonsoft.Json;

namespace AWeber.Examples.Models
{
    public class Campaign
    {
        [JsonProperty("campaign_type")] public string Type { get; set; }
        [JsonProperty("click_tracking_enabled")] public bool ClickTrackingEnabled { get; set; }
        [JsonProperty("content_type")] public string ContentType { get; set; }
        [JsonProperty("http_etag")] public string HttpEtag { get; set; }
        [JsonProperty("id")] public string Id { get; set; }
        [JsonProperty("is_archived")] public bool IsArchived { get; set; }
        [JsonProperty("links_collection_link")] public string LinksCollectionLink { get; set; }
        [JsonProperty("message_interval")] public int MessageInterval { get; set; }
        [JsonProperty("message_number")] public int MessageNumber { get; set; }
        [JsonProperty("messages_collection_link")] public string MessagesCollectionLink { get; set; }
        [JsonProperty("resource_type_link")] public string ResourceTypeLink { get; set; }
        [JsonProperty("scheduled_at")] public DateTime ScheduledAt { get; set; }
        [JsonProperty("self_link")] public string SelfLink { get; set; }
        [JsonProperty("sent_at")] public DateTime SentAt { get; set; }
        [JsonProperty("spam_assassin_score")] public double SpamAssassinScore { get; set; }
        [JsonProperty("stats_collection_link")] public string StatsCollectionLink { get; set; }
        [JsonProperty("subject")] public string Subject { get; set; }
        [JsonProperty("total_clicks")] public int TotalClicks { get; set; }
        [JsonProperty("total_opens")] public int TotalOpens { get; set; }
        [JsonProperty("total_sent")] public int TotalSent { get; set; }
        [JsonProperty("total_spam_complaints")] public int TotalSpamComplaints { get; set; }
        [JsonProperty("total_undelivered")] public int TotalUndelivered { get; set; }
        [JsonProperty("total_unsubscribes")] public int TotalUnsubscribes { get; set; }
        [JsonProperty("twitter_account_link")] public string TwitterAccountLink { get; set; }
    }
}