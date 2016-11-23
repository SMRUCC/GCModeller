# CircosAPI
_namespace: [SMRUCC.genomics.Visualize.Circos](./index.md)_

Shoal shell interaction with circos perl program to draw a circle diagram of a bacteria genome.



### Methods

#### __createGenomeCircle
```csharp
SMRUCC.genomics.Visualize.Circos.CircosAPI.__createGenomeCircle(SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.PTTDbLoader,System.Collections.Generic.IEnumerable{SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.RpsBLAST.MyvaCOG},System.String)
```
Creates the circos gene circle from the PTT database which is defined 
 in the ``*.ptt/*.rnt`` file, and you can download this directory from 
 the NCBI FTP website.

|Parameter Name|Remarks|
|--------------|-------|
|PTT|-|
|COG|-|
|defaultColor|-|


#### __geneHighlights
```csharp
SMRUCC.genomics.Visualize.Circos.CircosAPI.__geneHighlights(System.Collections.Generic.IEnumerable{SMRUCC.genomics.Assembly.NCBI.GenBank.CsvExports.GeneDumpInfo},System.Collections.Generic.Dictionary{System.String,System.String},SMRUCC.genomics.ComponentModel.Loci.Strands)
```
生成基因组的基因片段

|Parameter Name|Remarks|
|--------------|-------|
|anno|-|
|colors|-|
|strands|-|


#### AddPlotTrack
```csharp
SMRUCC.genomics.Visualize.Circos.CircosAPI.AddPlotTrack(SMRUCC.genomics.Visualize.Circos.Configurations.Circos@,SMRUCC.genomics.Visualize.Circos.Configurations.Nodes.Plots.ITrackPlot)
```
Adds a new circos plots element into the circos.conf object.

|Parameter Name|Remarks|
|--------------|-------|
|circos|-|
|track|-|


#### CircosOption
```csharp
SMRUCC.genomics.Visualize.Circos.CircosAPI.CircosOption(System.Boolean)
```
@``F:SMRUCC.genomics.Visualize.Circos.CircosAPI.yes``, @``F:SMRUCC.genomics.Visualize.Circos.CircosAPI.no``

|Parameter Name|Remarks|
|--------------|-------|
|b|-|


#### CreateDataModel
```csharp
SMRUCC.genomics.Visualize.Circos.CircosAPI.CreateDataModel
```
Creats a new @``T:SMRUCC.genomics.Visualize.Circos.Configurations.Circos`` plots configuration document.

_returns: @``M:SMRUCC.genomics.Visualize.Circos.Configurations.Circos.CreateObject``_

#### CreateGCContent
```csharp
SMRUCC.genomics.Visualize.Circos.CircosAPI.CreateGCContent(SMRUCC.genomics.SequenceModel.FASTA.FastaToken,System.Int32,System.Int32)
```
Adds the GC% content on the circos plots.

|Parameter Name|Remarks|
|--------------|-------|
|nt|
 The original nt sequence in the fasta format for the calculation of the GC% content in each slidewindow
 |
|winSize%|-|
|steps%|-|


#### CreateGCSkewPlots
```csharp
SMRUCC.genomics.Visualize.Circos.CircosAPI.CreateGCSkewPlots(SMRUCC.genomics.SequenceModel.I_PolymerSequenceModel,System.Int32,System.Int32)
```
Creates the circos circle plots of the genome gcskew.

|Parameter Name|Remarks|
|--------------|-------|
|SequenceModel|-|
|SlideWindowSize|-|
|steps|-|


#### CreateGenomeCircle
```csharp
SMRUCC.genomics.Visualize.Circos.CircosAPI.CreateGenomeCircle(System.Collections.Generic.IEnumerable{SMRUCC.genomics.Assembly.NCBI.GenBank.CsvExports.GeneDumpInfo},SMRUCC.genomics.SequenceModel.FASTA.FastaToken,System.String)
```
Creates the circos outside gene circle from the export csv data of the genbank database file.

|Parameter Name|Remarks|
|--------------|-------|
|anno|-|
|genome|-|
|defaultColor|-|


#### GenerateBlastnAlignment
```csharp
SMRUCC.genomics.Visualize.Circos.CircosAPI.GenerateBlastnAlignment(SMRUCC.genomics.Visualize.Circos.Configurations.Circos,SMRUCC.genomics.Interops.NCBI.Extensions.NCBIBlastResult.AlignmentTable,System.Double,System.Double,SMRUCC.genomics.Visualize.Circos.TrackDatas.Highlights.IdentityColors)
```
The blast result alignment will be mapping on the circos plot circle individual as the 
 highlights element in the circos plot.

|Parameter Name|Remarks|
|--------------|-------|
|doc|-|
|table|
 The ncbi blast alignment result table object which can be achive from the NCBI website.
 |
|r1|The max radius of the alignment circles.|
|rInner|-|


#### GenerateGeneCircle
```csharp
SMRUCC.genomics.Visualize.Circos.CircosAPI.GenerateGeneCircle(SMRUCC.genomics.Visualize.Circos.Configurations.Circos,System.Collections.Generic.IEnumerable{SMRUCC.genomics.Assembly.NCBI.GenBank.CsvExports.GeneDumpInfo},System.Boolean,System.String,System.Boolean,System.Boolean,System.Boolean)
```


|Parameter Name|Remarks|
|--------------|-------|
|doc|-|
|anno|-|
|IDRegex|
 Regular expression for parsing the number value in the gene's locus_tag.
 (基因的名称的正则表达式解析字符串。如果为空字符串，则默认输出全部的名称)
 |
|onlyGeneName|当本参数为真的时候，**` IDRegex `**参数失效|


#### GetCircosScript
```csharp
SMRUCC.genomics.Visualize.Circos.CircosAPI.GetCircosScript
```
Gets the circos Perl script file location automatically by search on the file system.

#### GetGenomeCircle
```csharp
SMRUCC.genomics.Visualize.Circos.CircosAPI.GetGenomeCircle(System.String,System.String,System.String)
```
The directory which contains the completed PTT data: ``*.ptt, *.rnt, *.fna``
 and so on which you can download from the NCBI FTP website.

|Parameter Name|Remarks|
|--------------|-------|
|PTT|
 The directory which contains the completed PTT data: *.ptt, *.rnt, *.fna and so on which you can download from the NCBI FTP website.
 |
|myvaCog|
 The csv file path of the myva cog value which was export from the alignment between
 the bacteria genome And the myva cog database Using the NCBI blast package In the GCModeller.
 |
|defaultColor|
 The default color of the gene which is not assigned to any COG will be have.
 |


#### GetIdeogram
```csharp
SMRUCC.genomics.Visualize.Circos.CircosAPI.GetIdeogram(SMRUCC.genomics.Visualize.Circos.Configurations.Circos)
```
Gets the ideogram configuration node in the circos document object.
 (还没有ideogram文档的时候，则会返回一个新的文档)

|Parameter Name|Remarks|
|--------------|-------|
|doc|-|


#### PlotsSeperatorLine
```csharp
SMRUCC.genomics.Visualize.Circos.CircosAPI.PlotsSeperatorLine(System.Int32,System.Int32)
```
Creates a new seperator object in the circos plot with the specific width of the line, default is ZERO, not display.

|Parameter Name|Remarks|
|--------------|-------|
|Length|-|
|width|-|


#### RNAVisualize
```csharp
SMRUCC.genomics.Visualize.Circos.CircosAPI.RNAVisualize(SMRUCC.genomics.Visualize.Circos.Configurations.Circos,SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.PTT)
```
使用Highlighs来显示RNA分子在基因组之上的位置

#### SetBasicProperty
```csharp
SMRUCC.genomics.Visualize.Circos.CircosAPI.SetBasicProperty(SMRUCC.genomics.Visualize.Circos.Configurations.Circos,SMRUCC.genomics.SequenceModel.FASTA.FastaToken,System.Collections.Generic.IEnumerable{Microsoft.VisualBasic.ComponentModel.TripleKeyValuesPair},System.Int32)
```


|Parameter Name|Remarks|
|--------------|-------|
|circos|-|
|NT|-|
|bands|-|
|loopHole|默认为0，没有缺口|


#### SetIdeogramRadius
```csharp
SMRUCC.genomics.Visualize.Circos.CircosAPI.SetIdeogramRadius(SMRUCC.genomics.Visualize.Circos.Configurations.Circos,System.Double)
```
Invoke set the radius value of the ideogram circle.

|Parameter Name|Remarks|
|--------------|-------|
|circos|-|
|r|-|

> 
>  圆圈的最大值只能够到达1.2了？？？
>  相对的大小是和ideogram有关的
>  1/ideogram.radius
>  

#### SetIdeogramWidth
```csharp
SMRUCC.genomics.Visualize.Circos.CircosAPI.SetIdeogramWidth(SMRUCC.genomics.Visualize.Circos.Configurations.Ideogram,System.Int32)
```
Invoke set the ideogram width in the circos plot drawing, if the width value is set to ZERO,
 then the ideogram circle will be empty on the drawing but this is different with the ideogram
 configuration document was not included in the circos main configuration.

|Parameter Name|Remarks|
|--------------|-------|
|idg|-|
|width|-|


#### SetPlotElementPosition
```csharp
SMRUCC.genomics.Visualize.Circos.CircosAPI.SetPlotElementPosition(SMRUCC.genomics.Visualize.Circos.Configurations.Nodes.Plots.ITrackPlot,System.String,System.String)
```


|Parameter Name|Remarks|
|--------------|-------|
|track|-|
|rOutside|The radius value of the outside for this circle element.|
|rInner|The radius value of the inner circle of this element.|


#### setProperty
```csharp
SMRUCC.genomics.Visualize.Circos.CircosAPI.setProperty(SMRUCC.genomics.Visualize.Circos.Configurations.Circos,System.String,System.String)
```
Invoke set of the property value in the circos document object.

|Parameter Name|Remarks|
|--------------|-------|
|circos|-|
|name|The property name in the circos document object, case insensitive.|
|value|String value of the circos document object property.|


#### SetRadius
```csharp
SMRUCC.genomics.Visualize.Circos.CircosAPI.SetRadius(SMRUCC.genomics.Visualize.Circos.Configurations.Circos,System.Collections.Generic.IEnumerable{System.Double[]})
```


|Parameter Name|Remarks|
|--------------|-------|
|circos|-|
|r|从外圈到内圈的|


#### SetTrackFillColor
```csharp
SMRUCC.genomics.Visualize.Circos.CircosAPI.SetTrackFillColor(SMRUCC.genomics.Visualize.Circos.Configurations.Nodes.Plots.ITrackPlot,System.String)
```
Invoke set the color of the circle element on the circos plots.

|Parameter Name|Remarks|
|--------------|-------|
|track|-|
|Color|The name of the color in the circos program.|


#### SetTrackOrientation
```csharp
SMRUCC.genomics.Visualize.Circos.CircosAPI.SetTrackOrientation(SMRUCC.genomics.Visualize.Circos.Configurations.Nodes.Plots.ITrackPlot,System.String)
```


|Parameter Name|Remarks|
|--------------|-------|
|track|-|
|orientation|ori = ""in"" or ""out""|


#### WriteData
```csharp
SMRUCC.genomics.Visualize.Circos.CircosAPI.WriteData(SMRUCC.genomics.Visualize.Circos.Configurations.Circos,System.String,SMRUCC.genomics.Visualize.Circos.DebugGroups)
```
Save the circos plots configuration object as the default configuration file: circos.conf

|Parameter Name|Remarks|
|--------------|-------|
|circos|-|
|outDIR|-|
|debug|-|



### Properties

#### null
This property have no data value
