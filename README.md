# public-api-examples
This repository contains examples for using AWeber's API to perform certain
actions. Each sub-directory contains example scripts in a specific
programming language.

## Pre-requisites
You need an **AWeber Developer account** as well as an **AWeber Customer
account** in order to run the examples. The AWeber API uses OAuth 2.0 to
control and secure access to our system. Each sub-directory contains a script
that will generate the necessary OAuth credentials but you need access to
**both** the AWeber Developer and Customer accounts to generate a credential
set.

**AWeber Developer accounts** are free accounts used to create applications
for use by *AWeber Customers*.  You can create a new *developer* account or
access an existing one at <https://labs.aweber.com/>.

**AWeber Customer accounts** are used by small businesses, entrepreneurs,
and email marketers to maintain relationships with their customers.  Customer
accounts applications created by developers to automate the management of
their customer data or integrate their AWeber account [with other services].
You can get a *trial customer* account at <https://www.aweber.com/order.htm>.

## Sample scripts
Once you have an *AWeber Developer account* and access to an *AWeber Customer
account*, you need to create an **application** in the *AWeber Developer
account*.  Applications are easy to create so you can simply create one to run 
examples with.  Log in to your Developer account and go to <https://labs.aweber.com/apps> 
to create a new application.  You will need the **Client ID** and **Client Secret**
that are displayed for the application to run the examples.

### Retrieving access tokens
Each sub-directory contains a script named **get-access-tokens** that will
connect your application to an *AWeber Customer account* and record the
generated tokens in a file that is used by the other scripts.  You will need
to run this script *before* you can run the other scripts.  The following
transcript is an example of running this script:

    prompt$ ./get-access-token
    Enter your client ID: pT9ObgUOU8E8jbDzSiejjEFC
    Enter your secret secret: *******
    Go to this url: https://auth.aweber.com/oauth2/authorize?oauth_token=...
    Log in and paste the returned URL here: https://localhost?code=iowejf2...
    Updated credentials.json with your new credentials

You enter the **Client ID** and **Client Secret** associated with your
application.  The Client ID is shown as `pT9ObgUOU8E8jbDzSiejjEFC` in the
example.  The script will initiate the OAuth handshake and generate a URL to
complete the connection.  Open the URL in a web browser to connect your
application to an *AWeber Customer account*.  After logging in you will be
directed to a page containing a **authorization code**. you will copy and
paste the URL when running the *get-access-tokens* script.  Once this process 
is completed, the OAuth tokens are stored in a file that is read in by the 
other scripts so that you do not need to paste the access tokens into each script.

### Running the examples
Once you have retrieved the access tokens, you can run any of the scripts
in the sub-directory that you retrieved access tokens in.  See the *README*
in each sub-directory for programming language specific instructions.


[with other services]: https://www.aweber.com/integrations/
