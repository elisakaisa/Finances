IF OBJECT_ID('Finances.dbo.Subcategories', 'U') IS NOT NULL
BEGIN
    SET IDENTITY_INSERT Subcategories OFF;
END
SET IDENTITY_INSERT Categories ON;

-- Expenses, savings, Income - 1, 2, 3 -> transactionType

-- Insert into Categories
INSERT INTO Categories (Id, Name, State, TransactionType)
VALUES 
    (1, 'Home', 0, 1),
    (2, 'Loans', 0, 1),
    (3, 'Groceries', 0, 1),
	(4, 'Food', 0, 1),
    (5, 'Pets', 0, 1),
    (6, 'Car', 0, 1),
	(7, 'Transport', 0, 1),
    (8, 'Purchases', 0, 1),
    (9, 'Leisure', 0, 1),
	(10, 'Vacation', 0, 1),
    (11, 'Health', 0, 1),
    (12, 'Communications', 0, 1),
	(13, 'Miscellaneous', 0, 1),
    (14, 'Saved expenses', 0, 1),
    (15, 'Income', 0, 2),
	(16, 'Savings', 0, 2);

SET IDENTITY_INSERT Categories OFF;


SET IDENTITY_INSERT Subcategories ON;

-- Insert into Subcategories
INSERT INTO Subcategories (Id, Name, State, TransactionType, DisplayOrder, CategoryId)
VALUES 
    (1, 'Fees',					0, 1, 1, 1),
    (2, 'Utilities',			0, 1, 2, 1),
    (3, 'Home insurance',		0, 1, 3, 1),
    (4, 'Furniture',			0, 1, 4, 1),
    (5, 'Home misc',			0, 1, 5, 1),
    (6, 'Interest',				0, 1, 6, 2),
	(7, 'Amortization',			0, 1, 7, 2),
	(8, 'Groceries',			0, 1, 8, 3),
    (9, 'Restaurants',			0, 1, 9, 4),
    (10, 'Work food',			0, 1, 10, 4),
    (11, 'Take out',			0, 1, 11, 4),
    (12, 'Fika',				0, 1, 12, 4),
    (13, 'Snacks',				0, 1, 13, 4),
    (14,'Alcohol & bar',		0, 1, 14, 4),
    (15, 'Food misc',			0, 1, 15, 4),
    (16, 'Pet health',			0, 1, 16, 5),
    (17, 'Pet store',			0, 1, 17, 5),
    (18, 'Pet misc',			0, 1, 18, 5),
	(19, 'Fuel',				0, 1, 19, 6),
	(20, 'Car repairs',			0, 1, 20, 6),
    (21, 'Parking space rent',	0, 1, 21, 6),
    (22, 'Parking',				0, 1, 22, 6),
    (23, 'Car insurance',		0, 1, 23, 6),
    (24, 'Trängselskatt',		0, 1, 24, 6),
	(25, 'Car misc',			0, 1, 25, 6),
    (26, 'Public transport',	0, 1, 26, 7),
    (27, 'Other transport',		0, 1, 27, 7),
    (28, 'Clothes',				0, 1, 28, 8),
    (29, 'Expenses gifts',		0, 1, 29, 8),
    (30, 'Other purchases',		0, 1, 30, 8),
	(31, 'Gym card',			0, 1, 31, 9),
	(32, 'Subscriptions',		0, 1, 32, 9),
    (33, 'Games',				0, 1, 33, 9),
    (34, 'Books',				0, 1, 34, 9),
    (35, 'Leisure misc',		0, 1, 35, 9),
    (36, 'Vacation living',		0, 1, 36, 10),
	(37, 'Vacation transport',	0, 1, 37, 10),
    (38, 'Vacation food',		0, 1, 38, 10),
    (39, 'Vacation misc',		0, 1, 39, 10),
    (40, 'Doctor & dentist',	0, 1, 40, 11),
    (41, 'Farmacy',				0, 1, 41, 11),
    (42, 'Health insurance',	0, 1, 42, 11),
	(43, 'Health & care misc',	0, 1, 43, 11),
	(44, 'Phone',				0, 1, 44, 12),
    (45, 'Internet',			0, 1, 45, 12),
    (46, 'Communications misc',	0, 1, 46, 12),
    (47, 'Bank & admin fees',	0, 1, 47, 13),
    (48, 'Union & A-kassa',		0, 1, 48, 13),
	(49, 'Misc',				0, 1, 49, 13),
	(50, 'Vacation savings',	0, 1, 50, 14),
    (51, 'Delayed consumption',	0, 1, 51, 14),
    (52, 'Salary',				0, 2, 52, 15),
    (53, 'Income gifts',		0, 2, 53, 15),
    (54, 'Income misc',			0, 2, 54, 15),
    (55, 'Buffer',				0, 3, 55, 16),
	(56, 'Investments',			0, 3, 56, 16),
	(57, 'Other savings',		0, 3, 57, 16);

SET IDENTITY_INSERT Subcategories OFF;



insert into Households (Id, Name)
values('10000000-0000-0000-0000-000000000000', 'Household1')

insert into Users (Id, Name, HouseholdId)
values
	('00000000-0000-0000-0000-000000000001', 'User1', '10000000-0000-0000-0000-000000000000'),
	('00000000-0000-0000-0000-000000000002', 'User2', '10000000-0000-0000-0000-000000000000')


-- Insert 20 transactions with unique GUIDs for Id

