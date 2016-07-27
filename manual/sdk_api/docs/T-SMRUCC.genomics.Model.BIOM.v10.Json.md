---
title: Json
---

# Json
_namespace: [SMRUCC.genomics.Model.BIOM.v10](N-SMRUCC.genomics.Model.BIOM.v10.html)_

##### The biom file format: Version 1.0
 
 The ``biom`` format is based on ``JSON`` to provide the overall structure for the format. 
 JSON is a widely supported format with native parsers available within many programming 
 languages.




### Properties

#### columns
``<list of objects>`` An ORDERED list of obj describing the columns
#### comment
``<string>`` A free text field containing any information that you
 feel Is relevant (Or just feel Like sharing)
#### data
``<list of lists>``, counts of observations by sample
 
 If matrix_type Is "sparse", 
 
 ```
 [[row, column, value],
 [row, column, value],
 ...]
 ```
 
 If matrix_type Is "dense", 
 
 ```
 [[value, value, value, ...],
 [value, value, value, ...],
 ...]
 ```
#### date
``<datetime>`` Date the table was built (ISO 8601 format)
#### format
``<string>`` The name and version of the current biom format
#### format_url
``<url>`` A string with a static URL providing format details
#### generated_by
``<string>`` Package and revision that built the table
#### id
``<string or null>`` a field that can be used to id a table (or null)
#### matrix_element_type
Value type in matrix (a controlled vocabulary)
 Acceptable values : 
 
 + "int" Integer
 + "float" : floating point
 + "unicode" : unicode string
#### matrix_type
``<string>`` Type of matrix data representation (a controlled vocabulary)
 Acceptable values : 
 
 + "sparse" : only non-zero values are specified
 + "dense" : every element must be specified
#### rows
``<list of objects>`` An ORDERED list of obj describing the rows
#### shape
``<list of ints>``, the number of rows and number of columns in data
#### type
``<string>`` Table type (a controlled vocabulary)
 Acceptable values : 
 
 + "OTU table"
 + "Pathway table"
 + "Function table"
 + "Ortholog table"
 + "Gene table"
 + "Metabolite table"
 + "Taxon table"
