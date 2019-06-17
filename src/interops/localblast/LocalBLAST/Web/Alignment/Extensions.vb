Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language
Imports r = System.Text.RegularExpressions.Regex

Namespace NCBIBlastResult.WebBlast

    <HideModuleName>
    Public Module Extensions

        ''' <summary>
        ''' 导出绘制的顺序
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>这里不能够使用并行拓展</remarks>
        ''' 
        <Extension>
        Public Function ExportOrderByGI(table As AlignmentTable) As String()
            Dim LQuery As String() = (From hit As HitRecord
                                      In table.Hits
                                      Select hit.GI.FirstOrDefault
                                      Distinct).ToArray
            Return LQuery
        End Function

        Const LOCUS_ID As String = "(emb|gb|dbj)\|[a-z]+\d+"

        <Extension>
        Public Function GetHitsEntryList(table As AlignmentTable) As String()
            Dim LQuery As String() =
                LinqAPI.Exec(Of String) <= From hit As HitRecord
                                           In table.Hits
                                           Let hitID As String = r.Match(hit.SubjectIDs, LOCUS_ID, RegexICSng).Value
                                           Where Not String.IsNullOrEmpty(hitID)
                                           Select hitID.Split(CChar("|")).Last
                                           Distinct
            Return LQuery
        End Function

        <Extension>
        Public Iterator Function TopBest(raw As IEnumerable(Of HitRecord)) As IEnumerable(Of HitRecord)
            Dim gg = From x As HitRecord In raw Select x Group x By x.QueryID Into Group

            For Each groups In gg
                Dim orders = From x As HitRecord
                             In groups.Group
                             Select x
                             Order By x.Identity Descending

                Yield orders.First
            Next
        End Function
    End Module
End Namespace