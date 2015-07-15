Cook Book for php Implementation
==

ConvertT8Links 
--

You will find here a new way of tracking links and openings in emails. This "T8 tracking" is faster, always redirects, and you don't have to declare links before sending. 

getTargetFromUnicity
--

This code example allows you to get the informations of a target (Guid, fields....following the model you can find on API documentation : https://v8.mailperformance.com/doc) giving his unicity criteria.

getTargetAndSendMessage
--

This code takes the id of a target (with the getTargetFromUnicity's informations) and send it a pre-created message (the idMessage) in your account.

postTarget
--

This code check if the target exist. If it is, the code will update the target with the new informations; or else the code will creat the new target with these informations.

Coming soon
--

Webhooks to listen clicks and openings.


Version
--

1.0 