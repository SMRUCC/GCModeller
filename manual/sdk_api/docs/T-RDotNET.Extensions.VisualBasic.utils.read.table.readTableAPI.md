---
title: readTableAPI
---

# readTableAPI
_namespace: [RDotNET.Extensions.VisualBasic.utils.read.table](N-RDotNET.Extensions.VisualBasic.utils.read.table.html)_

Reads a file in table format and creates a data frame from it, with cases corresponding to lines and variables to fields in the file.




### Properties

#### file
The name Of the file which the data are To be read from. Each row Of the table appears As one line Of the file. 
 If it does Not contain an absolute path, the file name Is relative To the current working directory, getwd(). 
 Tilde-expansion Is performed where supported. This can be a compressed file (see file).
 Alternatively, file can be a readable text-mode connection (which will be opened for reading if necessary, And if so closed (And hence destroyed) at the end of the function call). 
 (If stdin() Is used, the prompts for lines may be somewhat confusing. Terminate input with a blank line Or an EOF signal, Ctrl-D on Unix And Ctrl-Z on Windows. Any pushback on stdin() will be cleared before return.)
 file can also be a complete URL. (For the supported URL schemes, see the 'URLs’ section of the help for url.)
#### header
a logical value indicating whether the file contains the names of the variables as its first line. 
 If missing, the value is determined from the file format: header is set to TRUE if and only if the first row contains one fewer field than the number of columns.
