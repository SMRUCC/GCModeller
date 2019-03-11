#Region "Microsoft.VisualBasic::19faf5d4e49919596b99d45aea583b83, Bio.Assembly\SequenceModel\ISequenceModel.vb"

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

    '     Class ISequenceModel
    ' 
    '         Properties: IsProtSource, Length, SequenceData
    ' 
    '         Function: GetCompositionVector, IsProteinSource
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language

Namespace SequenceModel

    ''' <summary>
    ''' The biological sequence molecular model.(蛋白质序列，核酸序列都可以使用本对象来表示)
    ''' </summary>
    ''' <remarks></remarks>
    Public MustInherit Class ISequenceModel : Inherits BaseClass
        Implements IPolymerSequenceModel

#Region "Object properties"

        ''' <summary>
        ''' Sequence data in a string type.(字符串类型的序列数据)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overridable Property SequenceData As String Implements IPolymerSequenceModel.SequenceData

        ''' <summary>
        ''' This sequence is a protein type sequence?(判断这条序列是否为蛋白质序列)
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property IsProtSource As Boolean
            Get
                Return IsProteinSource(Me)
            End Get
        End Property

        ''' <summary>
        ''' The <see cref="SequenceData"/> string length.
        ''' </summary>
        ''' <returns></returns>
        Public Overridable ReadOnly Property Length As Integer
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return Len(SequenceData)
            End Get
        End Property
#End Region

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

#Region "Common shared functions"

        ''' <summary>
        ''' Get the composition vector for a sequence model using a specific composition description.
        ''' </summary>
        ''' <param name="seq"></param>
        ''' <param name="compositions">This always should be the constant string of <see cref="ISequenceModel.AA_CHARS_ALL">amino acid</see>
        ''' or <see cref="ISequenceModel.NA_CHARS_ALL">nucleotide</see>.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetCompositionVector(seq As IPolymerSequenceModel, compositions As Char()) As Integer()
            Dim composition%() = New Integer(compositions.Length - 1) {}

            With seq.SequenceData.ToUpper()
                For i As Integer = 0 To compositions.Length - 1
                    composition(i) = .Count(compositions(i))
                Next
            End With

            Return composition
        End Function

        ''' <summary>
        ''' 目标序列数据是否为一条蛋白质序列
        ''' </summary>
        ''' <param name="seq"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function IsProteinSource(seq As IPolymerSequenceModel) As Boolean
            Return seq.SequenceData _
                .ToUpper _
                .Where(Function(c) c <> "N"c AndAlso AA_CHARS_ALL.IndexOf(c) > -1) _
                .FirstOrDefault _
                .CharCode > 0
        End Function
#End Region
    End Class
End Namespace
