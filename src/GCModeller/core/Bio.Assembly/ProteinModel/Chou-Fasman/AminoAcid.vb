#Region "Microsoft.VisualBasic::9e504899a885eb633249aad2448ee196, GCModeller\core\Bio.Assembly\ProteinModel\Chou-Fasman\AminoAcid.vb"

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
    '    Code Lines: 43
    ' Comment Lines: 12
    '   Blank Lines: 12
    '     File Size: 2.32 KB


    '     Class AminoAcid
    ' 
    '         Properties: [StructureType], AminoAcid, Coil, HelixSheetOverlap, HelixTurnOverlap
    '                     SheetTurnOverlap, StructureChar
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports SMRUCC.genomics.SequenceModel

Namespace ProteinModel.ChouFasmanRules

    Public Class AminoAcid

        Public ReadOnly Property AminoAcid As Polypeptides.AminoAcid

        Protected Friend _MaskAlphaHelix As Boolean
        Protected Friend _MaskBetaSheet_ As Boolean
        Protected Friend _MastBetaTurn__ As Boolean

        ''' <summary>
        ''' 当前的这个氨基酸分子所处的二级结构特征
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property [StructureType] As SecondaryStructures

        ''' <summary>
        ''' 使用一个字符用来表示的二级结构特征
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property StructureChar As Char
            Get
                Return StructureTypesToChar(Me.StructureType).First
            End Get
        End Property

        Sub New(aa As Polypeptides.AminoAcid)
            _AminoAcid = aa
        End Sub

        Protected Friend ReadOnly Property HelixSheetOverlap As Boolean
            Get
                Return _MaskAlphaHelix AndAlso _MaskBetaSheet_
            End Get
        End Property

        Protected Friend ReadOnly Property HelixTurnOverlap As Boolean
            Get
                Return _MaskAlphaHelix AndAlso _MastBetaTurn__
            End Get
        End Property

        Protected Friend ReadOnly Property SheetTurnOverlap As Boolean
            Get
                Return _MaskBetaSheet_ AndAlso _MastBetaTurn__
            End Get
        End Property

        Protected Friend ReadOnly Property Coil As Boolean
            Get
                Return (_MaskAlphaHelix AndAlso _MaskBetaSheet_ AndAlso _MastBetaTurn__) OrElse
                    (Not _MaskAlphaHelix AndAlso Not _MaskBetaSheet_ AndAlso Not _MastBetaTurn__)
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return String.Format("{0};  [alpha-helix:={1},  beta_sheet:={2},  beta_turn:={3}]", _AminoAcid.ToString, _MaskAlphaHelix, _MaskBetaSheet_, _MastBetaTurn__)
        End Function
    End Class
End Namespace
