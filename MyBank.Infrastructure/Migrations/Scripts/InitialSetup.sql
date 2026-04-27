IF NOT EXISTS (SELECT * FROM sys.sequences WHERE name = 'IdSequence')
BEGIN
    CREATE SEQUENCE dbo.IdSequence START WITH 1 INCREMENT BY 1;
END

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Accounts')
BEGIN
    CREATE TABLE Accounts (
        Id INT PRIMARY KEY,
        Code NVARCHAR(100) NOT NULL, 
        UserId NVARCHAR(450) NOT NULL REFERENCES AspNetUsers(Id),
        Balance DECIMAL(18,2) DEFAULT 0,
        IsClosed BIT DEFAULT 0
    );
    
    CREATE INDEX IX_Accounts_UserId 
    ON Accounts(UserId);
END

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Transactions')
BEGIN
    CREATE TABLE Transactions (
        Id INT PRIMARY KEY,
        CreatedAt DATETIME NOT NULL,
        [Type] INT NOT NULL,
        Amount DECIMAL(18, 2) NOT NULL,
        SenderAccountId INT NULL REFERENCES Accounts(Id),
        RecipientAccountId INT NULL REFERENCES Accounts(Id),
        AccountId INT NULL REFERENCES Accounts(Id)
    );

    CREATE INDEX IX_Transactions_AccountId 
    ON Transactions (AccountId DESC);

    CREATE INDEX IX_Transactions_CreatedAt 
    ON Transactions (CreatedAt DESC);

    CREATE INDEX IX_Transactions_SenderAccountId 
    ON Transactions (SenderAccountId) 
    WHERE SenderAccountId IS NOT NULL;

    CREATE INDEX IX_Transactions_RecipientAccountId 
    ON Transactions (RecipientAccountId) 
    WHERE RecipientAccountId IS NOT NULL;
END

