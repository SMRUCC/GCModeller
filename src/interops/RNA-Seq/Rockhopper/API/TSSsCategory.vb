#Region "Microsoft.VisualBasic::c2cdff0a52082964ff3f4d7e36146da7, RNA-Seq\Rockhopper\API\TSSsCategory.vb"

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

    ' Module TSSsCategory
    ' 
    '     Function: [GetType], Category, CreateModel, GetCType, PutativemRNA
    '     Enum GeneTypes
    ' 
    '         CDS, misc_RNA
    ' 
    ' 
    ' 
    '  
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Linq
Imports System.Drawing
Imports System.Collections
Imports System.ComponentModel
Imports System.Windows.Forms
Imports System.Data
Imports System.IO
Imports System.Threading
Imports LANS.SystemsBiology.SequenceModel.NucleotideModels
Imports LANS.SystemsBiology.Toolkits.RNA_Seq.Rockhopper.AnalysisAPI.Transcripts
Imports LANS.SystemsBiology.ComponentModel.Loci
Imports Microsoft.VisualBasic.DocumentFormat.Csv.Extensions
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.CommandLine.Reflection

''' <summary>
''' 进行TSS位点的分类处理
''' </summary>
''' 
<PackageNamespace("TSS.Category", Publisher:="xie.guigang@gcmodeller.org", Category:=APICategories.ResearchTools)>
Public Module TSSsCategory

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="Transcript"></param>
    ''' <param name="Reader">请使用大写的序列</param>
    ''' <returns></returns>
    ''' 
    <ExportAPI("Is.Putative_mRNA")>
    Public Function PutativemRNA(Transcript As Rockhopper.AnalysisAPI.Transcripts, Reader As LANS.SystemsBiology.SequenceModel.NucleotideModels.SegmentReader) As Boolean
        If Reader Is Nothing Then '无法判断
            Return False
        End If

        Dim MTULoci = Transcript.GetTULoci

        If Not MTULoci.IsValid Then
            Return False '位点的数据无效
        End If

        Dim Sequence As String = If(MTULoci.Strand = Strands.Forward,   '由于是假定的mrna，所以没有ATG和TGA了，这里只能使用TSS和TTS来推测
                                    Reader.GetSegmentSequence(Transcript.TSSs, Transcript.TTSs),   '正向序列
                                    Reader.ReadComplement(Transcript.GetTULoci.Left, Transcript.TranscriptLength))  '反向互补
        Dim ATG, TGA As Integer

        If Putative_mRNA(Nt:=Sequence, ATG:=ATG, TGA:=TGA, ORF:=Sequence) Then '计算出最长的目标片段中的ORF

            If Transcript.GetLociStrand = Strands.Forward Then

                Transcript.ATG = ATG + MTULoci.Left
                Transcript.TGA = ATG + MTULoci.Left + Len(Sequence)  '将相对位置转换为在整个基因组之中的绝对位置

            Else

                Transcript.ATG = MTULoci.Right - ATG
                Transcript.TGA = Transcript.ATG - Len(Sequence)

            End If

            Return True
        Else
            Return False
        End If
    End Function

    ''' <summary>
    ''' 对rockhopper得到的结果之中的TSS位点进行分类操作
    ''' </summary>
    ''' <param name="Transcript"></param>
    ''' <param name="PTT"></param>
    ''' <param name="Reader">请使用大写的序列</param>
    ''' <returns></returns>
    ''' 
    <ExportAPI("Category")>
    Public Function Category(Transcript As Rockhopper.AnalysisAPI.Transcripts,
                             PTT As LANS.SystemsBiology.Assembly.NCBI.GenBank.TabularFormat.PTT,
                             Reader As LANS.SystemsBiology.SequenceModel.NucleotideModels.SegmentReader,
                             ByRef RelatedGene As LANS.SystemsBiology.Assembly.NCBI.GenBank.TabularFormat.ComponentModels.GeneBrief) As Categories

        Dim PTT_GeneObjects As System.Collections.Generic.IEnumerable(Of LANS.SystemsBiology.Assembly.NCBI.GenBank.TabularFormat.ComponentModels.GeneBrief) =
            PTT.GeneObjects

        If Transcript.Leaderless Then 'TSS位点和ATG重叠在一起了
            RelatedGene = PTT.GeneObject(Transcript.Synonym)
            Return Categories.lmTSS
        End If

        If Transcript.TSSs > 0 AndAlso Transcript.ATG > 0 Then  '有ATG位点和TSS位点，则认为是mRNA
            Dim d = Math.Abs(Transcript.TSSs - Transcript.ATG)
            If d.RangesAt(0, 300) Then '两个位点不重叠，并且TSS位点在ATG上游的0到300bp的区间内
                RelatedGene = PTT.GeneObject(Transcript.Synonym)
                Return Categories.mTSS
            Else
                If d.RangesAt(300, 500) Then  '超长的TSS位点，在ATG上游的300到500bp这个区间内
                    RelatedGene = PTT.GeneObject(Transcript.Synonym)
                    Return Categories.ULmTSS
                End If
            End If
        End If

        If Transcript.IsRNA AndAlso Not Transcript.Is_sRNA AndAlso
            (TSSsCategory.PutativemRNA(Transcript, Reader) OrElse
            String.Equals(Transcript.Synonym, "putative mRNA")) Then '在进行从头装配的时候，程序认为这个对象是一个mRNA分子，但是在PTT之中找不到任何定义的，则认为是一个假定的mRNA分子

            Dim pmRNALoci = LANS.SystemsBiology.ComponentModel.Loci.NucleotideLocation.CreateObject(Transcript.ATG, Transcript.TGA)
            Dim ORF = (From Gene As LANS.SystemsBiology.Assembly.NCBI.GenBank.TabularFormat.ComponentModels.GeneBrief
                       In PTT_GeneObjects
                       Where Gene.Location.Equals(pmRNALoci)'查找大概能和这个假定的mRNA的位置重叠在一起的基因
                       Select Gene).ToArray

            '找不到，则说明可能是一个假定的mRNA分子
            If ORF.IsNullOrEmpty Then
                RelatedGene = New Assembly.NCBI.GenBank.TabularFormat.ComponentModels.GeneBrief With {
                    .Synonym = "Putative mRNA",
                    .Location = New ComponentModel.Loci.NucleotideLocation With {
                        .Left = Transcript.ATG,
                        .Right = Transcript.TGA,
                        .Strand = Transcript.GetLociStrand
                    }
                }
                Return Categories.pmTSS
            Else

                '找到了一个基因，则说明这个假定的mRNA分子可能就是这个基因
                RelatedGene = ORF.First
                Return Categories.pmTSS

            End If
        End If

        Dim TULoci = Transcript.GetTULoci

        If Transcript.IsRNA Then
            Dim TSSsStrand = Transcript.GetLociStrand()  '这里只是说转录的rna分子应该和CDS重叠在一块，完全重叠（在内部）和部分重叠
            Dim LQuery = (From GeneObject In PTT_GeneObjects                                                              ' Sense TSS (seTSS) represent internal transcripts 
                          Let LociRelationship = GeneObject.Location.GetRelationship(TULoci)
                          Where (LociRelationship =SegmentRelationships.Inside OrElse  '当前的这个Transcript和相关的蛋白质编码基因的关系是在内部或者部分重叠
                              LociRelationship =SegmentRelationships.DownStreamOverlap OrElse
                              LociRelationship =SegmentRelationships.DownStreamOverlap) AndAlso
                              GeneObject.Location.Strand = TSSsStrand   ' in the same orientation as, And located within, 
                          Select GeneObject).ToArray                                                                      ' protein-coding genes. 
            If Not LQuery.IsNullOrEmpty Then
                RelatedGene = LQuery.First
                Return Categories.seTSS
            End If
        End If

        If Transcript.IsRNA Then

            Dim GeneObjects = (From GeneObject In PTT_GeneObjects
                               Let rr = GeneObject.Location.GetRelationship(TULoci)
                               Where rr = SegmentRelationships.Inside AndAlso
                                   TULoci.Strand <> GeneObject.Location.Strand    '在基因的内部，并且转录的方向和ORF的方向相反
                               Select GeneObject).ToArray
            If Not GeneObjects.IsNullOrEmpty Then
                RelatedGene = GeneObjects.First
                Return Categories.asTSS
            End If
        End If

        'If Transcript.Is_sRNA Then
        '    '在基因间隔区，离ATG的距离大于300？？？
        '    Dim Genes = ContextModel.GetRelatedGenes(
        '        PTT.GeneObjects,
        '        LociStart:=Transcript.TSSs,
        '        LociEnds:=Transcript.TTSs)

        '    Genes = (From Gene In Genes
        '             Where Gene.Relation =SegmentRelationships.UpStream OrElse  '只需要在基因间隔区就行了，所以上下游都可以么？？？？
        '                 Gene.Relation =SegmentRelationships.DownStream
        '             Select Gene).ToArray

        '    If Not Genes.IsNullOrEmpty Then
        '        RelatedGene = Genes.First.Gene
        '        Return Categories.sTSS
        '    End If
        'End If

        RelatedGene = New Assembly.NCBI.GenBank.TabularFormat.ComponentModels.GeneBrief With {
            .Synonym = "null",  '这个TSS位点找不到任何分类和相关的基因
            .Location = New ComponentModel.Loci.NucleotideLocation With {
                .Left = -1,
                .Right = -1,
                .Strand = Strands.Unknown
            }
        }

        If Transcript.IsRNA Then
            Return Categories.seTSS
        Else
            Return Categories.UnClassified
        End If
    End Function

    'Public Sub LoadData(path As String)
    '    Dim ptt = LANS.SystemsBiology.Assembly.NCBI.GenBank.TabularFormat.ProteinTable.Load("E:\GCModeller\GCI Project\Interops\Rockhopper\bin\Debug\ProteinTable1004_170151.txt").ToPTT

    '    Dim data = path.LoadCsv(Of TranscriptView)(False)
    '    Dim LQuery = (From item In data.AsParallel Select model = CreateModel(item), cat = GetCType(item.Category)).ToArray

    '    For Each item In LQuery

    '        Call Console.WriteLine($"{  item.model.Synonym }   {item.cat.ToString }   ---->   {TSSsCategory.Category(item.model, ptt, Nothing, Nothing).ToString}")

    '    Next
    'End Sub

    <ExportAPI("CreateModel")>
    Public Function CreateModel(Transcript As TranscriptView) As Rockhopper.AnalysisAPI.Transcripts
        Dim Type = TSSsCategory.GetType(Transcript.Type)
        Return New Rockhopper.AnalysisAPI.Transcripts With {
           .ATG = If(Type = GeneTypes.misc_RNA, 0, Transcript.gpStart),
           .TGA = If(Type = GeneTypes.misc_RNA, 0, Transcript.gpStop),
           .Name = Transcript.GeneId,
           .Synonym = Transcript.TSS_id,
           .Strand = Transcript.Strand,
           .TSSs = Transcript.pStart,
           .TTSs = Transcript.pStop}
    End Function

    'Public Function GetANNVector(Transcript As TranscriptView) As Double()
    '    Dim ChunkBuffer = New Double() {
    '         If(String.IsNullOrEmpty(Transcript.ATGDistance), -1, Val(Transcript.ATGDistance)),
    '        Transcript.gpStart,
    '        Transcript.gpStop,
    '        LANS.SystemsBiology.ComponentModel.Loci.NucleotideLocation.GetStrand(Transcript.gStrand),
    '        Transcript.pStart,
    '        Transcript.pStop,
    '         LANS.SystemsBiology.ComponentModel.Loci.NucleotideLocation.GetStrand(Transcript.Strand),
    '         TSSsCategory.GetType(Transcript.Type),
    '         TSSsCategory.GetCType(Transcript.Category)
    '        }
    '    Return ChunkBuffer
    'End Function

    <ExportAPI("GetEnum")>
    Public Function GetCType(str As String) As Rockhopper.AnalysisAPI.Transcripts.Categories
        Select Case str
            Case "sTSS" : Return Categories.sTSS
            Case "seTSS" : Return Categories.seTSS
            Case "asTSS" : Return Categories.asTSS
            Case "mTSS" : Return Categories.mTSS
            Case "pmTSS" : Return Categories.pmTSS
            Case "lmTSS" : Return Categories.lmTSS
            Case Else
                Return Categories.UnClassified

        End Select
    End Function

    <ExportAPI("GetType")>
    Public Function [GetType](str As String) As GeneTypes
        If String.Equals(str, GeneTypes.misc_RNA.ToString, StringComparison.OrdinalIgnoreCase) Then
            Return GeneTypes.misc_RNA
        ElseIf String.Equals(str, GeneTypes.CDS.ToString, StringComparison.OrdinalIgnoreCase) Then
            Return GeneTypes.CDS
        Else
            Return GeneTypes.misc_RNA
        End If
    End Function

    Public Enum GeneTypes
        misc_RNA
        CDS
    End Enum
End Module


'''' <summary>
'''' Summary description for Form1.
'''' </summary>
'Public Module MainForm
'    Private data As Double(,) = Nothing

