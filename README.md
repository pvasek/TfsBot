# TFS Bot 

This is a simple TFS bot based on Microsoft [Bot Framework](https://dev.botframework.com/).

It uses TFS webhooks to send messages to your team chats about _pull requests_ and _builds_.

We use it with Skype only. It was not tested with any other channel.

## How to use it

You can either use hosted version at [https://tfsbot.io](https://tfsbot.io) or host it yourself. After adding the bot to the check it will print you the links you need to use in TFS webhooks.

In TFS settings setup webhooks for repositories/types you want to be notified about. Use the following urls:
- for pull requests: _{bot_url}/api/webhooks/pullrequest/{your_secret}_ 
- for builds:  _{bot_url}/api/webhooks/build/{your_secret}_ 

## How does it work

After you send _setserver_ message to the bot. The secret (ServerId) is saved to Azure Table Storage in two forms:
__Client__
- _Partition Key_ first character from UserId
- _Row Key_ UserId + UserName
- ServerId

__ServerClient__
- _Partition Key_ ServerId
- _Row Key_ UserId
- BotInformation

In this way the bot can get back info about current server for the user (you can get it by sending a message _@{bot_name} getserver_) and also get all registered clients for the given server id (secret), that's needed once webhooks are called and messages needed to be sent to all registered clients (group chats).

## Lesson learned 
- don't write stupid messages in activity.CreateReply(message) if the message contains markup (&gt;tag&lt); out of the whitelist the message will not be delivered at all regardless of the success response.
