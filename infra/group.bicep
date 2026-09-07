extension microsoftGraphV1

param groupDisplayName string
param groupMailNickname string
param groupUniqueName string
param memberObjectId string

resource sqlAdminGroup 'Microsoft.Graph/groups@v1.0' = {
  displayName: groupDisplayName
  mailEnabled: false
  mailNickname: groupMailNickname
  securityEnabled: true
  uniqueName: groupUniqueName

  members: {
    relationshipSemantics: 'append'
    relationships: [
     memberObjectId
    ]
  }
}

output groupObjectId string = sqlAdminGroup.id

output groupName string = sqlAdminGroup.uniqueName
