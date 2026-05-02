IF NOT EXISTS (SELECT * FROM sys.sequences WHERE name = 'IdSequence')
BEGIN
    CREATE SEQUENCE dbo.IdSequence START WITH 1 INCREMENT BY 1;
END

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Clients')
BEGIN
    CREATE TABLE Clients (
        Id INT PRIMARY KEY,
        FirstName NVARCHAR(100) NOT NULL,
        LastName NVARCHAR(100) NOT NULL
    );
END

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Accounts')
BEGIN
    CREATE TABLE Accounts (
        Id INT PRIMARY KEY,
        Code NVARCHAR(100) NOT NULL, 
        ClientId INT NOT NULL REFERENCES Clients(Id),
        Balance DECIMAL(18,2) NOT NULL DEFAULT 0,
        IsClosed BIT NOT NULL DEFAULT 0
    );
    
    CREATE INDEX IX_Accounts_ClientId 
    ON Accounts(ClientId);
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

-- This table links clients to their user identities in the ASP.NET Identity system.
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ClientIdentity')
BEGIN
    CREATE TABLE ClientIdentity (
        ClientId INT NOT NULL UNIQUE REFERENCES Clients(Id),
        UserId NVARCHAR(450) NOT NULL UNIQUE REFERENCES AspNetUsers(Id)
    );
END



