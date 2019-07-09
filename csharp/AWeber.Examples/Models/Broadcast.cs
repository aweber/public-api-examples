using System.Collections.Generic;
using Newtonsoft.Json;

namespace AWeber.Examples.Models
{
    public class Broadcast
    {
        [JsonProperty("archive_url")] public string ArchiveUrl { get; set; }
        [JsonProperty("body_html")] public string BodyHtml { get; set; }
        [JsonProperty("body_text")] public string BodyText { get; set; }
        [JsonProperty("body_amp")] public string BodyAmp { get; set; }
        [JsonProperty("broadcast_id")] public int BroadcastId { get; set; }
        [JsonProperty("click_tracking_enabled")] public bool ClickTrackingEnabled { get; set; }
        [JsonProperty("created_at")] public string CreatedAt { get; set; }
        [JsonProperty("exclude_lists")] public IList<string> ExcludeLists { get; set; }
        [JsonProperty("facebook_integration")] public string FacebookIntegration { get; set; }
        [JsonProperty("has_customized_body_text")] public bool HasCustomizedBodyText { get; set; }
        [JsonProperty("include_lists")] public IList<string> IncludeLists { get; set; }
        [JsonProperty("is_archived")] public bool IsArchived { get; set; }
        [JsonProperty("links")] public IList<HtmlLink> Links { get; set; }
        [JsonProperty("notify_on_send")] public bool NotifyOnSend { get; set; }
        [JsonProperty("scheduled_for")] public string ScheduledFor { get; set; }
        [JsonProperty("segment_link")] public string SegmentLink { get; set; }
        [JsonProperty("segment_name")] public string SegmentName { get; set; }
        [JsonProperty("self_link")] public string SelfLink { get; set; }
        [JsonProperty("sent_at")] public string SentAt { get; set; }
        [JsonProperty("stats")] public BroadcastStats Stats { get; set; }
        [JsonProperty("status")] public string Status { get; set; }
        [JsonProperty("subject")] public string Subject { get; set; }
        [JsonProperty("twitter_integration")] public string TwitterIntegration { get; set; }
    }
}