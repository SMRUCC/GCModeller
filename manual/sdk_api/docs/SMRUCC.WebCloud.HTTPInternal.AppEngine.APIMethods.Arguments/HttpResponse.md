# HttpResponse
_namespace: [SMRUCC.WebCloud.HTTPInternal.AppEngine.APIMethods.Arguments](./index.md)_





### Methods

#### Close
```csharp
SMRUCC.WebCloud.HTTPInternal.AppEngine.APIMethods.Arguments.HttpResponse.Close
```
Closes the current StreamWriter object and the underlying stream.

#### Flush
```csharp
SMRUCC.WebCloud.HTTPInternal.AppEngine.APIMethods.Arguments.HttpResponse.Flush
```
Clears all buffers for the current writer and causes any buffered data to be
 written to the underlying stream.

#### FlushAsync
```csharp
SMRUCC.WebCloud.HTTPInternal.AppEngine.APIMethods.Arguments.HttpResponse.FlushAsync
```
Clears all buffers for this stream asynchronously and causes any buffered data
 to be written to the underlying device.

_returns: A task that represents the asynchronous flush operation._

#### Write
```csharp
SMRUCC.WebCloud.HTTPInternal.AppEngine.APIMethods.Arguments.HttpResponse.Write(System.Char[],System.Int32,System.Int32)
```
Writes a subarray of characters to the stream.

|Parameter Name|Remarks|
|--------------|-------|
|buffer|A character array that contains the data to write.|
|index|The character position in the buffer at which to start reading data.|
|count|The maximum number of characters to write.|


#### WriteAsync
```csharp
SMRUCC.WebCloud.HTTPInternal.AppEngine.APIMethods.Arguments.HttpResponse.WriteAsync(System.Char[],System.Int32,System.Int32)
```
Writes a subarray of characters to the stream asynchronously.

|Parameter Name|Remarks|
|--------------|-------|
|buffer|A character array that contains the data to write.|
|index|The character position in the buffer at which to begin reading data.|
|count|The maximum number of characters to write.|


_returns: A task that represents the asynchronous write operation._

#### WriteHTML
```csharp
SMRUCC.WebCloud.HTTPInternal.AppEngine.APIMethods.Arguments.HttpResponse.WriteHTML(System.Xml.Linq.XElement,System.Object[])
```
%s %d, etc

|Parameter Name|Remarks|
|--------------|-------|
|html|C language like printf function format usage.|
|args|-|


#### WriteLineAsync
```csharp
SMRUCC.WebCloud.HTTPInternal.AppEngine.APIMethods.Arguments.HttpResponse.WriteLineAsync(System.Char[],System.Int32,System.Int32)
```
Writes a subarray of characters followed by a line terminator asynchronously
 to the stream.

|Parameter Name|Remarks|
|--------------|-------|
|buffer|The character array to write data from.|
|index|The character position in the buffer at which to start reading data.|
|count|The maximum number of characters to write.|


_returns: A task that represents the asynchronous write operation._


