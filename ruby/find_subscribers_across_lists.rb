# frozen_string_literal: true

require 'faraday'
require 'json'
require 'time'

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
    collection.push(*page['entries'])
    url = page['next_collection_link']
  end
  collection
end

accounts =  get_collection(conn, "#{BASE_URL}accounts")

account_url = accounts[0]['self_link']  # choose the first account
params = {
  'ws.op' => 'findSubscribers',
  'email' => 'example@example.com'
}
find_url = "#{account_url}?#{URI.encode_www_form(params)}"
puts find_url
found_subscribers = get_collection(conn, find_url)
puts found_subscribers