'    Private learningRate As Double = 0.1
'    Private momentum As Double = 0.0
'    Private sigmoidAlphaValue As Double = 2.0
'    Private neuronsInFirstLayer As Integer = 20
'    Private iterations As Integer = 1000

'    Private workerThread As Thread = Nothing
'    Private needToStop As Boolean = False

'    ' Load data
'    Sub Main(MAT As Double()())

'        data = MAT.ToMatrix

'        Call Start()
'    End Sub

'    ' On button "Start"
'    Sub Start()
'        ' get learning rate
'        Try
'            learningRate = Math.Max(0.00001, Math.Min(1, 0.1))
'        Catch
'            learningRate = 0.1
'        End Try
'        ' get momentum
'        Try
'            momentum = Math.Max(0, Math.Min(0.5, 0.1))
'        Catch
'            momentum = 0
'        End Try
'        ' get sigmoid's alpha value
'        Try
'            sigmoidAlphaValue = Math.Max(0.001, Math.Min(50, 2))
'        Catch
'            sigmoidAlphaValue = 2
'        End Try
'        ' get neurons count in first layer
'        Try
'            neuronsInFirstLayer = Math.Max(5, Math.Min(50, 50))
'        Catch
'            neuronsInFirstLayer = 20
'        End Try

'        iterations = 1000

