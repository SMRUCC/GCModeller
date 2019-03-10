# TableExtensions
_namespace: [RDotNET.Extensions.VisualBasic](./index.md)_





### Methods

#### dataframe``1
```csharp
RDotNET.Extensions.VisualBasic.TableExtensions.dataframe``1(System.Collections.Generic.IEnumerable{``0})
```
Push this object collection into the R memory as dataframe object.

|Parameter Name|Remarks|
|--------------|-------|
|source|-|


_returns: Returns the temp variable name that reference to the dataframe object in R memory._

#### PushAsDataFrame
```csharp
RDotNET.Extensions.VisualBasic.TableExtensions.PushAsDataFrame(Microsoft.VisualBasic.Data.csv.DocumentStream.File,System.String,System.Collections.Generic.Dictionary{System.String,System.Type},System.Boolean,System.Collections.Generic.IEnumerable{System.String})
```
A data frame is used for storing data tables. It is a list of vectors of equal length. 
 For example, the following variable df is a data frame containing three vectors n, s, b.

 ```R
 n = c(2, 3, 5) 
 s = c("aa", "bb", "cc") 
 b = c(TRUE, FALSE, TRUE) 
 df = data.frame(n, s, b) # df Is a data frame
 
 # df
 # n s b
 # 1 2 aa TRUE
 # 2 3 bb FALSE
 # 3 5 cc TRUE
 ```

|Parameter Name|Remarks|
|--------------|-------|
|df|-|
|var|-|


#### PushAsDataFrame``1
```csharp
RDotNET.Extensions.VisualBasic.TableExtensions.PushAsDataFrame``1(System.Collections.Generic.IEnumerable{``0},System.String)
```
Push this object collection into the R memory as dataframe object.

|Parameter Name|Remarks|
|--------------|-------|
|source|-|
|var|-|


#### PushAsTable
```csharp
RDotNET.Extensions.VisualBasic.TableExtensions.PushAsTable(Microsoft.VisualBasic.Data.csv.DocumentStream.File,System.String,System.Boolean)
```
Push the table data in the VisualBasic into R system.

|Parameter Name|Remarks|
|--------------|-------|
|table|-|
|tableName|-|
|skipFirst|
 If the first column is the rows name, and you don't want these names, then you should set this as TRUE to skips this data.
 |



