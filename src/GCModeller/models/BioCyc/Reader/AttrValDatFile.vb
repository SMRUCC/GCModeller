
''' <summary>
''' ## Attribute-Value
''' 
''' Each attribute-value file contains data for one class of objects, 
''' such as genes or proteins.  A file is divided into entries, where 
''' one entry describes one database object.
''' An entry consists Of a Set Of attribute-value pairs, which describe 
''' properties Of the Object, And relationships Of the Object To other 
''' Object. Each attribute-value pair typically resides On a Single 
''' line Of the file, although In some cases For values that are Long 
''' strings, the value will reside On multiple lines.  An attribute-value 
''' pair consists Of an attribute name, followed by the String " - " 
''' And a value, For example
''' 
''' ```
''' LEFT - NADP
''' ```
''' 
''' A value that requires more than one line Is continued by a newline 
''' followed by a /. Thus, literal slashes at the beginning Of a line 
''' must be escaped As //. A line that contains only // separates objects. 
''' Comment lines can be anywhere In the file And must begin With the 
''' following symbol
''' 
''' ```
''' #
''' ```
''' 
''' Starting In version 6.5 Of Pathway Tools, attribute-value files can 
''' also contain annotation-value pairs.  Annotations are a mechanism For 
''' attaching labeled values To specific attribute values.  For example, 
''' we might want To specify a coefficient For a reactant In a chemical 
''' reaction. An annotations refers To the attribute value that immediately 
''' precedes the annotation.  An annotation-value pair consists Of a caret 
''' symbol "^" that points upward To indicate that the annotation annotates 
''' the preceding attribute value, followed by the annotation label, 
''' followed by the String " - ", followed by a value. The same attribute 
''' name Or annotation label With different values can appear any number 
''' Of times In an Object.  An example annotation-value pair that refers 
''' To the preceding attribute-value pair Is
''' 
''' ```
''' LEFT - NADP
''' ^COEFFICIENT - 1
''' ```
''' </summary>
Public Class AttrValDatFile



End Class
