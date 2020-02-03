
const fetch = require("node-fetch");
const credentials = require("./credentials.json");
const BASE_URL = 'https://api.aweber.com/1.0/';
const client_id = credentials['clientId'];
const client_secret = credentials['clientSecret'];
const accessToken = credentials['accessToken'];

const qs = require("querystring");


async function getCollection(accessToken, url) {
    let res;
    const collection = [];
    while (url) {
        res = await fetch(url, {
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


    const listsUrl = accounts[0]['lists_collection_link'];  // choose the first account
    const listName = 'abc';
    const params = {
        'ws.op' :'find',
        'name' :listName
    };
    const findListUrl = listsUrl + '?' + qs.stringify(params);
    const lists = await getCollection(accessToken, findListUrl);
    console.log({lists});

    if (lists[0]['self_link']) {
        const tagUrl = lists[0]['self_link'] + '/tags';  // choose the first list
        const request = await fetch(tagUrl, {
            'headers' : {
                'Authorization' : 'Bearer ' + accessToken
            }
        });
        const tags = await request.json();
        console.log(tags);
    } else {
        console.log('Could not find a list with name: '+ listName);
    }
})();
