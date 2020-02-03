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


const data = {
        // the broadcast html, this can be provided without body_text
    'body_html' : '<html><body>An html version of my email</body></html>',

    // if provided, this will be the plain text version of the email
    // if this is not provided, it will be auto-generated based on the body_html
    'body_text' : 'A plain text version of my email',

    // this is the broadcast subject line
    'subject' : 'This is an email created by the api!',

    // if there are links in this broadcast, track them
    'click_tracking_enabled' : 'true',

    // include or exclude subscribers on other lists in this broadcast
    // these are lists of URI's or list links
    'exclude_lists' : [],
    'include_lists' : [],

    // this means the broadcast will be available via a url after it's sent
    'is_archived' : 'true',

    // if notifications are enabled and notify_on_send is True, send an email
    // to the AWeber account holder when this broadcast stats' are available
    'notify_on_send' : 'true',

};




(async () => {

    // Get an account to search on
    const accounts = await getCollection(accessToken, BASE_URL + 'accounts');

    // Get a list to find broadcasts on
    const lists = await getCollection( accessToken, accounts[0]['lists_collection_link']);

    // if enabled, get the facebook url to share the broadcast to facebook
    const integrations = await getCollection( accessToken, accounts[0]['integrations_collection_link']);
    for (let integration of integrations) {
        if (integration['service_name'].toLowerCase() == 'facebook') {
            // there could be multiple, so pick the first one we find and break
            data['facebook_integration'] = integration['self_link'];
            break;
        }
    }
    // make the broadcast on the first list
    const broadcastResponse = await fetch(`${lists[0]['self_link']}/broadcasts`,{
        // this may need to be form encoded.
        method:"POST",
        body:qs.stringify(data),
        'headers' : {
            'Authorization' : 'Bearer ' + accessToken,
            "Content-Type":"application/x-www-form-urlencoded"
        }
    });
    const broadcast = await broadcastResponse.json();
    console.log({broadcast})

    // schedule the broadcast we made
    const timestamp =  new Date();
    console.log({timestamp});
    const scheduledFor = timestamp.toISOString(); // must be iso8601 compliant
    console.log({scheduledFor});
    console.log("self link",broadcast['self_link']);

    const scheduleResponse = await fetch(`${broadcast['self_link']}/schedule`, {
        method:"POST",
        body:qs.stringify({'scheduled_for' : scheduledFor}),
        'headers' :{
            'Authorization' : 'Bearer ' + accessToken,
            "Content-Type":"application/x-www-form-urlencoded"
        }
    });

    // retrieve the scheduled broadcast to see the updated scheduled_for
    const scheduledResponse = await fetch(broadcast['self_link'], {
        headers:{
            'Authorization' : 'Bearer ' + accessToken
        }
    });
    const scheduledBroadcast = await scheduledResponse.json();
    console.log({scheduledBroadcast});

    console.log( `Scheduled broadcast subject: ${scheduledBroadcast['subject']} on list: ${lists[0]['name']}`);
    console.log (` to be sent at: ${scheduledBroadcast['scheduled_for']}`);

})();
