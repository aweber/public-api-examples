
# Ruby code examples

This directory contains executable examples of how to use https://api.aweber.com/ from within ruby code.

If you haven't read [CONTRIBUTING], please take a moment to read it now.  It describes the basic configuration and process and this document picks up where it leaves off.

## Quickstart 

These examples use gem for dependency management, it comes installed with ruby.

## Installing requirements

```bash
gem install oauth2
gem install tty-prompt
```




## Running examples

If you already have a consumer key/secret pair, and acces token/secret pair, you can create a *credentials.json* in this directory:

```
{
  "clientId":"...",
  "clientSecret":"...",
  "accessToken":"...",
  "refreshToken":"..."
}
```

you can also create this file by running `ruby get_access_token.rb`


## Refreshing tokens
If your access token is expired, you can use the `ruby get_access_token.rb` script to refresh it.
