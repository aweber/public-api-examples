#!/usr/bin/env php
<?php
require 'vendor/autoload.php';
use GuzzleHttp\Client;
use GuzzleHttp\HandlerStack;
use GuzzleHttp\Subscriber\Oauth\Oauth1;

const OAUTH_URL = 'https://auth.aweber.com/1.0/oauth/';

echo 'Enter your consumer key: ';
$consumerKey = rtrim(fgets(STDIN), PHP_EOL);
echo 'Enter your consumer secret: ';
$consumerSecret = rtrim(fgets(STDIN), PHP_EOL);

// Create a Guzzle client configured to use OAuth for authentication
$stack = HandlerStack::create();
$client = new Client([
    'base_uri' => OAUTH_URL,
    'handler' => $stack,
    'auth' => 'oauth'
]);

// Use your consumer key and secret to get a request token and secret
$requestMiddleware = new Oauth1([
    'consumer_key'    => $consumerKey,
    'consumer_secret' => $consumerSecret,
]);
$stack->push($requestMiddleware);
$res = $client->post('request_token', ['form_params' => ['oauth_callback' => 'oob']]);
$params = [];
parse_str($res->getBody(), $params);

// Log in with to receive a verifier code for the request token
echo "Go to this url: " . OAUTH_URL . "authorize?oauth_token={$params['oauth_token']}\n";
echo 'Log in and paste the returned verifier code here: ';
$verifier = rtrim(fgets(STDIN), PHP_EOL);

// Trade the request token and secret and verifier for an access code and secret
$accessMiddleware = new Oauth1([
    'consumer_key'    => $consumerKey,
    'consumer_secret' => $consumerSecret,
    'token' => $params['oauth_token'],
    'token_secret' => $params['oauth_token_secret'],
]);
$stack->remove($requestMiddleware);
$stack->push($accessMiddleware);
$res = $client->post('access_token', ['form_params' => ['oauth_verifier' => $verifier]]);
$credentials = [];
parse_str($res->getBody(), $credentials);

$fp = fopen('credentials.ini', 'wt');
fwrite($fp,
"consumerKey = {$consumerKey}
consumerSecret = {$consumerSecret}
accessToken = {$credentials['oauth_token']}
tokenSecret = {$credentials['oauth_token_secret']}
");
fclose($fp);
echo "Updated credentials.ini with your new credentials\n";
