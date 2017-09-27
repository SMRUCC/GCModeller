CREATE DEFINER=`root`@`localhost` PROCEDURE `truncate_all_tables`(in db_name varchar(128))
BEGIN

/* 
 * This is a stored procedure to apply truncate table to all tables in a specific database 
 *
 * https://stackoverflow.com/questions/8398847/apply-mysql-query-to-each-table-in-a-database
 */

declare finish int default 0;
declare tab varchar(100);
declare cur_tables cursor for 
	select table_name 
    from information_schema.tables 
    where table_schema = db_name and table_type = 'base table';
    
declare continue handler for not found set finish = 1;

open cur_tables;

	my_loop:loop

		fetch cur_tables into tab;
		
        if finish = 1 then
			leave my_loop;
		end if;

		set @str = concat('truncate ', tab);
		prepare stmt from @str;
		execute stmt;
		deallocate prepare stmt;
	
    end loop;
    
close cur_tables;

END