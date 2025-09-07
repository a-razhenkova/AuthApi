BEGIN TRY
	BEGIN TRANSACTION T1

	INSERT INTO [dbo].[client]
	SELECT NULL AS [version],
		   'Auth API' AS [name],
		   'dba1d25a-0062-49e7-b4f0-31224a69f9e4' AS [key],
		   '818fec5e-bff4-4396-85a7-9cc2eccd166f' AS [secret],
		   0 AS [wrong_login_attempts_counter],
		   1 AS [is_internal]

	INSERT INTO [dbo].[client_status]
	SELECT NULL AS [version],
		   1 AS [client_id],
		   0 /* Active */ AS [status],
		   0 /* None */ AS [reason],
		   NULL AS [note]

	INSERT INTO [dbo].[client_right]
	SELECT NULL AS [version],
		   1 AS [client_id],
		   1 AS [can_notify_party]

	COMMIT TRANSACTION T1
END TRY
BEGIN CATCH
	ROLLBACK TRANSACTION T1;
	THROW;
END CATCH 