using Newtonsoft.Json;

namespace AWeber.Examples.Models
{
    public class List
    {
        [JsonProperty("id")] public string Id { get; set; }
        [JsonProperty("http_etag")] public string HttpEtag { get; set; }
        [JsonProperty("name")] public string Name { get; set; }
        [JsonProperty("campaigns_collection_link")] public string CampaignsCollectionLink { get; set; }
        [JsonProperty("custom_fields_collection_link")] public string CustomFieldsCollectionLink { get; set; }
        [JsonProperty("draft_broadcasts_link")] public string DraftBroadcastsLink { get; set; }
        [JsonProperty("scheduled_broadcasts_link")] public string ScheduledBroadcastsLink { get; set; }
        [JsonProperty("sent_broadcasts_link")] public string SentBroadcastsLink { get; set; }
        [JsonProperty("resource_type_link")] public string ResourceTypeLink { get; set; }
        [JsonProperty("segments_collection_link")] public string SegmentsCollectionLink { get; set; }
        [JsonProperty("self_link")] public string SelfLink { get; set; }
        [JsonProperty("subscribers_collection_link")] public string SubscribersCollectionLink { get; set; }
        [JsonProperty("total_subscribed_subscribers")] public int TotalSubscribedSubscribers { get; set; }
        [JsonProperty("total_subscribers")] public int TotalSubscribers { get; set; }
        [JsonProperty("total_subscribers_subscribed_today")] public int TotalSubscribersSubscribedToday { get; set; }
        [JsonProperty("total_subscribers_subscribed_yesterday")] public int TotalSubscribersSubscribedYesterday { get; set; }
        [JsonProperty("total_unconfirmed_subscribers")] public int TotalUnconfirmedSubscribers { get; set; }
        [JsonProperty("total_unsubscribed_subscribers")] public int TotalUnsubscribedSubscribers { get; set; }
        [JsonProperty("unique_list_id")] public string UniqueListId { get; set; }
        [JsonProperty("web_form_split_tests_collection_link")] public string WebFormSplitTestsCollectionLink { get; set; }
        [JsonProperty("web_forms_collection_link")] public string WebFormsCollectionLink { get; set; }
    }
}