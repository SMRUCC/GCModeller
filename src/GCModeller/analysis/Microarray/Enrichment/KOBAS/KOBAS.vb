#Region "Microsoft.VisualBasic::3fe09c495a34daa657a77fe379db2a49, GCModeller\analysis\Microarray\Enrichment\KOBAS\KOBAS.vb"

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

    '   Total Lines: 98
    '    Code Lines: 70
    ' Comment Lines: 13
    '   Blank Lines: 15
    '     File Size: 3.76 KB


    '     Module KOBAS
    ' 
    '         Function: GenelistEnrichment, (+2 Overloads) P, SplitTable
    ' 
    '         Sub: SplitData
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data
Imports Microsoft.VisualBasic.Data.csv

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
            Dim tmp = TempFileSystem.GetAppSysTempFile

            link = "http://kobas.cbi.pku.edu.cn/" & link.GetStackValue("'", "'").Trim("."c, "/"c)
            link.DownloadFile(tmp)
            tmp = tmp.ReadAllText

            Return tmp
        End Function

        Public Sub SplitData(path$, Optional EXPORT$ = Nothing)
            If EXPORT.StringEmpty Then
                EXPORT = path.TrimSuffix
            End If

            For Each d In SplitTable(path)
                Dim outName$ = path.BaseName & "-" & d.name.NormalizePathString(False)
                Dim file$ = EXPORT & $"/{outName}.csv"

                Call d.ToArray.SaveTo(file)
            Next
        End Sub

        <Extension>
        Public Iterator Function SplitTable(path As String) As IEnumerable(Of NamedCollection(Of EnrichmentTerm))
            Dim lines$() = path _
                .ReadAllLines _
                .Where(Function(s) Not s.StringEmpty AndAlso Not Regex.Match(s, "[-]+").Value = s) _
                .Skip(3) _
                .ToArray
            Dim terms = csv _
                .ImportsTsv(Of EnrichmentTerm)(lines) _
                .GroupBy(Function(t) t.Database) _
                .Where(Function(g)
                           Return Not g.Key.TextEquals("Database")
                       End Function)

            For Each part In terms
                Yield New NamedCollection(Of EnrichmentTerm) With {
                    .name = part.Key,
                    .value = part.ToArray
                }
            Next
        End Function

        ''' <summary>
        ''' ``-<see cref="Math.Log10(Double)"/>(<see cref="EnrichmentTerm.Pvalue"/>)``
        ''' </summary>
        ''' <param name="x"></param>
        ''' <returns></returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension> Public Function P(x As EnrichmentTerm, Optional correctedPvalue As Boolean = True) As Double
            If correctedPvalue Then
                Return -Math.Log10(x.CorrectedPvalue)
            Else
                Return -Math.Log10(x.Pvalue)
            End If
        End Function

        ''' <summary>
        ''' ``-<see cref="Math.Log10(Double)"/>(<see cref="EnrichmentTerm.Pvalue"/>)``
        ''' </summary>
        ''' <param name="x"></param>
        ''' <returns></returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension> Public Function P(x As IGoTermEnrichment) As Double
            Return -Math.Log10(x.Pvalue)
        End Function
    End Module
End Namespace
