USE [CoralTime]
GO

/****** Object:  StoredProcedure [dbo].[GetEntriesForDay]    Script Date: 16/11/2021 11:03:49 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetEntriesForDay]
	@date date
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
		select 
		tm.Id,
		pr.Name as Project,
		ts.Name as Task,
		ts.Description TaskDescription,
		tm.Date, 
		tm.LastUpdateDate,
		tm.Description, 
		tm.TimeActual,
		tm.TimeFrom,
		tm.TimeTo,
		(SELECT CONVERT(varchar, DATEADD(ms,tm.TimeFrom * 1000, 0), 114)) as FromTime, 
		(SELECT CONVERT(varchar, DATEADD(ms,tm.TimeTo * 1000, 0), 114)) as ToTime,
		tm.TimeTimerStart,
		(SELECT CONVERT(varchar, DATEADD(ms,tm.TimeTimerStart * 1000, 0), 114)) as timeStart,
		(SELECT CONVERT(varchar, DATEADD(ms,tm.TimeActual * 1000, 0), 114)) as Duration
		from TimeEntries tm 
		Inner Join Projects pr on tm.ProjectId=pr.Id
		Inner Join TaskTypes ts on tm.TaskTypesId = ts.Id
		where datediff(day, tm.CreationDate, @date) = 0
		order by tm.TimeTo desc
END
GO

/****** Object:  StoredProcedure [dbo].[GetEntryById]    Script Date: 16/11/2021 11:03:49 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetEntryById] 
	@id INT
AS
BEGIN -- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	-- Insert statements for procedure here
	SELECT tm.Id,
		pr.Name AS Project,
        pr.Id AS ProjectId,
		ts.Name AS Task,
        ts.Id AS TaskTypesId,
		ts.Description TaskDescription,
		tm.DATE,
		tm.LastUpdateDate,
		tm.Description,
		tm.TimeActual,
		tm.TimeFrom,
		tm.TimeTo,
		( SELECT CONVERT(VARCHAR, DATEADD(ms, tm.TimeFrom * 1000, 0), 114) ) AS FromTime,
		( SELECT CONVERT(VARCHAR, DATEADD(ms, tm.TimeTo * 1000, 0), 114) ) AS ToTime,
		tm.TimeTimerStart,
		( SELECT CONVERT(VARCHAR, DATEADD(ms, tm.TimeTimerStart * 1000, 0), 114) ) AS timeStart,
		( SELECT CONVERT(VARCHAR, DATEADD(ms, tm.TimeActual * 1000, 0), 114) ) AS Duration
	FROM TimeEntries tm
	INNER JOIN Projects pr ON tm.ProjectId = pr.Id
	INNER JOIN TaskTypes ts ON tm.TaskTypesId = ts.Id
	WHERE tm.Id = @id
END

/*
exec GetEntryById 235
*/
GO

/****** Object:  StoredProcedure [dbo].[InsetTimeEntry]    Script Date: 16/11/2021 11:03:49 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[InsetTimeEntry]
	@creationDate		datetime2(7),
	@creatorId			nvarchar(450),
	@date				datetime2(7),
	@description		nvarchar(1000),
	@isFromToShow		bit,
	@lastEditorUserId	nvarchar(450),
	@lastUpdateDate		datetime2(7),
	@memberId			int,
	@timeEstimated		int,
	@projectId			int,
	@taskTypesId		int,
	@timeActual			int,
	@timeFrom			int,
	@timeTimerStart		int,
	@timeTo				int,
	@new_identity		int = NULL OUTPUT
AS
BEGIN
	SET NOCOUNT ON;

	insert into TimeEntries (
		CreationDate,
		CreatorId,
		[Date],
		[Description],
		IsFromToShow,
		LastEditorUserId,
		LastUpdateDate,
		MemberId,
		TimeEstimated, 
		ProjectId,
		TaskTypesId,
		TimeActual,
		TimeFrom,
		TimeTimerStart,
		TimeTo
	) Values(
		@creationDate,
		@creatorId,
		@date,
		@description,
		@isFromToShow,
		@lastEditorUserId,
		@lastUpdateDate,
		@memberId,
		@timeEstimated,
		@projectId,
		@taskTypesId,
		@timeActual,
		@timeFrom,
		@timeTimerStart,
		@timeTo
	)
	SET @new_identity = SCOPE_IDENTITY();
END
GO

/****** Object:  StoredProcedure [dbo].[UpdateTimeEntry]    Script Date: 16/11/2021 11:03:49 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[UpdateTimeEntry] @timeEntryId INT,
	@fromTime NVARCHAR(255),
	@toTime NVARCHAR(255),
	@description NVARCHAR(1000),
	@projectId INT,
	@taskTypesId INT
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @fr INT,
		@to INT,
		@act INT

	SET @fr = (
			SELECT DATEDIFF(second, '2021-01-11 00:00:00:00', '2021-01-11 ' + @fromTime)
			)
	SET @to = (
			SELECT DATEDIFF(second, '2021-01-11 00:00:00:00', '2021-01-11 ' + @toTime)
			)
	SET @act = @to - @fr

	SELECT @fr,
		@to,
		@act

	UPDATE TimeEntries
	SET TimeFrom = @fr,
		TimeTo = @to,
		TimeActual = @act,
		[Description] = @description,
		ProjectId = @projectId,
		TaskTypesId = @taskTypesId
	WHERE id = @timeEntryId
END



/*
12:30:25:000
exec UpdateTimeEntry 2149, '12:30:41:26', '14:52:46:35'
*/
GO

