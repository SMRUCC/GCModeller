#Region "Microsoft.VisualBasic::e8fc29a0006168ee7646aa0bc7ec773e, CLI_tools\GCModeller\CLI\SeqDiff.vb"

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

    ' Class SeqDiff
    ' 
    '     Properties: [Date], data, Host, Location, Tag
    '                 uid
    ' 
    '     Function: Parser, ToString
    ' 
    '     Sub: __addData, __repeatsData, ApplyPalindrom, ApplyRepeats, GCOutlier
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.SequenceModel.NucleotideModels.NucleicAcidStaticsProperty
Imports SMRUCC.genomics.Analysis.SequenceTools.DNA_Comparative.GCOutlier
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Topologically
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Data.csv

Public Class SeqDiff : Implements INamedValue

    Public Property uid As String Implements INamedValue.Key
    Public Property Tag As String
    Public Property Location As String
    Public Property Host As String
    Public Property [Date] As String
    Public Property data As Dictionary(Of String, String)

    Public Shared Function Parser(fa As FastaSeq) As SeqDiff
        Dim attrs As String() = fa.Headers

        Return New SeqDiff With {
            .uid = attrs(Scan0),
            .Date = attrs.Last,
            .Tag = attrs(1),
            .Host = If(attrs.Length = 5 OrElse attrs.Length = 4, attrs(2), attrs(3)),
            .Location = If(attrs.Length = 4, attrs(2), If(attrs.Length = 5, attrs(3), attrs(4))),
            .data = New Dictionary(Of String, String)
        }
    End Function

    Public Shared Sub GCOutlier(mla As IEnumerable(Of FastaSeq), ByRef bufs As SeqDiff(), quantiles As Double(),
                                Optional winsize As Integer = 250,
                                Optional steps As Integer = 50,
                                Optional slideSize As Integer = 5)

        Dim gcSkewData = OutlierAnalysis(mla, quantiles, winsize, steps, slideSize, AddressOf GCSkew)
        Dim gcContentData = OutlierAnalysis(mla, quantiles, winsize, steps, slideSize, AddressOf GCContent)
        Dim dict As Dictionary(Of String, SeqDiff) = bufs.ToDictionary(Function(x) x.uid & x.Date)

        Call __addData(dict, gcSkewData, "GCSkew")
        Call __addData(dict, gcContentData, "GC%")

        For Each fa In mla
            Dim uid As String = fa.Headers.First & fa.Headers.Last
            dict(uid).data.Add("GCSkew (avg)", GCSkew(fa, winsize, steps, False).Average)
            dict(uid).data.Add("GC% (avg)", GCContent(fa, winsize, steps, False).Average)
        Next
    End Sub

    Private Shared Sub __addData(ByRef dict As Dictionary(Of String, SeqDiff), data As IEnumerable(Of lociX), title As String)
        Dim g = From x As lociX
                In data
                Let attrs As String() = x.Title.Split("|"c)
                Let uid = attrs.First & attrs.Last
                Select x, uid
                Group x By uid Into Group

        For Each x In g
            Dim qg = From o As lociX
                     In x.Group
                     Select o
                     Group o By o.qLevel Into Group

            For Each ql In qg
                Dim gg = ql.Group.ToArray
                dict(x.uid).data.Add($"{title}(quantile={ql.qLevel}) avg", gg.Average(Function(o) o.value))
                dict(x.uid).data.Add($"{title}(quantile={ql.qLevel}) count", gg.Length)
            Next
        Next
    End Sub

    ''' <summary>
    ''' <see cref="ImperfectPalindrome"/>
    ''' </summary>
    ''' <param name="mla"></param>
    ''' <param name="bufs"></param>
    ''' <param name="DIR"></param>
    ''' <param name="title"></param>
    Public Shared Sub ApplyPalindrom(mla As IEnumerable(Of FastaSeq), ByRef bufs As SeqDiff(), DIR As String, title As String)
        Dim files = (ls - l - r - wildcards("*.csv") <= DIR).ToDictionary(Function(x) x.BaseName)
        Dim dict = bufs.ToDictionary(Function(x) x.uid & x.Date)

        For Each fa In mla
            Dim key As String = fa.Title.NormalizePathString
            If Not files.ContainsKey(key) Then
                Call key.Warning
                Continue For
            End If
            Try
                Dim data = files(key).LoadCsv(Of ImperfectPalindrome)
                Dim uid As String = fa.Headers.First & fa.Headers.Last
                dict(uid).data.Add($"{title}.distance(Avg)", data.Average(Function(x) x.Distance))
                dict(uid).data.Add($"{title}.score(Avg)", data.Average(Function(x) x.Score))
                dict(uid).data.Add($"{title}.maxMatch(Avg)", data.Average(Function(x) x.MaxMatch))
                dict(uid).data.Add($"{title}.count", data.Count)
            Catch ex As Exception
                ex = New Exception(files(key).ToFileURL, ex)
                Call ex.PrintException
                Call App.LogException(ex)
            End Try
        Next
    End Sub

    ''' <summary>
    ''' 正向和反向
    ''' </summary>
    ''' <param name="mla"></param>
    ''' <param name="bufs"></param>
    ''' <param name="DIR"></param>
    ''' <param name="rev"></param>
    Public Shared Sub ApplyRepeats(mla As IEnumerable(Of FastaSeq), ByRef bufs As SeqDiff(), DIR As String, rev As Boolean)
        Dim files = (ls - l - r - wildcards("*.csv") <= DIR).ToDictionary(Function(x) x.BaseName)
        Dim dict = bufs.ToDictionary(Function(x) x.uid & x.Date)

        For Each fa In mla
            Dim key As String = fa.Title.NormalizePathString
            If Not files.ContainsKey(key) Then
                Continue For
            End If
            Dim uid As String = fa.Headers.First & fa.Headers.Last
            Dim seq As SeqDiff = dict(uid)
            Try
                If rev Then
                    Dim data = files(key).LoadCsv(Of ReverseRepeatsView)
                    Call __repeatsData(Of ReverseRepeatsView)(data, seq)
                Else
                    Dim data = files(key).LoadCsv(Of RepeatsView)
                    Call __repeatsData(Of RepeatsView)(data, seq)
                End If
            Catch ex As Exception
                ex = New Exception(files(key).ToFileURL, ex)
                Call ex.PrintException
                Call App.LogException(ex)
            End Try
        Next
    End Sub

    Private Shared Sub __repeatsData(Of T As RepeatsView)(data As IEnumerable(Of T), ByRef seq As SeqDiff)
        Dim key As String = GetType(T).Name
        seq.data.Add($"{key}.len(avg)", data.Average(Function(x) x.Length))
        seq.data.Add($"{key}.hot(avg)", data.Average(Function(x) x.Hot))
        seq.data.Add($"{key}.locis(avg)", data.Average(Function(x) x.RepeatsNumber))
        seq.data.Add($"{key}.count", data.Count)
    End Sub

    Public Overrides Function ToString() As String
        Return Me.GetJson
    End Function
End Class
