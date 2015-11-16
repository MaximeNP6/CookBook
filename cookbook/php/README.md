Cook Book for php Implementation
==


getTargetFromUnicity
--

This code example allows you to get the information of a target (Guid, fields....following the model you can find on API documentation: https://v8.mailperformance.com/doc) giving his unicity criteria.

getTargetAndSendMessage
--

This code takes the id of a target (with the getTargetFromUnicity's informations) and sends it a pre-created message (the idMessage) in your account.

postTarget
--

This code checks if the target exist. If it is, the code will update the target with the new information; or else the code will create the new target with these information.

creatTargetAndAddToSegment
--

This code checks if the target exist. If it is not, it will create the target. Then the code will add the target to a segment (using the id-segment).

sendHTML
--

This code sends a message like getTargetAndSendMessage. But you can customize the "html message", the "text message", the "subject of the message", the mail "from address" and the "reply to" address.

T8
--

This code converted the html links in a text into T8 links.

createField
--

This code can create a new field or update an existing.

createValueList
--

This code can create a new value list or update an existing.

createCategory
--

This code creates or updates a category for a campaign.

duplicateAndValidate
--

This code adds a target to a segment, then it duplicates an action, updates the new action with the segment modified and finally test and validates the action.

duplicateAndAddNewSegment
--

With this code you can create/modify a target, create/modify a segment, place target in segment, duplicate an action, make segment as new sending segment to the duplicated action and finally launch a test phase.


Version
--

1.0

Contact
--

Contact us at : http://www.np6.co.uk/contact-request/
