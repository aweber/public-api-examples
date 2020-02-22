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
puts account

lists_url = account['lists_collection_link']
lists = get_collection(conn, lists_url)
list = lists.first

custom_fields_url = list['custom_fields_collection_link']
custom_fields = get_collection(conn, custom_fields_url)

custom_fields.each do |entry|
  if ["Test2","Renamed2"].include? entry['name']
    puts "A custom field called #{entry['name']} already exists on #{list['name']}"
    exit
  end
end

create_response = conn.post(
  custom_fields_url,
  'ws.op' => 'create',
  'name' => 'Test2'
) do |req|
  req.headers['Content-Type'] = 'application/x-www-form-urlencoded'
  req.headers['Access-Control-Expose-Headers'] = 'Location'
end

field_url = create_response.headers['location']
puts "Create new custom field at #{field_url}"
update_response = conn.patch(
  field_url,
  'name' => 'Renamed2',
  'is_subscriber_updateable' => true
) do |req|
  req.headers['Content-Type'] = 'application/json'
end
update_field = JSON.parse(update_response.body)
puts "Update the custom field: "
puts updated_field

field_url_response = conn.delete(field_url)
puts "delete response #{field_url_response.body}"
puts "deleted the custom field"


