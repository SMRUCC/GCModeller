#Region "Microsoft.VisualBasic::b060967f0a2b4c92a027d1aabeb71951, Bio.Assembly\SequenceModel\TypeExtensions.vb"

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

    '     Module TypeExtensions
    ' 
    '         Properties: AA, NT
    ' 
    '         Function: GetSeqType, IsProteinSource
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices

Namespace SequenceModel

    ''' <summary>
    ''' Extensions helper function for different type of bio-sequence.
    ''' </summary>
    <HideModuleName>
    Public Module TypeExtensions

        ''' <summary>
        ''' Enumeration for nucleotide residues
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property NT As IReadOnlyCollection(Of Char) = {"A"c, "T"c, "G"c, "C"c}
        ''' <summary>
        ''' Enumeration for amino acid.
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property AA As IReadOnlyCollection(Of Char) = {"A"c, "R"c, "N"c, "D"c, "C"c, "E"c, "Q"c, "G"c, "H"c, "I"c, "L"c, "K"c, "M"c, "F"c, "P"c, "S"c, "T"c, "W"c, "Y"c, "V"c}

#Region "Constants"

        ''' <summary>
        ''' Enumerate all of the amino acid.(字符串常量枚举所有的氨基酸分子)
        ''' </summary>
        ''' <remarks></remarks>
        Public Const AA_CHARS_ALL As String = "BDEFHIJKLMNOPQRSVWXYZ"
        ''' <summary>
        ''' Enumerate all of the nucleotides.(字符串常量枚举所有的核苷酸分子类型)
        ''' </summary>
        ''' <remarks></remarks>
        Public Const NA_CHARS_ALL As String = "ATGCU"
#End Region

        <Extension>
        Public Function GetSeqType(seq As IPolymerSequenceModel) As SeqTypes
            If IsProteinSource(seq) Then
                Return SeqTypes.Protein
            ElseIf seq.SequenceData.Any(Function(c) c = "U"c) Then
                Return SeqTypes.RNA
            Else
                Return SeqTypes.DNA
            End If
        End Function

        ''' <summary>
        ''' 目标序列数据是否为一条蛋白质序列
        ''' </summary>
        ''' <param name="seq"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function IsProteinSource(seq As IPolymerSequenceModel) As Boolean
            Return seq.SequenceData _
                .ToUpper _
                .Where(Function(c) c <> "N"c AndAlso AA_CHARS_ALL.IndexOf(c) > -1) _
                .FirstOrDefault _
                .CharCode > 0
        End Function
    End Module
End Namespace
