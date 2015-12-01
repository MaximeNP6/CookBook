Exemples de code en Csharp
==


getTargetFromUnicity
--

Cet exemple de code permet d'obtenir les informations d'une cible avec uniquement les criteres d'unicites de la cible.

getTargetAndSendMessage
--

Ce code prend l'Id d'une cible (avec les informations de getTargetFromUnicity) et lui envoie un message deje cree dans votre compte (le 'idMessage').

Pour utiliser ce code vous devez importer la librairie JSON.Net .

postTarget
--

Ce code verifie que la cible existe. Si ce n'est pas le cas on cree la cible; sinon on modifie la cible avec les nouvelles informations.

Pour utiliser ce code vous devez importer la librairie JSON.Net .

creatTargetAndAddToSegment
--

Ce code verifie que la cible existe. Si ce n'est pas le cas, on cree la cible. Puis ce code va ajouter la cible a un segment (en utilisant l'Id du segment).

Pour utiliser ce code vous devez importer la librairie JSON.Net .

sendHTML
--

Ce code envoie un message comme getTargetAndSendMessage. Mais vous pouvez changer le "html message", le "text message", le "subject of the mesage", le mail "from address" et le "reply to".

Pour utiliser ce code vous devez importer la librairie JSON.Net .

T8
--

Ce code convertit les liens html dans un texte en lien T8.

createField
--

Ce code peut creer de nouveaux 'champs' ou en modifier.

Pour utiliser ce code vous devez importer la librairie JSON.Net .

createValueList
--

Ce code peut creer de nouvelles 'Listes de valeurs' ou en modifier.

Pour utiliser ce code vous devez importer la librairie JSON.Net .

createCategory
--

Ce code peut creer ou modifier une 'categorie' pour une campagne.

Pour utiliser ce code vous devez importer la librairie JSON.Net .

duplicateAndValidate
--

Ce code ajoute une cible a un segment, duplique une action, modifie la nouvelle action avec le segment modifie et finit par tester et valider l'action.

Pour utiliser ce code vous devez importer la librairie JSON.Net .

duplicateAndAddNewSegment
--

Ce code cree/modifie une cible, cree/modifie un segment, place la cible dans le segment, duplique une action, fait du segment le nouveau segment d'envoi de l'action duplique et enfin lance une phase de test.

Pour utiliser ce code vous devez importer la librairie JSON.Net .

SendMessageOnSegment
--

Ce code prend tous les ids des cibles d'un segment et lance un SendMessage avec toutes les cibles.

JSON.NET Librairie
--

Comment installer la librairie JSON.Net sur Visual Studio :

Clique droit sur le projet > Manage NuGet Packages... > Search Online (Ctrl + E) "Json.NET" > Install "Json.NET" > Close > OK


Version
--

1.0

Contact
--

Contactez-nous sur : http://www.np6.fr/demande-de-contact/
