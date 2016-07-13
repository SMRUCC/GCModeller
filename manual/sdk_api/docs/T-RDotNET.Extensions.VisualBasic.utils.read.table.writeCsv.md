---
title: writeCsv
---

# writeCsv
_namespace: [RDotNET.Extensions.VisualBasic.utils.read.table](N-RDotNET.Extensions.VisualBasic.utils.read.table.html)_

By default there is no column name for a column of row names. If col.names = NA and row.names = TRUE a blank column name is added, which is the convention used for CSV files to be read by spreadsheets. Note that such CSV files can be read in R by
 read.csv(file = "<filename>", row.names = 1)
 
 write.csv And write.csv2 provide convenience wrappers for writing CSV files. 
 They set sep And dec (see below), qmethod = "double", And col.names to NA if row.names = TRUE (the default) And to TRUE otherwise.
 write.csv uses "." for the decimal point And a comma for the separator.




