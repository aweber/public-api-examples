# frozen_string_literal: true

require 'faraday'
require 'json'
require 'time'

# Set the following to real values or adding the purchase will fail
EMAIL = 'example@example.com'
IP_ADDRESS = '127.0.0.1'  # required to be a PUBLIC IP address

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

event_time = Time.new()
purchase = {
  'event_time' => event_time.iso8601,
  'email' => EMAIL,
  'ip_address' => IP_ADDRESS,
  'currency' => 'USD',
  'event_note' => 'event identifier for purchase',
  'product_name' => 'product purchased',
  'value' => 1.0,
  'vendor' => 'my-vendor',
  'url' => 'https://example.com/link/to/product',
  'ad_tracking' => 'short note',
  'custom_fields' => {},
  'misc_notes' => 'some notes about the subscriber',
  'name' => 'Jane Doe',
  'tags' => []
}
response = conn.post(lists[0]['self_link'] + '/purchases', JSON.generate(purchase)) do |req|
  req.headers['Content-Type'] = 'application/json'
end
puts response.body

activity_url = response.headers['Location']

target_time = event_time.floor  # sub-seconds are truncated in tracked events
while true
  response = conn.get(activity_url)
  body = JSON.parse(response.body)
  matches = body['entries'].filter{|elm| target_time.eql? Time.parse(elm['event_time'])}
  if matches.any?
    puts "Found match #{matches}"
    break
  end
  puts 'event not found, sleeping'
  sleep 2.5
end
