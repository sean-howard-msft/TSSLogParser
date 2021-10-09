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
    CONSTRAINT [PK_EventLogs] PRIMARY KEY CLUSTERED ([RecordId] ASC, [MachineName] ASC, [LogName] ASC)
);


GO
CREATE NONCLUSTERED INDEX [idx_EventLogs_LogName_ProviderName_i_Message]
    ON [dbo].[EventLogs]([LogName] ASC, [ProviderName] ASC)
    INCLUDE([Message]);

