# Node.js code examples

This directory contains executable examples of how to use https://api.aweber.com/ from within PHP code.

If you haven't read [CONTRIBUTING], please take a moment to read it now.  It describes the basic configuration and process and this document picks up where it leaves off.

## Quickstart 

These examples use npm for dependency management, it comes installed with node.js.

## Installing requirements

Dependencies are in `package.json`. `npm install`


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

you can also create this file by running `node get-access-token.js`


## Refreshing tokens
If your access token is expired, you can use the `node get-access-token.js` script to refresh it.
