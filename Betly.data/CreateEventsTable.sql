-- Create Events Table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Events]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Events] (
        [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [Title] NVARCHAR(200) NOT NULL,
        [Description] NVARCHAR(1000) NULL,
        [Date] DATETIME2 NOT NULL,
        [TeamA] NVARCHAR(100) NOT NULL,
        [TeamB] NVARCHAR(100) NOT NULL,
        [OddsTeamA] DECIMAL(18, 2) NOT NULL,
        [OddsTeamB] DECIMAL(18, 2) NOT NULL,
        [OddsDraw] DECIMAL(18, 2) NOT NULL
    );
END
GO

-- Add foreign keys and new columns to Bets table
-- Assuming Bets table already exists. If not, include CREATE TABLE for Bets as well.
-- We are adding UserId, EventId, SelectedOutcome, Outcome.
-- Check if columns exist before adding to avoid errors if re-run.

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Bets]') AND name = 'UserId')
BEGIN
    ALTER TABLE [dbo].[Bets] ADD [UserId] INT NOT NULL DEFAULT 0; -- Set appropriate default or handle existing data
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Bets]') AND name = 'EventId')
BEGIN
    ALTER TABLE [dbo].[Bets] ADD [EventId] INT NOT NULL DEFAULT 0; 
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Bets]') AND name = 'SelectedOutcome')
BEGIN
    ALTER TABLE [dbo].[Bets] ADD [SelectedOutcome] NVARCHAR(50) NOT NULL DEFAULT '';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Bets]') AND name = 'Outcome')
BEGIN
    ALTER TABLE [dbo].[Bets] ADD [Outcome] NVARCHAR(50) NOT NULL DEFAULT 'Pending';
END
GO

-- Add Foreign Key Constraints (Adjust defaults/existing data first if necessary)
-- We assume User table exists as [dbo].[Users]

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Bets_Users_UserId]') AND parent_object_id = OBJECT_ID(N'[dbo].[Bets]'))
BEGIN
    ALTER TABLE [dbo].[Bets] WITH CHECK ADD CONSTRAINT [FK_Bets_Users_UserId] FOREIGN KEY([UserId])
    REFERENCES [dbo].[Users] ([Id])
    ON DELETE CASCADE;

    ALTER TABLE [dbo].[Bets] CHECK CONSTRAINT [FK_Bets_Users_UserId];
END
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Bets_Events_EventId]') AND parent_object_id = OBJECT_ID(N'[dbo].[Bets]'))
BEGIN
    ALTER TABLE [dbo].[Bets] WITH CHECK ADD CONSTRAINT [FK_Bets_Events_EventId] FOREIGN KEY([EventId])
    REFERENCES [dbo].[Events] ([Id])
    ON DELETE CASCADE;

    ALTER TABLE [dbo].[Bets] CHECK CONSTRAINT [FK_Bets_Events_EventId];
END
GO
