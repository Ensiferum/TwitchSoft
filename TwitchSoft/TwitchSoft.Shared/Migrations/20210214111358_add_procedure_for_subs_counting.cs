using Microsoft.EntityFrameworkCore.Migrations;

namespace TwitchSoft.Shared.Migrations
{
    public partial class add_procedure_for_subs_counting : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
CREATE PROCEDURE CalculateSubscriptionStatistic
	@StartDate datetime,
	@EndDate datetime
AS
BEGIN
	SET @StartDate = CAST(@StartDate AS DATE)
	SET @EndDate = CAST(@EndDate AS DATE)

	DECLARE @ChannelId bigint

	DECLARE channels_cursor CURSOR FOR
	SELECT Id FROM Users WHERE JoinChannel = 1

	OPEN channels_cursor

	FETCH NEXT FROM channels_cursor
	INTO @ChannelId

	WHILE @@FETCH_STATUS = 0 
	BEGIN
		DECLARE @Date datetime = @StartDate
		DECLARE @Count int

		WHILE @Date < @EndDate
		BEGIN
			SELECT @Count = COUNT(*) FROM Subscriptions 
			WHERE ChannelId = @ChannelId AND SubscribedTime >= @Date AND SubscribedTime < DATEADD(day, 1, @Date)

			IF @Count > 0 
				INSERT INTO SubscriptionStatistics VALUES (@ChannelId, @Count, @Date)

			SET @Date = DATEADD(day, 1, @Date)
		END

		FETCH NEXT FROM channels_cursor
		INTO @ChannelId
	END

	CLOSE channels_cursor;  
	DEALLOCATE channels_cursor;  
END
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
			migrationBuilder.Sql(@"DROP PROCEDURE CalculateSubscriptionStatistic");
        }
    }
}
