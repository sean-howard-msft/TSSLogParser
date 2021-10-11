

CREATE VIEW [dbo].[GlobalTotals]
AS
SELECT 
	msgCnt.LogName, 
	msgCnt.ProviderName, 
	msgCnt.TruncatedMessage, 
	machCnt.MachineCount, 
	msgCnt.LevelDisplayName,
	SUM(msgCnt.MessageCount) AS MessageCount
FROM GlobalMessageCounts AS msgCnt 
	INNER JOIN GlobalMachineCounts AS machCnt 
		ON msgCnt.LogName = machCnt.LogName 
		AND msgCnt.ProviderName = machCnt.ProviderName 
		AND msgCnt.TruncatedMessage = machCnt.TruncatedMessage 
WHERE (machCnt.MachineCount > 1)
GROUP BY 
	msgCnt.LogName, 
	msgCnt.ProviderName, 
	msgCnt.TruncatedMessage, 
	machCnt.MachineCount, 
	msgCnt.LevelDisplayName