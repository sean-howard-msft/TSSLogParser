CREATE PROCEDURE [dbo].[RunSSISPkgXMLImport] @FilePath NVARCHAR(255)
AS
BEGIN
    DECLARE @execution_id BIGINT;
    EXEC [$(SSISDB)].[catalog].[create_execution] @package_name = N'EventLogs.dtsx',
                                                  @execution_id = @execution_id OUTPUT,
                                                  @folder_name = N'TSSLogParser',
                                                  @project_name = N'TSSLogParser.SSIS',
                                                  @use32bitruntime = False,
                                                  @reference_id = NULL,
                                                  @runinscaleout = False;
    SELECT @execution_id;
    EXEC [$(SSISDB)].[catalog].[set_execution_parameter_value] @execution_id,
                                                               @object_type = 30,
                                                               @parameter_name = N'FilePath',
                                                               @parameter_value = @FilePath;
    DECLARE @var1 SMALLINT = 1;
    EXEC [$(SSISDB)].[catalog].[set_execution_parameter_value] @execution_id,
                                                               @object_type = 50,
                                                               @parameter_name = N'LOGGING_LEVEL',
                                                               @parameter_value = @var1;
    EXEC [$(SSISDB)].[catalog].[start_execution] @execution_id;

    DECLARE @status AS BIGINT = 1;
    WHILE (@status = 1 OR @status = 2 OR @status = 5 OR @status = 8)
    BEGIN
        PRINT @status;
        PRINT 'waiting 5 seconds for Package to finish';
        WAITFOR DELAY '00:00:5';

        SELECT @status = [Status]
        FROM [$(SSISDB)].[catalog].[executions]
        WHERE execution_id = @execution_id
    END;
END;