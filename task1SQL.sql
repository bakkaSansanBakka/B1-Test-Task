create database B1Test
go

use B1Test
go

create table Storage
(
	date date not null,
	stringEng nvarchar(100) not null,
	stringRu nvarchar(100) not null,
	numberInt integer not null,
	numberDec float not null
)
go

/*3. create procedure that imports lines from a text file into table*/
create procedure ImportFromFile
as
declare @date date, 
		@stringEng nvarchar(100),
		@stringRu nvarchar(100),
		@numberInt integer,
		@numberDec float,
		@linesAdded int = 0,
		@linesLeft int
truncate table Storage

declare db_cursor cursor for
select date, stringEng, stringRu, numberInt, numberDec
from openrowset
(
	bulk 'D:\B1TestTasks\Task1\Task1\bin\Debug\net6.0\MergedFiles\mergedFiles.txt',
	formatfile = 'D:\B1TestTasks\formatfile.xml',
	codepage = '65001'
)
as imported

open db_cursor

	fetch next from db_cursor into @date, @stringEng, @stringRu, @numberInt, @numberDec
	while @@FETCH_STATUS = 0
	begin
		print(@stringRu)
		insert into Storage(date, stringEng, stringRu, numberInt, numberDec)
			values(@date, @stringEng, @stringRu, @numberInt, @numberDec)
		set @linesAdded += 1
		set @linesLeft = @@CURSOR_ROWS
		print('Lines added: ' + cast(@linesAdded as nvarchar(10)) + ', lines left to add: ' + cast(@linesLeft as nvarchar(10)))
		fetch next from db_cursor into @date, @stringEng, @stringRu, @numberInt, @numberDec
	end

close db_cursor
go

/*check if procedure works*/
select * from Storage
exec ImportFromFile
select * from Storage
go

/*4. create procedure that counts sum of all integers and median of all floats*/
/*since sum of all integers causes an overflow error I decided to replace the operation of calculating sum 
with the operation of calculating the maximum of all integers in table*/
--sum can be find with the help of sum([column_name]) function
create procedure FindMaxOfIntegersAndMedianOfFloats
as
select max(numberInt) as Max_of_integers, 
	(SELECT MAX(numberDec) FROM (SELECT TOP 50 PERCENT numberDec FROM Storage ORDER BY numberDec) AS BottomHalf)
	+ (SELECT MIN(numberDec) FROM (SELECT TOP 50 PERCENT numberDec FROM Storage ORDER BY numberDec DESC) AS TopHalf) / 2 as MedianOfFloats
	from Storage
go

exec FindMaxOfIntegersAndMedianOfFloats
