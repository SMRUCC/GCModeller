---
title: writeTableAPI
---

# writeTableAPI
_namespace: [RDotNET.Extensions.VisualBasic.utils.read.table](N-RDotNET.Extensions.VisualBasic.utils.read.table.html)_

write.table prints its required argument x (after converting it to a data frame if it is not one nor a matrix) to a file or connection.

> 
>  If the table has no columns the rownames will be written only if row.names = TRUE, and vice versa.
>  Real And complex numbers are written to the maximal possible precision.
>  If a data frame has matrix-Like columns these will be converted To multiple columns In the result (via As.matrix) And so a character col.names Or a numeric quote should refer To the columns In the result, Not the input. Such matrix-Like columns are unquoted by Default.
>  Any columns In a data frame which are lists Or have a Class (e.g., dates) will be converted by the appropriate As.character method: such columns are unquoted by Default. 
>  On the other hand, any Class information For a matrix Is discarded And non-atomic (e.g., list) matrices are coerced To character.
>  Only columns which have been converted To character will be quoted If specified by quote.
>  The dec argument only applies To columns that are Not subject To conversion To character because they have a Class Or are part Of a matrix-Like column (Or matrix), In particular To columns Protected by I(). Use options("OutDec") To control such conversions.
>  In almost all cases the conversion of numeric quantities Is governed by the option "scipen" (see options), but with the internal equivalent of digits = 15. 
>  For finer control, use format to make a character matrix/data frame, And call write.table on that.
>  These functions check For a user interrupt every 1000 lines Of output.
>  If file Is a non-open connection, an attempt Is made To open it And Then close it after use.
>  To write a Unix-style file on Windows, use a binary connection e.g. file = file("filename", "wb").
>  



### Properties

#### append
logical. Only relevant if file Is a character string. If TRUE, the output Is appended to the file. If FALSE, any existing file of the name Is destroyed.
#### colNames
either a logical value indicating whether the column names of x are to be written along with x, or a character vector of column names to be written. See the section on ‘CSV files’ for the meaning of col.names = NA.
#### dec
the string to use for decimal points in numeric or complex columns: must be a single character.
#### eol
the character(s) to print at the end of each line (row). For example, eol = "\r\n" will produce Windows' line endings on a Unix-alike OS, and eol = "\r" will produce files as expected by Excel:mac 2004.
#### file
either a character string naming a file or a connection open for writing. "" indicates output to the console.
#### fileEncoding
character string: if non-empty declares the encoding to be used on a file (not a connection) so the character data can be re-encoded as they are written. See file.
#### na
the string to use for missing values in the data.
#### qmethod
a character string specifying how to deal with embedded double quote characters when quoting strings. 
 Must be one of "escape" (default for write.table), in which case the quote character is escaped in C style by a backslash, or "double" (default for write.csv and write.csv2), in which case it is doubled. You can specify just the initial letter.
#### quote
a logical value (TRUE or FALSE) or a numeric vector. If TRUE, any character or factor columns will be surrounded by double quotes. 
 If a numeric vector, its elements are taken as the indices of columns to quote. In both cases, row and column names are quoted if they are written. 
 If FALSE, nothing is quoted.
#### rowNames
either a logical value indicating whether the row names of x are to be written along with x, or a character vector of row names to be written.
#### sep
the field separator string. Values within each row of x are separated by this string.
#### x
the object to be written, preferably a matrix or data frame. If not, it is attempted to coerce x to a data frame.
