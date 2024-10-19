# generate proxies
npx swagger-typescript-api -p http://192.168.1.220/swagger/current/swagger.json -o . --modular --module-name-first-tag

# Update method names

# security controller
(Get-Content Security.ts) `
	-replace "securityLoginCreate", "login" `
	-replace "securityAccesstokenList", "getAccessTokenInfo" `
	-replace "securityPingList", "ping" `
	| Set-Content -Encoding utf8 -Path Security.ts

# variables controller
(Get-Content Variables.ts) `
	-replace "variablesValuesDetail", "getVariableValue" `
	-replace "variablesValuesList", "getVariableValues" `
	-replace "variablesList", "enumerateVariables" `
	-replace "variablesPingList", "ping" `
	| Set-Content -Encoding utf8 -Path Variables.ts

# system controller
(Get-Content System.ts) `
	-replace "systemstatsList", "getSystemStats" `
	-replace "systemPingList", "ping" `
	-replace "systemStatsCpuHistory5MinList", "getCpuHistory5Min" `
	| Set-Content -Encoding utf8 -Path System.ts

# literals controller
(Get-Content Literals.ts) `
	-replace "literalsList", "getLiterals" `
	-replace "literalsPingList", "ping" `
	| Set-Content -Encoding utf8 -Path Literals.ts
