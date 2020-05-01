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

lists_url = accounts[0]['lists_collection_link']
lists = get_collection(conn, lists_url)
list = lists.first

segments = get_collection(conn, list['segments_collection_link'])
puts "segments #{segments}"

segments.each do |segment|
  puts "#{segment['name']} #{segment['self_link']}"
end
