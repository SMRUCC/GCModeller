---
title: RegPreciseOperon
---

# RegPreciseOperon
_namespace: [SMRUCC.genomics.Data.Regprecise](N-SMRUCC.genomics.Data.Regprecise.html)_

Operon regulon model that reconstructed from the RegPrecise database.
 (使用RegPrecise数据库重构出来的Regulon数据)



### Methods

#### #ctor
```csharp
SMRUCC.genomics.Data.Regprecise.RegPreciseOperon.#ctor(SMRUCC.genomics.Data.Regprecise.Regulator,System.String[],System.String[],SMRUCC.genomics.ComponentModel.Loci.Strands,System.String[])
```
Copy regulon definition from @"T:SMRUCC.genomics.Data.Regprecise.Regulator"

|Parameter Name|Remarks|
|--------------|-------|
|regulon|-|
|TF|-|
|members|-|
|strand|-|
|bbhHits|-|



### Properties

#### bbhUID
Using for the CORN analysis or distinct
#### Effector
Active the regulation from @"P:SMRUCC.genomics.Data.Regprecise.RegPreciseOperon.TF_trace"
#### Operon
Operon members
#### Regulators
Mapping from @"P:SMRUCC.genomics.Data.Regprecise.RegPreciseOperon.TF_trace" by using protein ortholog analysis, such as ``bbh`` method.
#### TF_trace
Operon regulator TF
