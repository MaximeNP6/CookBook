Cook Book for java Implementation
==


getTargetFromUnicity
--

This code example allows you to get the information of a target (Guid, fields....following the model you can find on API documentation: https://v8.mailperformance.com/doc) giving his unicity criteria.

getTargetAndSendMessage
--

This code takes the id of a target (with the getTargetFromUnicity's informations) and sends it a pre-created message (the idMessage) in your account.
To use this code you must import the Java JSON library.

postTarget
--

This code checks if the target exist. If it is, the code will update the target with the new information; or else the code will create the new target with these information.
To use this code you must import the Java JSON library.

creatTargetAndAddToSegment
--

This code checks if the target exist. If it is not, it will create the target. Then the code will add the target to a segment (using the id-segment).
To use this code you must import the Java JSON library.

sendHTML
--

This code sends a message like getTargetAndSendMessage. But you can customize the "html message", the "text message", the "subject of the message", the mail "from address" and the "reply to" address.
To use this code you must import the Java JSON library.

T8
--

This code converted the html links in a text into T8 links.

createField
--

This code can create a new field or update an existing.
To use this code you must import the Java JSON library.

createValueList
--

This code can create a new value list or update an existing.
To use this code you must import the Java JSON library.

createCategory
--

This code creates or updates a category for a campaign.
To use this code you must import the Java JSON library.

duplicateAndValidate
--

This code adds a target to a segment, then it duplicates an action, updates the new action with the segment modified and finally test and validates the action.
To use this code you must import the Java JSON library.

duplicateAndAddNewSegment
--

With this code you can create/modify a target, create/modify a segment, place target in segment, duplicate an action, make segment as new sending segment to the duplicated action and finally launch a test phase.
To use this code you must import the Java JSON library.

JSON librairy
--

How install Java JSON library on Eclipse: extract: http://www.java2s.com/Code/JarDownload/java/java-json.jar.zip > Open Eclipse > Right click your Project > Build Path > Configure build path > Select Libraries tab > Click Add External Libraries/JARs > Select the Jar file Download > OK


Version
--

1.0

Contact
--

Contact us at : http://www.np6.co.uk/contact-request/
