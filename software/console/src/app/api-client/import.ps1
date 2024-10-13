# generate proxies
npx swagger-typescript-api -p http://192.168.1.220/swagger/current/swagger.json -o . --modular --module-name-first-tag

# Update method names

# security controller
(Get-Content Security.ts) `
	-replace "securityLoginCreate", "Login" `
	-replace "securityPingList", "Ping" `
	| Set-Content -Encoding utf8 -Path Security.ts

# variables controller
(Get-Content Variables.ts) `
	-replace "variablesValuesDetail", "GetVariableValue" `
	-replace "variablesValuesList", "GetVariableValues" `
	-replace "variablesList", "EnumerateVariables" `
	-replace "variablesPingList", "Ping" `
	| Set-Content -Encoding utf8 -Path Variables.ts

# system stats controller
(Get-Content SystemStats.ts) `
	-replace "systemstatsList", "GetSystemStats" `
	-replace "systemstatsPingList", "Ping" `
	| Set-Content -Encoding utf8 -Path SystemStats.ts
