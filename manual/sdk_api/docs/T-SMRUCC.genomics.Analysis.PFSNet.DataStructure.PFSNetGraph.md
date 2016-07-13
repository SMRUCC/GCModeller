---
title: PFSNetGraph
---

# PFSNetGraph
_namespace: [SMRUCC.genomics.Analysis.PFSNet.DataStructure](N-SMRUCC.genomics.Analysis.PFSNet.DataStructure.html)_

A metabolism pathway network or its calculated sub network.(一个代谢途径或者子网络，或者说是所属出的计算结果之中的一个子网络对象)




### Properties

#### Edges
Gene to gene interaction, ggi.(基因与基因之间的连接，即ggi，基因对基因的互作)
#### Length
The gene counts in the current calculated PfsNET sub network.
 (当前的这个PfsNET子网络之中所计算出来的基因节点的数目)
#### Node
Gets a specific gene node from its name property.(通过基因名来获取本网路对象之中的一个基因节点，当该节点不存在的时候会返回空值)
#### Nodes
The nodes in the PfsNET sub network.(网络之中的基因节点)
