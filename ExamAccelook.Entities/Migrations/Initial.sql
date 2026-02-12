CREATE DATABASE ExamAccelook

CREATE TABLE Category (
	CategoryId INT IDENTITY(1,1) PRIMARY KEY,
    CategoryName VARCHAR(100) NOT NULL UNIQUE,
    CategoryPrefix VARCHAR(10) NOT NULL UNIQUE,
    LastSequenceNumber INT NOT NULL DEFAULT 0
);

CREATE TABLE Ticket (
    TicketCode VARCHAR(50) NOT NULL PRIMARY KEY,
    CategoryId INT NOT NULL,
    TicketCreationOrder INT IDENTITY(1,1),
    TicketName VARCHAR(200) NOT NULL,
    SequenceNumber INT NOT NULL,
    Price DECIMAL(18,2) NOT NULL,
    EventDate DATETIME NOT NULL,
    Quota INT NOT NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,

    FOREIGN KEY (CategoryId) REFERENCES Category(CategoryId),
);

CREATE TABLE BookedTicket (
    BookedTicketId INT IDENTITY(1,1) PRIMARY KEY,
    BookingDate DATETIME NOT NULL DEFAULT GETDATE(),
    TotalAmount DECIMAL(18,2) NOT NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL
);

CREATE TABLE BookingDetails (
    BookingDetailId INT IDENTITY(1,1) PRIMARY KEY,
    BookedTicketId INT NOT NULL,
    TicketCode VARCHAR(50) NOT NULL,
    Quantity INT NOT NULL,
    UnitPrice DECIMAL(18,2) NOT NULL,
    Subtotal DECIMAL(18,2) NOT NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,

    FOREIGN KEY (BookedTicketId) REFERENCES BookedTicket(BookedTicketId),
    FOREIGN KEY (TicketCode) REFERENCES Ticket(TicketCode)
);

INSERT INTO Category (CategoryName, CategoryPrefix, LastSequenceNumber) VALUES
    ('Transportasi Laut', 'TL', 0),
    ('Transportasi Darat', 'TD', 0),
    ('Transportasi Udara', 'TU', 0),
    ('Cinema', 'C', 0),
    ('Hotel', 'H', 0),
    ('Konser', 'K', 0);


INSERT INTO Ticket (
    TicketCode,          
    CategoryId,
    TicketName,
    SequenceNumber,      
    Price,
    EventDate,
    Quota,
    CreatedAt
)
VALUES 
    ('TL001', 1, 'Kapar Ferri Jawa-Sumatra', 1, 50000000.00, '2030-03-01 12:00:00', 80, GETDATE()),
    ('TD001', 2, 'Bus Jawa-Sumatra', 1, 50000000.00, '2030-03-01 12:00:00', 70, GETDATE()),
    ('C001', 4, 'Ironman CGV', 1, 50000000.00, '2030-03-01 12:00:00', 99, GETDATE()),
    ('H001', 5, 'Ibis Hotel Jakarta 21-23', 1, 50000000.00, '2030-03-01 12:00:00', 76, GETDATE());

UPDATE Category 
SET LastSequenceNumber = 1 
WHERE CategoryId = 1;

UPDATE Category 
SET LastSequenceNumber = 1 
WHERE CategoryId = 2;

UPDATE Category 
SET LastSequenceNumber = 1 
WHERE CategoryId = 4;

UPDATE Category 
SET LastSequenceNumber = 1 
WHERE CategoryId = 5;

SELECT * FROM Ticket
SELECT * FROM Category
SELECT * FROM BookedTicket
SELECT * FROM BookingDetails