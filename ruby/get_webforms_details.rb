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

lists = get_collection(conn, account['list_collection_link'])


if lists.empty?
  puts "no lists"
  exit
end
list = lists.first


webforms = get_collection(conn, list['web_forms_collection_link'])

puts "Webforms for #{list['name']}"
if webforms.empty?
  puts "No webforms for #{list['name']}"
  exit
end

puts "Webforms for #{list['name']}:"
webforms.each do |webform|
  puts "    #{webform['name']}"
  print_webform_info webform
end

puts ""

split_tests = get_collection(conn, list['web_form_split_tests_collection_link'])

if split_tests.empty?
  puts "No webform split tests for #{list['name']}"
  exit
end

puts "Webform split tests for #{list['name']}:"

split_tests.each do |split_test|
  puts "    #{split_test['name']}: (#{split_test['javascript_source_link']})"
  components = get_collection(conn, split_test['components_collection_link'])
  components.each do |component|
    puts "    #{component['name']} (#{component['weight']}%)"
    print_webform_info(component, 12)
  end
end
