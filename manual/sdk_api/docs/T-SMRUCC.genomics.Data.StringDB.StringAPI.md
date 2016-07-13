---
title: StringAPI
---

# StringAPI
_namespace: [SMRUCC.genomics.Data.StringDB](N-SMRUCC.genomics.Data.StringDB.html)_

STRING has an application programming interface (API) which enables you to get the data without using the 
 graphical user interface of the web page. The API is convenient if you need to programmatically access some 
 information but still do not want to download the entire dataset. There are several scenarios when it is 
 practical to use it. For example, you might need to access some interaction from your own scripts or want to 
 incorporate some information in STRING to a web page.

 We currently provide an implementation using HTTP, where the database information is accessed by HTTP requests. 
 Due to implementation and licensing reasons, The API provide methods to query individual items only, similar to 
 the web site. If you need access to bulk data, you can download the entire dataset by signing the academic 
 license agreement.
 
 http://[database]/[access]/[format]/[request]?[parameter]=[value]



### Methods

#### DownloadInteractions
```csharp
SMRUCC.genomics.Data.StringDB.StringAPI.DownloadInteractions(System.Collections.Generic.IEnumerable{System.String},System.String)
```


|Parameter Name|Remarks|
|--------------|-------|
|IdList|-|

_returns: 返回成功的个数_
> http://string-db.org/api/psi-mi/interactions?identifier=XC_1184


### Properties

#### StitchEMBL
The sister database of STRING
#### StringDb
Main entry point of STRING
#### StringEMBL
Alternative entry point of STRING
