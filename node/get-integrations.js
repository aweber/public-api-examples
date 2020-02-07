
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

async function getCollection(accessToken, url) {
    let res;
    const collection = [];
    console.log({
        accessToken
    })
    while (url) {
        res = await request(url, {
            headers: {
                "Authorization": `Bearer ${accessToken}`
            }
        });
        let page = await res.json();
        console.log("got page", {
            page
        })
        collection.push(...page.entries);
        url = page.next_collection_link;
    }
    return collection;
}
(async () => {

    // Get an account to search on
    const accounts = await getCollection(accessToken, BASE_URL + 'accounts');

    // Get a list to find broadcasts on
    const lists = await getCollection( accessToken, accounts[0]['lists_collection_link']);

    // if enabled, get the facebook url to share the broadcast to facebook
    const integrations = await getCollection( accessToken, accounts[0]['integrations_collection_link']);
    console.log("integrations",integrations)
    for (let integration of integrations) {
        if(['twitter','facebook'].includes(integration['service_name'])) {
            console.log(`${integration['service_name']} ${integration['login']} ${integration['self_link']}`);
        }
    }
})();
