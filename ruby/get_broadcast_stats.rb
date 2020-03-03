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

puts "Broadcast id: #{broadcast['id']} Subject #{broadcast['subject']} Sent At: #{broadcast['sent_at']}"

stats = get_collection(conn, broadcast['stats_collection_link'])
stats.each do |stat|
  values = stat['value']
  puts values
  if values.is_a? Array
    puts "#{stat['description']}"
    puts "#{values}"

    values.each do |key, value|
      puts " VALUE #{value}"
      next if value.nil?
      value.each do |item_key, item_value|
        puts "    #{item_key}: #{item_value}"
      end
    end
  elsif values
    puts "#{stat['description']  }:#{values}"
  else
    puts "#{stat['description']}: N/A"
  end
end
