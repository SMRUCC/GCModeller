#Region "Microsoft.VisualBasic::40b17a37b210f4760d50f17f8acb877f, analysis\SequenceToolkit\SequenceAlignment\MSA\MSAOutput.vb"

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

    '   Total Lines: 93
    '    Code Lines: 72 (77.42%)
    ' Comment Lines: 3 (3.23%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 18 (19.35%)
    '     File Size: 3.26 KB


    '     Class MSAOutput
    ' 
    '         Properties: cost, edits, MSA, names, size
    ' 
    '         Function: PopulateAlignment, ToFasta, ToString
    ' 
    '         Sub: Print
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.SequenceModel.FASTA

Namespace MSA

    ''' <summary>
    ''' the result data of multiple sequence alignment
    ''' </summary>
    Public Class MSAOutput

        ''' <summary>
        ''' a vector of the MSA sequence name
        ''' </summary>
        ''' <returns></returns>
        Public Property names As String()
        ''' <summary>
        ''' A vector of the MSA sequence
        ''' </summary>
        ''' <returns></returns>
        Public Property MSA As String()
        Public Property edits As Integer()

        <XmlAttribute>
        Public Property cost As Double

        Public ReadOnly Property size As Integer
            Get
                Return MSA.Length
            End Get
        End Property

        Public Overrides Function ToString() As String
            With New MemoryStream
                Print(, New IO.StreamWriter(.ByRef))
                Return .ToArray.UTF8String
            End With
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function ToFasta() As FastaFile
            Return New FastaFile(PopulateAlignment)
        End Function

        Public Iterator Function PopulateAlignment() As IEnumerable(Of FastaSeq)
            Dim MSA As String() = Me.MSA

            For i As Integer = 0 To MSA.Length - 1
                Yield New FastaSeq With {
                    .Headers = {names.ElementAtOrDefault(i, $"seq_{i + 1}")},
                    .SequenceData = MSA(i)
                }
            Next
        End Function

        Public Sub Print(Optional maxNameWidth% = 10, Optional dev As TextWriter = Nothing)
            Dim n As Integer = MSA.Length
            Dim names = Me.names.ToArray
            Dim out As TextWriter = dev Or Console.Out.AsDefault
            Dim maxPadEditWidth As Integer = 8

            Call out.WriteLine()
            Call out.WriteLine($"> seq_name({n})".PadRight(maxNameWidth, " "c) & vbTab & "edits".PadRight(maxPadEditWidth, " "c) & " " & $"MSA Result(cost={cost})")
            Call out.WriteLine()

            For i As Integer = 0 To n - 1
                names(i) = Mid(names(i), 1, maxNameWidth)
                names(i) = names(i).PadLeft(maxNameWidth, " "c)

                Call out.WriteLine(names(i) & vbTab &
                                   edits(i).ToString.PadRight(maxPadEditWidth, " "c) & " " &
                                   MSA(i))
            Next

            Dim conserved$ = ""

            For j As Integer = 0 To MSA(0).Length - 1
                Dim index% = j
                Dim column = MSA.Select(Function(s) s(index)).ToArray

                If column.Distinct.Count = 1 Then
                    conserved &= "*"
                ElseIf column.Count(CenterStar.GapChar) / n > 0.8 Then
                    conserved &= "-"
                Else
                    conserved &= " "
                End If
            Next

            If Not Strings.Trim(conserved).StringEmpty Then
                Call out.WriteLine(New String(" "c, maxNameWidth) & vbTab &
                                   New String(" "c, maxPadEditWidth) & " " &
                                   conserved)
            End If

            Call out.Flush()
        End Sub
    End Class
End Namespace
