﻿#Region "Microsoft.VisualBasic::5d3f93ea4aa0a01264dc6061cbf13cff, analysis\SequenceToolkit\SequencePatterns.Abstract\Scanner.vb"

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

    '     Class IScanner
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: __complement, Complement, FindLocation, ToString
    ' 
    '     Class Scanner
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: (+2 Overloads) Scan
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Abstract.Motif.Patterns
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.NucleotideModels

Namespace Motif

    Public MustInherit Class IScanner

        Protected ReadOnly __nt As String

        Sub New(nt As IPolymerSequenceModel)
            __nt = nt.SequenceData.ToUpper
        End Sub

        Public Overrides Function ToString() As String
            Return $"{__nt.Length}bp...."
        End Function

        Public MustOverride Function Scan(pattern As String) As SimpleSegment()

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="Sequence"></param>
        ''' <param name="Loci"></param>
        ''' <returns></returns>
        ''' <remarks>这个位置查找函数是OK的</remarks>
        <ExportAPI("Loci.Find.Location", Info:="Found out all of the loci site on the target sequence.")>
        Public Shared Function FindLocation(Sequence As String, Loci As String) As Integer()
            Dim locis As New List(Of Integer)
            Dim p As Integer = 1

            Do While True
                p = InStr(Start:=p, String1:=Sequence, String2:=Loci)
                If p > 0 Then
                    locis += p
                    p += 1
                Else
                    Exit Do
                End If
            Loop

            Return locis.ToArray
        End Function

        Public Shared Function Complement(pattern As String) As String
            Dim tokens As String() = PatternParser.SimpleTokens(pattern)
            For i As Integer = 0 To tokens.Length - 1
                Dim s As String = tokens(i)
                If s.Length = 1 Then
                    s = CStr(__complement(s.First))
                Else
                    s = s.GetStackValue("[", "]")
                    Dim temp As New List(Of Char)

                    For Each c As Char In s
                        temp += __complement(c)
                    Next

                    s = New String("["c + temp + "]"c)
                End If

                tokens(i) = s
            Next

            tokens = tokens.Reverse.ToArray
            pattern = String.Join("", tokens)

            Return pattern
        End Function

        Private Shared Function __complement(c As Char) As Char
            Select Case c
                Case "A"c
                    Return "T"c
                Case "G"c
                    Return "C"c
                Case "C"c
                    Return "G"c
                Case "T"c
                    Return "A"c
                Case "N"c, "."c
                    Return "."c
                Case Else
                    Throw New Exception("Illegal characters in the pattern expression!")
            End Select
        End Function

    End Class

    ''' <summary>
    ''' 使用正则表达式扫描序列得到可能的motif位点
    ''' </summary>
    Public Class Scanner : Inherits IScanner

        Sub New(nt As IPolymerSequenceModel)
            Call MyBase.New(nt)
        End Sub

        Public Overrides Function Scan(pattern As String) As SimpleSegment()
            Return (Scan(__nt, pattern, "+"c).AsList + Scan(__nt, Complement(pattern), "-"c)).OrderBy(Function(x) x.Start).ToArray
        End Function

        Public Overloads Shared Function Scan(nt As String, pattern As String, strand As Char) As SimpleSegment()
            Dim ms$() = Regex.Matches(nt, pattern) _
                .ToArray _
                .Distinct _
                .ToArray
            Dim locis = LinqAPI.Exec(Of SimpleSegment) _
 _
                () <= From m As String
                      In ms
                      Let pos As Integer() = FindLocation(nt, m)
                      Let rc As String = New String(NucleicAcid.Complement(m).Reverse.ToArray)
                      Select LinqAPI.Exec(Of Integer, SimpleSegment)(pos) _
 _
                          <= Function(ind As Integer) As SimpleSegment
                                 Return New SimpleSegment With {
                                     .Ends = ind + m.Length,
                                     .Start = ind,
                                     .SequenceData = m,
                                     .Strand = strand.ToString,
                                     .Complement = rc
                                 }
                             End Function

            Return locis
        End Function
    End Class
End Namespace
