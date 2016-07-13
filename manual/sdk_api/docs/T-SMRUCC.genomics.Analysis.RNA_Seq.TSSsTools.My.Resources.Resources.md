---
title: Resources
---

# Resources
_namespace: [SMRUCC.genomics.Analysis.RNA_Seq.TSSsTools.My.Resources](N-SMRUCC.genomics.Analysis.RNA_Seq.TSSsTools.My.Resources.html)_

A strongly-typed resource class, for looking up localized strings, etc.




### Properties

#### Culture
Overrides the current thread's CurrentUICulture property for all
 resource lookups using this strongly typed resource class.
#### ResourceManager
Returns the cached ResourceManager instance used by this class.
#### TSSs_Enrichment
Looks up a localized string similar to Imports Assembler
Imports Assembler.TSSs

 Dim reads <- {reads.csv} As String # The reads mapping csv data file.
 reads <- $reads -> load.reads.blastnmapping /Trim # Only perfect alignment and unique alignment data will be used to downstream analysis
 transcripts <- TSSs.Shared.Enrichment mappings $reads
call $transcripts -> Write.Csv.Transcripts SaveTo {saved.csv}.
