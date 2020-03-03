require 'oauth2'
require "uri"
require "tty-prompt"

prompt = TTY::Prompt.new
createRefresh = prompt.ask("Do you wish to create(c) tokens or refresh(r) tokens?:")

SITE_URL = "https://auth.aweber.com"
AUTH_URL = '/oauth2/authorize'
TOKEN_URL = '/oauth2/token'
REDIRECT_URI = 'http://localhost:3000/callback'

scopes = [
    'account.read',
    'list.read',
    'list.write',
    'subscriber.read',
    'subscriber.write',
    'email.read',
    'email.write',
    'subscriber.read-extended'
]
SCOPE = scopes.join(" ")
puts SCOPE
class Encoder

  def self.encode(hash)
    URI.encode_www_form(hash)
  end

  def self.decode(string)
    URI.decode_www_form(hash)
  end
end
if createRefresh.upcase == "C"
    credentials = {}
    credentials[:client_id]  = prompt.ask("client id:")
    credentials[:client_secret] = prompt.ask("client secret:")

    client = OAuth2::Client.new(
        credentials[:client_id], 
        credentials[:client_secret], 
        :site => SITE_URL,
        :authorize_url => AUTH_URL,
        :token_url => TOKEN_URL,
        :connection_opts => {
          :request => {
            params_encoder: Encoder
          }
        },
        :scope => SCOPE
    )

    auth_url = client.auth_code.authorize_url(
      :redirect_uri => REDIRECT_URI,
      :scope => SCOPE
    )
    puts auth_url
    response_url = prompt.ask("visit the url above and paste the result here after you login:")

    code = CGI.parse(URI.parse(response_url).query)["code"].first

    puts "auth code is:"
    puts code

    token = client.auth_code.get_token(
        code,
        :redirect_uri => REDIRECT_URI
    )
    credentials[:access_token] = token.token
    credentials[:refresh_token] = token.refresh_token
else
    credentials = JSON.parse(File.read("./credentials.json"))

    client = OAuth2::Client.new(
        credentials["client_id"],
        credentials["client_secret"],
        :site => SITE_URL,
        :authorize_url => AUTH_URL,
        :token_url => TOKEN_URL,
        :scope => SCOPE
    )
    puts credentials["refresh_token"]
    token = OAuth2::AccessToken.new(
        client,
        credentials["access_token"],
        {
            refresh_token: credentials["refresh_token"],
            :scope => SCOPE
        }
    )
    new_token = token.refresh!(:scopes => SCOPE)

    credentials[:access_token] = new_token.token
    credentials[:refresh_token] = new_token.refresh_token

end

File.open("./credentials.json","w") do |f|
    f.write(credentials.to_json)
end

