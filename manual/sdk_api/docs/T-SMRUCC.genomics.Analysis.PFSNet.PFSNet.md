---
title: PFSNet
---

# PFSNet
_namespace: [SMRUCC.genomics.Analysis.PFSNet](N-SMRUCC.genomics.Analysis.PFSNet.html)_

Implements the pfsnet algorithm to calculates the significant and consist cellular network between two types of mutations.
 (计算两种突变体的共有的表型变化的PfsNET算法)



### Methods

#### computegenelist
```csharp
SMRUCC.genomics.Analysis.PFSNet.PFSNet.computegenelist(SMRUCC.genomics.Analysis.PFSNet.DataStructure.DataFrameRow[],System.Double)
```
```R
 list.mask<-apply(w,1,function(x){
 sum(x,na.rm=T)/sum(!is.na(x)) >= beta
 })
 list(gl=rownames(w)[list.mask])
 ```
 
 apply函数是对行进行统计的
 
 函数返回行的编号的列表

|Parameter Name|Remarks|
|--------------|-------|
|w|-|
|beta|-|


#### computew1
```csharp
SMRUCC.genomics.Analysis.PFSNet.PFSNet.computew1(SMRUCC.genomics.Analysis.PFSNet.DataStructure.DataFrameRow[],System.Double,System.Double)
```
```R
 ranks<-apply(expr,2,function(x){
 rank(x)/length(x)
 })
 ```
 
 apply函数之中的MARGIN参数的含义：
 MARGIN
 a vector giving the subscripts which the function will be applied over. E.g., for a matrix 1 indicates rows, 2 indicates columns, c(1, 2) indicates rows and columns. 
 Where X has named dimnames, it can be a character vector selecting dimension names.
 即上述的R函数是对矩阵之中的每一列进行计算

|Parameter Name|Remarks|
|--------------|-------|
|expr|-|
|theta1|-|
|theta2|-|


#### Internal_getFuzzyWeight
```csharp
SMRUCC.genomics.Analysis.PFSNet.PFSNet.Internal_getFuzzyWeight(System.Double,System.Double,System.Double,System.Double)
```
计算模糊权重

|Parameter Name|Remarks|
|--------------|-------|
|y|-|
|q1|-|
|q2|-|
|delta_q12|-|


#### InternalVg
```csharp
SMRUCC.genomics.Analysis.PFSNet.PFSNet.InternalVg(SMRUCC.genomics.Analysis.PFSNet.DataStructure.GraphEdge[],SMRUCC.genomics.Analysis.PFSNet.DataStructure.DataFrameRow[],SMRUCC.genomics.Analysis.PFSNet.DataStructure.DataFrameRow[])
```
函数会忽略掉边数目少于5的网络

|Parameter Name|Remarks|
|--------------|-------|
|data|-|
|w1matrix1|-|
|w1matrix2|-|



### Properties

#### OriginalRAlgorithm
The original pfsnet algorithm implemented in language R.
 
 ```R
 require(igraph)
 #require(rJava)

 loaddata<-function(file){
 a<-read.table(file,row.names=1)
 a
 }

 computew1<-function(expr,theta1,theta2){
 ranks<-apply(expr,2,function(x){
 rank(x)/length(x)
 })
 apply(ranks,2,function(x){
q2<-quantile(x,theta2,na.rm=T)
q1<-quantile(x,theta1,na.rm=T)
m<-median(x)
mx<-max(x)
sapply(x,function(y){
if(is.na(y)){
NA
}
else if(y>=q1)
1
else if(y>=q2)
(y-q2)/(q1-q2)
else
0
})
 })
 }

 pfsnet.computegenelist<-function(w,beta){
# within [rest of string was truncated]";.
```
