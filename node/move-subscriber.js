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

(async () =>{
    // get all the accounts entries
    const accounts = await getCollection(accessToken, BASE_URL + 'accounts');

    // get all the list entries for the first account
    const lists = await getCollection(accessToken, accounts[0]['lists_collection_link']);
    console.log({lists})

    // pick the list to move the subscriber from and to
    const originList = lists[0];
    const destinationList = lists[1];

    if (!originList || !destinationList) {
        console.log("You must have 2 lists to move a subscriber!") ;
        process.exit();
    }

    // get all subscribers from the first list
    const subscribers = await getCollection(accessToken, originList['subscribers_collection_link']);

    // pick the subscriber we want to move
    const subscriber = subscribers[0];

    if (!subscriber) {
        console.log(`You must have a subscriber on list: ${originList['name']}!`) ;
        process.exit();
    }

    const data = {
        'ws.op' : 'move',
        'list_link':  destinationList['self_link']
    } ;
    try {
        // attempt to move the subscriber to the second list
        // $moveResponse = $client->post($subscriber['self_link'], ['json' => $data]);
        const moveResponse = await request(subscriber['self_link'], {
            method:"POST",
            body:JSON.stringify(data),
            headers:{
                "authorization":`Bearer ${accessToken}`,
                "content-type":"application/json"
            }
        });
        console.log(`Moved subscriber ${subscriber['email']} from list: ${originList['name']} to list: ${destinationList['name']}`) ;
    } catch (e) {

        // For more info see: https://api.aweber.com/#tag/Troubleshooting
        console.log(await e.response.json());
        console.log (`Could not move subscriber!\n${e.message}`);
    }
})();
