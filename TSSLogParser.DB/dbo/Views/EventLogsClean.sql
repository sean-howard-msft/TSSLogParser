CREATE VIEW dbo.EventLogsClean
AS
SELECT 
	RecordId, 
	MachineName, 
	LogName, 
	TimeCreated, 
	LevelDisplayName, 
	[Level], 
	Id, 
	ProviderName, 
	[Message], 
	ContainerLog, 
	LEFT([Message], 
		IIF(CHARINDEX('.', [Message]) >= 50, 
			CHARINDEX('.', [Message]), 50)
		) AS TruncatedMessage
FROM  dbo.EventLogs
WHERE ([Message] IS NOT NULL) AND 
	  ([Message] <> '')