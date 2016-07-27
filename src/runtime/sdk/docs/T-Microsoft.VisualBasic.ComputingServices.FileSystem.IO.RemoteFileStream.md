---
title: RemoteFileStream
---

# RemoteFileStream
_namespace: [Microsoft.VisualBasic.ComputingServices.FileSystem.IO](N-Microsoft.VisualBasic.ComputingServices.FileSystem.IO.html)_

Provides a System.IO.Stream for a file, supporting both synchronous and asynchronous
 read and write operations.To browse the .NET Framework source code for this type,
 see the Reference Source.



### Methods

#### #ctor
```csharp
Microsoft.VisualBasic.ComputingServices.FileSystem.IO.RemoteFileStream.#ctor(System.String,System.IO.FileMode,System.IO.FileAccess,Microsoft.VisualBasic.ComputingServices.FileSystem.FileSystem)
```
Initializes a new instance of the System.IO.FileStream class with the specified
 path, creation mode, and read/write permission.

|Parameter Name|Remarks|
|--------------|-------|
|path|A relative or absolute path for the file that the current FileStream object will
 encapsulate.|
|mode|A constant that determines how to open or create the file.|
|access|A constant that determines how the file can be accessed by the FileStream object.
 This also determines the values returned by the System.IO.FileStream.CanRead
 and System.IO.FileStream.CanWrite properties of the FileStream object. System.IO.FileStream.CanSeek
 is true if path specifies a disk file.|
|remote|-|


#### BeginRead
```csharp
Microsoft.VisualBasic.ComputingServices.FileSystem.IO.RemoteFileStream.BeginRead(System.Byte[],System.Int32,System.Int32,System.AsyncCallback,System.Object)
```
Begins an asynchronous read operation. (Consider using System.IO.FileStream.ReadAsync(System.Byte[],System.Int32,System.Int32,System.Threading.CancellationToken)
 instead; see the Remarks section.)

|Parameter Name|Remarks|
|--------------|-------|
|array|The buffer to read data into.|
|offset|The byte offset in array at which to begin reading.|
|numBytes|The maximum number of bytes to read.|
|userCallback|The method to be called when the asynchronous read operation is completed.|
|stateObject|A user-provided object that distinguishes this particular asynchronous read request
 from other requests.|

_returns: An object that references the asynchronous read._

#### BeginWrite
```csharp
Microsoft.VisualBasic.ComputingServices.FileSystem.IO.RemoteFileStream.BeginWrite(System.Byte[],System.Int32,System.Int32,System.AsyncCallback,System.Object)
```
Begins an asynchronous write operation. (Consider using System.IO.FileStream.WriteAsync(System.Byte[],System.Int32,System.Int32,System.Threading.CancellationToken)
 instead; see the Remarks section.)

|Parameter Name|Remarks|
|--------------|-------|
|array|The buffer containing data to write to the current stream.|
|offset|The zero-based byte offset in array at which to begin copying bytes to the current
 stream.|
|numBytes|The maximum number of bytes to write.|
|userCallback|The method to be called when the asynchronous write operation is completed.|
|stateObject|A user-provided object that distinguishes this particular asynchronous write
 request from other requests.|

_returns: An object that references the asynchronous write._

#### Dispose
```csharp
Microsoft.VisualBasic.ComputingServices.FileSystem.IO.RemoteFileStream.Dispose(System.Boolean)
```
Releases the unmanaged resources used by the System.IO.FileStream and optionally
 releases the managed resources.

|Parameter Name|Remarks|
|--------------|-------|
|disposing|true to release both managed and unmanaged resources; false to release only unmanaged
 resources.|


#### EndRead
```csharp
Microsoft.VisualBasic.ComputingServices.FileSystem.IO.RemoteFileStream.EndRead(System.IAsyncResult)
```
Waits for the pending asynchronous read operation to complete. (Consider using
 System.IO.FileStream.ReadAsync(System.Byte[],System.Int32,System.Int32,System.Threading.CancellationToken)
 instead; see the Remarks section.)

|Parameter Name|Remarks|
|--------------|-------|
|asyncResult|The reference to the pending asynchronous request to wait for.|

_returns: The number of bytes read from the stream, between 0 and the number of bytes you
 requested. Streams only return 0 at the end of the stream, otherwise, they should
 block until at least 1 byte is available._

#### EndWrite
```csharp
Microsoft.VisualBasic.ComputingServices.FileSystem.IO.RemoteFileStream.EndWrite(System.IAsyncResult)
```
Ends an asynchronous write operation and blocks until the I/O operation is complete.
 (Consider using System.IO.FileStream.WriteAsync(System.Byte[],System.Int32,System.Int32,System.Threading.CancellationToken)
 instead; see the Remarks section.)

|Parameter Name|Remarks|
|--------------|-------|
|asyncResult|The pending asynchronous I/O request.|


#### Finalize
```csharp
Microsoft.VisualBasic.ComputingServices.FileSystem.IO.RemoteFileStream.Finalize
```
Ensures that resources are freed and other cleanup operations are performed when
 the garbage collector reclaims the FileStream.

#### Flush
```csharp
Microsoft.VisualBasic.ComputingServices.FileSystem.IO.RemoteFileStream.Flush(System.Boolean)
```
Clears buffers for this stream and causes any buffered data to be written to
 the file, and also clears all intermediate file buffers.

|Parameter Name|Remarks|
|--------------|-------|
|flushToDisk|true to flush all intermediate file buffers; otherwise, false.|


