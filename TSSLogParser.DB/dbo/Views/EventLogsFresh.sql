CREATE VIEW dbo.EventLogsFresh
AS
SELECT 
	el.AppService, 
	el.RecordId, 
	el.MachineName, 
	el.LogName, 
	el.TimeCreated, 
	el.LevelDisplayName, 
	el.[Level], 
	el.Id, 
	el.ProviderName, 
	el.[Message], 
	el.ContainerLog, 
	el.TruncatedMessage,
	el.MsftDocsSearch, 
	el.MsftDocsTopResult, 
	el.WebSearch, 
	el.WebTopResult,
	machData.CountryCode,
	machData.Region,
	machData.MachineType,
	machData.AppCode,
	machData.InfraCode,
	machData.InstanceNum,
	machData.Domain
FROM  dbo.EventLogsClean el
	JOIN dbo.MachineMetadata machData
		ON machData.MachineName = el.MachineName