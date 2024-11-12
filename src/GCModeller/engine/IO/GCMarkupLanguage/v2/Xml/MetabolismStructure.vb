﻿#Region "Microsoft.VisualBasic::3076da6bfa5e44f336e3375a9979f1c3, engine\IO\GCMarkupLanguage\v2\Xml\MetabolismStructure.vb"

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

'   Total Lines: 254
'    Code Lines: 138 (54.33%)
' Comment Lines: 69 (27.17%)
'    - Xml Docs: 95.65%
' 
'   Blank Lines: 47 (18.50%)
'     File Size: 8.60 KB


'     Class MetabolismStructure
' 
'         Properties: compounds, enzymes, maps, reactions
' 
'         Function: GetAllFluxID
' 
'     Class ReactionGroup
' 
'         Properties: enzymatic, etc, size
' 
'         Function: GenericEnumerator
' 
'     Class Compound
' 
'         Properties: ID, mass0, name
' 
'         Function: ToString
' 
'     Class Reaction
' 
'         Properties: bounds, Equation, ID, is_enzymatic, name
' 
'         Function: ToString
' 
'     Class FunctionalCategory
' 
'         Properties: category, pathways
' 
'         Function: ToString
' 
'     Class Pathway
' 
'         Properties: enzymes, ID, name
' 
'         Function: ToString
' 
'     Class Enzyme
' 
'         Properties: catalysis, ECNumber, geneID, KO
' 
'         Function: ToString
' 
'     Class Catalysis
' 
'         Properties: formula, parameter, PH, reaction, temperature
' 
'         Function: ToString
' 
'     Class KineticsParameter
' 
'         Properties: isModifier, name, target, value
' 
'         Function: ToString
' 
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.Scripting.MathExpression
Imports Microsoft.VisualBasic.Text.Xml.Models