#### FlushAsync
```csharp
Microsoft.VisualBasic.ComputingServices.FileSystem.IO.RemoteFileStream.FlushAsync(System.Threading.CancellationToken)
```
Asynchronously clears all buffers for this stream, causes any buffered data to
 be written to the underlying device, and monitors cancellation requests.

|Parameter Name|Remarks|
|--------------|-------|
|cancellationToken|The token to monitor for cancellation requests.|

_returns: A task that represents the asynchronous flush operation._

#### Read
```csharp
Microsoft.VisualBasic.ComputingServices.FileSystem.IO.RemoteFileStream.Read(System.Byte[],System.Int32,System.Int32)
```
Reads a block of bytes from the stream and writes the data in a given buffer.

|Parameter Name|Remarks|
|--------------|-------|
|array|
 When this method returns, contains the specified byte array with the values between
 offset and (offset + count - 1) replaced by the bytes read from the current source.
 |
|offset|The byte offset in array at which the read bytes will be placed.|
|count|The maximum number of bytes to read.|

_returns: The total number of bytes read into the buffer. This might be less than the number
 of bytes requested if that number of bytes are not currently available, or zero
 if the end of the stream is reached._

#### ReadAsync
```csharp
Microsoft.VisualBasic.ComputingServices.FileSystem.IO.RemoteFileStream.ReadAsync(System.Byte[],System.Int32,System.Int32,System.Threading.CancellationToken)
```
Asynchronously reads a sequence of bytes from the current stream, advances the
 position within the stream by the number of bytes read, and monitors cancellation
 requests.

|Parameter Name|Remarks|
|--------------|-------|
|buffer|The buffer to write the data into.|
|offset|The byte offset in buffer at which to begin writing data from the stream.|
|count|The maximum number of bytes to read.|
|cancellationToken|The token to monitor for cancellation requests.|

_returns: A task that represents the asynchronous read operation. The value of the TResult
 parameter contains the total number of bytes read into the buffer. The result
 value can be less than the number of bytes requested if the number of bytes currently
 available is less than the requested number, or it can be 0 (zero) if the end
 of the stream has been reached._

#### ReadByte
```csharp
Microsoft.VisualBasic.ComputingServices.FileSystem.IO.RemoteFileStream.ReadByte
```
Reads a byte from the file and advances the read position one byte.
_returns: The byte, cast to an System.Int32, or -1 if the end of the stream has been reached._

#### Seek
```csharp
Microsoft.VisualBasic.ComputingServices.FileSystem.IO.RemoteFileStream.Seek(System.Int64,System.IO.SeekOrigin)
```
Sets the current position of this stream to the given value.

|Parameter Name|Remarks|
|--------------|-------|
|offset|The point relative to origin from which to begin seeking.|
|origin|Specifies the beginning, the end, or the current position as a reference point
 for offset, using a value of type System.IO.SeekOrigin.|

_returns: The new position in the stream._

#### SetLength
```csharp
Microsoft.VisualBasic.ComputingServices.FileSystem.IO.RemoteFileStream.SetLength(System.Int64)
```
Sets the length of this stream to the given value.

|Parameter Name|Remarks|
|--------------|-------|
|value|The new length of the stream.|


#### Write
```csharp
Microsoft.VisualBasic.ComputingServices.FileSystem.IO.RemoteFileStream.Write(System.Byte[],System.Int32,System.Int32)
```
Writes a block of bytes to the file stream.

|Parameter Name|Remarks|
|--------------|-------|
|array|The buffer containing data to write to the stream.|
|offset|The zero-based byte offset in array from which to begin copying bytes to the
 stream.|
|count|The maximum number of bytes to write.|


#### WriteAsync
```csharp
Microsoft.VisualBasic.ComputingServices.FileSystem.IO.RemoteFileStream.WriteAsync(System.Byte[],System.Int32,System.Int32,System.Threading.CancellationToken)
```
Asynchronously writes a sequence of bytes to the current stream, advances the
 current position within this stream by the number of bytes written, and monitors
 cancellation requests.

|Parameter Name|Remarks|
|--------------|-------|
|buffer|The buffer to write data from.|
|offset|The zero-based byte offset in buffer from which to begin copying bytes to the
 stream.|
|count|The maximum number of bytes to write.|
|cancellationToken|The token to monitor for cancellation requests.|

_returns: A task that represents the asynchronous write operation._

#### WriteByte
```csharp
Microsoft.VisualBasic.ComputingServices.FileSystem.IO.RemoteFileStream.WriteByte(System.Byte)
```
Writes a byte to the current position in the file stream.

|Parameter Name|Remarks|
|--------------|-------|
|value|A byte to write to the stream.|



### Properties

#### CanRead
Gets a value indicating whether the current stream supports reading.
#### CanSeek
Gets a value indicating whether the current stream supports seeking.
#### CanWrite
Gets a value indicating whether the current stream supports writing.
#### FileHandle
File handle on the remote machine file system
#### Handle
Gets the operating system file handle for the file that the current FileStream
 object encapsulates.
#### hashInfo
port@remote_IP://hash+<path>
#### IsAsync
Gets a value indicating whether the FileStream was opened asynchronously or synchronously.
#### Length
Gets the length in bytes of the stream.
#### Name
Gets the name of the FileStream that was passed to the constructor.
#### Position
Gets or sets the current position of this stream.
