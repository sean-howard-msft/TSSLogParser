CREATE VIEW [dbo].[RegionalMessageCounts]
AS 

SELECT 
	el.LogName, 
	el.ProviderName, 
	el.TruncatedMessage, 
	el.LevelDisplayName,
	machData.MachineName,
	machData.CountryCode,
	machData.Region,
	machData.MachineType,
	machData.AppCode,
	machData.InfraCode,
	machData.InstanceNum,
	machData.Domain,
	COUNT(*) AS MessageCount
FROM EventLogsClean el
	INNER JOIN MachineMetadata AS machData 
		ON machData.MachineName = el.MachineName
GROUP BY 
	el.LogName, 
	el.ProviderName,
	el.TruncatedMessage, 
	el.LevelDisplayName,
	machData.MachineName,
	machData.CountryCode,
	machData.Region,
	machData.MachineType,
	machData.AppCode,
	machData.InfraCode,
	machData.InstanceNum,
	machData.Domain
HAVING (COUNT(*) > 1)