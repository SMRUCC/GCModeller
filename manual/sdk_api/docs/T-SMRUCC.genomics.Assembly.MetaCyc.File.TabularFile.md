---
title: TabularFile
---

# TabularFile
_namespace: [SMRUCC.genomics.Assembly.MetaCyc.File](N-SMRUCC.genomics.Assembly.MetaCyc.File.html)_

Each tabular file contains data for one class of objects, such as reactions or pathways.
 This type of file contains a single table of tab-delimited columns and newline-delimited
 rows. The first row contains headers which describe the data beneath them. Each of the
 remaining rows represents an object, and each column is an attribute of the object.

 Column names that would otherwise be the same contain a number x having values 1, 2, 3,
 etc. to distinguish them. Comment lines can be anywhere in the file and must begin with
 the following symbol:

 #




