targetScope = 'subscription'


param userObjectId string
param userFirewallIpAddress string



param now string = utcNow('MMddHHmm')

param deployName string = 'rd${now}'

param location string = deployment().location

var rootName = 'example-test'

var rgName = '${rootName}-rg'



module rg 'br/public:avm/res/resources/resource-group:0.4.4' = {
  name: '${deployName}-resource-group'
  params: {
    name: rgName
    location: location
  }
}

var deploymentRg = resourceGroup(rgName)


var groupUniqueName = '${rootName}-sql-admins'
module adminGroup 'group.bicep' =  {
  name: '${deployName}-sql-admin-group'
  scope: deploymentRg
  dependsOn: [ rg ]
  params: {
    groupDisplayName: groupUniqueName
    groupMailNickname: groupUniqueName
    groupUniqueName: groupUniqueName
    memberObjectId: userObjectId
  }
}

module sql 'br/public:avm/res/sql/server:0.22.0' = {
  name: '${deployName}-sql-server'
  scope: deploymentRg
  dependsOn: [ rg ]
  params: {
    name: '${rootName}-sql'
    administrators: {
      azureADOnlyAuthentication: true
      login: adminGroup.outputs.groupName
      principalType: 'Group'
      sid: adminGroup.outputs.groupObjectId
      tenantId: tenant().tenantId
    }
  }
}


var scratchDatabaseNames = [
  'ProphetsWay.Example'
  'EFToolsScratch_1'
  'EFToolsScratch_2'
]

module scratchDatabases 'br/public:avm/res/sql/server/database:0.3.0' = [
  for databaseName in scratchDatabaseNames: {
    name: '${deployName}-database-${databaseName}'
    scope: deploymentRg
    params: {
      name: databaseName
      serverName: sql.outputs.name
      location: location
      availabilityZone: -1
      zoneRedundant: false
      enableTelemetry: false
      sku: {
        name: 'Basic'
        tier: 'Basic'
        capacity: 5
      }
      maxSizeBytes: 2147483648
    }
  }
]


module firewallRuleDeployment 'br/public:avm/res/sql/server/firewall-rule:0.1.0' = {
  name: '${deployName}-deploy-sql-fw-client'
  scope: deploymentRg
  params: {
    name: 'example-test-sql-fw-client'
    serverName: sql.outputs.name
    startIpAddress: userFirewallIpAddress
    endIpAddress: userFirewallIpAddress
    enableTelemetry: false
  }
}
