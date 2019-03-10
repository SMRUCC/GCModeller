#Region "Microsoft.VisualBasic::dd5a79202309a4e434ef6bc4b207a848, Bio.Assembly\SequenceModel\FASTA\Extensions.vb"

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

    '     Module Extensions
    ' 
    '         Function: Index
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
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
        Public Function Index(fasta As FastaFile, idx$) As Integer
            For Each seq As SeqValue(Of FastaSeq) In fasta.SeqIterator
                If idx.TextEquals((+seq).Title) Then
                    Return seq.i
                End If
            Next

            If idx.IsNumeric Then
                Return CInt(Val(idx))
            Else
                Return -1
            End If
        End Function
    End Module
End Namespace
