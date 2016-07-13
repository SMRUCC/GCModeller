---
title: Control
---

# Control
_namespace: [RDotNET.Extensions.VisualBasic.base](N-RDotNET.Extensions.VisualBasic.base.html)_

These are the basic control-flow constructs of the R language. They function in much the same way as control statements in any Algol-like language. 
 They are all reserved words.



### Methods

#### if
```csharp
RDotNET.Extensions.VisualBasic.base.Control.if(System.String,System.Func{System.String},System.Func{System.String})
```
These are the basic control-flow constructs of the R language. They function in much the same way as control statements in any Algol-like language. They are all reserved words.

|Parameter Name|Remarks|
|--------------|-------|
|cond|
 A length-one logical vector that is not NA. Conditions of length greater than one are accepted with a warning, but only the first element is used. Other types are coerced to logical if possible, ignoring any class.
 |
|[true]|An expression in a formal sense. This is either a simple expression or a so called compound expression, usually of the form { expr1 ; expr2 }.|
|[else]|An expression in a formal sense. This is either a simple expression or a so called compound expression, usually of the form { expr1 ; expr2 }.|


#### ifelse
```csharp
RDotNET.Extensions.VisualBasic.base.Control.ifelse(System.String,System.String,System.String)
```
ifelse returns a value with the same shape as test which is filled with elements selected from either yes or no depending on whether the element of test is TRUE or FALSE.
 
 If yes or no are too short, their elements are recycled. yes will be evaluated if and only if any element of test is true, and analogously for no.
 Missing values In test give missing values In the result.

|Parameter Name|Remarks|
|--------------|-------|
|test|an object which can be coerced to logical mode.|
|yes|Return values For True elements Of test.|
|no|return values for false elements of test.|

_returns: 
 A vector of the same length and attributes (including dimensions and "class") as test and data values from the values of yes or no. 
 The mode of the answer will be coerced from logical to accommodate first any values taken from yes and then any values taken from no.
 _


