---
title: readTable
---

# readTable
_namespace: [RDotNET.Extensions.VisualBasic.utils.read.table](N-RDotNET.Extensions.VisualBasic.utils.read.table.html)_

[read.table]
 Reads a file in table format and creates a data frame from it, with cases corresponding to lines and variables to fields in the file.




### Properties

#### allowEscapes
logical. Should C-style escapes such as \n be processed Or read verbatim (the default)? 
 Note that if Not within quotes these could be interpreted as a delimiter (but Not as a comment character). For more details see scan.
#### asIs
the default behavior of read.table is to convert character variables (which are not converted to logical, numeric or complex) to factors. 
 The variable as.is controls the conversion of columns not otherwise specified by colClasses. 
 Its value is either a vector of logicals (values are recycled if necessary), or a vector of numeric or character indices which specify which columns should not be converted to factors.
 Note: to suppress all conversions including those of numeric columns, set colClasses = "character".
 Note that As.Is Is specified per column (Not per variable) And so includes the column of row names (if any) And any columns to be skipped.
#### blankLinesSkip
logical: if TRUE blank lines in the input are ignored.
#### checkNames
logical. If TRUE then the names of the variables in the data frame are checked to ensure that they are syntactically valid variable names. 
 If necessary they are adjusted (by make.names) so that they are, and also to ensure that there are no duplicates.
#### colClasses
character. A vector of classes to be assumed for the columns. Recycled as necessary. If named and shorter than required, names are matched to the column names with unspecified values are taken to be NA.
 Possible values are NA (the Default, When type.convert Is used), "NULL" (When the column Is skipped), one Of the atomic vector classes (logical, Integer, numeric, complex, character, raw), Or "factor", "Date" Or "POSIXct". Otherwise there needs To be an As method (from package methods) For conversion from "character" To the specified formal Class.
 Note that colClasses Is specified per column (Not per variable) And so includes the column Of row names (If any).
#### colNames
a vector of optional names for the variables. The default is to use "V" followed by the column number.
#### commentChar
character: a character vector of length one containing a single character or an empty string. Use "" to turn off the interpretation of comments altogether.
#### dec
the character used In the file For Decimal points.
#### encoding
encoding to be assumed for input strings. 
 It is used to mark character strings as known to be in Latin-1 or UTF-8 (see Encoding): it is not used to re-encode the input, but allows R to handle encoded strings in their native encoding (if one of those two). See ‘Value’ and ‘Note’.
#### fileEncoding
character string: if non-empty declares the encoding used on a file (not a connection) so the character data can be re-encoded. 
 See the ‘Encoding’ section of the help for file, the ‘R Data Import/Export Manual’ and ‘Note’.
#### fill
logical. If TRUE then in case the rows have unequal length, blank fields are implicitly added. See ‘Details’.
#### flush
logical: if TRUE, scan will flush to the end of the line after reading the last of the fields requested. This allows putting comments after the last field.
#### naStrings
a character vector of strings which are to be interpreted as NA values. Blank fields are also considered to be missing values in logical, integer, numeric and complex fields.
#### nrows
integer: the maximum number of rows to read in. Negative and other invalid values are ignored.
#### numerals
string indicating how to convert numbers whose conversion to double precision would lose accuracy, see type.convert. Can be abbreviated. (Applies also to complex-number inputs.)
#### quote
the set of quoting characters. To disable quoting altogether, use quote = "". See scan for the behaviour on quotes embedded in quotes. Quoting is only considered for columns read as character, which is all of them unless colClasses is specified.
#### rowNames
a vector of row names. This can be a vector giving the actual row names, or a single number giving the column of the table which contains the row names, or character string giving the name of the table column containing the row names.
 If there Is a header And the first row contains one fewer field than the number Of columns, the first column In the input Is used For the row names. Otherwise If row.names Is missing, the rows are numbered.
 Using row.names = NULL forces row numbering. Missing Or NULL row.names generate row names that are considered To be 'automatic’ (and not preserved by as.matrix).
#### sep
the field separator character. Values on each line of the file are separated by this character. If sep = "" (the default for read.table) the separator is ‘white space’, that is one or more spaces, tabs, newlines or carriage returns.
#### skip
integer: the number of lines of the data file to skip before beginning to read data.
#### skipNul
logical: should nuls be skipped?
#### stringsAsFactors
logical: should character vectors be converted to factors? Note that this is overridden by as.is and colClasses, both of which allow finer control.
#### stripWhite
logical. Used only when sep has been specified, and allows the stripping of leading and trailing white space from unquoted character fields (numeric fields are always stripped). 
 See scan for further details (including the exact meaning of ‘white space’), remembering that the columns may include the row names.
#### text
character string: if file is not supplied and this is, then data are read from the value of text via a text connection. 
 Notice that a literal string can be used to include (small) data sets within R code.
