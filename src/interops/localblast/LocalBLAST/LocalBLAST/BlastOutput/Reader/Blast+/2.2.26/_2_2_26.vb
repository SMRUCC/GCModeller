Imports System.Text
Imports System.Xml.Serialization
Imports System.Text.RegularExpressions
Imports LANS.SystemsBiology.NCBI.Extensions.LocalBLAST.BLASTOutput.Views
Imports Microsoft.VisualBasic.Extensions
Imports Microsoft.VisualBasic.Text

Namespace LocalBLAST.BLASTOutput.BlastPlus

    ''' <summary>
    ''' 2.2.26版本的Blast+程序的日志输出文件
    ''' </summary>
    ''' <remarks></remarks>
    Public Class _2_2_26 : Inherits LocalBLAST.BLASTOutput.IBlastOutput

        <XmlElement> Public Property Queries As Query()

        Protected Friend Const SECTION_SEPERATOR As String = "Effective search space used[:]\s*\d+"

        'Public Overrides Property FilePath As String

        Public Overrides Function Save(Optional FilePath As String = "", Optional Encoding As Encoding = Nothing) As Boolean
            If String.IsNullOrEmpty(FilePath) Then
                FilePath = Me.FilePath
            End If
            Call FileIO.FileSystem.WriteAllText(FilePath, text:=Me.GetXml, append:=False)

            Return True
        End Function

        Public Shared Function TryParse(LogFile As String) As _2_2_26
            Dim Array As String() = System.Text.RegularExpressions.Regex.Split(FileIO.FileSystem.ReadAllText(LogFile), NCBI.Extensions.LocalBLAST.BLASTOutput.BlastPlus._2_2_26.SECTION_SEPERATOR, Text.RegularExpressions.RegexOptions.Multiline)
            Dim Query = From Line As String In Array Where Not String.IsNullOrEmpty(Line) Let q = NCBI.Extensions.LocalBLAST.BLASTOutput.BlastPlus._2_2_26.Query.TryParse(Line) Select q Order By q.Query.Name Ascending '
            Dim Blastlog As _2_2_26 = New _2_2_26 With {.FilePath = LogFile & ".xml", .Queries = Query.ToArray}
            Dim ESS As Double() = (From Line As String In Array Where InStr(Line, "Effective search space used:") > 0 Select Line.Match("\d+").RegexParseDouble).ToArray
            Dim Queries = Blastlog.Queries

            For i As Integer = 0 To ESS.Count - 1
                Queries(i).EffectiveSearchSpace = ESS(i)
            Next

            Return Blastlog
        End Function

        Public Overrides Function Grep(Query As TextGrepMethod, Hits As TextGrepMethod) As IBlastOutput
            Return Me
        End Function

        Public Overrides Function ExportBestHit(Optional coverage As Double = 0.5, Optional identities_cutoff As Double = 0.15) As LocalBLAST.Application.BBH.BestHit()
            Throw New NotImplementedException
        End Function

        Public Overrides Function ExportOverview() As Overview
            Throw New NotImplementedException
        End Function

        Public Overrides Function ExportAllBestHist(Optional coverage As Double = 0.5, Optional identities_cutoff As Double = 0.15) As LocalBLAST.Application.BBH.BestHit()
            Throw New NotImplementedException
        End Function
    End Class
End Namespace