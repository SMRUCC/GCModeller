#Region "Microsoft.VisualBasic::78d0e340e4a0f48f14ae8e69ba5b9e05, data\Xfam\Pfam\Pipeline\DomainAnalysis.vb"

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


    ' Code Statistics:

    '   Total Lines: 355
    '    Code Lines: 269 (75.77%)
    ' Comment Lines: 47 (13.24%)
    '    - Xml Docs: 87.23%
    ' 
    '   Blank Lines: 39 (10.99%)
    '     File Size: 18.89 KB


    ' Module DomainAnalysis
    ' 
    '     Function: __createResult, __createResultPLinq, __createStructureRegion, __getFasta, CreatePfamString
    '               (+2 Overloads) EnzymeClassified, (+2 Overloads) FillChouFasmanData, LoadPfamDescribRes, PfamStringEquals, SavePfamString
    '               ToPfamString
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ApplicationServices.Terminal.Utility
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.Framework
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Parallel.Linq
Imports Microsoft.VisualBasic.Scripting
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Assembly.Expasy.AnnotationsTool
Imports SMRUCC.genomics.Assembly.Expasy.Database
Imports SMRUCC.genomics.Assembly.KEGG.DBGET
Imports SMRUCC.genomics.Data.Xfam.Pfam.Pipeline.LocalBlast
Imports SMRUCC.genomics.Data.Xfam.Pfam.ProteinDomainArchitecture
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput.BlastPlus
Imports SMRUCC.genomics.ProteinModel
Imports SMRUCC.genomics.ProteinModel.ChouFasmanRules
Imports SMRUCC.genomics.SequenceModel

<Package("Pfam.Domain.Analysis",
                    Publisher:="xie.guigang@gcmodeller.org",
                    Category:=APICategories.ResearchTools,
                    Description:="Tools for analysis the protein domains in your annotated bacterial genome.")>