'        Call SearchSolution()

'    End Sub



'    ' Worker thread
'    Private Sub SearchSolution()
'        ' number of learning samples
'        Dim samples As Integer = data.GetLength(0)
'        ' data transformation factor
'        '     Dim yFactor As Double = 1.7 / Chart.RangeY.Length
'        '  Dim yMin As Double = Chart.RangeY.Min
'        '  Dim xFactor As Double = 2.0 / Chart.RangeX.Length
'        '   Dim xMin As Double = Chart.RangeX.Min

'        ' prepare learning data
'        Dim input As Double()() = New Double(samples - 1)() {}
'        Dim output As Double()() = New Double(samples - 1)() {}
'        Dim ChunkBuffer = data.MatrixToVectorList


'        For i As Integer = 0 To samples - 1
'            input(i) = ChunkBuffer(i).Take(ChunkBuffer(i).Count - 1).ToArray
'            output(i) = New Double(0) {ChunkBuffer(i).Last}

'            ' set input
'            '   input(i)(0) = (data(i, 0) - xMin) * xFactor - 1.0
'            ' set output
'            '  output(i)(0) = (data(i, 1) - yMin) * yFactor - 0.85
'        Next

'        ' create multi-layer neural network
'        Dim network As New ActivationNetwork(New AForge.Neuro.BipolarSigmoidFunction(), input.First.Count, neuronsInFirstLayer, 30, 20, 1)
'        ' create teacher
'        Dim teacher As New BackPropagationLearning(network)
'        ' set learning rate and momentum
'        teacher.LearningRate = learningRate
'        teacher.Momentum = momentum

