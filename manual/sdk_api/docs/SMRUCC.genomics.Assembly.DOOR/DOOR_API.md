# DOOR_API
_namespace: [SMRUCC.genomics.Assembly.DOOR](./index.md)_

We present a database DOOR (Database for prOkaryotic OpeRons) containing computationally predicted operons of all the sequenced prokaryotic genomes. 
 All the operons In DOOR are predicted Using our own prediction program, which was ranked To be the best among 14 operon prediction programs by a recent independent review. 
 Currently, the DOOR database contains operons for 675 prokaryotic genomes, And supports a number of search capabilities to facilitate easy access And utilization of the information stored in it. 
 
 + (1) Querying the database: the database provides a search capability For a user To find desired operons And associated information through multiple querying methods. 
 + (2) Searching For similar operons: the database provides a search capability For a user To find operons that have similar composition And Structure To a query operon. 
 + (3) Prediction Of cis-regulatory motifs: the database provides a capability For motif identification In the promoter regions Of a user-specified group Of possibly coregulated operons, Using motif-finding tools. 
 + (4) Operons For RNA genes: the database includes operons For RNA genes. (5) OperonWiki: the database provides a wiki page (OperonWiki) To facilitate interactions between users And the developer Of the database. 
 
 We believe that DOOR provides a useful resource To many biologists working On bacteria And archaea, which can be accessed at http://csbl1.bmb.uga.edu/OperonDB.



### Methods

#### CreateOperonView
```csharp
SMRUCC.genomics.Assembly.DOOR.DOOR_API.CreateOperonView(SMRUCC.genomics.Assembly.DOOR.DOOR)
```
{OperonID, GeneId()}()

#### GetOprFirst
```csharp
SMRUCC.genomics.Assembly.DOOR.DOOR_API.GetOprFirst(System.String,SMRUCC.genomics.Assembly.DOOR.DOOR)
```
Gets the first gene in the operon of the struct gene that inputs from the parameter.

|Parameter Name|Remarks|
|--------------|-------|
|struct|操纵子里面的某一个结构基因成员的基因编号|


#### LoadDocument
```csharp
SMRUCC.genomics.Assembly.DOOR.DOOR_API.LoadDocument(System.String[],System.String)
```
从文档之中的数据行之中加载数据

|Parameter Name|Remarks|
|--------------|-------|
|s_Data|-|



