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
    console.log({
        accessToken
    })
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
/**
 * Return the full name of a campaign type code
 *
 * @param string $campaignType
 * @return string
 */
function campaignTypeName(campaignType) {
    if (campaignType == 'b') {
        return 'broadcast';
    } else if (campaignType == 'f') {
        return 'followup';
    } else {
        return `"${campaignType}" type campaign`;
    }
}

(async () => {
    // get all the accounts entries
    const accounts = await getCollection(accessToken, BASE_URL + 'accounts');
    console.log({
        accounts
    })
    const accountUrl = accounts[0]['self_link'];

    // get all the list entries for the first account
    const listsUrl = accounts[0]['lists_collection_link'];
    const lists = await getCollection(accessToken, listsUrl);

    // get a sent broadcast
    const params = {
        'ws.op': 'find',
        'campaign_type': 'b'
    };

    const campaignsUrl = lists[0]['campaigns_collection_link'];
    const campaigns = await getCollection(accessToken, campaignsUrl);
    const campaign = campaigns[0]; // choose the first campaign
    const type = campaignTypeName(campaign['campaign_type']);
    console.log(`Clicks for ${type} "${campaign['subject']}" by link:`);

    // Cache subscriber email addresses to avoid looking them up repeatedly
    const subscriberEmails = {};

    // Get all the links included in the message
    const linksUrl = campaign['links_collection_link'];
    const links = await getCollection(accessToken, linksUrl);
    for(let link of links) {
        console.log("${link['url']}\n") ;
        // Get all the clicks for each link
        const clicksUrl = link['clicks_collection_link'];
        const clicks = await getCollection(accessToken, clicksUrl);
        for(let click of clicks) {
            // Look up the email address of each subscriber who clicked
            const subscriberUrl = click['subscriber_link'];
            if (subscriberEmails[subscriberUrl]) {
                // First time looking up a subscriber: save the email for next time
                const subscriberResponse = await fetch(subscriberUrl) ;
                const subscriber = await subscriberResponse.json();
                subscriberEmails[subscriberUrl] = subscriber['email'];
            }
            const email = subscriberEmails[subscriberUrl];
            console.log (`    ${click['event_time']}: ${email}`);
        }
    }
})();
