#Region "Microsoft.VisualBasic::7f4fafbf372cb0e788329d0f53bd55db, GCModeller\analysis\SequenceToolkit\gwANI\gwANIPrinter.vb"

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

    '   Total Lines: 49
    '    Code Lines: 36
    ' Comment Lines: 3
    '   Blank Lines: 10
    '     File Size: 1.54 KB


    ' Module gwANIPrinter
    ' 
    '     Sub: print, print_header, print_matrix
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.csv.IO

Public Module gwANIPrinter

    <Extension>
    Public Sub print(dataset As DataSet(), out As TextWriter)
        Dim sequence_names$() = dataset.Keys

        Call print_header(out, sequence_names)
        Call print_matrix(out, dataset, sequence_names)
    End Sub

    ''' <summary>
    ''' 打印出列标题
    ''' </summary>
    Private Sub print_header(out As TextWriter, sequence_names$())
        Dim number_of_samples% = sequence_names.Length

        For i As Integer = 0 To number_of_samples - 1
            Call out.Write(vbTab & "{0}", sequence_names(i))
        Next

        Call out.WriteLine()
    End Sub

    Private Sub print_matrix(out As TextWriter, dataset As DataSet(), sequence_names$())
        Dim number_of_samples% = sequence_names.Length
        Dim similarity_percentage As Double()
        Dim id$

        For i As Integer = 0 To number_of_samples - 1
            similarity_percentage = dataset(i)(sequence_names)
            id = sequence_names(i)

            For j As Integer = 0 To number_of_samples - 1
                If similarity_percentage(j) < 0 Then
                    out.Write(vbTab & "-")
                Else
                    out.Write(vbTab & "{0:f}", similarity_percentage(j))
                End If
            Next

            out.Write(vbLf)
        Next
    End Sub
End Module
