USE [BulkCopyDemo]
GO
/****** Object:  StoredProcedure [dbo].[UpdateWinlossDatasToUsers]    Script Date: 2023/12/12 ¤W¤È 10:47:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[UpdateWinlossDatasToUsers]

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	UPDATE [BulkCopyDemo].[dbo].[Member]
    SET 
        Credit = Credit + Stock.Winloss
    FROM [BulkCopyDemo].[dbo].[Member]
    INNER JOIN [BulkCopyDemo].[dbo].[StockTemp] Stock
    ON Stock.Id = [BulkCopyDemo].[dbo].[Member].Id

	TRUNCATE TABLE [BulkCopyDemo].[dbo].[StockTemp]
END
