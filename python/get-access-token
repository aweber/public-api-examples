#!/usr/bin/env python
from __future__ import print_function
import getpass
import json

from requests_oauthlib import OAuth1Session

try:
    input = raw_input  # use raw_input in python2
except NameError:
    pass

OAUTH_URL = 'https://auth.aweber.com/1.0/oauth/'


def main():
    consumer_key = input('Enter your consumer key: ')
    consumer_secret = getpass.getpass('Enter your consumer secret: ')

    # Use your consumer key and secret to get a request token and secret
    oauth = OAuth1Session(consumer_key,
                          client_secret=consumer_secret,
                          callback_uri='oob')
    oauth.fetch_request_token(OAUTH_URL + 'request_token')

    # Log in to receive a verifier code for the request token
    href = oauth.authorization_url(OAUTH_URL + 'authorize')
    print('Go to this url: ' + href)
    verifier = input('Log in and paste the returned verifier code here: ')
    try:
        verifier = verifier.decode()  # decode byte string in python2
    except AttributeError:
        pass

    # Trade the request token/secret and verifier for an access token/secret
    credentials = oauth.fetch_access_token(OAUTH_URL + 'access_token',
                                           verifier)

    with open('credentials.json', 'wt') as creds_file:
        json.dump({'consumer_key': consumer_key,
                   'consumer_secret': consumer_secret,
                   'access_token': credentials['oauth_token'],
                   'token_secret': credentials['oauth_token_secret']},
                  creds_file)
    print('Updated credentials.json with your new credentials')


if __name__ == '__main__':
    main()
