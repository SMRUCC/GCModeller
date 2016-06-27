Imports System.Runtime.CompilerServices
Imports LANS.SystemsBiology.Assembly.NCBI.GenBank.TabularFormat
Imports LANS.SystemsBiology.Assembly.NCBI.GenBank.TabularFormat.ComponentModels
Imports LANS.SystemsBiology.ContextModel
Imports Microsoft.VisualBasic.DocumentFormat.Csv
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.UnixBash

Namespace NCBIBlastResult

    ''' <summary>
    ''' <see cref="Analysis.BestHit"/> -> <see cref="AlignmentTable"/>
    ''' </summary>
    Public Module BBHMetaAPI

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="bbh"></param>
        ''' <param name="PTT">因为这个是blastp BBH的结果，所以没有基因组的位置信息，在这里使用PTT文档来生成绘图时所需要的位点信息</param>
        ''' <param name="visualGroup">
        ''' 由于在进行blast绘图的时候，程序是按照基因组来分组绘制的，而绘制的对象不需要显示详细的信息，所以在这里为True的话，会直接使用基因组tag来替换名称进而用于blast作图
        ''' </param>
        ''' <returns></returns>
        Public Function DataParser(bbh As Analysis.BestHit,
                                   PTT As PTT,
                                   Optional visualGroup As Boolean = False,
                                   Optional scoreMaps As Func(Of Analysis.Hit, Double) = Nothing) As AlignmentTable

            If scoreMaps Is Nothing Then
                scoreMaps = Function(x) x.Identities
            End If

            Return New AlignmentTable With {
                .Database = bbh.sp,
                .Program = GetType(Analysis.BestHit).FullName,
                .Query = bbh.sp,
                .RID = Now.ToString,
                .Hits = LinqAPI.Exec(Of HitRecord) <= From prot As Analysis.HitCollection
                                                      In bbh.hits
                                                      Let ORF As GeneBrief = PTT(prot.QueryName)
                                                      Select From hit As Analysis.Hit
                                                             In prot.Hits
                                                             Select New HitRecord With {
                                                                 .QueryID = prot.QueryName,
                                                                 .Identity = scoreMaps(hit),
                                                                 .AlignmentLength = ORF.Length,
                                                                 .BitScore = ORF.Length,
                                                                 .QueryEnd = ORF.Location.Ends,
                                                                 .QueryStart = ORF.Location.Start,
                                                                 .SubjectEnd = ORF.Location.Ends,
                                                                 .SubjectStart = ORF.Location.Start,
                                                                 .SubjectIDs = If(visualGroup, hit.tag, hit.HitName),
                                                                 .DebugTag = hit.tag
                                                             }
            }
        End Function

        <Extension>
        Public Function DensityScore(DIR As String, Optional scale As Double = 1) As Func(Of Analysis.Hit, Double)
            Dim datas As IEnumerable(Of Density) =
                LinqAPI.Exec(Of Density) <= From path As String
                                            In ls - l - r - wildcards("*.csv") <= DIR
                                            Select path.LoadCsv(Of Density)
            Dim hash As Dictionary(Of String, Density) =
                (From x As Density
                 In datas
                 Select x
                 Group x By x.locus_tag Into Group) _
                      .ToDictionary(Function(x) x.locus_tag,
                                    Function(x) x.Group.First)
            Return Function(x) _
                If(hash.ContainsKey(x.HitName),
                hash(x.HitName).Abundance * scale,
                0R)
        End Function
    End Module
End Namespace