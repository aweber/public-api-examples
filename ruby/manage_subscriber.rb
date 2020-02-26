# frozen_string_literal: true

require 'faraday'
require 'json'
require 'time'

BASE_URL = 'https://api.aweber.com/1.0/'
credentials = JSON.parse(File.read('./credentials.json'))
puts credentials
conn = Faraday.new(
  headers: {
    Authorization: "Bearer #{credentials['access_token']}"
  }
) do |f|
  # f.response :raise_error
end

def get_collection(conn, url)
  puts "getting #{url}"
  collection = []
  while url
    res = conn.get(url)
    page = JSON.parse(res.body)
    collection.push(*page['entries'])
    url = page['next_collection_link']
  end
  collection
end

accounts = get_collection(conn, "#{BASE_URL}accounts")

lists_url = accounts[0]['lists_collection_link']
lists = get_collection(conn, lists_url)

email = 'example@example.com'
params = {
  'ws.op' => 'find',
  'email' => email
}
subs_url = lists[0]['subscribers_collection_link']
find_url = "#{subs_url}?#{URI.encode_www_form(params)}"
found_subscribers = get_collection(conn, find_url)

puts 'Found subscribers: '
puts found_subscribers

if found_subscribers.dig(0,'subscription_collection_link')
  data = {
    'custom_fields' => {
      'awesomeness' => 'really awesome'
    },
    'tags' => {
      'add' => ['prospect']
    }
  }
  subscriber_url = found_subscribers.first['self_link']
  subscriber_response = conn.patch(subscriber_url,JSON.generate(data)) do |req|
    req.headers['Content-Type'] = 'application/json'
  end
  subscriber = JSON.parse(subscriber_response.body)
  puts "Updated subscriber"
else
  data = {
    'email' => email,
    'custom_fields' => {
      'awesomeness' => 'somewhat'
    },
    'tags' => ['prospect']
  }
  puts "subs_url #{subs_url}"
  puts "json data #{data.to_json}"
  body = conn.post(subs_url, JSON.generate(data)) do |req|
    req.headers['Access-Control-Expose-Headers'] = 'Location'
    req.headers['Content-Type'] = 'application/json'
  end
  subscriber_url = body.headers['location']
  subscriber_response = conn.get(subscriber_url)
  subscriber = JSON.parse(subscriber_response.body)
  puts 'Created Subscriber:'
end
puts subscriber

params = {
  'ws.op' => 'getActivity'
}

activity_url = "#{subscribe_url}?#{URI.encode_www_form(params)}"
activity = conn.get(activity_url)
puts "Subscriber Activity:"
puts JSON.parse(activity.body)

if subscriber['status'] == 'subscribed'
  conn.delete(subscriber_url)
  puts "Deleted subscriber with email: #{email}"
end
