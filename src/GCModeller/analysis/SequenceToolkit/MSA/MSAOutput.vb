#Region "Microsoft.VisualBasic::e59d9f2391130265c3f83d3c5051b1f9, GCModeller\analysis\SequenceToolkit\MSA\MSAOutput.vb"

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

    '   Total Lines: 67
    '    Code Lines: 54
    ' Comment Lines: 0
    '   Blank Lines: 13
    '     File Size: 1.94 KB


    ' Class MSAOutput
    ' 
    '     Properties: cost, MSA, names, size
    ' 
    '     Function: ToFasta, ToString
    ' 
    '     Sub: Print
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.SequenceModel.FASTA

Public Class MSAOutput

    Public Property names As String()
    Public Property MSA As String()
    Public Property cost As Double

    Public ReadOnly Property size As Integer
        Get
            Return MSA.Length
        End Get
    End Property

    Public Overrides Function ToString() As String
        With New MemoryStream
            Print(, New StreamWriter(.ByRef))
            Return .ToArray.UTF8String
        End With
    End Function

    Public Function ToFasta() As FastaFile
        Dim MSA = Me.MSA
        Dim seqs = names _
            .Select(Function(name, i)
                        Return New FastaSeq With {
                            .Headers = {name},
                            .SequenceData = MSA(i)
                        }
                    End Function)

        Return New FastaFile(seqs)
    End Function

    Public Sub Print(Optional maxNameWidth% = 10, Optional dev As TextWriter = Nothing)
        Dim n = MSA.Length
        Dim names = Me.names.ToArray
        Dim out As TextWriter = dev Or Console.Out.AsDefault

        For i As Integer = 0 To n - 1
            names(i) = Mid(names(i), 1, maxNameWidth)
            names(i) = names(i) & New String(" "c, maxNameWidth - names(i).Length)
            out.WriteLine(names(i) & vbTab & MSA(i))
        Next

        Dim conserved$ = ""

        For j As Integer = 0 To MSA(0).Length - 1
            Dim index% = j
            Dim column = MSA.Select(Function(s) s(index)).ToArray

            If column.Distinct.Count = 1 Then
                conserved &= "*"
            Else
                conserved &= " "
            End If
        Next

        If Not Strings.Trim(conserved).StringEmpty Then
            out.WriteLine(New String(" "c, maxNameWidth) & vbTab & conserved)
        End If

        out.Flush()
    End Sub
End Class
