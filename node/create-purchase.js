
const fetch = require('node-fetch');
const credentials = require('./credentials.json');
const accessToken = credentials['accessToken'];

// Set the following to real values or adding the purchase will fail
const EMAIL = 'example@example.com';
const IP_ADDRESS = '127.0.0.1';  // required to be a PUBLIC IP address

const BASE_URL = 'https://api.aweber.com/1.0/';

async function request(url,options) {
    if (options == null) options = {};
    if (options.credentials == null) options.credentials = 'same-origin';
    return fetch(url, options).then(function(response) {
        if (response.status >= 200 && response.status < 300) {
            return Promise.resolve(response);
        } else {
            console.log('error on',url,options)
            var error = new Error(response.statusText || response.status);
            error.response = response;
            return Promise.reject(error);
        }
    });
}

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
    while(url) {
        res = await request(url,{
            headers:{
                'Authorization':`Bearer ${accessToken}`
            }
        });
        let page = await res.json();
        collection.push(...page.entries);
        url = page.next_collection_link;
    }
    return collection;
}

(async () => {
    // get all the accounts entries
    const accounts = await getCollection(accessToken, BASE_URL + 'accounts');

    // get all the list entries for the first account
    const listsUrl = accounts[0]['lists_collection_link'];
    const lists = await getCollection( accessToken, listsUrl);
    const listUrl = lists[0]['self_link'];

    const eventTime = new Date();
    console.log('creating new event with event_time', eventTime)
    const purchaseResponse = await request(`${listUrl}/purchases`, {
        method: 'post',
        body: JSON.stringify({
            event_time: eventTime,
            email: EMAIL,
            ip_address: IP_ADDRESS,
            currency: 'USD',
            event_note: 'event identifier for purchase',
            product_name: 'product purchased',
            value: 1.0,
            vendor: 'my-vendor',
            url: 'https://example.com/link/to/product',
            ad_tracking: 'short note',
            custom_fields: {},
            misc_notes: 'some notes about the subscriber',
            name: 'Joe Public',
            tags: [],
        }),
        headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${accessToken}`,
        }
    })
    console.log('Created purchase')

    // The Location header points to the subscriber's activity stream
    const activityLink = purchaseResponse.headers.get('location')

    // tracked events truncate the event time to the second
    const targetTime = Math.trunc(eventTime.getTime() / 1000.0) * 1000

    // retrieve the activity feed until the event shows up
    while (true) {
        const activities = await getCollection(accessToken, activityLink)
        const matches = activities.filter((elm) => (
            elm.type === 'tracked_event' && Date.parse(elm.event_time) === targetTime
        ))
        if (matches.length) {
            console.log('Found new tracked event')
            console.log(matches[0])
            break
        }
        console.log('New tracked event does not exist yet, sleeping for 2.5s')
        await new Promise(resolve => setTimeout(resolve, 2500))
    }

})();