INSERT INTO [Finances].[dbo].[Transactions] (
      [Id]
    , [Description]
    , [Date]
    , [FromOrTo]
    , [Location]
    , [ExcludeFromSummary]
    , [TransactionType]
    , [SplitType]
    , [UserShare]
    , [Amount]
    , [ToVerify]
    , [ModeOfPayment]
    , [FinancialMonth]
    , [SubcategoryId]
    , [UserId]
)
VALUES
	-- individual transactions
    (NEWID(), 'Transaction 1', '2024-10-01', 'Sender', 'Location 1',	0, 1, 0, null,	1245,		0, 0, '202410', 53, '00000000-0000-0000-0000-000000000001'),
    (NEWID(), 'Transaction 2', '2024-10-01', 'Receiver', 'Location 2',	0, 1, 0, null,	2.032,		0, 1, '202410', 53, '00000000-0000-0000-0000-000000000002'),
    (NEWID(), 'Transaction 3', '2024-10-01', 'Sender', 'Location 3',	0, 0, 0, null,	-2564.2,	0, 2, '202410', 25, '00000000-0000-0000-0000-000000000001'),
    (NEWID(), 'Transaction 4', '2024-10-01', 'Receiver', 'Location 4',	0, 0, 0, null,	546,		0, 3, '202410', 45, '00000000-0000-0000-0000-000000000002'),
    (NEWID(), 'Transaction 5', '2024-10-01', 'Sender', 'Location 5',	0, 0, 0, null,	5468,		0, 0, '202410', 3, '00000000-0000-0000-0000-000000000001'),
	(NEWID(), 'Transaction 4', '2024-10-01', 'Receiver', 'Location 4',	0, 2, 0, null,	562,		0, 1, '202410', 56, '00000000-0000-0000-0000-000000000002'),
    (NEWID(), 'Transaction 5', '2024-10-01', 'Sender', 'Location 5',	0, 2, 0, null,	9652,		0, 2, '202410', 55, '00000000-0000-0000-0000-000000000001'),
	-- even transactions
    (NEWID(), 'Transaction 1', '2024-10-01', 'Sender', 'Location 1',	0, 0, 1, null,	-56,		0, 5, '202410', 3, '00000000-0000-0000-0000-000000000001'),
    (NEWID(), 'Transaction 2', '2024-10-01', 'Receiver', 'Location 2',	0, 0, 1, null,	5464,		0, 4, '202409', 7, '00000000-0000-0000-0000-000000000002'),
    (NEWID(), 'Transaction 3', '2024-10-01', 'Sender', 'Location 3',	0, 0, 1, null,	75.12,		0, 3, '202410', 25, '00000000-0000-0000-0000-000000000001'),
    (NEWID(), 'Transaction 4', '2024-10-01', 'Receiver', 'Location 4',	0, 0, 1, null,	962.12,		0, 2, '202410', 2, '00000000-0000-0000-0000-000000000002'),
    (NEWID(), 'Transaction 20', '2024-10-01', 'Sender', 'Location 20',	0, 0, 1, null,	62.321,		0, 1, '202410', 1, '00000000-0000-0000-0000-000000000001'),
	-- income transactions
    (NEWID(), 'Transaction 1', '2024-10-01', 'Sender', 'Location 1',	0, 0, 2, null,	54.21,		0, 1, '202410', 1, '00000000-0000-0000-0000-000000000001'),
    (NEWID(), 'Transaction 2', '2024-10-01', 'Receiver', 'Location 2',	0, 0, 2, null,	63.21,		0, 2, '202409', 25, '00000000-0000-0000-0000-000000000002'),
    (NEWID(), 'Transaction 3', '2024-10-01', 'Sender', 'Location 3',	0, 0, 2, null,	6.325,		0, 3, '202409', 6, '00000000-0000-0000-0000-000000000001'),
    (NEWID(), 'Transaction 4', '2024-10-01', 'Receiver', 'Location 4',	0, 0, 2, null,	983.21,		0, 4, '202410', 47, '00000000-0000-0000-0000-000000000002'),
    (NEWID(), 'Transaction 20', '2024-10-01', 'Sender', 'Location 20',	0, 0, 2, null,	85684,		0, 5, '202410', 16, '00000000-0000-0000-0000-000000000001'),
	-- custom transactions
    (NEWID(), 'Transaction 1', '2024-10-01', 'Sender', 'Location 1',	0, 0, 3, 0.21,	12345,		0, 5, '202410', 3, '00000000-0000-0000-0000-000000000001'),
    (NEWID(), 'Transaction 2', '2024-10-01', 'Receiver', 'Location 2',	0, 0, 3, 0.96,	6512,		0, 4, '202409', 3, '00000000-0000-0000-0000-000000000002'),
    (NEWID(), 'Transaction 3', '2024-10-01', 'Sender', 'Location 3',	0, 0, 3, 0.3,	6541,		0, 3, '202409', 12, '00000000-0000-0000-0000-000000000001'),
    (NEWID(), 'Transaction 4', '2024-10-01', 'Receiver', 'Location 4',	0, 0, 3, 0.12,	32.021,		0, 2, '202410', 23, '00000000-0000-0000-0000-000000000002'),
    (NEWID(), 'Transaction 20', '2024-10-01', 'Sender', 'Location 20',	0, 0, 3, 0.32,	6532.2,		0, 1, '202410', 8, '00000000-0000-0000-0000-000000000001');
