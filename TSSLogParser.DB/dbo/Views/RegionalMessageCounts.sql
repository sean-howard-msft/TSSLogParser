CREATE VIEW [dbo].[RegionalMessageCounts]
AS 

SELECT 
	LogName, 
	ProviderName, 
	LevelDisplayName,
	MachineName,
	TruncatedMessage,
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
FROM EventLogsFresh
GROUP BY 
	LogName, 
	ProviderName, 
	TruncatedMessage, 
	LevelDisplayName,
	MachineName,
	CountryCode,
	Region,
	MachineType,
	AppCode,
	InfraCode,
	InstanceNum,
	Domain