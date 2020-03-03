# frozen_string_literal: true

require 'faraday'
require 'json'

BASE_URL = 'https://api.aweber.com/1.0/'
credentials = JSON.parse(File.read('./credentials.json'))

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
    collection.push(*page["entries"])
    url = page["next_collection_link"]
  end
  collection
end

# Get an account to search on
accounts = get_collection(conn, "#{BASE_URL}accounts")
account = accounts.first

lists_url = account['lists_collection_link']
lists = get_collection(conn, lists_url)
list = lists.first

origin_list = lists.first
destination_list = lists[1]

if !origin_list
  puts "You must have 2 lists to move a subscriber!"
  exit
end

subscribers = get_collection(conn, origin_list['subscribers_collection_link'])
subscriber = subscribers.first

unless subscriber
  puts "you must have a subscriber on list: #{origin_list['name']}"
  exit
end

data = {
  'ws.op' => 'move',
  'list_link' => destination_list['self_link']
}
puts "data #{data}"

conn.post(subscriber['self_link'], data.to_json) do |req|
  req.headers['Content-Type'] = 'application/json'
end

puts "Moved subscriber #{subscriber['email']} from list #{origin_list['name']} to list: #{destination_list['name']}"
