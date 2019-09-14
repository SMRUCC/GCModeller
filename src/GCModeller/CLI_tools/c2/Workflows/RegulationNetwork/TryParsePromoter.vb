#Region "Microsoft.VisualBasic::78db05abe186f16dd4b377d7841c6ed7, CLI_tools\c2\Workflows\RegulationNetwork\TryParsePromoter.vb"

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

    ' Class CoExpressionOperon
    ' 
    '     Properties: DoorOperonView
    ' 
    '     Constructor: (+2 Overloads) Sub New
    '     Function: CreateObject, GetPossibleCoExpression
    ' 
    ' Class TryParsePromoter
    ' 
    '     Properties: DoorOperonView
    ' 
    '     Constructor: (+2 Overloads) Sub New
    '     Function: CreateOperonView, Proceeding, SelectGenes, (+2 Overloads) TryParse
    ' 
    ' /********************************************************************************/

#End Region

Public Class CoExpressionOperon

    ''' <summary>
    ''' 经过计算之后得到的PCC数据
    ''' </summary>
    ''' <remarks></remarks>
    Dim PCC As LANS.SystemsBiology.Toolkits.RNASeq.ExprSamples()
    ''' <summary>
    ''' 原始的RPKM值
    ''' </summary>
    ''' <remarks></remarks>
    Dim ExpressionProfile As LANS.SystemsBiology.Toolkits.RNASeq.ExprSamples()
    Public Property DoorOperonView As LANS.SystemsBiology.Assembly.Door.OperonView

    Sub New(ExpressionProfile As LANS.SystemsBiology.Toolkits.RNASeq.ExprSamples(), DoorOperonView As LANS.SystemsBiology.Assembly.Door.OperonView)
        Me.ExpressionProfile = ExpressionProfile
        Me.PCC = LANS.SystemsBiology.Toolkits.RNASeq.GenesCOExpr.CalculatePccMatrix(ExpressionProfile)
        Me.DoorOperonView = DoorOperonView
    End Sub

    Protected Friend Sub New()
    End Sub

    Public Shared Function CreateObject(ExpressionProfile As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File, FirstRowIsTitle As Boolean, DoorOperonView As LANS.SystemsBiology.Assembly.Door.OperonView) As CoExpressionOperon
        '   Return New CoExpressionOperon(c2.Pcc.Convert(ExpressionProfile, FirstRowIsTitle), DoorOperonView)
        Return New CoExpressionOperon() With {.DoorOperonView = DoorOperonView}
    End Function

    ''' <summary>
    ''' Get all possible coexpression operons.(获得与目标基因可能共表达的操纵子对象)
    ''' </summary>
    ''' <param name="LocusId">Target gene id.</param>
    ''' <returns>
    ''' 对于共表达的基因，先假设其表达量的变化趋势可能会一致，故而在理想情况下（即假设二者仅受一个相同的调控因子调控）芯片数据中的每一次实验，共表达的基因的RPKM值之间的差值会比较小：
    ''' 但是，由于每一个基因可能会被多种调控因子所调控，故而两个基因之间的RPKM值并不会期望其差值会比较小，但是应该要小于一定的阈值
    ''' </returns>
    ''' <remarks></remarks>
    Public Function GetPossibleCoExpression(LocusId As String, pccCutOff As Double, STDCutOff1 As Double, STDCutOff2 As Double) As LANS.SystemsBiology.Assembly.Door.Operon()
        Dim IdList = LANS.SystemsBiology.Toolkits.RNASeq.GenesCOExpr.SortMaxliklyhood(PCC, LocusId, pccCutOff)
        If IdList.IsNullOrEmpty Then
            Return New LANS.SystemsBiology.Assembly.Door.Operon() {}
        End If

        Dim Target = Microsoft.VisualBasic.IEnumerations.GetItem(LocusId, ExpressionProfile)
        Dim List2 As List(Of String) = New List(Of String)
        Dim countsCutOff As Double = Target.SampleValues.Count * 0.4 'Considering that one gene may be regulated by more than one regulator.  由于会受多种调控因子影响，故而可能在几次试验中差值会过大；当满足标准差小于阈值的实验次数大于设定的试验次数的时候，认为两个基因可能共表达

        For Each ItemObj In IdList
            Dim Ce = Microsoft.VisualBasic.IEnumerations.GetItem(ItemObj.Key, ExpressionProfile)
            Dim LQuery = (From i As Integer In Target.SampleValues.Sequence Let stdValue As Double = New Double() {Target.SampleValues(i), Ce.SampleValues(i)}.STD Where stdValue <= STDCutOff1 Select stdValue).ToArray

            If Not LQuery.IsNullOrEmpty Then
                Dim resultSTD As Double = LQuery.STD
                If LQuery.Count >= countsCutOff AndAlso resultSTD <= STDCutOff2 Then Call List2.Add(Ce.GeneId)
            End If
        Next
        Dim Operons = DoorOperonView.Select(List2.ToArray)
        Return Operons
    End Function
End Class

Public Class TryParsePromoter

    Dim MetaCyc As LANS.SystemsBiology.Assembly.MetaCyc.File.FileSystem.DatabaseLoadder
    Public Property DoorOperonView As LANS.SystemsBiology.Assembly.Door.OperonView

    Sub New(MetaCyc As LANS.SystemsBiology.Assembly.MetaCyc.File.FileSystem.DatabaseLoadder, ExpressionProfile As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File, DoorFile As String)
        Me.MetaCyc = MetaCyc
        Me.DoorOperonView = LANS.SystemsBiology.Assembly.Door.Load(DoorFile).DoorOperonView
    End Sub

    Sub New(MetaCyc As LANS.SystemsBiology.Assembly.MetaCyc.File.FileSystem.DatabaseLoadder, DoorOperonView As LANS.SystemsBiology.Assembly.Door.OperonView)
        Me.MetaCyc = MetaCyc
        Me.DoorOperonView = DoorOperonView
    End Sub

    Public Function Proceeding(Pathways As PwyFilters.Pathway(), ExportedDir As String, GenomeOperonView As LANS.SystemsBiology.Assembly.Door.OperonView, SegmentLength As Integer()) As String()
        Dim Genome = New LANS.SystemsBiology.SequenceModel.NucleotideModels.SegmentReader(CType(MetaCyc.Database.WholeGenome, LANS.SystemsBiology.SequenceModel.NucleotideModels.NucleicAcid), LinearMolecule:=False)
        Dim Genes = MetaCyc.Database.FASTAFiles.DNAseq
        Dim MetaCycGenes = MetaCyc.GetGenes

        Call FileIO.FileSystem.CreateDirectory(ExportedDir)

        Dim Export = Function(pathway As PwyFilters.Pathway) As String
                         Dim idlist = (From item In MetaCycGenes.Takes(pathway.AssociatedGenes) Select item.Accession1).ToArray
                         Dim AssociatedOperons = GenomeOperonView.Select(idlist)

                         For Each length In SegmentLength
                             Dim Data As LANS.SystemsBiology.SequenceModel.FASTA.FastaToken() = New LANS.SystemsBiology.SequenceModel.FASTA.FastaToken(AssociatedOperons.Count - 1) {}
                             Dim Path As String = String.Format("{0}_{1}bp/{2}.fsa", ExportedDir, length, pathway.Identifier)

                             For i As Integer = 0 To Data.Count - 1
                                 Dim Operon = AssociatedOperons(i)
                                 Dim FirstGene = Operon.FirstGene
                                 Dim PromoterFsa As LANS.SystemsBiology.SequenceModel.FASTA.FastaToken =
                                     New LANS.SystemsBiology.SequenceModel.FASTA.FastaToken With {
                                         .Attributes = New String() {String.Format("lcl_{0} [PathwayId={1}] [AssociatedOperon={2}] [OperonPromoter={3}; {4}] [OperonGenes={5}]",
                                                                                   i + 1, pathway.Identifier, Operon.Key, FirstGene.Synonym, FirstGene.Location.ToString, LANS.SystemsBiology.Assembly.Door.OperonView.GenerateLstIdString(Operon))}}
                                 Dim Location = FirstGene.Location
                                 If Location.Strand = LANS.SystemsBiology.ComponentModel.Loci.NucleotideLocation.Strands.Forward Then
                                     PromoterFsa.SequenceData = Genome.TryParse(Location.Left, length, directionDownstream:=False)
                                 Else
                                     PromoterFsa.SequenceData = Genome.ReadComplement(Location.Right, length, True)
                                 End If

                                 Data(i) = PromoterFsa
                             Next

                             Call CType(Data, LANS.SystemsBiology.SequenceModel.FASTA.FastaFile).Save(Path)
                         Next

                         Return ""
                     End Function
#If DEBUG Then
        Dim ExportFiles = (From pwy In Pathways Where (Not pwy.SuperPathway) AndAlso Not pwy.AssociatedGenes.IsNullOrEmpty Select Export(pwy)).ToArray
#Else
        Dim ExportFiles = (From pwy In Pathways.AsParallel Where (Not pwy.SuperPathway) AndAlso Not pwy.AssociatedGenes.IsNullOrEmpty Select Export(pwy)).ToArray
#End If

        Return ExportFiles
    End Function

    Private Function CreateOperonView(pathway As PwyFilters.Pathway, DoorOperon As LANS.SystemsBiology.Assembly.Door.Door, MetaCycGenes As LANS.SystemsBiology.Assembly.MetaCyc.File.DataFiles.Genes) _
        As KeyValuePair(Of String, LANS.SystemsBiology.Assembly.Door.GeneBrief())()

        Dim Operons As String() = (From Gene In pathway.AssociatedGenes Let GeneId As String = MetaCycGenes.Select(UniqueId:=Gene).Accession1 Select DoorOperon(GeneId).OperonID Distinct Order By OperonID Ascending).ToArray
        Dim LQuery = (From OperonId As String In Operons Select New KeyValuePair(Of String, LANS.SystemsBiology.Assembly.Door.GeneBrief())(OperonId, DoorOperon.Select(OperonId))).ToArray
        Return LQuery
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="IdList">基因编号列表</param>
    ''' <param name="SegmentLength">片段的长度</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Shared Function TryParse(IdList As String(), SequenceData As LANS.SystemsBiology.Assembly.NCBI.SequenceDump.Gene(),
                                              GenomeSequence As LANS.SystemsBiology.SequenceModel.NucleotideModels.SegmentReader,
                                              Optional SegmentLength As Integer = 150) _
        As LANS.SystemsBiology.SequenceModel.FASTA.FastaFile

        Dim Data As LANS.SystemsBiology.SequenceModel.FASTA.FastaToken() = New LANS.SystemsBiology.SequenceModel.FASTA.FastaToken(IdList.Count - 1) {}
        For i As Integer = 0 To Data.Count - 1
            Dim AccessionId As String = IdList(i)
            Dim FindResult = (From item In SequenceData Where String.Equals(item.LocusTag, AccessionId) Select item).ToArray
            If FindResult.IsNullOrEmpty Then
                Continue For
            End If

            Dim Gene = FindResult.First
            Dim PromoterFsa As LANS.SystemsBiology.SequenceModel.FASTA.FastaToken =
                New LANS.SystemsBiology.SequenceModel.FASTA.FastaToken With {.Attributes =
                    New String() {
                        "Promoter", Gene.LocusTag, Gene.CommonName, Gene.Location.ToString}}
            Dim Location = Gene.Location

            If Location.Strand = LANS.SystemsBiology.ComponentModel.Loci.NucleotideLocation.Strands.Forward Then
                PromoterFsa.SequenceData = GenomeSequence.TryParse(Location.Left, SegmentLength, directionDownstream:=False)
            Else
                PromoterFsa.SequenceData = GenomeSequence.ReadComplement(Location.Right, SegmentLength, True)
            End If

            Data(i) = PromoterFsa
        Next

        Return CType((From item In Data Where Not item Is Nothing Select item).ToArray, LANS.SystemsBiology.SequenceModel.FASTA.FastaFile)
    End Function

    ''' <summary>
    ''' 尝试从MetaCyc数据库之中解析出所有基因的启动子序列
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function TryParse(SegmentLength As Integer) As LANS.SystemsBiology.SequenceModel.FASTA.FastaFile
        Dim Genome = New LANS.SystemsBiology.SequenceModel.NucleotideModels.SegmentReader(CType(MetaCyc.Database.WholeGenome, LANS.SystemsBiology.SequenceModel.NucleotideModels.NucleicAcid))
        Dim Genes = MetaCyc.Database.FASTAFiles.DNAseq
        Dim Data As LANS.SystemsBiology.SequenceModel.FASTA.FastaToken() = New LANS.SystemsBiology.SequenceModel.FASTA.FastaToken(Genes.Count - 1) {}

        For i As Integer = 0 To Data.Count - 1
            Dim Gene = Genes(i)
            Dim PromoterFsa As LANS.SystemsBiology.SequenceModel.FASTA.FastaToken =
                New LANS.SystemsBiology.SequenceModel.FASTA.FastaToken With
                {
                    .Attributes = New String() {"Promoter", Gene.AccessionId, Gene.UniqueId, Gene.Location.ToString, Gene.UniqueId}}

            Dim Location = Gene.Location

            If Location.Strand = LANS.SystemsBiology.ComponentModel.Loci.NucleotideLocation.Strands.Forward Then
                PromoterFsa.SequenceData = Genome.TryParse(Location.Left, SegmentLength, directionDownstream:=False)
            Else
                PromoterFsa.SequenceData = Genome.ReadComplement(Location.Right, SegmentLength)
            End If

            Data(i) = PromoterFsa
        Next

        Return CType(Data, LANS.SystemsBiology.SequenceModel.FASTA.FastaFile)
    End Function

    Private Shared Function SelectGenes(DNAseq As LANS.SystemsBiology.Assembly.MetaCyc.File.FileSystem.FastaObjects.Gene(), IdList As String()) _
        As LANS.SystemsBiology.Assembly.MetaCyc.File.FileSystem.FastaObjects.Gene()
        Dim LQuery = (From Gene In DNAseq Where Array.IndexOf(IdList, Gene.UniqueId) > -1 Select Gene).ToArray
        Return LQuery
    End Function
End Class
