CREATE TABLE [dbo].[TransactionsHistory]
(
    [TransactionId] BIGINT NOT NULL IDENTITY(1,1),
    [BankCode] NVARCHAR(50) NOT NULL,
    [FromAccountId] INT NOT NULL,
    [ToAccountId] INT NOT NULL,
    [TransactionType] CHAR(3) NOT NULL,
    [Amount] DECIMAL(18,2) NOT NULL,
    [TransactionDate] DATETIME NOT NULL DEFAULT(GETDATE()),

    CONSTRAINT [PK_TransactionsHistory] PRIMARY KEY ([TransactionId]),
    CONSTRAINT [FK_TransactionsHistory_ToAccount] FOREIGN KEY ([ToAccountId]) REFERENCES [dbo].[Accounts]([AccountId])
);