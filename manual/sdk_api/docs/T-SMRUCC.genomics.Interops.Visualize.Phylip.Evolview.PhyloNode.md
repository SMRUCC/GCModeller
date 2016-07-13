---
title: PhyloNode
---

# PhyloNode
_namespace: [SMRUCC.genomics.Interops.Visualize.Phylip.Evolview](N-SMRUCC.genomics.Interops.Visualize.Phylip.Evolview.html)_





### Methods

#### AddColorToBranch
```csharp
SMRUCC.genomics.Interops.Visualize.Phylip.Evolview.PhyloNode.AddColorToBranch(System.String,System.String)
```
* branch colors

|Parameter Name|Remarks|
|--------------|-------|
|colorsetID|-|
|color|-|


#### addColorToLeaf
```csharp
SMRUCC.genomics.Interops.Visualize.Phylip.Evolview.PhyloNode.addColorToLeaf(System.String,System.String)
```
* leaf colors

|Parameter Name|Remarks|
|--------------|-------|
|colorsetID|-|
|color|-|


#### Clone
```csharp
SMRUCC.genomics.Interops.Visualize.Phylip.Evolview.PhyloNode.Clone
```
make a new copy of current node


### Properties

#### __descendentsList
multiple (>= two) descendents are allowed
#### _Processed
May 20, 2011; check if current node is processed; used to fix a bug
#### Branchlength_To_Root
total branch length from current node to root - max_distance_to_tip (height) is the max branchlength to get to the tip;
#### Descendents
The descendents of current node.(��ǰ�ڵ�ĺ���ڵ�)
#### ID
id is actually a lable that will be printed
#### LevelHorizontal
ˮƽ������������λ��

 ' April 8, 2011; NOTE: use this instead of branchcolors, leafcolors and leafbackground colors ...

 * level_vertical is the relative vertical level from the top, start with 1;
 * for example, given a tree like the following:
 * |----- A 1
 * ---| C |--- E 2
 * |------| B 2.5 a sample tree
 * |----F 3
 *
 * A is level 1, E is level 2, F is 3, B is 2.5 (2+3)/2; C is (2.5 + 1) / 2 = 1.725
 *
 * level_horizontal is the relative horizontal level from the right, start with 1; therefore all leaves are at level 1
 * in this sample tree, AEF are at level 1, B is 2, C is 3
 *
 * here I use two integers to store the two levels
 *
 * >>>> how levels are assigned >>>>
 * vertical levels: in a tree like ((A,B), (C,D)), levels for ABCD are 1,2,3,4 respectively; internal levels are assigned using a function: ""
 * horizontal levels: all leaf are assigned with 1, internal levels are assigned using a function: ""
 *
#### LevelVertical
��ֱ������������λ��
#### LevelVerticalSlanted
jan 7, 2011; if current node is leaf, level_vertical_slanted = level_vertical
 so there is not necessary to assign level_vertical_slanted to leaf node;
 for slanted middle mode;
#### Max_Distance_To_Tip
total branch length from current node to root - max_distance_to_tip (height) is the max branchlength to get to the tip;
#### minLeafVerticalLevel
jan 7, 2011; if leaf node, min_leaf_vertical_level and max_leaf_vertical_level = level_vertical
 for slanted normal mode
