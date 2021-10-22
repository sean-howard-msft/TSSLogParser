CREATE TABLE [dbo].[System] (
    [System_Id]     NUMERIC (20)   NOT NULL,
    [EventID]       INT            NULL,
    [Level]         TINYINT        NULL,
    [Task]          TINYINT        NULL,
    [Keywords]      NVARCHAR (255) NULL,
    [EventRecordID] INT            NULL,
    [Channel]       NVARCHAR (255) NULL,
    [Computer]      NVARCHAR (255) NULL,
    [Security]      NVARCHAR (255) NULL,
    [Event_Id]      NUMERIC (20)   NULL
);



