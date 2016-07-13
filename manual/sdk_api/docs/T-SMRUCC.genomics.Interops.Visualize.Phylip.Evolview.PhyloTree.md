---
title: PhyloTree
---

# PhyloTree
_namespace: [SMRUCC.genomics.Interops.Visualize.Phylip.Evolview](N-SMRUCC.genomics.Interops.Visualize.Phylip.Evolview.html)_





### Methods

#### #ctor
```csharp
SMRUCC.genomics.Interops.Visualize.Phylip.Evolview.PhyloTree.#ctor(System.String,System.String,System.String)
```


|Parameter Name|Remarks|
|--------------|-------|
|treename|ϵͳ������������|
|TreestrData|�������������ļ����ı�����|
|format|�ļ���ʽ��nhx, newick, nexus��Ĭ�ϸ�ʽΪphylip��Ĭ�������ʽnewick|


#### InternalReCalcLevels
```csharp
SMRUCC.genomics.Interops.Visualize.Phylip.Evolview.PhyloTree.InternalReCalcLevels
```
* >>>>>>>> >>>>>>>>> some important methods; I set them 'protected/private'
 * so that they are invisible to users


 * calculate horizontal and vertical levels for internal nodes, vertical
 * level = (min(levels of all descendents) + max(levels of all
 * descendents)) / 2; horizontal level = max(levels of all descendents)
 * + 1; therefore root has the max horizontal level
 *
 * jan 7, 2011; add level_vertical_slanted = (min(levels of all
 * descendents) + (max(levels of all descendents)) - min) / 2;



 * parameters are: node, array ref to hold horizontal levels of all its
 * descendents array ref to hold vertical levels of all its descendents
 * array ref to hold horizontal levels of its parent array ref to hold
 * vertical levels of its parent array ref to hold vertical levels of
 * all its leaf descendents of the parent node array ref to hold
 * vertical levels of all its leaf descendents of the current node

#### MakeNewInternalNode
```csharp
SMRUCC.genomics.Interops.Visualize.Phylip.Evolview.PhyloTree.MakeNewInternalNode(System.String,System.String,System.Boolean,SMRUCC.genomics.Interops.Visualize.Phylip.Evolview.PhyloNode)
```
Dec 5, 2011; can be used to make rootnode

|Parameter Name|Remarks|
|--------------|-------|
|id|-|
|internal_id|-|
|isroot|-|
|parentnode|-|


#### newickParser
```csharp
SMRUCC.genomics.Interops.Visualize.Phylip.Evolview.PhyloTree.newickParser(System.String,System.Collections.Generic.Dictionary{System.String,System.String})
```
April 4, 2013;
 bug fix; tree like this ((a:1,b):3,(c:1,(d:1,e:3):1):2); causes bootstrap value == true

 Sep 10, 2013 : bug fix, tree with bootstrap but no branch length : ((a,b)0.88,(c,d)0.99)0.99;

 Oct 19, 2013: nexus tree with bootstrap scores like this:

#### NexusParser
```csharp
SMRUCC.genomics.Interops.Visualize.Phylip.Evolview.PhyloTree.NexusParser(System.String)
```
* Dec 1-2, 2011; nexus format; note only the tree part will be processed;
 * other data will be ignored see :
 * http://molecularevolution.org/resources/treeformats for more details
 *
 * a typical nexsus tree looks like: #nexus ... begin trees; translate 1
 * Ephedra, 2 Gnetum, 3 Welwitschia, 4 Ginkgo, 5 Pinus ; tree one = [&U]
 * (1,2,(3,(4,5)); tree two = [&U] (1,3,(5,(2,4)); end;

|Parameter Name|Remarks|
|--------------|-------|
|treestr|-|


#### nhxParser
```csharp
SMRUCC.genomics.Interops.Visualize.Phylip.Evolview.PhyloTree.nhxParser(System.String)
```
'
 * Nov 28, 2011; nhx format see here for more details:
 * http://phylosoft.org/NHX/ please note that using nhx is now discoraged;
 * use phyloXML instead
 *
 * nhx format shares certain similarities with newick, so sode codes were
 * copied from the newick parser
 *
 * a typical nhx tree would look like:
 *
 * (((ADH2:0.1[&&NHX:S=human:E=1.1.1.1],
 * ADH1:0.11[&&NHX:S=human:E=1.1.1.1]):0.05[&&NHX:S=Primates:E=1.1.1.1:D=Y:B=100],
 * ADHY:0.1[&&NHX:S=nematode:E=1.1.1.1],ADHX:0.12[&&NHX:S=insect:E=1.1.1.1]):0.1[&&NHX:S=Metazoa:E=1.1.1.1:D=N],
 * (ADH4:0.09[&&NHX:S=yeast:E=1.1.1.1],ADH3:0.13[&&NHX:S=yeast:E=1.1.1.1],
 * ADH2:0.12[&&NHX:S=yeast:E=1.1.1.1],
 * ADH1:0.11[&&NHX:S=yeast:E=1.1.1.1]):0.1
 * [&&NHX:S=Fungi])[&&NHX:E=1.1.1.1:D=N];

|Parameter Name|Remarks|
|--------------|-------|
|treestr|-|


#### parseInforAndMakeNewLeafNode
```csharp
SMRUCC.genomics.Interops.Visualize.Phylip.Evolview.PhyloTree.parseInforAndMakeNewLeafNode(System.String,System.Collections.Generic.Dictionary{System.String,System.String},SMRUCC.genomics.Interops.Visualize.Phylip.Evolview.PhyloNode)
```
created on Oct 20, 2013
 input: the leafstr to be parsed, the internal node the leaf node has to be added to

#### reCalcDistanceToRoot
```csharp
SMRUCC.genomics.Interops.Visualize.Phylip.Evolview.PhyloTree.reCalcDistanceToRoot
```
* recalculate distance_to_root for each node; distance to root is the
 * total branch length from a given node to the root I use a nested
 * function to do the calculation
 *
 * NOTE: this program will continue if only there is valid branchlength

#### reCalcMaxDistanceToTip
```csharp
SMRUCC.genomics.Interops.Visualize.Phylip.Evolview.PhyloTree.reCalcMaxDistanceToTip
```
* recalculate height for each node; height is the max branchlength to
 * get to the tip; start with leaf nodes; calculate accumulative branch
 * length from it to internal nodes

#### reMakeEssentialVariables
```csharp
SMRUCC.genomics.Interops.Visualize.Phylip.Evolview.PhyloTree.reMakeEssentialVariables(SMRUCC.genomics.Interops.Visualize.Phylip.Evolview.PhyloNode,System.Int32)
```
Oct 25, 2013; this is a recursive function
 the four global variables will be changed in this function:
 allNodes, leafNodes, hashID2Nodes, hsInternalID2externalID

 also fix the parent and descendent relationships

|Parameter Name|Remarks|
|--------------|-------|
|node|-|



### Properties

#### AllNodes
*******************************************************
 all nodes *******************************************************
#### LeafNodes
*******************************************************
 leaf nodes and leaf node names.
 *******************************************************
#### treeFormat

