#Region "Microsoft.VisualBasic::57f2e9eb437171566473b533d9a7cc9a, core\Bio.Assembly\SequenceModel\KSeqCartesianProduct.vb"

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

    '   Total Lines: 50
    '    Code Lines: 35 (70.00%)
    ' Comment Lines: 8 (16.00%)
    '    - Xml Docs: 87.50%
    ' 
    '   Blank Lines: 7 (14.00%)
    '     File Size: 1.76 KB


    '     Class KSeqCartesianProduct
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: KmerSeeds
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.base
Imports SMRUCC.genomics.SequenceModel.Polypeptides

Namespace SequenceModel

    ''' <summary>
    ''' kmer sequence generator
    ''' </summary>
    Public Class KSeqCartesianProduct

        ReadOnly alphabet As Char()

        Sub New(seqtype As SeqTypes)
            Select Case seqtype
                Case SeqTypes.DNA : alphabet = {"A", "T", "G", "C"}
                Case SeqTypes.RNA : alphabet = {"A", "U", "G", "C"}
                Case SeqTypes.Protein
                    alphabet = Polypeptide.ToChar.Values.ToArray
                Case Else
                    Throw New InvalidDataException("unknown sequence data type!")
            End Select
        End Sub

        ''' <summary>
        ''' 根据序列类型和长度生成所有可能的排列组合
        ''' </summary>
        ''' <returns>包含所有排列组合结果的列表</returns>
        Public Iterator Function KmerSeeds(k As Integer) As IEnumerable(Of String)
            If k <= 0 Then
                Return
            ElseIf k = 1 Then
                For Each c As Char In alphabet
                    Yield CStr(c)
                Next
            Else
                Dim nddata As Char()() = New Char(k - 1)() {}

                For i As Integer = 0 To k - 1
                    nddata(i) = alphabet.ToArray
                Next

                ' create kmers via N-dimension cartesian product
                For Each chs As Char() In NDimensionCartesianProduct.CreateMultiCartesianProduct(Of Char)(nddata)
                    Yield New String(chs)
                Next
            End If
        End Function
    End Class
End Namespace
