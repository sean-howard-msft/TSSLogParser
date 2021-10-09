CREATE VIEW [dbo].[RegionalTotals]
AS 

SELECT 
	msgCnt.LogName, 
	msgCnt.ProviderName, 
	msgCnt.TruncatedMessage,
	machData.CountryCode,
	machData.Region,
	SUM(msgCnt.MessageCount) AS MessageCount
FROM MessageCount AS msgCnt 
	INNER JOIN MachineMetadata AS machData 
		ON machData.MachineName = msgCnt.MachineName
WHERE (msgCnt.MessageCount > 1)
GROUP BY 
	msgCnt.LogName, 
	msgCnt.ProviderName, 
	msgCnt.TruncatedMessage,
	machData.CountryCode,
	machData.Region
--ORDER BY 
--	msgCnt.LogName, 
--	msgCnt.ProviderName, 
--	msgCnt.TruncatedMessage