Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Text.Parser.HtmlParser
Imports SMRUCC.genomics.Data.SABIORK.SBML

Public Module docuRESTfulWeb

    Public Const KEGG_QUERY_ENTRY As String = "http://sabiork.h-its.org/sabioRestWebServices/reactions/reactionIDs?q=KeggReactionID:"

    ''' <summary>
    ''' Get a single kinetic law entry by SABIO entry ID
    ''' </summary>
    ''' <param name="id"></param>
    ''' <returns></returns>
    Public Function getModelById(id As String, Optional level As Integer = 2, Optional version As Integer = 3, Optional annotation As String = "identifier") As SabiorkSBML
        Dim url = SabiorkSBML.URL_SABIORK_KINETIC_LAWS_QUERY & id
        Dim sbml As String = url.GET

        Return sbml.LoadXml(Of SabiorkSBML)
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="IdList"></param>
    ''' <param name="ExportDir"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' 因为会存在非常多的废弃的id编号，所以这个函数应该会被废弃掉
    ''' </remarks>
    <ExportAPI("Query.KEGG")>
    <Extension>
    Public Iterator Function QueryUsing_KEGGId(IdList As String(), ExportDir As String) As IEnumerable(Of String)
        For Each Id As String In IdList
            Dim url As String = (KEGG_QUERY_ENTRY & Id)
            Dim PageContent = url.GET
            Dim Entries As String() = (From m As Match In Regex.Matches(PageContent, "<SabioReactionID>\d+</SabioReactionID>") Select m.Value.GetValue).ToArray

            For Each Entry In Entries
                Dim File = String.Format("{0}/{1}-{2}.sbml", ExportDir, Id, Entry)

                url = SabiorkSBML.URL_SABIORK_KINETIC_LAWS_QUERY & Entry
                Call url.GET.SaveTo(File)
            Next
        Next
    End Function

    <ExportAPI("SABIORK.Downloads")>
    Public Function Download(Dir As String) As Integer
        Dim c As Integer = 0

        For i As Integer = FileIO.FileSystem.GetFiles(Dir).Count + 1 To Integer.MaxValue
            Dim id As String = "kinlawids_" & i
            Dim url As String = SabiorkSBML.URL_SABIORK_KINETIC_LAWS_QUERY & i
            Dim File = String.Format("{0}/{1}.sbml", Dir, id)

            Call url.GET.SaveTo(File)

            If FileIO.FileSystem.GetFileInfo(File).Length < 100 Then
                c += 1
                If c > 1500 Then
                    Exit For
                End If
            Else
                c = 0
            End If

            Call Threading.Thread.Sleep(10)
        Next

        Return 0
    End Function
End Module
