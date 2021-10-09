

CREATE VIEW [dbo].[GlobalCounts]
AS
SELECT 
	msgCnt.LogName, 
	msgCnt.ProviderName, 
	msgCnt.TruncatedMessage, 
	machCnt.MachineCount, 
	SUM(msgCnt.MessageCount) AS MessageCount
FROM MessageCount AS msgCnt 
	INNER JOIN MachineCount AS machCnt 
		ON msgCnt.LogName = machCnt.LogName 
		AND msgCnt.ProviderName = machCnt.ProviderName 
		AND msgCnt.TruncatedMessage = machCnt.TruncatedMessage 
WHERE (machCnt.MachineCount > 1)
GROUP BY 
	msgCnt.LogName, 
	msgCnt.ProviderName, 
	msgCnt.TruncatedMessage, 
	machCnt.MachineCount
--ORDER BY 
--	msgCnt.LogName, 
--	msgCnt.ProviderName, 
--	machCnt.MachineCount DESC, 
--	msgCnt.TruncatedMessage
