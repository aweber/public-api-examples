const fetch = require("node-fetch");
const credentials = require("./credentials.json");
const BASE_URL = 'https://api.aweber.com/1.0/';
const client_id = credentials['clientId'];
const client_secret = credentials['clientSecret'];
const accessToken = credentials['accessToken'];

const qs = require("querystring");

/**
 * Get all of the entries for a collection
 *
 * @param string $accessToken Access token to pass in as an authorization header
 * @param string $url Full url to make the request
 * @return array Every entry in the collection
 */

async function getCollection(accessToken,url) {
    let res;
    const collection = [];
    console.log({accessToken})
    while(url) {
        res = await fetch(url,{
            headers:{
                "Authorization":`Bearer ${accessToken}`
            }
        });
        let page = await res.json();
        console.log("got page",{page})
        collection.push(...page.entries);
        url = page.next_collection_link;
    }
    return collection;
}

/**
 * Get a URL, retrying on rate limit errors
 *
 * @param Client client guzzle client instance
 * @param string url URL to get
 * @return \Psr\Http\Message\ResponseInterface
 */
const MAX_RETRIES = 5;
const wait = (ms) => new Promise(resolve => setTimeout(resolve,ms));
// throw an error if non success response
async function request(url,options) {
    if (options == null) options = {};
    if (options.credentials == null) options.credentials = 'same-origin';
    return fetch(url, options).then(function(response) {
        if (response.status >= 200 && response.status < 300) {
            return Promise.resolve(response);
        } else {
            var error = new Error(response.statusText || response.status);
            error.response = response;
            return Promise.reject(error);
        }
    });
}
async function getWithRetry(accessToken, url) {
    for (let i = 0;i< MAX_RETRIES; i++ ) {
        try {
            const response = await request(url,{
                'headers' : {
                    'Authorization': 'Bearer ' + accessToken
                }
            });
            return response;

        } catch(e) {
            // Only retry on a 403 (forbidden) status code with a rate limit error
            if (!e.response || e.response.statusCode !== 403) {
                throw e;
            }

            const body = await e.response.json();

            if (!body['error']['message'].match(/rate limit/i)) {
                throw new Error(body['error']['message']);
            }
            console.log("Request was rate limited\n");
            if (i < MAX_RETRIES) {
                // Wait longer between every attempt
                await wait(2 * i * 1000);
                console.log(`Retry ${i}...\n`);
            }
            continue;
        }
    }

    throw new Error ("Giving up after " + MAX_RETRIES + " tries");
}

(async () => {

    // get all the accounts entries
    const accounts = await getCollection(accessToken, BASE_URL + 'accounts');
    console.log({accounts})
    const accountUrl = accounts[0]['self_link'];

    // get all the list entries for the first account
    const listsUrl = accounts[0]['lists_collection_link'];
    const lists = await getCollection( accessToken, listsUrl);

    // get a sent broadcast
    const params =  {
        'ws.op' : 'find',
        'campaign_type':  'b'
    }
    const campaignsUrl = lists[0]['campaigns_collection_link'];
    const broadcastsUrl = campaignsUrl + '?' + qs.stringify(params);
    console.log(broadcastsUrl)
    const sentBroadcasts = await getCollection( accessToken, broadcastsUrl);
    const broadcast = sentBroadcasts[0];
    console.log ('Broadcast: ');
    console.log(broadcast);

    // A broadcast is the receipt of sending email messages to a list.
    // Messages are each email sent to a subscriber; they
    // can be opened and clicked by each subscriber multiple times.
    const messagesUrl = broadcast['messages_collection_link'];
    const messages = await getCollection(accessToken, messagesUrl);

    // mapping of subscriber url to email address
    const subscriberCache = {};

    console.log ("Opens for broadcast:");
    for (let message of messages) {
        if (message['total_opens'] > 0) {
            // You could also paginate the opens collection of each message,
            // but the open count and the last_opened timestamp are in the message entry
            // so unless we need exact times of each non-unique open, we use the message entry.
            const openSubLink = message['subscriber_link'];
            const openSub = await getWithRetry( accessToken, openSubLink);
            const openSubBody = await openSub.json();
            console.log (`     ${message['last_opened']}: ${openSubBody['email']}`);
            // First time looking up a subscriber; save them for next time
            subscriberCache[openSubLink] = openSubBody['email'];
        }
    }

    const links = await getCollection(accessToken, broadcast['links_collection_link']);

    console.log ("Clicks for broadcast:");
    for (let link of links) {
        console.log( "{link['url']}\n");
        const clicksUrl = link['clicks_collection_link'];
        const clicks = await getCollection( accessToken, clicksUrl);
        for (let click of clicks) {
            const clickSubLink = click['subscriber_link'];
            const cachedSubscriber = subscriberCache[clickSubLink];
            let email;
            if (cachedSubscriber) {
                email = cachedSubscriber['email'];
            } else {
                const clickSub = await getWithRetry( accessToken, clickSubLink);
                const clickSubBody = await clickSub.json();
                email = clickSubBody['email'];
            }
            console.log(`    ${click['event_time']}: ${email}`);
        }
    }
})();
