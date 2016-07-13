---
title: EvaluationException
---

# EvaluationException
_namespace: [RDotNET](N-RDotNET.html)_

Exception signaling that the R engine failed to evaluate a statement



### Methods

#### #ctor
```csharp
RDotNET.EvaluationException.#ctor(System.String,System.Exception)
```
Create an exception for a statement that failed to be evaluate by e.g. R_tryEval

|Parameter Name|Remarks|
|--------------|-------|
|errorMsg|The last error message of the failed evaluation in the R engine|
|innerException|The exception that was caught and triggered the creation of this evaluation exception|