'        ' iterations
'        Dim iteration As Integer = 1

'        ' solution array
'        Dim solution As Double(,) = New Double(49, 1) {}
'        Dim networkInput As Double() = New Double(0) {}

'        '' calculate X values to be used with solution function
'        'For j As Integer = 0 To 49
'        '    solution(j, 0) = Chart.RangeX.Min + CDbl(j) * Chart.RangeX.Length / 49
'        'Next
'        Dim classType As Double
'        ' loop
'        While Not needToStop
'            ' run epoch of learning procedure
'            Dim [error] As Double = teacher.RunEpoch(input, output) / samples
'            '     Dim j As Integer = 0

'            ' calculate error
'            '  Dim learningError As Double = 0.0
'            '   Dim k As Integer = data.GetLength(0)

'            ' j = 0
'            ' While j < k
'            'networkInput(0) = input(j)(0)
'            ' learningError += Math.Abs(data(j, 1) - ((network.Compute(networkInput)(0) + 0.85) / yFactor + yMin))
'            'j += 1
'            ' End While

'            ' increase current iteration
'            iteration += 1

'            For i As Integer = 0 To output.Count - 1
'                classType = network.Compute(input(i)).First
'                Call Console.WriteLine($"{output(i).First}  <---->  {classType }  /// {   CInt(classType) = CInt(output(i).First)}   {NameOf([error])}={[error] }")
'            Next

'            ' check if we need to stop
'            If (iterations <> 0) AndAlso (iteration > iterations) Then
'                Exit While
'            Else
'                Call Console.Write(".")
'            End If
'        End While

'        Call Pause()
'    End Sub
'End Module
