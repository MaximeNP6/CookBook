Cook Book for java Implementation
==


getTargetFromUnicity
--

This code example allows you to get the informations of a target (Guid, fields....following the model you can find on API documentation : https://v8.mailperformance.com/doc) giving his unicity criteria.

getTargetAndSendMessage
--

This code takes the id of a target (with the getTargetFromUnicity's informations) and send it a pre-created message (the idMessage) in your account. 
To use this code you must import the Java JSON library.

postTarget
--

This code check if the target exist. If it is, the code will update the target with the new informations; or else the code will creat the new target with these informations. 
To use this code you must import the Java JSON library.

creatTargetAndAddToSegment
--

This code check if the target exist. If it is not, it will create the target. Then the code will add the target to a segment (using the id-segment).
To use this code you must import the Java JSON library.

JSON librairy
--

How install Java JSON library on Eclipse : extract : http://www.java2s.com/Code/JarDownload/java/java-json.jar.zip > Open Eclipse > Right click your Project > Build Path > Configure build path > Select Libraries tab > Click Add External Libraries/JARs > Select the Jar file Download > OK

Coming soon
--
Webhooks to listen clicks and openings.


Version
--

1.0 