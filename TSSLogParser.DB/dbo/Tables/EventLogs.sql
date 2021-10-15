CREATE TABLE [dbo].[EventLogs] (
    [RecordId]         INT            NOT NULL,
    [MachineName]      NVARCHAR (50)  NOT NULL,
    [LogName]          NVARCHAR (100) NOT NULL,
    [TimeCreated]      DATETIME2 (7)  NOT NULL,
    [LevelDisplayName] NVARCHAR (255) NULL,
    [Level]            INT            NULL,
    [Id]               INT            NULL,
    [ProviderName]     NVARCHAR (255) NULL,
    [Message]          NVARCHAR (MAX) NULL,
    [ContainerLog]     NVARCHAR (255) NOT NULL,
    [MsftDocsSearch] [nvarchar](max) NULL,
	[MsftDocsTopResult] [nvarchar](255) NULL,
	[WebSearch] [nvarchar](max) NULL,
	[WebTopResult] [nvarchar](255) NULL,
    CONSTRAINT [PK_EventLogs] PRIMARY KEY CLUSTERED ([RecordId] ASC, [MachineName] ASC, [LogName] ASC)
);






GO



GO
CREATE NONCLUSTERED INDEX [idx_EventLogs_MachineName_All]
    ON [dbo].[EventLogs]([MachineName] ASC)
    INCLUDE([TimeCreated], [LevelDisplayName], [Level], [Id], [ProviderName], [Message], [ContainerLog]);

