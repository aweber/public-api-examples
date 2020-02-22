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

data = {
  # the broadcast html, this can be provided without body_text
  'body_html' => '<html><body>An html version of my email</body></html>',

  # if provided, this will be the plain text version of the email
  # if this is not provided, it will be auto-generated based on the body_html
  'body_text' => 'A plain text version of my email',

  # this is the broadcast subject line
  'subject' => 'This is an email created by the api!',

  # if there are links in this broadcast, track them
  'click_tracking_enabled' => 'true',

  # include or exclude subscribers on other lists in this broadcast
  # these are lists of URI's or list links
  'exclude_lists' => [],
  'include_lists' => [],

  # this means the broadcast will be available via a url after it's sent
  'is_archived' => 'true',

  # if notifications are enabled and notify_on_send is True, send an email
  # to the AWeber account holder when this broadcast stats' are available
  'notify_on_send' => 'true',

}

# Get an account to search on
accounts = get_collection(conn, "#{BASE_URL}accounts")

# Get a list to find broadcasts on
lists = get_collection(conn, accounts[0]['lists_collection_link'])

# if enabled, get the facebook url to share the broadcast to facebook
integrations = get_collection( conn, accounts[0]['integrations_collection_link']);

integrations.each do |integration|
  if integration['service_name'].downcase == 'facebook'
    # there could be multiple, so pick the first one we find and break
    data['facebook_integration'] = integration['self_link']
    break
  end
end

# make the broadcast on the first list
broadcast_response = conn.post(
  "#{lists[0]['self_link']}/broadcasts",
  data
) do |req|
  req.headers['Content-Type'] = 'application/x-www-form-urlencoded'
end
broadcast = JSON.parse(broadcast_response.body)
puts "broadcast #{broadcast}"

# schedule the broadcast we made
timestamp = Time.now
puts timestamp
scheduled_for = timestamp.utc.iso8601 # must be iso8601 compliant
puts scheduled_for
puts "self link #{broadcast['self_link']}"

conn.post("#{broadcast['self_link']}/schedule", scheduled_for: scheduled_for) do |req|
  req.headers['Content-Type'] = 'application/x-www-form-urlencoded'
end

# retrieve the scheduled broadcast to see the updated scheduled_for
scheduled_response = conn.get(broadcast['self_link'])
schedule_broadcast = JSON.parse(scheduled_response.body)
puts("scheduled prodcast #{schedule_broadcast}")

puts "Scheduled broadcast subject: #{schedule_broadcast['subject']} on list: #{lists[0]['name']}"
puts " to be sent at: #{schedule_broadcast['scheduled_for']}"
