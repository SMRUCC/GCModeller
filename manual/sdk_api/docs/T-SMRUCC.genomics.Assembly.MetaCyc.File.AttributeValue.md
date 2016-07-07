---
title: AttributeValue
---

# AttributeValue
_namespace: [SMRUCC.genomics.Assembly.MetaCyc.File](N-SMRUCC.genomics.Assembly.MetaCyc.File.html)_

Attribute-Value: Each attribute-value file contains data for one class of objects,
 such as genes or proteins. A file is divided into entries, where one entry describes
 one database object.

> 
>  An entry consists of a set of attribute-value pairs, •which describe properties of
>  the object, and relationships of the object to other object. Each attribute-value
>  pair typically resides on a single line of the file, although in some cases for
>  values that are long strings, the value will reside on multiple lines. An attribute-
>  value pair consists of an attribute name, followed by the string " - " and a value,
>  for example:
> 
>  LEFT - NADP
> 
>  A value that requires more than one line is continued by a newline followed by a /.
>  Thus, literal slashes at the beginning of a line must be escaped as //. A line that
>  contains only // separates objects. Comment lines can be anywhere in the file and
>  must begin with the following symbol:
> 
>  #
> 
>  Starting in version 6.5 of Pathway Tools, attribute-value files can also contain
>  annotation-value pairs. Annotations are a mechanism for attaching labeled values
>  to specific attribute values. For example, we might want to specify a coefficient
>  for a reactant in a chemical reaction. An annotations refers to the attribute value
>  that immediately precedes the annotation.
>  An annotation-value pair consists of a caret symbol "^" that points upward to indicate
>  that the annotation annotates the preceding attribute value, followed by the annotation
>  label, followed by the string " - ", followed by a value. The same attribute name or
>  annotation label with different values can appear any number of times in an object.
>  An example annotation-value pair that refers to the preceding attribute-value pair is:
> 
>  LEFT - NADP
>  ^COEFFICIENT - 1
>  



### Properties

#### DbProperty
The database property in the head section.
#### Objects
Slots objects reader model.
