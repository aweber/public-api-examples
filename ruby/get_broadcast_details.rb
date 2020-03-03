# frozen_string_literal: true

require 'faraday'
require 'json'
require 'time'



BASE_URL = 'https://api.aweber.com/1.0/'
MAX_RETRIES = 5
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
account = accounts[0]

lists = get_collection(conn, account['lists_collection_link'])

list_url = lists[0]['self_link']
%w[draft scheduled sent].each do |status|
  total_url = "#{list_url}/broadcasts/total?status=#{status}"
  total_response = get_with_retry(total_url, credentials)
  total_body = JSON.parse(total_response.body)
  total = total_body['total_size']
  puts "Total #{status} broadcasts #{total}"
end

%w[draft scheduled sent].each do |status|
  link_key = "#{status}_broadcast_link"
  broadcasts = get_collection(conn, lists[0][link_key])
  subject = broadcasts.dig(0, 'subject')
  puts "First #{status} broadcast subject: #{subject}"
end
