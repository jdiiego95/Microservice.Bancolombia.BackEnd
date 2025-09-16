CREATE TABLE [dbo].[Accounts] 
(
    [AccountId] INT NOT NULL,
    [CustomerName] NVARCHAR(100) NOT NULL,
    [TotalBalance] DECIMAL(18,2) NOT NULL,
    CONSTRAINT [PK_Accounts] PRIMARY KEY ([AccountId])
);
