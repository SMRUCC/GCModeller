#Region "Microsoft.VisualBasic::253ebfe694456e9f053fd9795e39002f, DataMySql\CEG\Tools.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



    ' /********************************************************************************/

    ' Summaries:

    '     Module Tools
    ' 
    '         Function: DeltaHomogeneity, EssentialGeneClusterExport, ExportClusterNt, ExportEssentialGeneCluster, ImportsProteinSequence
    '                   InstallDatabase, InternalEssentialGeneCluster, InternalGetEssentialGene, InternalGetPttData, LoadAnnotation
    '         Class EssentialGeneCluster
    ' 
    '             Properties: ClusterID, Headers, Nt, Sp, Species
    '                         St, Title
    ' 
    '             Function: ToString
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.Repository
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.SequenceTools.DNA_Comparative
Imports SMRUCC.genomics.Analysis.SequenceTools.DNA_Comparative.DeltaSimilarity1998
Imports SMRUCC.genomics.Assembly.NCBI.GenBank
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports PathEntry = Microsoft.VisualBasic.ComponentModel.DataSourceModel.NamedValue(Of String)

Namespace CEG

    <Package("CEG")>
    Public Module Tools

        <ExportAPI("CEG.Install")>
        Public Function InstallDatabase(<Parameter("Dir.CEG")> CEG As String, <Parameter("CEG.Installed")> Installed As String) As Boolean
            Return CEGAssembly.InstallDatabase(CEG, Installed)
        End Function

        ''' <summary>
        ''' 结合CEG数据库从Ptt数据库之中导出每一个基因Cluster的核酸Nt序列
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' 
        <ExportAPI("Export.Nt.Cluster")>
        Public Function ExportClusterNt(<Parameter("Fasta.Nt")> Nt As FastaSeq,
                                        <Parameter("Annotation.Ptt")> Ptt As PTT,
                                        <Parameter("Assembly.CEG")> CEG As CEG.CEGAssembly) As FastaFile
            Dim GetClusterLQuery = (From Cluster As CEG.GeneCluster In CEG.GeneClusters.AsParallel
                                    Let ClusterGenes = (From GeneObject As CEG.ClusterGene
                                                        In Cluster.GeneClusters
                                                        Let FoundGene = ObjectQuery.MatchGene(Ptt, GeneObject.Annotation.GeneName, {GeneObject.Annotation.Description})
                                                        Where Not FoundGene Is Nothing
                                                        Select FoundGene).ToArray
                                    Where Not ClusterGenes.IsNullOrEmpty AndAlso ClusterGenes.Count = Cluster.GeneClusters.Count
                                    Select Cluster, ClusterGenes).First  '筛选出完整的基因簇
            Throw New NotImplementedException
        End Function

        <ExportAPI("Imports.ProtAA")>
        Public Function ImportsProteinSequence(ProtAA As String, Annotation As String) As FastaFile
            Dim Annotations = (From gene As CEG.Annotation
                               In CEG.Annotation.LoadDocument(Annotation)
                               Select gene
                               Group By gene.GId Into Group) _
                                    .ToDictionary(Function(gene) gene.GId,
                                                  Function(gene) gene.Group.First)
            Dim Proteins = (From strLine As String
                            In Strings.Split(FileIO.FileSystem.ReadAllText(ProtAA), vbCrLf)
                            Let CsvParsingRow = IO.RowObject.TryParse(strLine.Replace(vbLf, ""))
                            Let Gid As String = CsvParsingRow(0)
                            Where Annotations.ContainsKey(Gid)
                            Let AnnotationData = Annotations(Gid)
                            Select New FastaSeq With {
                                .Headers = New String() {AnnotationData.GeneName},
                                .SequenceData = CsvParsingRow(2)}).ToArray
            Dim Fasta As FastaFile = CType(Proteins, FastaFile)
            Return Fasta
        End Function

        Public Class EssentialGeneCluster : Implements IAbstractFastaToken

            ''' <summary>
            ''' 该基因簇的编号
            ''' </summary>
            ''' <value></value>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Public Property ClusterID As String
            ''' <summary>
            ''' 终止位置
            ''' </summary>
            ''' <value></value>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Public Property Sp As Integer
            ''' <summary>
            ''' 起始位置
            ''' </summary>
            ''' <value></value>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Public Property St As Integer
            ''' <summary>
            ''' 该基因簇的核酸序列
            ''' </summary>
            ''' <value></value>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Public Property Nt As String Implements IPolymerSequenceModel.SequenceData

            Public Property Species As String

            Public ReadOnly Property Title As String Implements SequenceModel.FASTA.IAbstractFastaToken.Title
                Get
                    Return ClusterID
                End Get
            End Property

            Public Property Headers As String() Implements IAbstractFastaToken.Headers

            Public Overrides Function ToString() As String
                Return ClusterID
            End Function
        End Class

        <ExportAPI("Export.Essential.Gene.Cluster", Info:="Batch export the nt sequence data of the CEG essentially gene cluster.")>
        Public Function EssentialGeneClusterExport(<Parameter("Dir.Source.Ptt", "The path of the ptt data directory which contains some bacteria genome source data. " &
                                                   "You can download this directory data form the ncbi FTP website: ftp://ftp.ncbi.nlm.nih.gov/genomes/Bacteria/")> PttSource As String,
                                                   <Parameter("CEG.Annotation", "The essential gene cluster ceg database functional annotation " &
                                                       "data: annotation.csv, which you can downloaded from the CEG database website.")> Annotation As Generic.IEnumerable(Of CEG.Annotation),
                                                   <Parameter("ClusterGaps.Allowed")> Optional AllowedGaps As Integer = 3) As EssentialGeneCluster()
            Dim BacteriaList = (From dir As String In FileIO.FileSystem.GetDirectories(PttSource) Select ID = FileIO.FileSystem.GetDirectoryInfo(dir).Name, Path = dir).ToArray
            Dim setValue = New SetValue(Of EssentialGeneCluster) <= NameOf(EssentialGeneCluster.Species)
            Dim LQuery = (From Bacteria In BacteriaList.AsParallel
                          Let EntryData = InternalGetPttData(Bacteria.Path)
                          Let ClusterData = (From Entry As PathEntry
                                             In EntryData
                                             Let Nt As FastaSeq = FastaSeq.Load(Entry.Value)
                                             Let Ptt = TabularFormat.PTT.Load(Entry.Name)
                                             Let Cluster = Function() As EssentialGeneCluster()
                                                               Try
                                                                   Return InternalEssentialGeneCluster(Nt, Ptt, Annotation, AllowedGaps)
                                                               Catch ex As Exception
                                                                   Return Nothing
                                                               End Try
                                                           End Function()
                                             Where Not Cluster.IsNullOrEmpty
                                             Select Cluster).ToArray.Unlist
                          Select Bacteria, ClusterNT = (From Region As EssentialGeneCluster
                                                        In ClusterData.AsParallel
                                                        Select setValue(Region, Bacteria.ID)).ToArray).ToArray
            Dim CEGCluster = (From Bacteria In LQuery Select Bacteria.ClusterNT).ToArray.ToVector
            Return CEGCluster
        End Function

        ''' <summary>
        ''' 利用CEG看家基因簇数据进行批量的核酸序列同质性的检测
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' 
        <ExportAPI("Measure.Homogeneity", Info:="Batch measuring the homogeneity property using a specific rule sequence which was generated from the CEG essential gene cluster data.")>
        Public Function DeltaHomogeneity(<Parameter("CEG.Cluster", "The essential gene cluster nt data which was batch calculated from the function  " &
                                         "Export.Essential.Gene.Cluster")> CEGCluster As IEnumerable(Of EssentialGeneCluster),
                                         <Parameter("Test.Genome.Partitioning", "The genome nt partitioning sequence data base on its functional annotation.")>
                                         TestGenomePartitions As IEnumerable(Of PartitioningData)) As IO.File
            Dim Bacteria = (From ClusterNt As EssentialGeneCluster
                            In CEGCluster
                            Select ClusterNt
                            Group ClusterNt By ClusterNt.Species Into Group).ToArray '按照基因组进行分组操作
            Dim LQuery = (From Genome
                          In Bacteria.AsParallel
                          Select (From i As Integer In TestGenomePartitions.Sequence
                                  Let f = New NucleotideModels.NucleicAcid(TestGenomePartitions(i))
                                  Select i, DeltaValue = (From ClusterRegion As EssentialGeneCluster
                                                          In Genome.Group
                                                          Let g = New NucleotideModels.NucleicAcid(ClusterRegion)
                                                          Select Idx = i,
                                                              Delta = 1000 * Sigma(f, g), ClusterRegion).ToArray).ToArray).ToVector
            Dim GroupData = (From Region In (From Data In LQuery Select Data.DeltaValue).IteratesALL Select Region Group Region By Region.ClusterRegion.Species Into Group).ToArray '按照标尺基因组进行分组
            Dim CsvObject As New IO.File
            Call CsvObject.AppendLine({"#Tag", "Cluster.ID"})
            Call CsvObject.First.AddRange((From Region In TestGenomePartitions Select Region.PartitioningTag).ToArray)
            Call CsvObject.AppendLine({"GC %", ""})
            Call CsvObject.Last.AddRange((From Region In TestGenomePartitions Select strValue = Region.GC.ToString).ToArray)
            Call CsvObject.AppendLine({"St", ""})
            Call CsvObject.Last.AddRange((From Region In TestGenomePartitions Select strValue = Region.LociLeft.ToString).ToArray)
            Call CsvObject.AppendLine({"Sp", ""})
            Call CsvObject.Last.AddRange((From Region In TestGenomePartitions Select strValue = Region.LociRight).ToArray)

            For Each Genome In GroupData  '每一行代表一个基因组
                Dim ClusterData = (From Region In Genome.Group Select Region Group By Region.ClusterRegion.ClusterID Into Group).ToArray

                For Each cregion In ClusterData
                    Dim bufs = (From Segment In cregion.Group Select Segment Order By Segment.Idx Ascending).ToArray
                    Dim CsvRow As New IO.RowObject
                    Call CsvRow.AddRange({Genome.Species, cregion.ClusterID})
                    Call CsvRow.AddRange((From Segment In bufs Select strValue = Segment.Delta).ToArray)
                    Call CsvObject.AppendLine(CsvRow)
                Next
            Next

            Return CsvObject
        End Function

        ''' <summary>
        ''' {Ptt, Fna}
        ''' </summary>
        ''' <param name="DIR">一个基因组的文件夹可能包含有染色体基因组和质粒基因组的数据</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function InternalGetPttData(DIR As String) As PathEntry()
            Dim Data = From path As PathEntry
                       In DIR.LoadEntryList("*.fna", "*.ptt")
                       Select path
                       Group path By path.Name Into Group
            Dim LQuery = (From g
                          In Data
                          Let InternalCreateData = Function() As PathEntry
                                                       Dim Ptt = (From Entry As PathEntry In g.Group.ToArray Where String.Equals(Entry.Value.Split(CChar(".")).Last, "ptt", StringComparison.OrdinalIgnoreCase) Select Entry).ToArray
                                                       Dim Fna = (From Entry As PathEntry In g.Group.ToArray Where String.Equals(Entry.Value.Split(CChar(".")).Last, "fna", StringComparison.OrdinalIgnoreCase) Select Entry).ToArray

                                                       If Ptt.IsNullOrEmpty OrElse Fna.IsNullOrEmpty Then
                                                           Return Nothing
                                                       Else
                                                           Return New PathEntry(Ptt.First.Value, Fna.First.Value)
                                                       End If

                                                   End Function()
                          Where Not String.IsNullOrEmpty(InternalCreateData.Name)
                          Select InternalCreateData).ToArray
            Return LQuery
        End Function

        Private Function InternalGetEssentialGene(Annotation As IEnumerable(Of CEG.Annotation), PTT As PTT) As GeneBrief()
            Dim setValue = New SetValue(Of GeneBrief) <= NameOf(GeneBrief.Gene)
            Dim GetEssentialGeneLQuery As GeneBrief() =
                LinqAPI.Exec(Of GeneBrief) <= From anno As CEG.Annotation
                                              In Annotation.AsParallel
                                              Let FoundGene = Function() As GeneBrief
                                                                  Call Console.Write(".")
                                                                  Return ObjectQuery.MatchGene(PTT, anno.GeneName, {anno.Description})
                                                              End Function()
                                              Where Not FoundGene Is Nothing
                                              Select GeneObject = setValue(FoundGene, anno.GeneName)
                                              Distinct
                                              Order By GeneObject.Synonym Ascending

            ' Call New SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.PTT(GetEssentialGeneLQuery, Ptt.Title, Ptt.OsNtLength).Save("./test.ptt")

            Return GetEssentialGeneLQuery
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="Nt"></param>
        ''' <param name="Ptt"></param>
        ''' <param name="Annotation"></param>
        ''' <param name="AllowedGaps">所允许的基因簇之中的最大的基因空缺数目</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExportAPI("Export.Essential.Gene.Cluster")>
        Public Function InternalEssentialGeneCluster(<Parameter("Fasta.Nt")> Nt As FastaSeq,
                                                     <Parameter("Annotation.Ptt")> Ptt As PTT,
                                                     <Parameter("Path.CEG.Annotation")> Annotation As IEnumerable(Of CEG.Annotation),
                                                     <Parameter("ClusterGaps.Allowed")> Optional AllowedGaps As Integer = 3) As EssentialGeneCluster()

            Dim Genes = (From GeneObject As GeneBrief
                         In Ptt.GeneObjects
                         Select GeneObject
                         Order By GeneObject.Synonym Ascending).ToArray

            '将所有的连续的基因取出来，这认为这些基因为一个cluster
            Dim GetEssentialGeneLQuery As SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels.GeneBrief()

#Const DEBUG = 0

#If DEBUG Then
            GetEssentialGeneLQuery = SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.PTT.Load("E:\CEG\test.ptt").GeneObjects
#Else
            GetEssentialGeneLQuery = InternalGetEssentialGene(Annotation, Ptt)
#End If
            Call Console.WriteLine("There are {0} essential genes was found in bacteria genome   {1}", GetEssentialGeneLQuery.Count, Ptt.Title)
            Call Console.WriteLine("Get essential gene operation job done!")

            Dim CurrentGeneCluster As New List(Of SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels.GeneBrief)
            Dim p_PTT As Integer = 0
            Dim GeneClusterList As New List(Of SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels.GeneBrief())

            Call Console.WriteLine("Start to export the essential gene cluster for the genome:   " & Ptt.Title)

            For Each EssGene As SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels.GeneBrief In GetEssentialGeneLQuery
                Dim CurrentPTTGene = Genes(p_PTT)

                Call Console.Write(".")

                If String.Equals(EssGene.Synonym, CurrentPTTGene.Synonym) Then
                    Call CurrentGeneCluster.Add(EssGene)
                Else
                    Dim PrePointer = p_PTT

                    Do While Not String.Equals(CurrentPTTGene.Synonym, EssGene.Synonym)
                        p_PTT += 1
                        CurrentPTTGene = Genes(p_PTT)
                    Loop

                    If p_PTT - PrePointer <= AllowedGaps Then
                    Else  '断裂了，这里是另外的一个新的基因簇
                        Call GeneClusterList.Add(CurrentGeneCluster.ToArray)
                        'Call Console.WriteLine(String.Join(",  ", (From GeneObject In CurrentGeneCluster Select GeneObject.Synonym)).ToArray)
                        Call CurrentGeneCluster.Clear()
                    End If

                    Call CurrentGeneCluster.Add(EssGene) '空缺在允许的范围之内
                End If

                p_PTT += 1
            Next

            Call Console.WriteLine("Start to parsing the cluster nt sequence data.")

            Dim ClusterRegion = (From Cluster In GeneClusterList.AsParallel
                                 Where Not Cluster.IsNullOrEmpty
                                 Let pList = (From Gene In Cluster Select New Integer() {Gene.Location.Left, Gene.Location.Right}).ToArray.Unlist
                                 Select ClusterID = String.Join("_", (From Gene In Cluster Select Gene.Gene).ToArray) & "|" & String.Join(",", (From Gene In Cluster Select Gene.Synonym).ToArray), pList.Max, pList.Min).ToArray
            Dim NtRegions = (From Cluster In ClusterRegion Select Cluster, NtData = Nt.CutSequenceBylength(Cluster.Min, Cluster.Max - Cluster.Min), Cluster.ClusterID).ToArray
            Dim ClusterData = LinqAPI.Exec(Of EssentialGeneCluster) <=
                From Cluster
                In NtRegions
                Select New EssentialGeneCluster With {
                    .ClusterID = Cluster.ClusterID,
                    .Nt = Cluster.NtData.SequenceData,
                    .Sp = Cluster.Cluster.Min,
                    .St = Cluster.Cluster.Max,
                    .Species = Ptt.Title
                }
            Call Console.WriteLine("Export the essential gene cluster operation job done!")

            Return ClusterData
        End Function

        <ExportAPI("Read.Csv.CEG_Annotation")>
        Public Function LoadAnnotation(Path As String) As CEG.Annotation()
            Dim ChunkBuffer As CEG.Annotation() = CEG.Annotation.LoadDocument(Path)
            ChunkBuffer = (From GeneObject As CEG.Annotation
                       In ChunkBuffer
                           Where Not String.Equals(GeneObject.GeneName, "-")
                           Select GeneObject).ToArray
            Return ChunkBuffer
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="Nt"></param>
        ''' <param name="Ptt"></param>
        ''' <param name="Annotation"></param>
        ''' <param name="AllowedGaps">所允许的基因簇之中的最大的基因空缺数目</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExportAPI("Export.Essential.Gene.Cluster")>
        Public Function ExportEssentialGeneCluster(<Parameter("Fasta.Nt")> Nt As SMRUCC.genomics.SequenceModel.FASTA.FastaSeq,
                                                   <Parameter("Annotation.Ptt")> Ptt As SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.PTT,
                                                   <Parameter("Path.CEG.Annotation")> Annotation As String,
                                                   <Parameter("ClusterGaps.Allowed")> Optional AllowedGaps As Integer = 3) As SMRUCC.genomics.SequenceModel.FASTA.FastaFile
            Dim AnnotationData = LoadAnnotation(Annotation)
            Dim Fasta = (From Cluster As EssentialGeneCluster
                         In InternalEssentialGeneCluster(Nt, Ptt, AnnotationData, AllowedGaps).AsParallel
                         Select New SMRUCC.genomics.SequenceModel.FASTA.FastaSeq With
                                {
                                    .SequenceData = Cluster.Nt,
                                    .Headers = New String() {Cluster.ClusterID}}).ToArray
            Return CType(Fasta, SMRUCC.genomics.SequenceModel.FASTA.FastaFile)
        End Function
    End Module
End Namespace
