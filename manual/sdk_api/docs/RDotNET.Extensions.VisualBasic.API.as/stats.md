# stats
_namespace: [RDotNET.Extensions.VisualBasic.API.as](./index.md)_





### Methods

#### ts
```csharp
RDotNET.Extensions.VisualBasic.API.as.stats.ts(System.String,System.String[])
```
as.ts and is.ts coerce an object to a time-series and test whether an object is a time series.

|Parameter Name|Remarks|
|--------------|-------|
|x|an arbitrary R object.|
|additionals|arguments passed to methods (unused for the default method).|


_returns: as.ts is generic. Its default method will use the tsp attribute of the object if it has one to set the start and end times and frequency._


