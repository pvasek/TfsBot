# TfsBot

This is simple skype bot implementation with _BotFramework_. It listens for TFS webhooks and send messages based on that.

## How to debug

The bot is just web API controller that needs to be accessible from internet. The bot framework server needs to connect to it.

For basic testing you can use BotEmulator https://docs.microsoft.com/en-us/bot-framework/debug-bots-emulator.
It use a trick backed in the BotConnector authentication where there is predefined name which is treated as valid token. 
That means you can easily test the logic but not the authentication itself that needs to be tested by exposing the web API
endpoint with ngrok.
