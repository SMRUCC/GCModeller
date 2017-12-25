#Region "Microsoft.VisualBasic::8f5b2a2d8df05af2788034f017d73c82, ..\GCModeller\core\Bio.Assembly\SequenceModel\FASTA\Extensions.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
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
            For Each seq As SeqValue(Of FastaToken) In fasta.SeqIterator
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
