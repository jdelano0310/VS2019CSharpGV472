select * from tblProducts;

select * from tblCategories;
select max(NumberOfCategories) 
	from 
		(select count(id) as NumberOfCategories 
			from tblCategories 
				group by ProductID) as tbl1;