#Region "Microsoft.VisualBasic::400c9db3bc7457ddf6bc65dc3e6b217f, core\Bio.Assembly\SequenceModel\ISequenceModel.vb"

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

    '   Total Lines: 96
    '    Code Lines: 42 (43.75%)
    ' Comment Lines: 44 (45.83%)
    '    - Xml Docs: 95.45%
    ' 
    '   Blank Lines: 10 (10.42%)
    '     File Size: 3.89 KB


    '     Class ISequenceModel
    ' 
    '         Properties: IsProtSource, Length, SequenceData
    ' 
    '         Function: (+2 Overloads) GetCompositionVector
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

Namespace SequenceModel

    ''' <summary>
    ''' The biological sequence molecular model.
    ''' </summary>
    ''' <remarks>(蛋白质序列，核酸序列都可以使用本对象来表示)</remarks>
    Public MustInherit Class ISequenceModel
        Implements IPolymerSequenceModel
        Implements ISequenceData(Of Char, String)

#Region "Object properties"

        ''' <summary>
        ''' Sequence data in a string type.(字符串类型的序列数据)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overridable Property SequenceData As String Implements IPolymerSequenceModel.SequenceData,
            ISequenceData(Of Char, String).SequenceData

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

        ''' <summary>
        ''' Get the composition vector for a sequence model using a specific composition description.
        ''' </summary>
        ''' <param name="seq"></param>
        ''' <param name="compositions">
        ''' This always should be the constant string of <see cref="TypeExtensions.AA_CHARS_ALL">amino acid
        ''' </see>
        ''' or <see cref="TypeExtensions.NA_CHARS_ALL">nucleotide</see>.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function GetCompositionVector(seq As IPolymerSequenceModel, compositions As IReadOnlyCollection(Of Char)) As Integer()
            Return GetCompositionVector(seq.SequenceData, compositions)
        End Function

        ''' <summary>
        ''' Get the composition vector for a sequence model using a specific composition description.
        ''' </summary>
        ''' <param name="seq"></param>
        ''' <param name="compositions">
        ''' This always should be the constant string of <see cref="TypeExtensions.AA_CHARS_ALL">amino acid
        ''' </see>
        ''' or <see cref="TypeExtensions.NA_CHARS_ALL">nucleotide</see>.</param>
        ''' <returns>
        ''' the generated vector size is always equals to the char set in the <paramref name="compositions"/>
        ''' missing value from the input char set will be set to ZERO.
        ''' </returns>
        ''' <remarks></remarks>
        Public Shared Function GetCompositionVector(seq As String, compositions As IReadOnlyCollection(Of Char)) As Integer()
            Dim nsize As Integer = compositions.Count
            Dim composition%() = New Integer(nsize - 1) {}
            Dim i As Integer = Scan0

            If Not (seq Is Nothing OrElse seq = "") Then
                With seq.ToUpper()
                    For Each c As Char In compositions
                        composition(i) = .Count(c)
                        i += 1
                    Next
                End With
            Else
                ' sequence is empty
            End If

            Return composition
        End Function
    End Class
End Namespace
