CREATE SEQUENCE dbo.IdSequence START WITH 1 INCREMENT BY 1;

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Accounts')
BEGIN
    CREATE TABLE Accounts (
        Id INT PRIMARY KEY,
        Code NVARCHAR(100) NOT NULL, 
        UserId NVARCHAR(450) NOT NULL,
        Balance DECIMAL(18,2) DEFAULT 0,
        IsClosed BIT DEFAULT 0,
        CONSTRAINT FK_Accounts_Users FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id)
    );
END

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Transactions')
BEGIN
    CREATE TABLE Transactions (
        Id INT PRIMARY KEY,
        CreatedAt DATETIME NOT NULL,
        [Type] INT NOT NULL,
        Amount DECIMAL(18, 2) NOT NULL,
        SenderAccountId INT NULL,
        RecipientAccountId INT NULL,
        AccountId INT NULL

        CONSTRAINT FK_Transactions_Sender FOREIGN KEY (SenderAccountId) 
            REFERENCES Accounts(Id),
            
        CONSTRAINT FK_Transactions_Recipient FOREIGN KEY (RecipientAccountId) 
            REFERENCES Accounts(Id),
            
        CONSTRAINT FK_Transactions_Account FOREIGN KEY (AccountId) 
            REFERENCES Accounts(Id)
    );
END