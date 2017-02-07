#Region "Microsoft.VisualBasic::ef49e9c5dd3533fb8d0f86761904d9a2, ..\GCModeller\analysis\Microarray\Enrichment\KOBAS.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports System.Collections.Specialized
Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.Data
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace KOBAS

    ''' <summary>
    ''' 做富集分析使用的
    ''' </summary>
    Public Module KOBAS

        Public Function GenelistEnrichment(list$, SpeciesSearch$, Optional input_type$ = "id:uniprot") As String
            Dim args As New Dictionary(Of String, String())

            If list.FileExists(True) Then
                list = list.ReadAllText
            End If

            Call args.Add(NameOf(SpeciesSearch), {SpeciesSearch})
            Call args.Add(NameOf(input_type), {input_type})
            Call args.Add("input_seq1", {list})
            Call args.Add("kobasdb[]", {"G", "K"})
            Call args.Add("Run", {"Run"})

            Dim html$ = "http://kobas.cbi.pku.edu.cn/run_kobas.php".POST(args, Referer:="http://kobas.cbi.pku.edu.cn/anno_iden.php")
            Dim link As String = Regex.Match(html, "javascript:window.location.href='./download_file.php?type=run_kobas&userid=.+?'", RegexICSng).Value
            Dim tmp = App.GetAppSysTempFile

            link = "http://kobas.cbi.pku.edu.cn/" & link.GetStackValue("'", "'").Trim("."c, "/"c)
            link.DownloadFile(tmp)
            tmp = tmp.ReadAllText

            Return tmp
        End Function

        Public Sub SplitData(path$, Optional EXPORT$ = Nothing)
            Dim lines$() = path _
                .ReadAllLines _
                .Where(Function(s) Not s.IsBlank AndAlso Not Regex.Match(s, "[-]+").Value = s) _
                .Skip(3) _
                .ToArray
            Dim terms = csv.ImportsTsv(Of EnrichmentTerm)(lines).GroupBy(Function(t) t.Database)

            If EXPORT.IsBlank Then
                EXPORT = path.TrimSuffix
            End If

            For Each d In terms
                Dim file$ = EXPORT & "/" & path.BaseName & "-" & d.Key.NormalizePathString(False) & ".csv"
                Call d.ToArray.SaveTo(file)
            Next
        End Sub

        ''' <summary>
        ''' ``-<see cref="Math.Log10(Double)"/>(<see cref="EnrichmentTerm.Pvalue"/>)``
        ''' </summary>
        ''' <param name="x"></param>
        ''' <returns></returns>
        <Extension> Public Function P(x As EnrichmentTerm, Optional correctedPvalue As Boolean = True) As Double
            If correctedPvalue Then
                Return -Math.Log10(x.CorrectedPvalue)
            Else
                Return -Math.Log10(x.Pvalue)
            End If
        End Function
    End Module

    Public Class EnrichmentTerm

        <Column("#Term")>
        Public Property Term As String
        Public Property Database As String
        Public Property ID As String

        ''' <summary>
        ''' Input number
        ''' </summary>
        ''' <returns></returns>
        <Column("Input number")> Public Property number As Integer

        ''' <summary>
        ''' Background number
        ''' </summary>
        ''' <returns></returns>
        <Column("Background number")> Public Property Backgrounds As Integer

        ''' <summary>
        ''' P-Value
        ''' </summary>
        ''' <returns></returns>
        <Column("P-Value")> Public Property Pvalue As Double

        ''' <summary>
        ''' Corrected P-Value
        ''' </summary>
        ''' <returns></returns>
        <Column("Corrected P-Value")> Public Property CorrectedPvalue As Double

        ''' <summary>
        ''' The group of this input gene id list
        ''' </summary>
        ''' <returns></returns>
        Public Property Input As String
        Public Property ORF As String()

        <Column("Hyperlink")>
        Public Property link As String

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class
End Namespace
