---
title: RScripts
---

# RScripts
_namespace: [RDotNET.Extensions.VisualBasic](N-RDotNET.Extensions.VisualBasic.html)_





### Methods

#### c
```csharp
RDotNET.Extensions.VisualBasic.RScripts.c(System.Object[])
```
c(....)

|Parameter Name|Remarks|
|--------------|-------|
|x|-|


#### commandArgs
```csharp
RDotNET.Extensions.VisualBasic.RScripts.commandArgs(System.Boolean)
```
Provides access to a copy of the command line arguments supplied when this R session was invoked.

|Parameter Name|Remarks|
|--------------|-------|
|trailingOnly|logical. Should only arguments after --args be returned?|

_returns: 
 A character vector containing the name of the executable and the user-supplied command line arguments.
 The first element is the name of the executable by which R was invoked.
 The exact form of this element is platform dependent: it may be the fully qualified name, or simply the last component (or basename) of the application, or for an embedded R it can be anything the programmer supplied.
 If trailingOnly = True, a character vector Of those arguments (If any) supplied after --args.
 _
> 
>  These arguments are captured before the standard R command line processing takes place. This means that they are the unmodified values.
>  This is especially useful with the --args command-line flag to R, as all of the command line after that flag is skipped.
>  

#### dim
```csharp
RDotNET.Extensions.VisualBasic.RScripts.dim(System.String)
```
Retrieve or set the dimension of an object.

|Parameter Name|Remarks|
|--------------|-------|
|x|
 an R object, for example a matrix, array or data frame.
 For the default method, either NULL or a numeric vector, which is coerced to integer (by truncation).
 |

_returns: 
 For an array (And hence in particular, for a matrix) dim retrieves the dim attribute of the object. It Is NULL Or a vector of mode integer.
 The replacement method changes the "dim" attribute (provided the New value Is compatible) And removes any "dimnames" And "names" attributes.
 _
> 
>  Details
> 
>  The functions Dim And Dim<- are internal generic primitive functions.
>  Dim has a method For data.frames, which returns the lengths Of the row.names attribute Of x And Of x (As the numbers Of rows And columns respectively).
>  

#### list
```csharp
RDotNET.Extensions.VisualBasic.RScripts.list(System.String[])
```
Functions to construct, coerce and check for both kinds of R lists.

|Parameter Name|Remarks|
|--------------|-------|
|x|-|


#### median
```csharp
RDotNET.Extensions.VisualBasic.RScripts.median(System.String)
```


|Parameter Name|Remarks|
|--------------|-------|
|x|是一个对象，不是字符串|


#### names
```csharp
RDotNET.Extensions.VisualBasic.RScripts.names(System.String)
```
Functions to get or set the names of an object.

|Parameter Name|Remarks|
|--------------|-------|
|x|an R object.|

_returns: 
 a character vector of up to the same length as x, or NULL.

 Value
 For names, NULL Or a character vector of the same length as x. (NULL Is given if the object has no names, including for objects of types which cannot have names.)
 For an environment, the length Is the number of objects in the environment but the order of the names Is arbitrary.
 For names<-, the updated object. (Note that the value of names(x) <- value Is that of the assignment, value, Not the return value from the left-hand side.)

 Note
 For vectors, the names are one of the attributes with restrictions on the possible values. For pairlists, the names are the tags And converted To And From a character vector.
 For a one-dimensional array the names attribute really Is dimnames[[1]].
 Formally classed aka “S4” objects typically have slotNames() (And no names()).
 _
> 
>  names is a generic accessor function, and names<- is a generic replacement function. The default methods get and set the "names" attribute of a vector (including a list) or pairlist.
>  For an environment env, names(env) gives the names of the corresponding list, i.e., names(as.list(env, all.names = TRUE)) which are also given by ls(env, all.names = TRUE, sorted = FALSE). If the environment Is used as a hash table, names(env) are its “keys”.
>  If value Is shorter than x, it Is extended by character NAs To the length Of x.
>  It Is possible to update just part of the names attribute via the general rules: see the examples. This works because the expression there Is evaluated As z <- "names<-"(z, "[<-"(names(z), 3, "c2")).
>  The name "" Is special: it Is used to indicate that there Is no name associated with an element of a (atomic Or generic) vector. Subscripting by "" will match nothing (Not even elements which have no name).
>  A name can be character NA, but such a name will never be matched And Is likely To lead To confusion.
>  Both are primitive functions.
>  

#### rep
```csharp
RDotNET.Extensions.VisualBasic.RScripts.rep(System.String[])
```
rep replicates the values in x. It is a generic function, and the (internal) default method is described here.

|Parameter Name|Remarks|
|--------------|-------|
|x|a vector (of any mode including a list) or a factor or (for rep only) a POSIXct or POSIXlt or Date object; or an S4 object containing such an object.|


#### t
```csharp
RDotNET.Extensions.VisualBasic.RScripts.t(System.String)
```
Given a matrix or data.frame x, t returns the transpose of x.

|Parameter Name|Remarks|
|--------------|-------|
|x|a matrix or data frame, typically.|

_returns: A matrix, with dim and dimnames constructed appropriately from those of x, and other attributes except names copied across._
> 
>  This is a generic function for which methods can be written. The description here applies to the default and "data.frame" methods.
>  A data frame Is first coerced To a matrix: see as.matrix. When x Is a vector, it Is treated as a column, i.e., the result Is a 1-row matrix.
>  

#### UnixPath
```csharp
RDotNET.Extensions.VisualBasic.RScripts.UnixPath(System.String,System.Boolean)
```
Normalize the file path as the URL format in Unix system.

|Parameter Name|Remarks|
|--------------|-------|
|file|The file path string|
|extendsFull|是否转换为全路径？默认不转换|



### Properties

#### NA
"NA" 字符串，而不是NA空值常量
