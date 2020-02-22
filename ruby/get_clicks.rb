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

lists_url = accounts.first['lists_collection_link']
lists = get_collection(conn, lists_url)

params = {
  'ws.op' => 'find',
  'campaign_type' => 'b'
}
campaigns_url = lists.first['campaigns_collection_link']
campaigns = get_collection(conn, campaigns_url)
campaign = campaigns.first
type = campaign_type_name(campaign['campaign_type'])

puts "Clicks for #{type} #{campaign['subject']} by link:"

subscriber_emails = {}

links_url = campaign['links_collection_link']

links.each do |link|
  puts link['url']
  clicks_url = link['clicks_collection_link']
  clicks = get_collection(conn, clicks_url)

  clicks.each do |click|
    subscriber_url = click['subscriber_link']

    if subscriber_emails[subscriber_url]
      subscriber_response = con.get(subscriber_url)
      subscriber = JSON.parse(subscriber_response.body)
      subscriber_emails[subscriber_url] = subscriber['email']
    end
    email = subscriber_emails[subscriber_url]
    puts "    #{click['event_time']}: #{email}"
  end

end
