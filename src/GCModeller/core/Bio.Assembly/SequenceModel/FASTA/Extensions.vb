﻿#Region "Microsoft.VisualBasic::788ab58e4d570be079a34dd23a672164, core\Bio.Assembly\SequenceModel\FASTA\Extensions.vb"

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

'   Total Lines: 32
'    Code Lines: 21 (65.62%)
' Comment Lines: 6 (18.75%)
'    - Xml Docs: 100.00%
' 
'   Blank Lines: 5 (15.62%)
'     File Size: 1.04 KB


'     Module Extensions
' 
'         Function: Index
' 
' 
' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq

Namespace SequenceModel.FASTA

    Public Module Extensions

        ''' <summary>
        ''' 函数返回-1表示找不到
        ''' </summary>
        ''' <param name="fasta"></param>
        ''' <param name="idx$">对index的描述，可以是title也可以直接是index数字</param>
        ''' <returns></returns>
        <Extension>
        Public Function Index(fasta As FastaFile, idx As [Variant](Of Integer, String)) As Integer
            If idx Like GetType(String) Then
                Dim title As String = idx

                For Each seq As SeqValue(Of FastaSeq) In fasta.SeqIterator
                    If title.TextEquals(seq.value.Title) Then
                        Return seq.i
                    End If
                Next

                Return -1
            Else
                Return idx
            End If
        End Function

        ''' <summary>
        ''' Writes the sequences in FASTA format to a file with the given name </summary>
        ''' <param name="sequences"> </param>
        ''' <param name="filename"> </param>
        ''' 
        <Extension>
        Public Sub writeFasta(sequences As IEnumerable(Of String), filename As String,
                              Optional break As Integer = 80,
                              Optional prefix As String = "sequence")

            Using printWriter As New IO.StreamWriter(filename.Open(FileMode.OpenOrCreate, doClear:=True))
                Dim i As Integer = 0

                For Each sequence As String In sequences
                    Dim stringBuilder As New StringBuilder(sequence)
                    i += 1

                    Call Enumerable _
                        .Range(0, sequence.Length / break) _
                        .ForEach(Sub(j, o)
                                     Call stringBuilder.Insert(j + break * (j + 1), vbLf)
                                 End Sub)

                    stringBuilder.Insert(0, String.Format("> {0}{1}" & vbLf, prefix, i))
                    printWriter.WriteLine(stringBuilder.ToString())
                Next

                Call printWriter.Flush()
            End Using
        End Sub
    End Module
End Namespace
