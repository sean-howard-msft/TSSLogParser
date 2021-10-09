CREATE VIEW [dbo].[RegionalCounts]
AS 

SELECT 
	msgCnt.LogName, 
	msgCnt.ProviderName, 
	msgCnt.TruncatedMessage,
	machData.MachineName,
	machData.CountryCode,
	machData.Region,
	machData.MachineType,
	machData.AppCode,
	machData.InfraCode,
	machData.InstanceNum,
	machData.Domain,
	SUM(msgCnt.MessageCount) AS MessageCount
FROM MessageCount AS msgCnt 
	INNER JOIN MachineMetadata AS machData 
		ON machData.MachineName = msgCnt.MachineName
WHERE (msgCnt.MessageCount > 1)
GROUP BY 
	msgCnt.LogName, 
	msgCnt.ProviderName, 
	msgCnt.TruncatedMessage,
	machData.MachineName,
	machData.CountryCode,
	machData.Region,
	machData.MachineType,
	machData.AppCode,
	machData.InfraCode,
	machData.InstanceNum,
	machData.Domain
--ORDER BY 
--	msgCnt.LogName, 
--	msgCnt.ProviderName, 
--	msgCnt.TruncatedMessage