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

broadcasts_url = lists[0]['sent_broadcasts_link']
puts broadcasts_url

sent_broadcasts = get_collection(conn, broadcasts_url)
broadcast_response = get_with_retry(sent_broadcasts[0]['self_link'], credentials)
broadcast = JSON.parse(broadcast_response.body)
puts "Broadcast:"
puts broadcast

puts "Opens for broadcast:"
opens_url = broadcast['opens_collection_link']
opens = get_collection(conn, opens_url)

opens.each do |open|
  puts "    [#{open['event_time']}]: #{open['email']}"
end

puts "Clicks for broadcast:"
clicks_url = broadcast['clicks_collection_link']
clicks = get_collection(conn, clicks_url)

clicks.each do |open|
  puts "    [#{open['event_time']}]: #{open['email']}"
end
