const fetch = require("node-fetch");
const credentials = require("./credentials.json");
const BASE_URL = 'https://api.aweber.com/1.0/';
const client_id = credentials['clientId'];
const client_secret = credentials['clientSecret'];
const accessToken = credentials['accessToken'];

const qs = require("querystring");

async function request(url,options) {
    if (options == null) options = {};
    if (options.credentials == null) options.credentials = 'same-origin';
    return fetch(url, options).then(async function(response) {
        if (response.status >= 200 && response.status < 300) {
            return Promise.resolve(response);
        } else {
            console.log("error on",{url,options});
            console.log(await response.json())
            var error = new Error(response.statusText || response.status);
            error.response = response;
            return Promise.reject(error);
        }
    });
}

async function getCollection(accessToken,url) {
    let res;
    const collection = [];
    console.log({accessToken})
    while(url) {
        res = await request(url,{
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
    };

    const campaignsUrl = lists[0]['campaigns_collection_link'];
    const broadcastsUrl = campaignsUrl + '?' + qs.stringify(params);
    console.log(broadcastsUrl);
    const sentBroadcasts = await getCollection( accessToken, broadcastsUrl);
    const broadcast = sentBroadcasts[0];
    console.log ('Broadcast: ');
    console.log(broadcast);
    console.log (`Broadcast id: ${broadcast['id']} Subject: ${broadcast['subject']} Sent At: ${broadcast['sent_at']}`);

    // get the stats for the first broadcast
    const stats = await getCollection(accessToken, broadcast['stats_collection_link']);

    // look at every stat for the first broadcast
    for (let stat of stats) {
        let values = stat['value'];

        // stats entries can be in many formats
        // daily stats are shown over 14 days
        // hourly stats are shown over 24 hours
        // some stats show the top 10 (e.g. webhits_by_link)
        // some stats show total counts (e.g. total_clicks)
        console.log({values})

        let keys = Object.keys(values)
        if (keys.length > 0) {
            console.log `${stat['description']}:`;

            let value;
            for (let key of keys) {
                value = values[key];
                console.log("");
                let itemKeys = Object.keys(value);
                let itemValue;
                for (let itemKey of itemKeys) {
                    itemValue = value[itemKey];
                    console.log (`    ${itemKey}: ${itemValue}`);
                }
            }
        } else if (values) {
            console.log (`${stat['description']}: ${values}\n`);
        } else {
            console.log(`${stat['description']}: N/A\n`) ;
        }
    }
})();
