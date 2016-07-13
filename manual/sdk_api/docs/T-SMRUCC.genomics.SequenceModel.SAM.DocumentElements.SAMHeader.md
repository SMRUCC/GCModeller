---
title: SAMHeader
---

# SAMHeader
_namespace: [SMRUCC.genomics.SequenceModel.SAM.DocumentElements](N-SMRUCC.genomics.SequenceModel.SAM.DocumentElements.html)_

实际上就相当于一个字典来的



### Methods

#### #ctor
```csharp
SMRUCC.genomics.SequenceModel.SAM.DocumentElements.SAMHeader.#ctor(System.String)
```


|Parameter Name|Remarks|
|--------------|-------|
|str|
 Each header line begins with character `@' followed by a two-letter record type code. In the header,
 each line Is TAB-delimited And except the @CO lines, each data field follows a format `TAG:VALUE'
 where TAG Is a two-letter String that defines the content And the format Of VALUE. Each header line
 should match :  /^@[A-Za-z][A-Za-z](\t[A-Za-z][A-Za-z0-9]:[ -~]+)+$/ Or /^@CO\t.*/. Tags
 containing lowercase letters are reserved For End users.
 (每一行都是从@符号开始，后面跟随者两个字母的数据类型码，使用TAB进行分割除了@CO行，每一个域都以键值对的形式出现:  TAG:Value)
 |



