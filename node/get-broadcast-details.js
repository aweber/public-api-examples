
const fetch = require("node-fetch");
const credentials = require("./credentials.json");
const BASE_URL = 'https://api.aweber.com/1.0/';
const client_id = credentials['clientId'];
const client_secret = credentials['clientSecret'];
const accessToken = credentials['accessToken'];

const qs = require("querystring");


async function getCollection(accessToken,url) {
    let res;
    const collection = [];
    while(url) {
        res = await fetch(url,{
            headers:{
                "Authorization":`Bearer ${accessToken}`
            }
        });
        let page = await res.json();
        collection.push(...page.entries);
        url = page.next_collection_link;
    }
    return collection;
}

/**
 * Get a URL, retrying on rate limit errors
 *
 * @param Client $client guzzle client instance
 * @param string $url URL to get
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
                console.log(`Retry ${$i}...\n`);
            }
            continue;
        }
    }

    throw new Error ("Giving up after " + MAX_RETRIES + " tries");
}


(async () => {
    
    // Get an account to search on
    const accounts = await getCollection(accessToken, BASE_URL + 'accounts');
    const account = accounts[0];  // choose the first account

    // Get a list to find broadcasts on
    const lists = await getCollection(accessToken, account['lists_collection_link']);
    const listUrl = lists[0]['self_link'];  // choose the first list

    // Get broadcast totals
    for (let status of ['draft', 'scheduled', 'sent']) {
        const totalUrl = `${listUrl}/broadcasts/total?status=${status}`;
        const totalResponse = await getWithRetry(accessToken, totalUrl);
        const totalBody = await totalResponse.json();
        const total = totalBody['total_size'];
        console.log (`Total ${status} broadcasts: ${total}`);
    }

    // Get the first broadcast in each category
    for (let status  of ['draft', 'scheduled', 'sent']) {
        const linkKey = `${status}_broadcasts_link`;
        const broadcasts = await getCollection(accessToken, lists[0][linkKey]);
        const subject = broadcasts && broadcasts[0] ? broadcasts[0]['subject'] : 'N/A';
        console.log (`First ${status} broadcast subject: ${subject}`);
    }
})();
