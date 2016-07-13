---
title: ResourceMapper
---

# ResourceMapper
_namespace: [SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.GCTabular.FileStream.XmlFormat](N-SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.GCTabular.FileStream.XmlFormat.html)_






### Properties

#### CheBMethylesterase
[MCP][CH3] -> MCP + -CH3 Enzyme:[CheB][PI]

 Protein L-glutamate O(5)-methyl ester + H(2)O = protein L-glutamate + methanol
 C00132

 METOH
#### CheBPhosphate
CheB + [ChA][PI] -> [CheB][PI] + CheA
#### CheRMethyltransferase
MCP + -CH3 -> [MCP][CH3] Enzyme:CheR
 S-adenosyl-L-methionine
 S-ADENOSYLMETHIONINE
 C00019

 S-ADENOSYLMETHIONINE ADENOSYL-HOMO-CYS
 S-adenosyl-L-methionine + protein L-glutamate = S-adenosyl-L-homocysteine + protein L-glutamate methyl ester.
#### CrossTalk
[CheAHK][PI] + RR -> [RR][PI] + CheAHK
 [CheAHK][PI] + OCS -> CheAHK + [OCS][PI]
#### GenomeAnnotiation
@"T:SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.GCTabular.FileStream.GeneObject"
#### HkAutoPhosphorus
CheAHK + ATP -> [CheAHK][PI] + ADP Enzyme: [MCP][CH3][Inducer]
#### MetabolismModel
@"T:SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.GCTabular.FileStream.MetabolismFlux"
#### MetabolitesModel
@"T:SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.GCTabular.FileStream.Metabolite"
#### ObjectiveFunctionModel
@"T:SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.GCTabular.FileStream.ObjectiveFunction"
#### Transcript
@"T:SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.GCTabular.FileStream.Transcript"
#### TranscriptionModel
@"T:SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.GCTabular.FileStream.TranscriptUnit"
