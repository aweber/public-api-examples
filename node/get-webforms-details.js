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
/**
 * Print details for a webform or split test component.
 *
 * @param array $data webform or split test data
 * @param int $indent number of spaces to indent
 */
function printWebformInfo(data, indent = 8) {
    const prefix = ' '.repeat( indent);
    console.log(`${prefix} Type: ${data['type']}`);
    console.log(`${prefix} HTML source: ${data['html_source_link']}`);
    console.log(`${prefix} JS source: ${data['javascript_source_link']}`) ;
    console.log(`${prefix} Displays: ${data['total_displays']} ({$data['total_unique_displays']} unique)`);
    console.log(`${prefix} Submissions: ${data['total_submissions']}`);
    console.log(`${prefix} Conversion: ${data['conversion_percentage'].toFixed(1)}% `);
    console.log(`(${data['unique_conversion_percentage'].toFixed(1)}% unique)`);
}

(async () => {

    // Get an account to manage custom fields on
    const accounts = await getCollection( accessToken, BASE_URL + 'accounts');
    const account = accounts[0];  // choose the first account

    // Get a list to get webforms for
    const lists = await getCollection(accessToken, account['lists_collection_link']);
    const list = lists[0];  // choose the first list


    // Get all the webforms for a list
    const webforms = await getCollection( accessToken, list['web_forms_collection_link']);
    if (webforms.length === 0) {
        console.log( `No webforms for ${list['name']}`);
        process.exit();
    }

    console.log( `Webforms for {$list['name']}:`);
    for (let webform of webforms) {
        console.log (`    {$webform['name']}:`);
        printWebformInfo(webform);
    }

    console.log("");

    // Get all the webform split tests for a list
    const splitTests = await getCollection(accessToken, list['web_form_split_tests_collection_link']);
    if (splitTests.length === 0) {
        console.log (`No webform split tests for ${list['name']}`);
        process.exit();
    }

    console.log(`Webform split tests for {$list['name']}:`) ;
    for (let splitTest of splitTests) {
        console.log(`    {$splitTest['name']}: {$splitTest['javascript_source_link']}`) ;
        const components = await getCollection(accessToken, splitTest['components_collection_link']);
        for (let component of components) {
            console.log(`        ${component['name']} (${component['weight']}%)`) ;
            printWebformInfo(component, 12);
        }
    }
})();
