use B1Test;
go

create table Classes
(
	ClassId int primary key,
	Name nvarchar(300)
)

insert into Classes(ClassId, Name)
	values (1, 'Денежные средства, драгоценные металлы и межбанковские операции'),
			(2, 'Кредитные и иные активные операции с клиентами'),
			(3, 'Счета по операциям клиентов'),
			(4, 'Ценные бумаги'),
			(5, 'Долгосрочные финансовые вложения в уставные фонды юридических лиц, основные средства и прочее имущество'),
			(6, 'Прочие активы и прочие пассивы'),
			(7, 'Собственный капитал банка'),
			(8, 'Доходы банка'),
			(9, 'Расходы банка')
go

create table Balance_Accounts
(
	AccountId int primary key,
	Class int foreign key references Classes(ClassId),
)
go

begin if not exists (select * from Balance_Accounts where AccountId = 1010)	begin insert into Balance_Accounts(AccountId, Class) values(1010, 1) end end

select * from Balance_Accounts

create table BalanceAccounts_Files
(
	AccountId int foreign key references Balance_Accounts(AccountId),
	FileName nvarchar(100)
)
go

create table Opening_Balance
(
	AccountId int foreign key references Balance_Accounts(AccountId),
	Assets money,
	Liabilities money,
)
go

create table Money_Turnover
(
	AccountId int foreign key references Balance_Accounts(AccountId),
	Debit money,
	Credit money,
)
go

create table Closing_Balance
(
	AccountId int foreign key references Balance_Accounts(AccountId),
	Assets money,
	Liabilities money,
)
go

create procedure insert_opening_balance
	@accountId int,
	@assets money,
	@liabilities money
as
	insert into Opening_Balance(AccountId, Assets, Liabilities)
		values (@accountId, @assets, @liabilities)
go

create procedure insert_money_turnover
	@accountId int,
	@debit money,
	@credit money
as
	insert into Money_Turnover(AccountId, Debit, Credit)
		values (@accountId, @debit, @credit)
go

create procedure insert_closing_balance
	@accountId int,
	@assets money,
	@liabilities money
as
	insert into Closing_Balance(AccountId, Assets, Liabilities)
		values (@accountId, @assets, @liabilities)
go


create procedure get_data @fileName nvarchar(100)
as
	select distinct acc.AccountId, ob.Assets, ob.Liabilities, 
			mt.Debit, mt.Credit, cb.Assets, cb.Liabilities, acc.Class, cl.Name
	from Balance_Accounts as acc join Opening_Balance as ob on acc.AccountId = ob.AccountId
		join Money_Turnover as mt on acc.AccountId = mt.AccountId
		join Closing_Balance as cb on cb.AccountId = acc.AccountId
		join Classes cl on cl.ClassId = acc.Class
		join BalanceAccounts_Files baf on baf.AccountId = acc.AccountId
	where baf.FileName = @fileName

go