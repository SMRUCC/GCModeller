Imports System.Text
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Linq.Extensions

Namespace Interpro.Xml

    <PackageNamespace("Interpro.DbTools", Category:=APICategories.ResearchTools, Publisher:="")>
    Public Module ToolsAPI

        Sub New()
            Call Settings.Session.Initialize()
        End Sub

        <ExportAPI("LoadDB")>
        Public Function LoadDb(<Parameter("Interpro.Xml", "If this parameter is null, then the default file path in the GCModeller repository database will be used.")>
                               Optional path As String = "") As DbArchive
            If Not path.FileExists Then
                path = GCModeller.FileSystem.InterproXml
            End If

            Dim Trim As String = __trim(path)
            Dim DbXml As DbArchive = Trim.CreateObjectFromXml(Of DbArchive)()
            DbXml.FilePath = path
            Return DbXml
        End Function

        Private Function __trim(path As String) As String
            Dim source As String = FileIO.FileSystem.ReadAllText(path)
            Dim Tokens As String() = Strings.Split(source, "<abstract>")
            Dim sbr As StringBuilder = New StringBuilder(Tokens(Scan0))

            For Each strToken As String In Tokens.Skip(1)
                Call sbr.Append("<abstract>")
                Call sbr.Append(__trimSegment(strToken))
            Next

            source = sbr.ToString
#If DEBUG Then
            Dim idx As Integer = InStr(source, "</abstract>") + Len("</abstract>")
            Call Mid(source, 1, idx).__DEBUG_ECHO
#End If
            Return source
        End Function

        Private Function __trimSegment(afterTag As String) As String
            Dim Tokens As String() = Strings.Split(afterTag, "</abstract>")
            Dim sbr As StringBuilder = New StringBuilder(Tokens(Scan0))
            Call sbr.Replace("<", "&lt;")
            Call sbr.Append("</abstract>")
            Call sbr.Append(Tokens(1))

            Return sbr.ToString
        End Function

        <ExportAPI("FamilyView.Build")>
        Public Function BuildFamilies(DbXml As DbArchive) As Family()
            Dim dict = DbXml.interpro.ToDictionary(Function(x) x.id)
            Dim LQuery = (From interpro In dict.AsParallel
                          Where String.Equals(interpro.Value.type, "family", StringComparison.OrdinalIgnoreCase)
                          Select Family.CreateObject(interpro.Value, dict)).ToArray
            Return LQuery
        End Function
    End Module
End Namespace