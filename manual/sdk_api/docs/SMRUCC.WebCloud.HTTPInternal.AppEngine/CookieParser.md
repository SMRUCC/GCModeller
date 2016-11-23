# CookieParser
_namespace: [SMRUCC.WebCloud.HTTPInternal.AppEngine](./index.md)_



> 
>  https://github.com/LYF610400210/CookieParser
>  


### Methods

#### FindIndex
```csharp
SMRUCC.WebCloud.HTTPInternal.AppEngine.CookieParser.FindIndex(System.String,System.String[])
```
从数组中查找指定的值，并返回其Index

|Parameter Name|Remarks|
|--------------|-------|
|Value|查找什么？|
|Source|在哪个数组中找？|


_returns: Index或-1_

#### isPathDomainOrDate
```csharp
SMRUCC.WebCloud.HTTPInternal.AppEngine.CookieParser.isPathDomainOrDate(System.String)
```
检测Name=Value是不是服务器Set-Cookie标头里的path、domain和过期日期
 有则返回空字符串，以去掉那些不能放在Cookie标头上的信息

|Parameter Name|Remarks|
|--------------|-------|
|input|-|


#### ParseOneNameAndValue
```csharp
SMRUCC.WebCloud.HTTPInternal.AppEngine.CookieParser.ParseOneNameAndValue(System.String)
```
判断一个Name=Value的值是不是一个真正的Cookies
 若是两个Cookies，则把两个Cookies用分号隔开；不是，则返回输入的NameAndValue值。

|Parameter Name|Remarks|
|--------------|-------|
|NameAndValue|Cookies Name=Value|


#### ParseSetCookie
```csharp
SMRUCC.WebCloud.HTTPInternal.AppEngine.CookieParser.ParseSetCookie(System.String)
```
把服务器返回的Set-Cookie标头信息翻译成
 能放在Cookie标头上的信息

|Parameter Name|Remarks|
|--------------|-------|
|CookieStr|Set-Cookie标头信息|



