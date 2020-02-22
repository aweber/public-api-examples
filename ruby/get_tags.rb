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
accounts = get_collection(conn, 'accounts')


lists_url = accounts[0]['lists_collection_link']  # choose the first account
list_name = 'abc'
params = {
  'ws.op' => 'find',
  'name' => list_name
}
find_list_url = "#{lists_url}?#{URI.encode_www_form(params)}"
lists = get_collection(conn, find_list_url)
puts(lists)

if lists[0] && lists[0]['self_link']
  tag_url = lists[0]['self_link'] + '/tags'# choose the first list
  response = conn.get(tag_url)

  tags = JSON.parse(response.body)

  puts(tags)
else
  puts "Could not find a list with name: #{listName}"
end
