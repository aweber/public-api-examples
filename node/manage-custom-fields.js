
const fetch = require(`node-fetch`);
const credentials = require(`./credentials.json`);
const BASE_URL = 'https://api.aweber.com/1.0/';
const client_id = credentials['clientId'];
const client_secret = credentials['clientSecret'];
const accessToken = credentials['accessToken'];

const qs = require(`querystring`);


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
        console.log(`got page`, {
            page
        })
        collection.push(...page.entries);
        url = page.next_collection_link;
    }
    return collection;
}

(async () => {

    // Get an account to manage custom fields on
    const accounts = await getCollection(accessToken, BASE_URL + 'accounts');
    const account = accounts[0];  // choose the first account

    // Get a list to manage custom fields on
    const listsUrl = account['lists_collection_link'];
    const lists = await getCollection(accessToken, listsUrl);
    const list = lists[0];  // choose the first list

    // Check if custom fields already exist with the names we use below
    const customFieldsUrl = list['custom_fields_collection_link'];
    const customFields = await getCollection(accessToken, customFieldsUrl);
    for (let entry of customFields) {
        if (['Test', 'Renamed'].includes (entry['name'])) {
            console.log (`A custom field called ${entry['name']} already exists on ${list['name']}`);
            process.exit();
        }
    }

    // Create a custom field called Test
    const createResponse = await fetch(customFieldsUrl, {
        method:"POST",
        'body' :qs.stringify({
            'ws.op' : 'create',
            'name' : 'Test'
        }),
        'headers' : {
            "Access-Control-Expose-Headers": "Location",
            "Content-Type":"application/x-www-form-urlencoded",
            'Authorization' : 'Bearer ' + accessToken
        }
    });
    console.log(createResponse.headers);
    const fieldUrl = createResponse.headers.get("location");
    console.log (`Create new custom field at ${fieldUrl}`);

    // Update the custom field
    const updateResponse = await fetch(fieldUrl, {
        method:"patch",
        data : JSON.stringify({
            'name' : 'Renamed',
            'is_subscriber_updateable' : true
        }),
        'headers' :{
            'Authorization' : 'Bearer ' + accessToken,
            "Content-Type":"application/json"
        }
    });
    const updatedField = await updateResponse.json();
    console.log ("Updated the custom field: ");
    console.log(updatedField);

    // Delete the custom field
    await fetch(fieldUrl, {
        'headers' :{
            'Authorization' : 'Bearer ' + accessToken
        } 
    });
    console.log ("Deleted the custom field");
})();
