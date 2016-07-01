Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.DocumentFormat.Csv.Extensions
Imports Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.CsvExports
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BatchParallel
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH

Namespace NCBIBlastResult

    <[PackageNamespace]("SBH_Tabular",
                        Publisher:="xie.guigang@gmail.com",
                        Category:=APICategories.UtilityTools)>
    Public Module SBH_Tabular

        ''' <summary>
        ''' BBH的文件夹
        ''' </summary>
        ''' <param name="QueryID"></param>
        ''' <param name="sbhDIR">Directory path which it contains the sbh result data.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' 
        <ExportAPI("Alignment.Table.From.bbh.Orthologous")>
        Public Function CreateFromBBHOrthologous(QueryID As String, sbhDIR As String, <Parameter("Query.Info")> queryInfo As IEnumerable(Of GeneDumpInfo)) As AlignmentTable
            Dim Entries = (From path As KeyValuePair(Of String, String)
                           In sbhDIR.LoadSourceEntryList({"*.csv"})
                           Let Log As AlignEntry = LogNameParser(path.Value)
                           Select ID = path.Key,
                               LogEntry = Log,
                               besthitData = path.Value.LoadCsv(Of BestHit)(False).ToArray).ToArray
            Dim querySide = From entry In Entries
                            Where String.Equals(entry.LogEntry.QueryName, QueryID, StringComparison.OrdinalIgnoreCase)
                            Select entry '得到Query的比对方向的数据
            Dim hitSide = (From entry In Entries
                           Where String.Equals(entry.LogEntry.HitName, QueryID, StringComparison.OrdinalIgnoreCase)
                           Select entry).ToArray
            Dim BBH = (From query
                       In querySide.AsParallel
                       Let subject = query.LogEntry.SelectEquals(hitSide, Function(Entry) Entry.LogEntry)
                       Let bbhData As BiDirectionalBesthit() =
                           (LinqAPI.Exec(Of BiDirectionalBesthit) <= From __bbh As BiDirectionalBesthit
                                                                     In BBHParser.GetBBHTop(
                                                                         query.besthitData,
                                                                         subject.besthitData)
                                                                     Where __bbh.Matched
                                                                     Select __bbh)
                       Select query.ID,
                           query.LogEntry,
                           bbhData).ToArray

            Dim queryDict As Dictionary(Of GeneDumpInfo) = queryInfo.ToDictionary
            Dim hits As HitRecord() =
                LinqAPI.Exec(Of HitRecord) <= From genome
                                              In BBH
                                              Select From gene As BiDirectionalBesthit
                                                     In genome.bbhData
                                                     Let QueryGene As GeneDumpInfo = queryDict(gene.QueryName)
                                                     Select New HitRecord With {
                                                         .Identity = gene.Identities,
                                                         .QueryStart = QueryGene.Left,
                                                         .QueryEnd = QueryGene.Right,
                                                         .SubjectIDs = genome.LogEntry.HitName
                                                     }
            Dim Table As New AlignmentTable With {
                .Database = sbhDIR,
                .Hits = hits.ToArray,
                .Program = "BBH",
                .Query = QueryID,
                .RID = Now.ToString
            }
            Return Table
        End Function
    End Module
End Namespace