# PHP code examples

This directory contains executable examples of how to use https://api.aweber.com/ from within PHP code.

If you haven't read [CONTRIBUTING], please take a moment to read it now.  It describes the basic configuration and process and this document picks up where it leaves off.

## Quickstart
These examples use composer for dependency management.  [Download composer](https://getcomposer.org/download/) if
you don't have it already.

## Installing requirements
The requirements are in *composer.json*. Install them using composer:

    composer install
    
## Running examples
If you already have a consumer key/secret pair and access token/secret pair, you can create *credentials.ini* in
this directory:

    clientId = '****'
    clientSecret = '****'
    accessToken = '****'
    refreshToken = '****'

You can also create the *credentials.ini* file using your existing client ID and secret from the
[My Apps page](https://labs.aweber.com/apps):

    prompt$ php get-access-token
    Do you wish to create(c) tokens or refresh(r) tokens? c
    Enter your client ID: ****
    Enter your client secret: ****
    Go to this url: https://auth.aweber.com/oauth2/authorize?******
    Log in and paste the returned URL here: ****
    Updated credentials.ini with your new credentials
    
The rest of the scripts will use this file for authentication. 

## Refreshing tokens
If your access token is expired, you can use the `get-access-token` script to refresh it.

    prompt$ php get-access-token
    Do you wish to create(c) tokens or refresh(r) tokens? r
    Updated credentials.ini with your new credentials

[CONTRIBUTING]: https://github.com/aweber/public-api-examples/blob/master/CONTRIBUTING.md

