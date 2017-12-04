#Region "Microsoft.VisualBasic::3c882c457df6db36e68261b6d818d367, ..\GCModeller\analysis\SequenceToolkit\SmithWaterman\MotifScanner.vb"

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

Imports System.Linq
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.ListExtensions
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Motif
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.NucleotideModels

Public Class MotifScanner : Inherits IScanner

    Sub New(nt As IPolymerSequenceModel)
        Call MyBase.New(nt)
    End Sub

    Public Overrides Function Scan(pattern As String) As SimpleSegment()
        Return (Scan(__nt, pattern, AddressOf Equals).AsList +
            Scan(__nt, Complement(pattern), AddressOf Equals)) _
            .OrderBy(Function(x) x.Start) _
            .ToArray
    End Function

    ReadOnly __rand As Random = New Random

    Public Overloads Function Equals(pattern As String, residue As String) As Integer
        Dim r As Char = residue.FirstOrDefault(ASCII.NUL)

        If pattern.Length = 1 Then
            Dim p As Char = pattern.FirstOrDefault(ASCII.NUL)
            If p = "."c OrElse p = "N"c Then
                Return 10
            End If
            If Char.IsUpper(p) Then
                If p = r Then
                    Return 10
                Else
                    Return -10
                End If
            Else  ' 小写的，有一定的概率是别的字符
                If p = Char.ToLower(r) Then
                    Return 5
                Else
                    If __rand.NextDouble < 0.75 Then  '  例如a大多数情况下是A
                        Return -5
                    Else
                        Return 5
                    End If
                End If
            End If
        Else
            ' []匹配
            For Each c As Char In pattern
                If c = r Then
                    Return 10
                End If
            Next

            Return -10
        End If
    End Function

    Public Overloads Shared Function Scan(nt As String, pattern As String, equals As ISimilarity(Of String)) As SimpleSegment()
        Dim words As String() = Patterns.SimpleTokens(pattern)
        Dim subject As String() = nt.Select(Function(c) CStr(c)).ToArray
        Dim GSW As New GSW(Of String)(words, subject, equals, AddressOf ToChar)
        Dim out As Output = GetOutput(GSW, 0, (2 / 3) * words.Length)

        Return LinqAPI.Exec(Of SimpleSegment) <= From x As HSP
                                                 In out.HSP
                                                 Select New SimpleSegment With {
                                                     .SequenceData = x.Subject,
                                                     .Start = x.FromB,
                                                     .Ends = x.ToB
                                                 }
    End Function

    ''' <summary>
    '''
    ''' </summary>
    ''' <param name="cutoff">0%-100%</param>
    ''' <returns></returns>
    Public Shared Function GetOutput(this As GSW(Of String), cutoff As Double, minW As Integer) As Output
        Return Output.CreateObject(this, Function(x) x, cutoff, minW)
    End Function

    Public Shared Function ToChar(s As String) As Char
        If s.Length = 1 Then
            Dim c As Char = s.FirstOrDefault(ASCII.NUL)
            If c = "." Then
                Return "N"c
            Else
                Return c
            End If
        Else
            Dim c As Char = s.FirstOrDefault(ASCII.NUL)
            Return Char.ToLower(c)
        End If
    End Function
End Class
