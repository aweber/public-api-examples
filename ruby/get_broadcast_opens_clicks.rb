# frozen_string_literal: true

require 'faraday'
require 'json'
require 'time'



MAX_RETRIES = 5
BASE_URL = 'https://api.aweber.com/1.0/'
credentials = JSON.parse(File.read('./credentials.json'))
puts credentials
conn = Faraday.new(
  headers: {
    Authorization: "Bearer #{credentials['access_token']}"
  }
) do |f|
  f.response :raise_error
end

def get_collection(conn, url)
  collection = []
  while url
    res = conn.get(url)
    page = JSON.parse(res.body)
    collection.push(*page['entries'])
    url = page['next_collection_link']
  end
  collection
end

def get_with_retry(url,credentials)
  Faraday.new(
    headers: {
      Authorization: "Bearer #{credentials['access_token']}"
    }
  ) do |f|
    f.request :retry, { max: MAX_RETRIES, interval: 2 }
  end.get(url)
end
accounts = get_collection(conn, "#{BASE_URL}accounts")

lists_url = accounts[0]['lists_collection_link']
lists = get_collection(conn, lists_url)

params = {
  'ws.op' => 'find',
  'campaign_type' => 'b'
}
campaigns_url = lists[0]['campaigns_collection_link']
broadcasts_url = "#{campaigns_url}?#{URI.encode_www_form(params)}"
puts broadcasts_url

sent_broadcasts = get_collection(conn, broadcasts_url)
broadcast = sent_broadcasts[0]
puts "Broadcast:"
puts broadcast

messages_url = broadcast['message_collection_link']
messages = get_collection(conn, messages_url)
subscriber_cache = {}

messages.each do |message|
  if message['total_opens'] > 0
    open_sub_link = message['subscriber_link']
    open_sub = get_with_retry(open_sub_link,credentials)
    open_sub_body = JSON.parse(open_sub.body)
    subscriber_cache[open_sub_link] = open_sub_body['email']
  end
end

links = get_collection(conn, broadcast['links_collection_link'])
puts "Clicks for broadcast:"
links.each do |link|
  clicks_url = link['clicks_collection_link']
  clicks = get_collection(conn, clicks_url)
  clicks.each do |click|
    click_sub_link = click['subscriber_link']
    cached_subscriber = subscriber_cache[click_sub_link]
    if(cached_subscriber)
      email = cached_subscriber['email']
    else
      click_sub = get_with_retry(click_sub_link,credentials)
      click_sub_body = JSON.parse(click_sub.body)
      email = click_sub_body['email']
    end
    puts "    #{click['event_time']}: #{email}"
  end
end
