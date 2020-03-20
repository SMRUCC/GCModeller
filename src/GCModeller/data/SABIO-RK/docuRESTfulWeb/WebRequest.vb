Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.Text.Parser.HtmlParser
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.BriteHEntry
Imports SMRUCC.genomics.Data.SABIORK.SBML
Imports r = System.Text.RegularExpressions.Regex
Imports sbXML = SMRUCC.genomics.Model.SBML.Level3.XmlFile

Public Module WebRequest

    Public Const KEGG_QUERY_ENTRY As String = "http://sabiork.h-its.org/sabioRestWebServices/reactions/reactionIDs?q=KeggReactionID:"

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="IdList"></param>
    ''' <param name="ExportDir"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' 因为会存在非常多的废弃的id编号，所以这个函数应该会被废弃掉
    ''' </remarks>
    Public Iterator Function QueryUsing_KEGGId(IdList As String(), ExportDir As String) As IEnumerable(Of String)
        For Each Id As String In IdList
            Dim url As String = (KEGG_QUERY_ENTRY & Id)
            Dim PageContent = url.GET
            Dim Entries As String() = (From m As Match In r.Matches(PageContent, "<SabioReactionID>\d+</SabioReactionID>") Select m.Value.GetValue).ToArray

            For Each Entry In Entries
                Dim File = String.Format("{0}/{1}-{2}.sbml", ExportDir, Id, Entry)

                url = SabiorkSBML.URL_SABIORK_KINETIC_LAWS_QUERY & Entry
                Call url.GET.SaveTo(File)
            Next
        Next
    End Function

    Public Iterator Function QueryByECNumbers(enzymes As htext, export$) As IEnumerable
        Dim ecNumbers As BriteHText() = enzymes.Hierarchical _
            .EnumerateEntries _
            .ToArray
        Dim saveXml As String
        Dim cache$ = $"{export}/.cache"
        Dim q As Dictionary(Of QueryFields, String)

        For Each id As BriteHText In ecNumbers
            saveXml = id.BuildPath(export)
            q = New Dictionary(Of QueryFields, String) From {
                {QueryFields.ECNumber, id.entryID}
            }

            Call docuRESTfulWeb.searchKineticLaws(q, cache) _
                .GetXml _
                .SaveTo(saveXml)
        Next
    End Function

    Public Function QueryByECNumber(ECNumber As String, Optional cache$ = "./.cache") As sbXML
        Dim q As New Dictionary(Of QueryFields, String) From {
            {QueryFields.ECNumber, ECNumber}
        }
        Dim xml As sbXML = docuRESTfulWeb.searchKineticLaws(q, cache)

        Return xml
    End Function
End Module
