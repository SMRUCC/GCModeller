---
title: BoyerMooreAlgorithmSearcher
---

# BoyerMooreAlgorithmSearcher
_namespace: [SMRUCC.genomics.Analysis.CRISPR.CRT.SearchingModel](N-SMRUCC.genomics.Analysis.CRISPR.CRT.SearchingModel.html)_

Boyer Moore algorithm copyright by Michael Lecuyer 1998. Slight modification below.



### Methods

#### __search
```csharp
SMRUCC.genomics.Analysis.CRISPR.CRT.SearchingModel.BoyerMooreAlgorithmSearcher.__search(System.Byte[],System.Int32,System.Int32)
```
Search for the compiled pattern in the given text.
 A side effect of the search is the notion of a partial
 match at the end of the searched buffer.
 This partial match is helpful in searching text files when
 the entire file doesn't fit into memory.

|Parameter Name|Remarks|
|--------------|-------|
|text| Buffer containing the text |
|start| Start position for search |
|length| Length of text in the buffer to be searched.
 |

_returns:  position in buffer where the pattern was found. _

#### BoyerMooreSearch
```csharp
SMRUCC.genomics.Analysis.CRISPR.CRT.SearchingModel.BoyerMooreAlgorithmSearcher.BoyerMooreSearch(System.String,System.String)
```
Using this function to search the pattern occurring in the target text data. 
 If the pattern is found in the **text** then the 
 index of the pattern occurring in the text will be returned, if not then 
 the value -1 will be return.

|Parameter Name|Remarks|
|--------------|-------|
|text|-|
|pattern|-|



### Properties

#### d
Internal BM table
#### MAXCHAR
Maximum chars in character set.
#### partial
Bytes of a partial match found at the end of a text buffer.
 The position at the end of the text buffer where a partial match was found.
 (-1 the number of bytes that formed a partial match, -1 if no partial match)
#### pat
Byte representation of pattern
#### patLen

