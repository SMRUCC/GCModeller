#Region "Microsoft.VisualBasic::97ead1b23fdb5e0ea2a8b56b0a235557, G:/GCModeller/src/GCModeller/data/Xfam/Pfam//MPAlignment/MotifParallelAlignment.vb"

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

    '   Total Lines: 207
    '    Code Lines: 132
    ' Comment Lines: 53
    '   Blank Lines: 22
    '     File Size: 12.12 KB


    '     Module MotifParallelAlignment
    ' 
    '         Function: __idEquals, AlignmentOutput2Csv, (+2 Overloads) AlignProteins, (+2 Overloads) Convert, (+2 Overloads) SelectSource
    '                   SelectUniprot, (+2 Overloads) WriteOutput, WriteResult
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv.Extensions
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH.Abstract
Imports SMRUCC.genomics.SequenceModel

Namespace ProteinDomainArchitecture.MPAlignment

    ''' <summary>
    ''' 从双向比对结果之中根据PfamString结果来取等价的蛋白质
    ''' </summary>
    ''' <remarks></remarks>
    '''
    <Package("MPAlignment",
                        Description:="Alignment for the protein domains, this method is useful for the protein family classification annotation automatically.",
                        Category:=APICategories.ResearchTools,
                        Publisher:="xie.guigang@gcmodeller.org")>
    Public Module MotifParallelAlignment

        ''' <summary>
        ''' 使用这个函数进行比对操作
        ''' </summary>
        ''' <param name="besthit"></param>
        ''' <param name="query"></param>
        ''' <param name="subject"></param>
        ''' <param name="highlyScoringThreshold"></param>
        ''' <returns></returns>
        <ExportAPI("Domain.Alignment")>
        Public Function AlignProteins(besthit As IEnumerable(Of BiDirectionalBesthit),
                                      <Parameter("Pfam.Query")> query As IEnumerable(Of PfamString.PfamString),
                                      <Parameter("Pfam.Subject")> subject As IEnumerable(Of PfamString.PfamString),
                                      <Parameter("Threshold.HighlyScoring")>
                                      Optional highlyScoringThreshold As Double = 0.9) As List(Of LevAlign)

            Dim Queries = (From QueryId As String
                           In (From hitPair As BBH.BiDirectionalBesthit
                               In besthit
                               Select hitPair.QueryName
                               Distinct).ToArray.AsParallel
                           Let QueryPfamString = (From item In query Where String.Equals(item.ProteinId, QueryId) Select item).FirstOrDefault
                           Where Not QueryPfamString Is Nothing  ' query缺少pfam-string数据，无法比对
                           Let BestHits = (From hit In besthit Where String.Equals(QueryId, hit.QueryName) Select hit).ToArray  ' 得到和这个query匹配的所有的hits
                           Let HitsIdCollection = (From item In BestHits Select item.HitName).ToArray
                           Let HitsPfamString = (From item In subject Where Array.IndexOf(HitsIdCollection, item.ProteinId) > -1 Select item).ToArray ' 得到hits的pfam数据
                           Select QueryId,
                               BestHits,
                               QueryPfamString,
                               HitsPfamString).ToArray

            Dim DomainAlignments = (From item
                                    In Queries.AsParallel
                                    Select (From Hit As PfamString.PfamString
                                            In item.HitsPfamString
                                            Select Algorithm.PfamStringEquals(
                                                item.QueryPfamString,
                                                Hit,
                                                highlyScoringThreshold))).ToArray

            Return DomainAlignments.Unlist
        End Function

        <ExportAPI("Align.Output2Csv")>
        Public Function AlignmentOutput2Csv(output As Generic.IEnumerable(Of AlignmentOutput)) As MPCsvArchive()
            Dim LQuery = (From item In output.AsParallel Select MPCsvArchive.CreateObject(item)).ToArray
            Return LQuery
        End Function

        <ExportAPI("Write.Csv.Align.Output")>
        Public Function WriteOutput(data As Generic.IEnumerable(Of MPCsvArchive),
                                    <Parameter("Path.Csv")> Csv As String) As Boolean
            Return data.SaveTo(Csv, False)
        End Function

        <ExportAPI("Write.Csv.Align.Output")>
        Public Function WriteOutput(data As Generic.IEnumerable(Of AlignmentOutput),
                                    <Parameter("Path.Csv")> Csv As String) As Boolean
            Return AlignmentOutput2Csv(data).SaveTo(Csv, False)
        End Function

        Public Function AlignProteins(Of T As I_BlastQueryHit)(
                                         Besthit As IEnumerable(Of T),
                                         QueryPfam As IEnumerable(Of PfamString.PfamString),
                                         SubjectPfam As IEnumerable(Of PfamString.PfamString),
                                         highlyScoringThreshold As Double) As List(Of LevAlign)

            Dim Queries = (From QueryId As String
                           In (From item In Besthit Select item.QueryName Distinct).ToArray.AsParallel
                           Let BestHits = (From item In Besthit Where String.Equals(QueryId, item.QueryName) Select item).ToArray
                           Let HitsIdCollection = (From item In BestHits Select item.HitName).ToArray
                           Let QueryPfamString = (From item In QueryPfam Where String.Equals(item.ProteinId, QueryId) Select item).FirstOrDefault
                           Let HitsPfamString = (From item In SubjectPfam Where Array.IndexOf(HitsIdCollection, item.ProteinId) > -1 Select item).ToArray
                           Select QueryId, BestHits, QueryPfamString, HitsPfamString).ToArray
            Dim DomainAlignments = (From item In Queries.AsParallel
                                    Select (From Hit As PfamString.PfamString
                                            In item.HitsPfamString
                                            Select Algorithm.PfamStringEquals(item.QueryPfamString, Hit, highlyScoringThreshold)).ToArray).ToArray

            Return DomainAlignments.Unlist
        End Function

        ''' <summary>
        ''' 将比对结果转换为Csv文件，之后可以在Excel之中按照自己的需求进行数据筛选
        ''' </summary>
        ''' <param name="Besthits"></param>
        ''' <param name="DomainAlign"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        '''
        <ExportAPI("Export.DomainAlignment")>
        Public Function Convert(Besthits As IEnumerable(Of BBH.BiDirectionalBesthit), DomainAlign As IEnumerable(Of AlignmentOutput)) As BiDirectionalBesthit()
            Dim LQuery = (From item As BBH.BiDirectionalBesthit
                          In Besthits.AsParallel
                          Let Alignment As AlignmentOutput = (From dal In DomainAlign
                                                              Where String.Equals(item.QueryName, dal.ProteinQuery.ProteinId) AndAlso String.Equals(item.HitName, dal.ProteinSbjct.ProteinId)
                                                              Select dal).FirstOrDefault
                          Select BiDirectionalBesthit.Upgrade(item, Alignment)).ToArray
            Return LQuery
        End Function

        ''' <summary>
        ''' 将比对结果转换为Csv文件，之后可以在Excel之中按照自己的需求进行数据筛选
        ''' </summary>
        ''' <param name="Besthits"></param>
        ''' <param name="DomainAlign"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        '''
        Public Function Convert(Of TQueryHit As I_BlastQueryHit,
                                   TOutput As BiDirectionalBesthit.IMPAlignmentResult)(
                                   Besthits As IEnumerable(Of TQueryHit),
                                   DomainAlign As IEnumerable(Of AlignmentOutput),
                                   UpgradeMethod As Func(Of AlignmentOutput, TQueryHit, TOutput)) As TOutput()

            Dim LQuery = (From item As TQueryHit
                          In Besthits.AsParallel
                          Let Alignment As AlignmentOutput = (
                              From dal In DomainAlign
                              Where String.Equals(item.QueryName, dal.ProteinQuery.ProteinId) AndAlso String.Equals(item.HitName, dal.ProteinSbjct.ProteinId)
                              Select dal).FirstOrDefault
                          Select UpgradeMethod(Alignment, item)).ToArray
            Return LQuery
        End Function

        <ExportAPI("Write.Csv.MPAignment")>
        Public Function WriteResult(data As IEnumerable(Of BiDirectionalBesthit), saveCsv As String) As Boolean
            Return data.SaveTo(saveCsv, False)
        End Function

        ''' <summary>
        ''' 这个是为了Pfam-A分析而准备的，比对Pfam-A数据库会产生很大的数据，则在比对之前先使用本方法挑选出符合条件的Subject，以减少BLASTP的时间以及日志文件的大小
        ''' </summary>
        ''' <param name="Besthits"></param>
        ''' <param name="SubjectFasta"></param>
        ''' <param name="SelectMethod"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function SelectSource(Of TBesthit As BBH.BiDirectionalBesthit)(
                                        Besthits As IEnumerable(Of TBesthit),
                                        SubjectFasta As FASTA.FastaFile,
                                        SelectMethod As Func(Of BBH.BiDirectionalBesthit, FASTA.FastaSeq, Boolean)) As FASTA.FastaFile

            Dim LQuery = (From SequenceItem As FASTA.FastaSeq
                          In SubjectFasta.AsParallel
                          Where (From item In Besthits Where SelectMethod(item, SequenceItem) = True Select 100).FirstOrDefault > 50
                          Select SequenceItem).ToArray
            Return New FASTA.FastaFile(LQuery)
        End Function

        ''' <summary>
        ''' 这个是为了Pfam-A分析而准备的，比对Pfam-A数据库会产生很大的数据，则在比对之前先使用本方法挑选出符合条件的Subject，以减少BLASTP的时间以及日志文件的大小
        ''' </summary>
        ''' <param name="Besthits"></param>
        ''' <param name="SubjectFasta"></param>
        ''' <param name="SelectMethod"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function SelectSource(Of TBesthit As BBH.BestHit)(
                                        Besthits As IEnumerable(Of TBesthit),
                                        SubjectFasta As FASTA.FastaFile,
                                        SelectMethod As Func(Of BBH.BestHit, FASTA.FastaSeq, Boolean)) As FASTA.FastaFile

            Dim LQuery = (From SequenceItem As FASTA.FastaSeq
                          In SubjectFasta.AsParallel
                          Where (From item In Besthits Where SelectMethod(item, SequenceItem) = True Select 100).FirstOrDefault > 50
                          Select SequenceItem).ToArray
            Return New FASTA.FastaFile(LQuery)
        End Function

        ''' <summary>
        ''' 根据基因组和Uniprot的双向比对结果，来选择需要进行Pfam-A分析的Uniprot蛋白序列
        ''' </summary>
        ''' <param name="Besthits"></param>
        ''' <param name="subjects"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        '''
        <ExportAPI("Select.Uniprot")>
        Public Function SelectUniprot(Besthits As IEnumerable(Of BBH.BestHit), subjects As FASTA.FastaFile) As FASTA.FastaFile
            Return SelectSource(Besthits, subjects, AddressOf __idEquals)
        End Function

        Private Function __idEquals(besthit As BBH.BestHit, Fasta As FASTA.FastaSeq) As Boolean
            Return String.Equals(besthit.HitName, Fasta.Headers(1))
        End Function
    End Module
End Namespace
