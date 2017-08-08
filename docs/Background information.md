# Some information from background in this program
## Synchronized variable
|| Outlook ContactItem || Google.Contacts.Contact || Comments ||
| Title | Name.NamePrefix | |
| FirstName | Name.GivenName | |
| MiddleName | Name.AdditionalName | |
| LastName | Name.FamilyName | |
| Suffix | Name.NameSuffix | |
| Birthday | ContactEntry.Birthday | |
| Anniversary | {"ContactEntry.Events[]().Relation == "anniversary""} | now not working |
| {"Attachments[]()().SaveAsFile"} | GetPicture() | O:based on HasPicture and {"Attachments[]()().DisplayName == "ContactPicture.jpg""}, G: Based on PhotoEtag |
| IMAddress | {"IMs[]()().Address"} | G: primary uses IMs []()().Protocol == "MSN" |
| MobileTelephoneNumber | {"Phonenumbers[]()().Value"} | G: Phonenumbers[]()().Rel= ContactsRelationships.IsMobile |
| BusinessTelephoneNumber | {"Phonenumbers[]()().Value"} | G: Phonenumbers[]()().Rel= ContactsRelationships. IsWork |
| Business2TelephoneNumber | {"Phonenumbers[]()().Value"} | G: Phonenumbers[]()().Rel= ContactsRelationships.IsWork |
| HomeTelephoneNumber | {"Phonenumbers[]()().Value"} | G: Phonenumbers[]()().Rel= ContactsRelationships.IsHome |
| OtherTelephoneNumber | {"Phonenumbers[]()().Value"} | G: Phonenumbers[]()().Rel= ContactsRelationships.IsOther |
| BusinessFaxNumber | {"Phonenumbers[]()().Value"} | G: Phonenumbers[]()().Rel= ContactsRelationships.IsWorkFax |
| HomeFaxNumber | {"Phonenumbers[]()().Value"} | G: Phonenumbers[]()().Rel= ContactsRelationships.IsHomeFax |
| BusinessAddress | {"PostalAddresses []()()"} | G: PostalAddresses[]()().Rel= ContactsRelationships. IsWork |
| HomeAddress | {"PostalAddresses[]()()"} | G: PostalAddresses[]()().Rel= ContactsRelationships. IsHome |
| OtherAddress | {"PostalAddresses[]()()"} | G: PostalAddresses[]()().Rel= ContactsRelationships. IsOther |
| Email1Address | {"Emails[]()().Address"} | G: Emails[]()().Rel= ContactsRelationships.IsWork |
| Email2Address | {"Emails[]()().Address"} | G: Emails[]()().Rel= ContactsRelationships.IsHome |
| Email3Address | {"Emails[]()().Address"} | G: Emails[]()().Rel= ContactsRelationships.IsOther |
| Companies | Organizations[]().Name | |
| Department | Organizations[]().Department | |
| JobTitle | Organizations[]().Title | |
| WebPage | {"ContactEntry.Websites[]()().Href"} | G: ContactEntry.Websites[]()().Rel=”work” |
| Categories | GroupMembership[]().Title | G: All except System Group: Contacts |


## Address details
|| Outlook ContactItem || Google.Contacts.Contact ||
| xxxAddressStreet | PostalAddresses[]().Street |
| xxxAddressPostOfficeBox | PostalAddresses[]().Pobox |
| xxxAddressCity | PostalAddresses[]().City |
| xxxAddressPostalCode | PostalAddresses[]().Postcode |
| xxxAddressState | PostalAddresses[]().Region |
| xxxAddressCountry | PostalAddresses[]().Country |
xxx is one of this Business, Home, Other

## Details about Categories
Every categories used in any contact from Outlook are creating in Google Contacts as Contact group. Any used Google Contacts Groups are created in Outlook. Google system group uses System name and System Group Contacts isn’t create in Outlook. This is use for all added or updated contact in Google as primary group.
Created Outlook categories or Google Contact group doesn’t delete when you stop their using in contacts. Outlook uses same categories for other items.