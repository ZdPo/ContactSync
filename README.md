# ContactSync

Office 2010 Add-In for synchronization contact Outlook contact and Google Contact.
Project based on .NET Google Data API version 3.0 and Microsoft Office 2010 API in VSTO.
This Add-In is complete written in C#.

# Pre final update 2012-06-15

## Now work:
 - First synchronization setup
 - Synchronization setup is possible one-way or both-way
 - Synchronization contact image
 - Synchronize Google contact groups with Outlook Category (Google system category shows in Outlook as “System Group: name”)
 - Cache data on disk for speed up synchronization is configurable by settings per system
 - New authentication schema for Google APIs 

## Improvements:
 - System upgrade to use OAuth 2.0 technology (based on this: https://developers.google.com/accounts/docs/OAuth2)
 - New location for log file move from LocalApplicationData to LocalTempPath
 - Logging files deleted after regular time no more then 5 log files
 
## In development:
 - Test new approach and for synchronizing use separate threads
 
## Planed features:
 - Using batch request to Google
 - Synchronization based on time schedule
 - Review data before insert, update or delete
 
## Know issue:
 - Anniversary doesn't synchronize; Google data schema doesn't support it
 - First synchronization can’t both-way
 - Reading from Google is to slow with switch off cache for Google
 - Not correct catch all exception
