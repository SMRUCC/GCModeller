#Region "Microsoft.VisualBasic::7927af07dd036bef83649e39d041c835, GCModeller\data\RegulonDatabase\Regprecise\RegpreciseBBHAPI.vb"

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

    '   Total Lines: 293
    '    Code Lines: 122
    ' Comment Lines: 143
    '   Blank Lines: 28
    '     File Size: 18.01 KB


    '     Module RegpreciseBidirectionalBh_Methods
    ' 
    '         Function: __applyProperty, __gettfbs, Convert, FamilyStatics, (+2 Overloads) Match
    '                   MPCutoff, ReadData, WriteData
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.Extensions
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Data.Xfam.Pfam
Imports SMRUCC.genomics.Data.Xfam.Pfam.PfamString
Imports SMRUCC.genomics.Data.Xfam.Pfam.ProteinDomainArchitecture
Imports SMRUCC.genomics.Data.Xfam.Pfam.ProteinDomainArchitecture.MPAlignment
Imports SMRUCC.genomics.Interops.NCBI.Extensions
Imports SMRUCC.genomics.Interops.NCBI.Extensions.Pipeline.COG
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.FASTA

Namespace Regprecise

    <Package("Regprecise.Bidirectional_Bh", Publisher:="xie.guigang@gcmodeller.org")>
    Public Module RegpreciseBidirectionalBh_Methods

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="data"></param>
        ''' <param name="cutoff">Cutoff value for property <see cref="RegpreciseMPBBH.Similarity"></see>, a recommended value is 0.85</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        '''
        <ExportAPI("Regprecise.MP_Cutoff", Info:="cutoff is recommended using value 0.85")>
        Public Function MPCutoff(data As IEnumerable(Of RegpreciseMPBBH), cutoff As Double) As RegpreciseMPBBH()
            Dim LQuery = (From item In data.AsParallel Where item.Similarity > cutoff Select item).ToArray
            Return LQuery
        End Function

        '<ExportAPI("Regprecise.MP_Alignment")>
        'Public Function MPAlignment(Besthit As IEnumerable(Of RegpreciseMPBBH),
        '                            QueryPfam As IEnumerable(Of PfamString),
        '                            SubjectPfam As IEnumerable(Of PfamString),
        '                            Optional highlyScoringThreshold As Double = 0.9) As List(Of LevAlign)

        '    Dim setValue = New SetValue(Of PfamString)().GetSet(NameOf(PfamString.ProteinId))
        '    SubjectPfam =
        '        LinqAPI.Exec(Of PfamString) <= From sbjPfam As PfamString
        '                                       In SubjectPfam
        '                                       Let pId As String = Regex.Replace(sbjPfam.ProteinId, "lcl\d+\|", "")
        '                                       Select setValue(sbjPfam, pId)
        '    Return MotifParallelAlignment.AlignProteins(Of RegpreciseMPBBH)(
        '        Besthit,
        '        QueryPfam,
        '        SubjectPfam,
        '        highlyScoringThreshold)
        'End Function

        '<ExportAPI("regprecise.export_mp_alignment")>
        'Public Function MatchAlignment(Besthits As IEnumerable(Of RegpreciseMPBBH), Alignment_Output As IEnumerable(Of MPAlignment.AlignmentOutput)) As RegpreciseMPBBH()
        '    Dim UpgradeMethod = Function(AlignmentResult As AlignmentOutput, Besthit As RegpreciseMPBBH)
        '                            Besthit.Similarity = AlignmentResult.Similarity
        '                            Besthit.MPScore = AlignmentResult.Score
        '                            Besthit.SubjectPfamString = AlignmentResult.ProteinSbjct.get__PfamString

        '                            Return Besthit
        '                        End Function

        '    Return MotifParallelAlignment.Convert(Of RegpreciseMPBBH, RegpreciseMPBBH)(Besthits, Alignment_Output, UpgradeMethod)
        'End Function

        ''' <summary>
        ''' 统计出所比对的蛋白质的家族分布,由于一个蛋白质可能会比对上对各家族，故而总和不会等于最佳比对的结果数目
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        '''
        <ExportAPI("Family.distributions")>
        Public Function FamilyStatics(data As IEnumerable(Of RegpreciseMPBBH)) As IO.File
            Dim ProteinFamilies = (From Protein As String
                                   In (From item In data Select item.QueryName Distinct).ToArray
                                   Let Families = (From item In data Where String.Equals(Protein, item.QueryName) Select item.Family Distinct).ToArray
                                   Select Protein, Families).ToArray
            Dim FamilyDistributions = (From Family As String
                                       In (From item In data Select item.Family Distinct).ToArray
                                       Let FamilyProteins = (From Protein In ProteinFamilies Where Array.IndexOf(Protein.Families, Family) > -1 Select Protein.Protein).ToArray
                                       Select Family, FamilyProteins).ToArray
            Dim DataFile As IO.File = New IO.File From {New String() {"Family", "Counts", "ProteinId"}}
            Call DataFile.AppendRange((From item In FamilyDistributions
                                       Select New IO.RowObject From {
                                           item.Family,
                                           item.FamilyProteins.Count,
                                           String.Join("; ", item.FamilyProteins)}).ToArray)
            Return DataFile
        End Function

        '<ExportAPI("Pfam.Select.Regprecise_Regulators")>
        'Public Function SelectPfamSource(Besthits As IEnumerable(Of RegpreciseMPBBH), RegpreciseRegulators As FASTA.FastaFile) As FASTA.FastaFile
        '    Return MotifParallelAlignment.SelectSource(Besthits,
        '                                               RegpreciseRegulators,
        '                                               Function(besthit As RegpreciseMPBBH, Fasta As FASTA.FastaSeq) String.Equals(besthit.HitName, Fasta.Headers(1).Split.First))
        'End Function

        ''' <summary>
        ''' 通过最佳双向比对来匹配目标蛋白质集合中在Regprecise数据库中存在的记录
        ''' </summary>
        ''' <param name="RegpreciseRegulators">Regprecise调控因子序列数据库</param>
        ''' <param name="Query">所将要进行匹配的目标蛋白质集合</param>
        ''' <param name="LocalBLAST">本地BLAST服务</param>
        ''' <param name="QueryGrep">可选参数，目标蛋白质集合的ID号的解析脚本</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        '''
        <ExportAPI("Process.Auto", Info:="Matches basic data auto generation from the begining of the blastp operation.")>
        Public Function Match(RegpreciseRegulators As FASTA.FastaFile,
                              RegpreciseTfbs As FASTA.FastaFile,
                              Query As FASTA.FastaFile,
                              LocalBLAST As LocalBLAST.InteropService.InteropService,
                              Optional QueryGrep As String = "",
                              Optional WorkDir As String = "",
                              Optional ExportAll As Boolean = True) As RegpreciseMPBBH()

            Dim siteInfoList = (From regFasta As FASTA.FastaSeq
                                In RegpreciseTfbs
                                Let site As String = Regex.Match(regFasta.Title, "gene=[^]]+").Value.Split(CChar("=")).Last
                                Select New KeyValuePair(Of String, String)(site, regFasta.Headers.First.Split.First)).ToArray
            ' Dim BesthitBLAST = New BidirectionalBesthit_BLAST(LocalBLAST, If(String.IsNullOrEmpty(WorkDir), My.Computer.FileSystem.SpecialDirectories.Temp, WorkDir))
            'Dim bhArray = BesthitBLAST.Peformance(Query.FilePath,
            '                                      RegpreciseRegulators.FilePath,
            '                                      TextGrepScriptEngine.Compile(QueryGrep).PipelinePointer,
            '                                      TextGrepScriptEngine.Compile("tokens ' ' 0;tokens | last").PipelinePointer,
            '                                      "1e-3", ExportAll:=ExportAll)
            Dim ExtractedTfbsInfo = (From regulator As FastaSeq
                                     In RegpreciseRegulators
                                     Let tfbs As String() = regulator.__gettfbs(siteInfoList)
                                     Select New KeyValuePair(Of String, String())(regulator.Title.Split.First.Split(CChar("|")).Last, tfbs)).ToArray
            Dim LQuery = LinqAPI.Exec(Of RegpreciseMPBBH) _
 _
                () <= From bbhReg As LocalBLAST.Application.BBH.BiDirectionalBesthit
                      In {}' bhArray
                      Where Not String.IsNullOrEmpty(bbhReg.HitName)
                      Let ids = (From tfbs As KeyValuePair(Of String, String())
                                 In ExtractedTfbsInfo.AsParallel
                                 Where String.Equals(bbhReg.HitName, tfbs.Key)
                                 Select tfbs.Value).FirstOrDefault
                      Select New RegpreciseMPBBH With {
                          .HitName = bbhReg.HitName,
                          .QueryName = bbhReg.QueryName,
                          .Tfbs = ids
                      }

            Return LQuery
        End Function

        <Extension>
        Private Function __gettfbs(regFasta As FastaSeq, siteInforList As KeyValuePair(Of String, String)()) As String()
            Dim Tokens = Regex.Match(regFasta.Headers.Last, "tfbs=[^]]+").Value.Split(CChar("=")).Last.Split(CChar(";"))
            Dim GetTfbs = (From site As KeyValuePair(Of String, String) In siteInforList Where Array.IndexOf(Tokens, site.Key) > -1 Select site.Value).ToArray
            Return GetTfbs
        End Function

        '''' <summary>
        '''' 可能会有一部分数据是在Regprecise之中没有记录的，但是在研究之中也将其添加进入数据源之中，在此时，需要一个额外的Fasta文件来匹配所缺失的数据
        '''' </summary>
        '''' <param name="ResultRegpreciseBidirectionalBh"></param>
        '''' <param name="Regprecise"></param>
        '''' <param name="Myva_COG"></param>
        '''' <param name="PfamStrings"></param>
        '''' <returns></returns>
        '''' <remarks></remarks>
        '<ExportAPI("Data.Match", Info:="Data enrichment of the basic matches data.")>
        'Public Function Match(ResultRegpreciseBidirectionalBh As RegpreciseMPBBH(),
        '                      Regprecise As Regprecise.TranscriptionFactors,
        '                      Optional RegpreciseRegulators_Fasta As FASTA.FastaFile = Nothing,
        '                      Optional Myva_COG As IEnumerable(Of MyvaCOG) = Nothing,
        '                      Optional PfamStrings As PfamString() = Nothing) As RegpreciseMPBBH()

        '    Dim RegpreciseRegulators As Regulator() = Regprecise.FilteRegulators(Types.TF)
        '    Dim MyvaCogDict As Dictionary(Of String, MyvaCOG) = If(Myva_COG Is Nothing, New Dictionary(Of MyvaCOG), Myva_COG.ToDictionary)
        '    Dim PfamStringDict As Dictionary(Of String, PfamString) =
        '        If(PfamStrings Is Nothing, New Dictionary(Of PfamString), PfamStrings.ToDictionary)
        '    Dim GetRegpreciseRegulator = Function(Id As String) As Regulator
        '                                     Dim LQuery = From item In RegpreciseRegulators
        '                                                  Where String.Equals(item.LocusId, Id)
        '                                                  Select item
        '                                     Return LQuery.FirstOrDefault
        '                                 End Function
        '    Dim RegulatorFasta = If(RegpreciseRegulators_Fasta.IsNullOrEmpty, Nothing, FastaReaders.Regulator.LoadDocument(RegpreciseRegulators_Fasta).ToDictionary(distinct:=True))
        '    Dim GetFastaRecord = If(RegpreciseRegulators_Fasta.IsNullOrEmpty, AddressOf FastaReaders.Regulator.NullDictionary, Function(UniqueId As String) RegulatorFasta(UniqueId))
        '    Dim ChunkBuffer = (From MatchedItem As RegpreciseMPBBH
        '                       In ResultRegpreciseBidirectionalBh.AsParallel
        '                       Select __applyingProperty(
        '                           MatchedItem,
        '                           MyvaCogDict,
        '                           PfamStringDict,
        '                           GetRegpreciseRegulator,
        '                           GetFastaRecord)).ToArray
        '    Return ChunkBuffer
        'End Function

        'Private Function __applyingProperty(MatchedItem As RegpreciseMPBBH,
        '                                    MyvaCogDict As Dictionary(Of String, MyvaCOG),
        '                                    PfamStringDict As Dictionary(Of String, PfamString),
        '                                    GetRegpreciseRegulator As Func(Of String, Regprecise.Regulator),
        '                                    GetFastaRecord As Func(Of String, FastaReaders.Regulator)) As RegpreciseMPBBH
        '    Dim ProteinId As String = MatchedItem.QueryName
        '    Dim Cog = If(MyvaCogDict.ContainsKey(MatchedItem.QueryName), MyvaCogDict(ProteinId), Nothing)
        '    Dim Pfam = If(PfamStringDict.ContainsKey(MatchedItem.QueryName), PfamStringDict(ProteinId), Nothing)
        '    Dim RegulatorId As String = MatchedItem.HitName.Split(CChar(":")).Last
        '    Dim RegpreciseRegulator = GetRegpreciseRegulator(RegulatorId)

        '    If Cog IsNot Nothing Then
        '        MatchedItem.term = Cog.Category
        '        MatchedItem.description = Cog.Description
        '        MatchedItem.length = If(Cog.Length > 0, CStr(Cog.Length), "")
        '    End If

        '    If Pfam IsNot Nothing Then
        '        MatchedItem.description = Pfam.Description
        '        MatchedItem.length = Pfam.Length
        '        MatchedItem.PfamString = Pfam.get__PfamString
        '    End If

        '    If Not RegpreciseRegulator Is Nothing Then
        '        MatchedItem.pathway = RegpreciseRegulator.biological_process.JoinBy("; ")
        '        MatchedItem.effectors = If(String.IsNullOrEmpty(RegpreciseRegulator.effector), Nothing, Strings.Split(RegpreciseRegulator.effector, "; "))
        '        MatchedItem.Family = RegpreciseRegulator.family
        '        MatchedItem.Tfbs = (From site In RegpreciseRegulator.regulatorySites Select String.Join(":", site.locus_tag, site.position)).ToArray
        '        MatchedItem.regulationMode = RegpreciseRegulator.regulationMode
        '    Else
        '        Dim FastaRegulatorRecord = GetFastaRecord(RegulatorId)
        '        If Not FastaRegulatorRecord Is Nothing Then
        '            MatchedItem.Family = FastaRegulatorRecord.Family
        '            MatchedItem.Tfbs = FastaRegulatorRecord.Sites
        '        End If
        '    End If

        '    MatchedItem.HitName = MatchedItem.HitName.Split(CChar("|")).Last

        '    Return MatchedItem
        'End Function

        <ExportAPI("Write.Csv.Regprecise.bbh")>
        Public Function WriteData(data As IEnumerable(Of RegpreciseMPBBH), saveto As String) As Boolean
            Return data.SaveTo(saveto, False)
        End Function

        <ExportAPI("Read.Csv.Regprecise.bbh")>
        Public Function ReadData(path As String) As RegpreciseMPBBH()
            Return path.LoadCsv(Of RegpreciseMPBBH)(False).ToArray
        End Function

        ''' <summary>
        ''' 从最佳双向BLAST比对结果之中得到所需要的基础数据
        ''' </summary>
        ''' <param name="BLASTbh"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        '''
        <ExportAPI("bh2Regprecise.bbh", Info:="Create basic data for the matches data.")>
        Public Function Convert(BLASTbh As LocalBLAST.Application.BBH.BiDirectionalBesthit()) As RegpreciseMPBBH()
            Dim LQuery As RegpreciseMPBBH() =
                LinqAPI.Exec(Of RegpreciseMPBBH) <= From item As LocalBLAST.Application.BBH.BiDirectionalBesthit
                                                    In BLASTbh.AsParallel
                                                    Where Not String.IsNullOrEmpty(item.HitName)
                                                    Select New RegpreciseMPBBH With {
                                                        .QueryName = item.QueryName,
                                                        .HitName = item.HitName,
                                                        .length = item.length,
                                                        .description = item.description
                                                    }
            Return LQuery
        End Function

        Public Function Match(BidirectionalBhRegulators As IEnumerable(Of RegpreciseMPBBH), Regprecise As Regprecise.TranscriptionFactors) As RegpreciseMPBBH()
            Dim RegpreciseRegulators = Regprecise.ListAllRegulators.ToEntryDictionary
            Dim LQuery = (From MatchedItem As RegpreciseMPBBH In BidirectionalBhRegulators.AsParallel
                          Let TfId As String = MatchedItem.HitName.Split(CChar(":")).Last
                          Let RegpreciseTF = If(RegpreciseRegulators.ContainsKey(TfId), RegpreciseRegulators(TfId), Nothing)
                          Select __applyProperty(RegpreciseTF, MatchedItem)).ToArray
            Return LQuery
        End Function

        Private Function __applyProperty(RegpreciseTF As Regprecise.Regulator, MatchedItem As RegpreciseMPBBH) As RegpreciseMPBBH
            If RegpreciseTF IsNot Nothing Then
                MatchedItem.pathway = RegpreciseTF.biological_process.JoinBy("; ")
                MatchedItem.effectors = Strings.Split(RegpreciseTF.effector, "; ")
                MatchedItem.Family = RegpreciseTF.family
            End If

            Return MatchedItem
        End Function
    End Module
End Namespace
