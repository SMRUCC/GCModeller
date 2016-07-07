---
title: SignalTransductionNetwork
---

# SignalTransductionNetwork
_namespace: [SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.ObjectModels.SubSystem](N-SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.ObjectModels.SubSystem.html)_






### Properties

#### CheBMethylesterase
[MCP][CH3] -> MCP + -CH3 Enzyme:[CheB][PI]
 
 Protein L-glutamate O(5)-methyl ester + H(2)O = protein L-glutamate + methanol
 C00132

 METOH
#### CheBPhosphate
CheB + [ChA][PI] -> [CheB][PI] + CheA
#### ChemotaxisSensing
[MCP][CH3] + Inducer <--> [MCP][CH3][Inducer]
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
#### HkAutoPhosphorus
CheAHK + ATP -> [CheAHK][PI] + ADP Enzyme: [MCP][CH3][Inducer]
#### TFActive
连接信号转导网络和调控模型的属性
