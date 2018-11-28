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

    consumerKey = ****
    consumerSecret = ****
    accessKey = ****
    accessSecret = ****

You can also create the *credentials.ini* file using your existing consumer key and secret from the
[My Apps page](https://labs.aweber.com/apps):

    prompt$ php get-access-token
    Enter your consumer key: ****
    Enter your consumer secret: ****
    Go to this url: https://auth.aweber.com/1.0/oauth/authorize?oauth_token=****
    Log in and paste the returned verifier code here: ****
    Updated credentials.ini with your new credentials
    
The rest of the scripts will use this file for authentication. 

[CONTRIBUTING]: https://github.com/aweber/public-api-examples/blob/master/CONTRIBUTING.md

