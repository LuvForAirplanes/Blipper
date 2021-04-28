# Blipper, aka Studious Parakeet
Relays Telegram messages to SMS. Message are read from Telegram by a bot, which forwards them onto SMS via Twilio APIs.

Requirements for Bot:
 - Must be in Group
 - Have access to messages

Twilio:
 - Must have a Twilio account (click _Upgrade Account_ after account is created)
 - Twilio number
 - Fill in the Account SID and Secret Token into your appsettings.json (from Twilio account)
 - Also add the relay to number, and the Twilio Number from which to send messages
 - Add your public url to appsettings.json and also add it to your twilio account https://{yourdomain}/api/sms/incoming

Chat Id is hard coded in the code. Also remember to provide the Bot Id in the appsettings.json

DM me for details.
