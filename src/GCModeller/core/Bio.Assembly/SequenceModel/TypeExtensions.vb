#Region "Microsoft.VisualBasic::4e116e0a8e48bedc5a6ef8b404ccd8e9, core\Bio.Assembly\SequenceModel\TypeExtensions.vb"

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

    '   Total Lines: 117
    '    Code Lines: 61 (52.14%)
    ' Comment Lines: 43 (36.75%)
    '    - Xml Docs: 95.35%
    ' 
    '   Blank Lines: 13 (11.11%)
    '     File Size: 4.15 KB


    '     Module TypeExtensions
    ' 
    '         Properties: AA, NT, RNA
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: GetSeqType, GetVector, IsProteinSource, ParseSeqType
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.SequenceModel.Polypeptides

Namespace SequenceModel

    ''' <summary>
    ''' Extensions helper function for different type of bio-sequence.
    ''' </summary>
    <HideModuleName>
    Public Module TypeExtensions

        ''' <summary>
        ''' Enumeration for nucleotide residues
        ''' </summary>
        ''' <returns>[ATGC]</returns>
        Public ReadOnly Property NT As IReadOnlyCollection(Of Char) = {"A"c, "T"c, "G"c, "C"c}

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns>[AUGC]</returns>
        Public ReadOnly Property RNA As IReadOnlyCollection(Of Char) = {"A"c, "U"c, "G"c, "C"c}

        ''' <summary>
        ''' Enumeration for amino acid.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' X is unknown amino acid inside a sequence
        ''' </remarks>
        Public ReadOnly Property AA As IReadOnlyCollection(Of Char) = AminoAcidObjUtility.AminoAcidLetters _
            .Join({"X"c}) _
            .ToArray

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

        ReadOnly type_parser As New Dictionary(Of String, SeqTypes)

        Sub New()
            For Each type As SeqTypes In Enums(Of SeqTypes)()
                type_parser(type.ToString) = type
                type_parser(type.ToString.ToLower) = type
                type_parser(type.Description) = type
                type_parser(CInt(type).ToString) = type
            Next
        End Sub

        Public Function ParseSeqType(desc As String, Optional [default] As SeqTypes = SeqTypes.Generic) As SeqTypes
            If type_parser.ContainsKey(desc) Then
                Return type_parser(desc)
            Else
                Return [default]
            End If
        End Function

        ''' <summary>
        ''' Get composition vector by sequence type flag
        ''' </summary>
        ''' <param name="seq"></param>
        ''' <returns></returns>
        ''' 
        <Extension>
        Public Function GetVector(seq As SeqTypes) As IReadOnlyCollection(Of Char)
            Select Case seq
                Case SeqTypes.DNA : Return NT
                Case SeqTypes.RNA : Return RNA
                Case SeqTypes.Protein
                    Return AA
                Case Else
                    Return AA
            End Select
        End Function

        ''' <summary>
        ''' try to measure the sequence type based on the given sequence chars
        ''' </summary>
        ''' <param name="seq"></param>
        ''' <returns></returns>
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

        Public Function CheckSeqType(seq As String) As SeqTypes
            If seq.StringEmpty(, True) Then
                Return SeqTypes.Generic
            ElseIf IsProteinSource(seq) Then
                Return SeqTypes.Protein
            ElseIf seq.Any(Function(c) c = "U"c) Then
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
            Return IsProteinSource(seq.SequenceData)
        End Function

        ''' <summary>
        ''' 目标序列数据是否为一条蛋白质序列
        ''' </summary>
        ''' <param name="seq"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function IsProteinSource(seq As String) As Boolean
            Return seq _
                .ToUpper _
                .Where(Function(c) c <> "N"c AndAlso AA_CHARS_ALL.IndexOf(c) > -1) _
                .FirstOrDefault _
                .CharCode > 0
        End Function
    End Module
End Namespace
