# generate proxies
npx swagger-typescript-api -p http://192.168.1.220/swagger/current/swagger.json -o . --modular --module-name-first-tag

# Update method names

# security controller
(Get-Content Security.ts) `
	-replace "securityLoginCreate", "login" `
	-replace "securityAccesstokenList", "getAccessTokenInfo" `
	-replace "securityUsersList", "enumerateUsers" `
	-replace "securityUsersDetail", "getUser" `
	-replace "securityUsersCreate", "createUser" `
	-replace "securityUsersUpdate", "editUser" `
	-replace "securityUsersDelete", "deleteUser" `
	-replace "securityUsersProfileCreate", "updateUserProfile" `
	-replace "securityPingList", "ping" `
	| Set-Content -Encoding utf8 -Path Security.ts

# variables controller
(Get-Content Variables.ts) `
	-replace "variablesList", "enumerateVariables" `
	-replace "variablesDetail", "getVariable" `
	-replace "variablesCreate", "createVariable" `
	-replace "variablesUpdate", "editVariable" `
	-replace "variablesValuesList", "getVariableValues" `
	-replace "variablesValuesDetail", "getVariableValue" `
	-replace "variablesValuesCreate", "setVariableValue" `
	-replace "variablesDelete", "deleteVariable" `
	-replace "variablesPingList", "ping" `
	| Set-Content -Encoding utf8 -Path Variables.ts

# system controller
(Get-Content System.ts) `
	-replace "systemstatsList", "getSystemStats" `
	-replace "systemPingList", "ping" `
	-replace "systemStatsCpuHistory5MinList", "getCpuHistory5Min" `
	| Set-Content -Encoding utf8 -Path System.ts

# entities controller
(Get-Content Entities.ts) `
	-replace "entitiesBasicList", "getBasicEntities" `
	-replace "entitiesBasicLiteralsList", "getLiterals" `
	-replace "literalsPingList", "ping" `
	| Set-Content -Encoding utf8 -Path Entities.ts
