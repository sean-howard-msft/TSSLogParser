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
	LEFT([Message], CHARINDEX('.', [Message])) AS TruncatedMessage
FROM  dbo.EventLogs
WHERE ([Message] IS NOT NULL) AND 
	  ([Message] <> '')