Public Module DomainAnalysis

    ''' <summary>
    ''' 所有经过MPAlignment比对的符合阈值筛选条件的都会被看作为编号
    ''' </summary>
    ''' <param name="data"></param>
    ''' <param name="MPAlignment"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    '''
    <ExportAPI("Enzyme.Classification",
               Info:="This function export all of the annotation result which was match in the mpalignment filtering result.
               data parameter is the blast query raw data and the mpalignment parameter is the data filtering resulr from the mpalignment.")>
    Public Function EnzymeClassified(<Parameter("data",
                                                "data parameter is the blast query raw data and the mpalignment parameter is the data filtering resulr from the mpalignment.")>
                                     data As IEnumerable(Of T_EnzymeClass_BLAST_OUT),
                                     MPAlignment As IEnumerable(Of Pfam.ProteinDomainArchitecture.MPAlignment.MPCsvArchive),
                                     Expasy As NomenclatureDB,
                                     <Parameter("KEGG.Reactions")>
                                     Optional KEGG_Reactions As IEnumerable(Of bGetObject.Reaction) = Nothing) As EnzymeClass()
        Dim Annotations As EnzymeClass() = InvokeAnnotations(Expasy, data)
        If Not KEGG_Reactions Is Nothing Then
            Annotations = InvokeKEGGAnnotations(Annotations, KEGG_Reactions)
        End If

        Return Annotations
    End Function

    ''' <summary>
    ''' 这个方法只会筛选出可能的最佳的分类注释
    ''' </summary>
    ''' <param name="data"></param>
    ''' <param name="MPAlignment"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <ExportAPI("Enzyme.Classification", Info:="This function export the possible best annotation result of the enzyme class")>
    Public Function EnzymeClassified(data As IEnumerable(Of T_EnzymeClass_BLAST_OUT), MPAlignment As IEnumerable(Of MPAlignment.MPCsvArchive)) As T_EnzymeClass_BLAST_OUT()
        Dim Grouped = (From align As MPAlignment.MPCsvArchive
                       In MPAlignment
                       Select align
                       Group align By align.QueryName Into Group) _
                            .ToDictionary(keySelector:=Function(item) item.QueryName,
                                          elementSelector:=Function(item) (From align As ProteinDomainArchitecture.MPAlignment.MPCsvArchive
                                                                           In item.Group.ToArray
                                                                           Select align
                                                                           Order By align.Similarity Descending).ToArray)
        '使用MPAlignment的结果用于计算重复的酶分类
        Dim Handle As _____ENZYME_CLASS_HANDLER_ =
            Function(DuplicatedData As T_EnzymeClass_BLAST_OUT()) As T_EnzymeClass_BLAST_OUT
                Dim ProteinId As String = (From item As T_EnzymeClass_BLAST_OUT
                                           In DuplicatedData
                                           Let strId As String = item.ProteinId
                                           Select strId
                                           Distinct).FirstOrDefault     '取出MPAlignment的结果，然后按照相似性从高到低排序去第一个几个
                Dim MPAlignmentResult As ProteinDomainArchitecture.MPAlignment.MPCsvArchive =
                    Grouped(ProteinId).First  '在生成字典的时候已经排过序了，直接取第一个元素即可
                Dim LQuery = (From align As T_EnzymeClass_BLAST_OUT
                              In DuplicatedData
                              Where String.Equals(MPAlignmentResult.QueryName, align.ProteinId)
                              Select align).FirstOrDefault       '选重复的数据
                Return LQuery
            End Function

        Dim Result = EnzymeClassification(data, Handle)
        Return Result
    End Function

    <ExportAPI("Write.Csv.Pfam-String")>
    Public Function SavePfamString(data As IEnumerable(Of PfamString.PfamString), SaveTo As String) As Boolean
        Return data.SaveTo(SaveTo)
    End Function

    ''' <summary>
    '''
    ''' </summary>
    ''' <param name="blastOutput">不需要进行<see cref="TextGrepScriptEngine.Grep">格式化操作</see></param>
    ''' <param name="query">是进过grep操作之后的数据</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    '''
    <ExportAPI("PfamString.Creates",
               Info:="The blast_output is not recommend using grep operation if the data source is download from KEGG database;
               and the query fasta parameter is need for unique id grep operation. The default query operation time out threshold is 5min.")>
    Public Function CreatePfamString(<Parameter("Out.Blast+", "Blastp of the proteins and the pfam.fasta output data.")>
                                     blastOutput As v228,
                                     <Parameter("Query.Fasta", "The blastp query fasta source, this value is using for the chou-fasman data calculation,
                                     and if you don't want this structure data be calculated, then you can just leave this parameter empty.")>
                                     Optional query As FASTA.FastaFile = Nothing,
                                     <Parameter("opr.Timeout")>
                                     Optional timeOut As Integer = 5 * 60,
                                     Optional num_threads As Integer = 12,
                                     <Parameter("Ultralarge.Disable")>
                                     Optional disableUltralarge As Boolean = False,
                                     Optional evalue As Double = Evalue1En5,
                                     Optional coverage As Double = 0.85,
                                     Optional identities As Double = 0.3,
                                     Optional offset As Double = 0.1) As PfamString.PfamString()

        ' Show parameters for debug
        Call MethodBase.GetCurrentMethod().GetFullName.debug
        Call $"{NameOf(timeOut)}            => {timeOut}".debug
        Call $"{NameOf(num_threads)}        => {num_threads}".debug
        Call $"{NameOf(disableUltralarge)}  => {disableUltralarge}".debug
        Call $"{NameOf(evalue)}             => {evalue}".debug
        Call $"{NameOf(coverage)}           => {coverage}".debug
        Call $"{NameOf(identities)}         => {identities}".debug
        Call $"{NameOf(offset)}             => {offset}".debug

        Using busy As CBusyIndicator = New CBusyIndicator()
            Call "Start to create basic pfam-string information....".debug
            Call busy.Start()

            If blastOutput.Queries.Length > 5000 AndAlso Not disableUltralarge Then _
                Return __createResult(blastOutput, query, timeOut, num_threads)

            Return __createResultPLinq(blastOutput,
                                       query, evalue:=evalue,
                                       offset:=offset,
                                       identities:=identities,
                                       coverage:=coverage)
        End Using
    End Function

    Private Function __createResultPLinq(blast_output As BlastPlus.v228,
                                         query As FASTA.FastaFile,
                                         evalue As Double,
                                         coverage As Double,
                                         identities As Double,
                                         offset As Double) As PfamString.PfamString()
        Dim result As PfamString.PfamString()

#If DEBUG Then
        result = blast_output.Queries.Select(
            Function(queryHits)
                Return ToPfamString(queryHits,
                                             evalue:=evalue,
                                             coverage:=coverage,
                                             identities:=identities,
                                             offset:=offset)
            End Function).ToArray
#Else
        result = blast_output.Queries.Select(Function(queryHits) ToPfamString(queryHits)).ToArray
#End If
        If query.IsNullOrEmpty Then
            Return result
        Else
            Call "End of create basic pfam-string, start Chou-Fasman calculation threads....".debug
        End If

        Dim Cache = (From prot As PfamString.PfamString
                     In result
                     Let fasta As FASTA.FastaSeq = __getFasta(prot.ProteinId, query)
                     Select FastaObject = fasta,
                         pfam_string = prot).ToArray
        Dim LQuery = (From prot
                      In Cache.AsParallel
                      Select FillChouFasmanData(prot.pfam_string, prot.FastaObject)).ToArray

        Return LQuery
    End Function

    Private Function __createResult(blast_output As BlastPlus.v228, query As FASTA.FastaFile, time_out As Integer, num_threads As Integer) As PfamString.PfamString()
        Dim data = LQuerySchedule.LQuery(blast_output.Queries, AddressOf ToPfamString, 20000)

        If query.IsNullOrEmpty Then
            Return data
        Else
            Call "End of create basic pfam-string, start Chou-Fasman calculation threads....".debug
        End If

        Dim Cache = (From item In data Let fasta = __getFasta(item.ProteinId, query) Select fasta, pfam_string = item).ToArray

        Dim QueryHandle = Function(argv As Object) As PfamString.PfamString
                              Dim FastaObject As FASTA.FastaSeq = DirectCast(argv.fasta, FASTA.FastaSeq)
                              Dim PfamString As PfamString.PfamString = DirectCast(argv.pfam_string, PfamString.PfamString)

                              If FastaObject Is Nothing Then
                                  Dim msg As String = "[DEBUG] fasta sequence data not found for protein " & PfamString.ProteinId & vbCrLf
                                  msg &= "This may caused by the error grep script, try modify the grep script for the fasta title or make sure the protein is exists in the dataset and then run this program again!"
                                  Call Console.WriteLine(msg)
                                  Throw New Exception(msg)
                              End If

                              Return __createStructureRegion(PfamString, FastaObject)
                          End Function
        data = LQuerySchedule.LQuery(Cache, QueryHandle, 20000)
        Return data
    End Function

    Private Function __getFasta(id As String, Fasta As FASTA.FastaFile) As FASTA.FastaSeq
        Dim LQuery = (From item As FASTA.FastaSeq
                      In Fasta.AsParallel
                      Where String.Equals(id, item.Headers.First.Split.First, StringComparison.OrdinalIgnoreCase)
                      Select item).FirstOrDefault
        Return LQuery
    End Function

    <ExportAPI("PfamString.Fill.ChouFasman")>
    Public Function FillChouFasmanData(data As IEnumerable(Of PfamString.PfamString), Fasta As FASTA.FastaFile) As PfamString.PfamString()
        Dim Cache = (From pfString As PfamString.PfamString
                     In data
                     Let FastaObject As FASTA.FastaSeq = __getFasta(pfString.ProteinId, Fasta)
                     Select FastaObject, pfam_string = pfString).ToArray '经过修正之后，只有一个attribute值，即第一个attribute值为目标基因的编号
        Dim LQuery = (From cfRaw In Cache.AsParallel
                      Select FillChouFasmanData(cfRaw.pfam_string, cfRaw.FastaObject)).ToArray
        Return LQuery
    End Function

    ''' <summary>
    ''' 假若没有找到相应的fasta序列，则会返回原来的数据
    ''' </summary>
    ''' <param name="pfString"></param>
    ''' <param name="Fasta">经过了Grep操作的fasta序列数据</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function FillChouFasmanData(pfString As PfamString.PfamString, Fasta As FASTA.FastaSeq) As PfamString.PfamString
        If Fasta Is Nothing Then '没有找到
            Call $"{pfString.ProteinId} sequence fasta not found!".debug
            Return pfString
        End If

        Dim cfResult = __createStructureRegion(pfString, Fasta)
        Return cfResult
    End Function

    ''' <summary>
    ''' 将两个Pfam结构域之间的序列取出来，使用Chou-Fasman计算二级结构
    ''' </summary>
    ''' <param name="describ"></param>
    ''' <param name="sequence"></param>
    ''' <returns>[ABCT](start|ends)</returns>
    ''' <remarks></remarks>
    Private Function __createStructureRegion(describ As PfamString.PfamString, sequence As FASTA.FastaSeq) As PfamString.PfamString
        If describ.PfamString.IsNullOrEmpty OrElse describ.PfamString.Length = 1 Then
            Return describ
        End If

        Dim Domains = (From item In describ.GetDomainData(True) Select item Order By item.Position.left).ToArray
        Dim p As Integer = 0
        Dim ChunkBuffer As List(Of ProteinModel.DomainObject) = New List(Of ProteinModel.DomainObject)

        Do While p < Domains.Count - 1
            Dim currentDomain_p = Domains(p).Position.right
            Dim nextDomain_p = Domains(p + 1).Position.left

            If currentDomain_p >= nextDomain_p Then
                p += 1
                Continue Do
            End If

            Dim SequenceData As String = Mid(sequence.SequenceData, currentDomain_p, nextDomain_p - currentDomain_p)
            Dim ss = ChouFasman.Calculate(SequenceData)
            Dim doData As New DomainObject With {
                .Name = $"[{New String((From aa As AminoAcid
                                              In ss
                                        Select aa.StructureChar).ToArray)}]",
                .Position = New ComponentModel.Loci.Location(currentDomain_p, nextDomain_p)
            }

            Call ChunkBuffer.Add(doData)
            p += 1
        Loop

        Call ChunkBuffer.AddRange(Domains)
        ChunkBuffer = (From item In ChunkBuffer Select item Order By item.Position.left Ascending).AsList
        describ.PfamString = (From item In ChunkBuffer Select PfamString.ToPfamStringToken(item)).ToArray

        Return describ
    End Function

    <ExportAPI("PfamString.Equals")>
    Public Function PfamStringEquals(a As PfamString.PfamString, b As PfamString.PfamString,
                                     <Parameter("Threshold.HighlyScoring")>
                                     Optional highlyScoringThreshold As Double = 0.9) As MPAlignment.LevAlign
        Return MPAlignment.Algorithm.PfamStringEquals(a, b, highlyScoringThreshold)
    End Function

    ''' <summary>
    ''' 将blastp比对数据转换为Pfam-String数据.(这个函数导出来的是query为待注释的蛋白序列，数据库为Pfam序列数据库的比较结果)
    ''' </summary>
    ''' <param name="query"></param>
    ''' <param name="offset">0.11</param>
    ''' <param name="identities">暂时无用</param>
    ''' <returns></returns>
    ''' 
    <Extension>
    Public Function ToPfamString(query As BlastPlus.Query,
                                 Optional evalue As Double = Evalue1En5,
                                 Optional coverage As Double = 0.85,
                                 Optional identities As Double = 0.3,
                                 Optional offset As Double = 0.1) As PfamString.PfamString
        Dim locusId As String = query.QueryName.Split.First
        Dim Description As String

        If String.Equals(locusId, query.QueryName, StringComparison.OrdinalIgnoreCase) Then
            Description = ""
        Else
            Description = Mid(query.QueryName, Len(locusId) + 1).Trim
        End If

        If query.SubjectHits.IsNullOrEmpty Then
            Return New PfamString.PfamString With {
                .ProteinId = locusId,
                .Length = query.QueryLength,
                .Description = Description
            }
        End If

        Dim Domains As NamedCollection(Of DomainModel) = Annotation.ParserRaw(
            query,
            evalue:=evalue,
            coverage:=coverage,
            identities:=identities,
            offset:=offset
        )
        Dim Protein As New PfamString.PfamString With {
            .ProteinId = locusId,
            .Description = Description,
            .Length = query.QueryLength,
            .Domains = (From d As DomainModel In Domains Select $"{d.DomainId}:{d.DomainId}" Distinct).ToArray,
            .PfamString = Domains.Select(Function(x) $"{x.DomainId}({x.start}|{x.ends})").Distinct.ToArray
        }
        Return Protein
    End Function

    <ExportAPI("Pfam.Describ.LoadRes")>
    Public Function LoadPfamDescribRes(xml As String) As SiteSearch.PfamFamily
        Return xml.LoadXml(Of SiteSearch.PfamFamily)
    End Function
End Module
