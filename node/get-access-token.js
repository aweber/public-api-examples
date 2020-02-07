const ClientOAuth2 = require("client-oauth2"),
      util = require("util"),
      readline = require("readline"),
      fs = require("fs"),
      Path = require("path"),
      url = require("url");

const rl = readline.createInterface({
    input: process.stdin,
    output: process.stdout
});

const question = (q) => new Promise((resolve) => rl.question(q,resolve));
const writeFile = util.promisify(fs.writeFile);

const state = Date.now();

const OAUTH_URL = 'https://auth.aweber.com/oauth2',
      TOKEN_URL = 'https://auth.aweber.com/oauth2/token',
      scopes = [
          'account.read',
          'list.read',
          'list.write',
          'subscriber.read',
          'subscriber.write',
          'email.read',
          'email.write',
          'subscriber.read-extended'
      ];



(async () => {

    let createRefresh = await question("Do you wish to create(c) tokens or refresh(r) tokens?:");
    let user;
    let credentials, clientId, clientSecret;

    if(createRefresh.toUpperCase() === "C") {

        clientId = await question("clientId:");
        clientSecret = await question("clientSecret:");

        const aweberAuth = new ClientOAuth2({
            clientId, clientSecret,
            accessTokenUri:TOKEN_URL,
            authorizationUri:`${OAUTH_URL}/authorize`,
            redirectUri:"http://localhost:3000/callback",
            scopes
        });
        const authorizationUrl = aweberAuth.code.getUri({state});
        console.log("Go to this url",authorizationUrl);

        const authorizationResponse = await question("Login and paste the returned URL here:");

        console.log("authorization response",authorizationResponse);
        user = await aweberAuth.code.getToken(authorizationResponse);

    } else if(createRefresh.toUpperCase() === "R") {

        credentials = require("./credentials.json");
        clientId = credentials.clientId;
        clientSecret = credentials.clientSecret;


        if(!credentials.accessToken ||
           !credentials.refreshToken ||
           !credentials.clientId ||
           !credentials.clientSecret) {
            throw new Error("credentials.json is missing fields");
        }

        const aweberAuth = new ClientOAuth2({
            clientId:credentials.clientId,
            clientSecret:credentials.clientSecret,
            accessTokenUri:TOKEN_URL,
            authorizationUri:`${OAUTH_URL}/authorize`,
            redirectUri:"http://localhost:3000/callback",
            scopes
        });
        user = await aweberAuth.createToken(
            credentials.accessToken,
            credentials.refreshToken,
            "bearer"
        ).refresh();
    }
    else {
        throw new Error("Invalid entry. You must enter 'c' or 'r'");
    }

    await writeFile(Path.join(__dirname,"credentials.json"),JSON.stringify({
        clientSecret:clientSecret,
        clientId:clientId,
        accessToken:user.accessToken,
        refreshToken:user.refreshToken
    }))
    process.exit();
})();
