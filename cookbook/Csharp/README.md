Cook Book for Csharp Implementation
==


getTargetFromUnicity
--

This code example allows you to get the informations of a target (Guid, fields....following the model you can find on API documentation : https://v8.mailperformance.com/doc) giving his unicity criteria.

getTargetAndSendMessage
--

This code takes the id of a target (with the getTargetFromUnicity's informations) and send it a pre-created message (the idMessage) in your account.
To use this code you must import the JSON.Net library.

postTarget
--

This code check if the target exist. If it is, the code will update the target with the new informations; or else the code will creat the new target with these informations. 
To use this code you must import the JSON.NET library.

JSON.NET librairy
--

How install JSON.Net library on Visual Studio : > Right click the Project > Manage NuGet Packages... > Search Online (Ctrl + E) "Json.NET" > Install "Json.NET" > Close

Coming soon
--
Webhooks to listen clicks and openings.


Version
--

1.0 