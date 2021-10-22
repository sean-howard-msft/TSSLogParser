CREATE VIEW [dbo].[RegionalMessageCounts]
AS 

SELECT 
	AppService, 
	LogName, 
	ProviderName, 
	LevelDisplayName,
	MachineName,
	TruncatedMessage,
	Id,
	MAX([Message]) FullMessage, 
	MAX(MsftDocsSearch) MsftDocsSearch, 
	MAX(MsftDocsTopResult) MsftDocsTopResult, 
	MAX(WebSearch) WebSearch, 
	MAX(WebTopResult) WebTopResult,
	CountryCode,
	Region,
	MachineType,
	AppCode,
	InfraCode,
	InstanceNum,
	Domain,
	COUNT(*) AS MessageCount
FROM dbo.EventLogsFresh
GROUP BY 
	AppService, 
	LogName, 
	ProviderName, 
	TruncatedMessage, 
	Id,
	LevelDisplayName,
	MachineName,
	CountryCode,
	Region,
	MachineType,
	AppCode,
	InfraCode,
	InstanceNum,
	Domain