Namespace v2

    ''' <summary>
    ''' model of the metabolic network
    ''' </summary>
    <XmlType("metabolome", [Namespace]:=VirtualCell.GCMarkupLanguage)>
    Public Class MetabolismStructure

        <XmlArray("compounds")> Public Property compounds As Compound()
        ''' <summary>
        ''' 在这个属性之中包含有所有的代谢反应过程的定义
        ''' </summary>
        ''' <returns></returns>
        <XmlElement("reactions")> Public Property reactions As ReactionGroup

        ''' <summary>
        ''' 在这个属性里面只会出现具有KO分类编号的蛋白序列，如果需要找所有基因的数据，可以
        ''' 读取<see cref="Genome.replicons"/>里面的基因的数据
        ''' </summary>
        ''' <returns></returns>
        <XmlArray("enzymes")> Public Property enzymes As Enzyme()

        ''' <summary>
        ''' the pathway function category groups of the enzymes
        ''' </summary>
        ''' <returns></returns>
        <XmlArray("pathwayMaps")>
        Public Property maps As FunctionalCategory()

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function GetAllFluxID() As String()
            Return reactions _
                .SafeQuery _
                .Select(Function(r) r.ID) _
                .ToArray
        End Function
    End Class

    ''' <summary>
    ''' the reaction collection
    ''' </summary>
    Public Class ReactionGroup : Implements IList(Of Reaction)

        <XmlAttribute>
        Public Property size As Integer Implements IList(Of Reaction).size
            Get
                Return enzymatic.Length + etc.Length
            End Get
            Set(value As Integer)
                ' do nothing
            End Set
        End Property

        ''' <summary>
        ''' enzymatic reactions
        ''' </summary>
        ''' <returns></returns>
        Public Property enzymatic As Reaction()
        ''' <summary>
        ''' non-enzymatic reactions
        ''' </summary>
        ''' <returns></returns>
        Public Property etc As Reaction()

        Public Iterator Function GenericEnumerator() As IEnumerator(Of Reaction) Implements Enumeration(Of Reaction).GenericEnumerator
            For Each reaction In enzymatic
                Yield reaction
            Next
            For Each reaction In etc
                Yield reaction
            Next
        End Function

        Public Shared Widening Operator CType(reactions As Reaction()) As ReactionGroup
            Dim twoGroup = reactions _
                .GroupBy(Function(r) r.is_enzymatic) _
                .ToDictionary(Function(g) g.Key.ToString,
                              Function(g)
                                  Return g.ToArray
                              End Function)

            Return New ReactionGroup With {
                .enzymatic = twoGroup.TryGetValue(True.ToString, [default]:={}),
                .etc = twoGroup.TryGetValue(False.ToString, [default]:={})
            }
        End Operator
    End Class

    <XmlType("compound", [Namespace]:=VirtualCell.GCMarkupLanguage)>
    Public Class Compound : Implements INamedValue

        <XmlAttribute>
        Public Property ID As String Implements IKeyedEntity(Of String).Key

        <XmlText> Public Property name As String

        ''' <summary>
        ''' the initialize mass content of current compound.
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property mass0 As Double = 1000

        Public Overrides Function ToString() As String
            Return $"[{ID}] {name}"
        End Function

    End Class

    ''' <summary>
    ''' the reaction graph model
    ''' </summary>
    <XmlType("reaction", [Namespace]:=VirtualCell.GCMarkupLanguage)>
    Public Class Reaction : Implements INamedValue

        ''' <summary>
        ''' unique reference id of current reaction link
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property ID As String Implements IKeyedEntity(Of String).Key
        <XmlAttribute> Public Property name As String
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

        Public Property substrate As CompoundFactor()
        Public Property product As CompoundFactor()

        Public ReadOnly Property equation As String
            Get
                Return substrate.Select(Function(a) a.factorString).JoinBy(" + ") &
                    " <=> " &
                    product.Select(Function(a) a.factorString).JoinBy(" + ")
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return $"({ID}: {name}) {equation}"
        End Function

    End Class

    Public Class CompoundFactor

        <XmlAttribute>
        Public Property factor As Double
        <XmlText>
        Public Property compound As String

        Sub New()
        End Sub

        Sub New(factor As Double, compound As String)
            Me.factor = factor
            Me.compound = compound
        End Sub

        Public Overrides Function ToString() As String
            Return compound
        End Function

        Friend Function factorString() As String
            If factor <= 1 Then
                Return compound
            Else
                Return factor & " " & compound
            End If
        End Function

    End Class

    Public Class FunctionalCategory

        <XmlAttribute>
        Public Property category As String
        <XmlElement("pathway")>
        Public Property pathways As Pathway()

        Public Overrides Function ToString() As String
            Return category
        End Function

    End Class

    <XmlType("pathway", [Namespace]:=VirtualCell.GCMarkupLanguage)>
    Public Class Pathway : Implements INamedValue

        <XmlAttribute> Public Property ID As String Implements IKeyedEntity(Of String).Key
        <XmlAttribute> Public Property name As String

        ''' <summary>
        ''' 属性的值含义如下：
        ''' 
        ''' + <see cref="[Property].name"/>: protein_id
        ''' + <see cref="[Property].value"/>: KO number
        ''' + <see cref="[Property].Comment"/>: gene locus_tag
        ''' </summary>
        ''' <returns></returns>
        <XmlElement("enzyme")>
        Public Property enzymes As [Property]()

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overrides Function ToString() As String
            Return $"[{ID}] {name} with {enzymes.Length} enzymes"
        End Function

    End Class

    ''' <summary>
    ''' 酶分子对象
    ''' </summary>
    <XmlType("enzyme", [Namespace]:=VirtualCell.GCMarkupLanguage)>
    Public Class Enzyme : Implements INamedValue

        <XmlAttribute> Public Property geneID As String Implements IKeyedEntity(Of String).Key
        <XmlAttribute> Public Property KO As String
        <XmlAttribute> Public Property ECNumber As String

        ''' <summary>
        ''' 酶分子所催化的反应列表
        ''' </summary>
        ''' <returns></returns>
        <XmlElement>
        Public Property catalysis As Catalysis()

        Public Overrides Function ToString() As String
            Return geneID
        End Function

    End Class

    ''' <summary>
    ''' 酶分子对目标反应过程的催化动力学函数
    ''' </summary>
    <XmlType("catalysis", [Namespace]:=VirtualCell.GCMarkupLanguage)>
    Public Class Catalysis

        ''' <summary>
        ''' The reaction id
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property reaction As String
        <XmlAttribute> Public Property PH As Double
        ''' <summary>
        ''' 单位为摄氏度的温度参数值
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property temperature As Double

        ''' <summary>
        ''' 通过sabio-rk数据库得到的动力学函数方程
        ''' </summary>
        ''' <returns></returns>
        Public Property formula As FunctionElement

        ''' <summary>
        ''' 动力学方程的参数列表
        ''' </summary>
        ''' <returns></returns>
        ''' 
        <XmlElement>
        Public Property parameter As KineticsParameter()

        Sub New()
        End Sub

        Sub New(id As String)
            reaction = id
        End Sub

        Public Overrides Function ToString() As String
            If formula Is Nothing Then
                Return "null"
            Else
                Return formula.lambda
            End If
        End Function

    End Class

    Public Class KineticsParameter

        <XmlAttribute> Public Property name As String
        ''' <summary>
        ''' kegg compound id
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property target As String
        <XmlAttribute> Public Property isModifier As Boolean

        <XmlText>
        Public Property value As Double

        Public Overrides Function ToString() As String
            If Not target.StringEmpty Then
                Return $"Dim {name} As {target} = {value}"
            Else
                Return $"Dim {name} = {value}"
            End If
        End Function

    End Class
End Namespace
