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
        console.log({page});
        collection.push(...page.entries);
        url = page.next_collection_link;
    }
    return collection;
}

(async ()=> {

    const accounts = await getCollection(accessToken, BASE_URL + 'accounts');
    const accountUrl = accounts[0]['self_link'];
    const params = {
        'ws.op': 'findSubscribers',
        'email': 'example@example.com'  // email to search for
    }
    const findUrl = accountUrl + '?' + qs.stringify(params);
    const foundSubscribers = await getCollection(accessToken, findUrl);
    console.log(foundSubscribers);
})();
