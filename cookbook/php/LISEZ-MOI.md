Exemples de code en php
==


getTargetFromUnicity
--

Cet exemple de code permet d'obtenir les informations d'une cible avec uniquement les criteres d'unicites de la cible.

getTargetAndSendMessage
--

Ce code prend l'Id d'une cible (avec les informations de getTargetFromUnicity) et lui envoie un message deja cree dans votre compte (le 'idMessage').

postTarget
--

Ce code verifie que la cible existe. Si ce n'est pas le cas on cree la cible; sinon on modifie la cible avec les nouvelles informations.

creatTargetAndAddToSegment
--

Ce code verifie que la cible existe. Si ce n'est pas le cas, on cree la cible. Puis ce code va ajouter la cible e un segment (en utilisant l'Id du segment).

sendHTML
--

Ce code envoie un message comme getTargetAndSendMessage. Mais vous pouvez changer le "html message", le "text message", le "subject of the message", le mail "from address" et le "reply to".

T8
--

Ce code convertit les liens html d'un texte en liens T8.

createField
--

Ce code peut creer de nouveaux 'champs' ou en modifier.

createValueList
--

Ce code peut creer de nouvelles 'Listes de valeurs' ou en modifier.

createCategory
--

Ce code peut creer ou modifier une 'categorie' pour une campagne.

duplicateAndValidate
--

Ce code ajoute une cible a un segment, duplique une action, modifie la nouvelle action avec le segment modifie et finit par tester et valider l'action.

duplicateAndAddNewSegment
--

Ce code cree/modifie une cible, cree/modifie un segment, place la cible dans le segment, duplique une action, fait du segment le nouveau segment d'envoi de l'action duplique et enfin lance une phase de test.


Version
--

1.0
