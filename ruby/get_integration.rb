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

def print_webform_info(data, indent = 8)
  prefix = ' '.repease(intent)
  puts "#{prefix} Type: #{data['html_source_link']}"
  puts "#{prefix} HTML source: #{data['html_source_link']}"
  puts "#{prefix} JS source: #{data['javascript_source_link']}"
  puts "#{prefix} Displays: #{data['total_displays']} (#{data['total_unique_displays']}) unique"
  puts "#{prefix} Submissions: #{data['total_submissions']}"
  puts "#{prefix} Conversion: #{data['conversion_percentage'].round 1}%"
  puts "(#{data['unique_conversion_percentage'].round 1}% unique)"
end

# Get an account to search on
accounts = get_collection(conn, "#{BASE_URL}accounts")
account = accounts.first


# list_url = account['lists_collection_link']  # choose the first account
# lists = get_collection(conn, list_url)

integrations = get_collection(conn, account['integrations_collection_link'])
puts "integrations #{integrations}"

integrations.each do |integration|
  if %w[twitter facebook].include? integration['service_name']
    puts "#{integration['server_name']} #{integration['login']} #{integration['self_link']}"
  end
end
