#Region "Microsoft.VisualBasic::6ab20a029ac976f53348fa92421bafa0, engine\IO\GCMarkupLanguage\v2\Xml\Metabolism\Reaction.vb"

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

'   Total Lines: 219
'    Code Lines: 144 (65.75%)
' Comment Lines: 43 (19.63%)
'    - Xml Docs: 97.67%
' 
'   Blank Lines: 32 (14.61%)
'     File Size: 8.07 KB


'     Class Transportation
' 
'         Properties: enzymes, guid, membrane
' 
'         Function: ToString
' 
'     Class ReactionGroup
' 
'         Properties: enzymatic, none_enzymatic, size, transportation
' 
'         Constructor: (+2 Overloads) Sub New
'         Function: CompoundLinks, GenericEnumerator
' 
'     Class Reaction
' 
'         Properties: baseline, bounds, compartment, ec_number, equation
'                     ID, is_enzymatic, name, note, product
'                     substrate
' 
'         Function: CheckTransportation, GenericEnumerator, ToString
' 
'     Class CompoundFactor
' 
'         Properties: cid, compartment, compound, factor
' 
'         Constructor: (+3 Overloads) Sub New
'         Function: factorString, ToString
' 
' 
' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.ComponentModel.EquaionModel
Imports SMRUCC.genomics.ComponentModel.EquaionModel.DefaultTypes
Imports SMRUCC.genomics.MetabolicModel

Namespace v2

    ''' <summary>
    ''' 反应的热力学方向。
    ''' </summary>
    Public Enum ReactionDirection
        ''' <summary>
        ''' 方向未知：需要根据反应的速度边界进行推断
        ''' </summary>
        unknown
        ''' <summary>
        ''' 只能从左向右进行
        ''' </summary>
        leftToRight
        ''' <summary>
        ''' 只能从右向左进行
        ''' </summary>
        rightToLeft
        ''' <summary>
        ''' 正反两个方向都可以进行
        ''' </summary>
        reversible
    End Enum

    ''' <summary>
    ''' the reaction graph model
    ''' </summary>
    <XmlType("reaction", [Namespace]:=VirtualCell.GCMarkupLanguage)>
    Public Class Reaction : Implements INamedValue, Enumeration(Of CompoundFactor)

        ''' <summary>
        ''' unique reference id of current reaction link
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property ID As String Implements IKeyedEntity(Of String).Key
        <XmlElement> Public Property name As String
        <XmlElement> Public Property note As String
        ''' <summary>
        ''' 这个反应模型是否是需要酶促才会发生了生化反应过程？
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property is_enzymatic As Boolean
        ''' <summary>
        ''' [forward, reverse] boundary of the reaction speed
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property bounds As Double()
        <XmlAttribute> Public Property baseline As Double()

        <XmlElement> Public Property ec_number As String()
        ''' <summary>
        ''' the compartment location of the reaction
        ''' </summary>
        ''' <returns></returns>
        <XmlElement> Public Property compartment As String()
        <XmlElement> Public Property substrate As CompoundFactor()
        <XmlElement> Public Property product As CompoundFactor()

        <XmlElement> Public Property gibbs As Double

        ''' <summary>
        ''' 反应的热力学方向。
        ''' 
        ''' 在旧版本的数据结构之中需要通过 ``bounds`` 来进行猜测，在新版本之中直接使用
        ''' 这个显式的方向标志来表示反应在生理条件之下的实际方向。
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property direction As ReactionDirection

        ''' <summary>
        ''' 当前反应是否是自发反应(不需要酶的催化就可以发生)
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property spontaneous As Boolean

        ''' <summary>
        ''' 标准自由能变化(kJ/mol)
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property deltaG As Double

        ''' <summary>
        ''' 标准自由能变化的计算误差(kJ/mol)
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property deltaGError As Double

        ''' <summary>
        ''' 反应的平衡常数
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property equilibriumConstant As Double

        ''' <summary>
        ''' 反应的标准还原电位
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property stdReductionPotential As Double

        ''' <summary>
        ''' 反应前后的原子映射关系
        ''' </summary>
        ''' <returns></returns>
        <XmlElement("atomMapping")> Public Property atomMappings As String()

        ''' <summary>
        ''' 当前反应所属于的代谢通路的编号列表
        ''' </summary>
        ''' <returns></returns>
        <XmlElement("pathway")> Public Property pathways As String()

        ''' <summary>
        ''' 催化当前反应的酶的蛋白质编号列表
        ''' </summary>
        ''' <returns></returns>
        <XmlElement("enzyme")> Public Property enzymes As String()

        ''' <summary>
        ''' 当前反应所关联的文献证据编号
        ''' </summary>
        ''' <returns></returns>
        <XmlElement("evidence")> Public Property evidence As String()

        ''' <summary>
        ''' the debug view of the current equation model
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property equation As String
            Get
                Dim left As String = substrate.Select(Function(a) a.factorString).JoinBy(" + ")
                Dim right As String = product.Select(Function(a) a.factorString).JoinBy(" + ")
                Dim arrow As String = If(CheckReversible(),
                    EquationBuilder.EQUATION_DIRECTIONS_REVERSIBLE,
                    EquationBuilder.EQUATION_DIRECTIONS_INREVERSIBLE)

                Return left & " " & arrow & " " & right
            End Get
        End Property

        Public Function CheckTransportation() As Boolean
            Return substrate.Any(Function(s) product.Any(Function(a) a.compound = s.compound))
        End Function

        Public Function CheckReversible() As Boolean
            Select Case direction
                Case ReactionDirection.reversible
                    Return True
                Case ReactionDirection.leftToRight, ReactionDirection.rightToLeft
                    Return False
            End Select

            If bounds.IsNullOrEmpty OrElse bounds.Length < 2 Then
                ' 没有给出反应的速度边界的时候，默认当前反应是可逆的
                Return True
            End If

            Dim lb As Double = bounds(0)
            Dim rb As Double = bounds(1)

            ' 只有在正向速度和逆向速度都允许的时候，反应才是可逆的：
            ' lb < 0 < rb
            Return lb < 0 AndAlso rb > 0
        End Function

        Public Overrides Function ToString() As String
            Return $"({ID}: {name}) {equation}"
        End Function

        Public Function BuildModel() As MetabolicReaction
            Dim left = substrate.Select(Function(c) New CompoundSpecieReference(c, c.compartment)).ToArray
            Dim right = product.Select(Function(c) New CompoundSpecieReference(c, c.compartment)).ToArray

            Return New MetabolicReaction With {
                .description = note,
                .ECNumbers = ec_number,
                .gibbs = gibbs,
                .id = ID,
                .is_reversible = CheckReversible(),
                .is_spontaneous = spontaneous OrElse (ec_number.IsNullOrEmpty AndAlso enzymes.IsNullOrEmpty),
                .name = name,
                .left = left,
                .right = right
            }
        End Function

        Public Iterator Function GenericEnumerator() As IEnumerator(Of CompoundFactor) Implements Enumeration(Of CompoundFactor).GenericEnumerator
            For Each c As CompoundFactor In substrate.SafeQuery
                Yield c
            Next
            For Each c As CompoundFactor In product.SafeQuery
                Yield c
            Next
        End Function
    End Class

End Namespace
