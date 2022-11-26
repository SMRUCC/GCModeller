#Region "Microsoft.VisualBasic::b89f9e625ef16ebe6b3a460acee32a2d, GCModeller\core\Bio.Assembly\Assembly\KEGG\DBGET\Objects\Pathway\Reaction\Reaction.vb"

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

    '   Total Lines: 171
    '    Code Lines: 96
    ' Comment Lines: 54
    '   Blank Lines: 21
    '     File Size: 6.04 KB


    '     Class Reaction
    ' 
    '         Properties: [Class], [Module], Comments, CommonNames, DBLink
    '                     Definition, Enzyme, Equation, ID, Orthology
    '                     Pathway, ReactionModel, Reversible
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: GetSubstrateCompounds, IsConnectWith, LoadXml, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.ComponentModel.DBLinkBuilder
Imports SMRUCC.genomics.ComponentModel.EquaionModel
Imports r = System.Text.RegularExpressions.Regex

Namespace Assembly.KEGG.DBGET.bGetObject

    ''' <summary>
    ''' KEGG reaction annotation data.
    ''' </summary>
    ''' <remarks></remarks>
    <XmlRoot("reaction", [Namespace]:=Reaction.Xmlns)>
    Public Class Reaction : Inherits XmlDataModel
        Implements INamedValue

        Public Const Xmlns$ = "http://GCModeller.org/core/KEGG/DBGET/Reaction.xsd"

        ''' <summary>
        ''' 代谢反应的KEGG编号，格式为``R\d+``，同时这个属性也是<see cref="INamedValue.Key"/>
        ''' </summary>
        ''' <returns></returns>
        <XmlElement("ID")>
        Public Property ID As String Implements INamedValue.Key
        <XmlElement("commonNames")>
        Public Property CommonNames As String()
        <XmlElement("def")>
        Public Property Definition As String

        ''' <summary>
        ''' 使用KEGG compound编号作为代谢物的反应过程的表达式
        ''' </summary>
        ''' <returns></returns>
        ''' 
        <XmlElement("equation")>
        Public Property Equation As String

        ''' <summary>
        ''' 标号： <see cref="Expasy.Database.Enzyme.Identification"></see>
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' 
        <XmlArray("enzymes")>
        Public Property Enzyme As String()
        <XmlElement("comment")>
        Public Property Comments As String
        <XmlArray("pathway")>
        Public Property Pathway As NamedValue()
        <XmlArray("module")>
        Public Property [Module] As NamedValue()

        ''' <summary>
        ''' KO list
        ''' </summary>
        ''' <returns></returns>
        <XmlElement("orthology")>
        Public Property Orthology As OrthologyTerms

        ''' <summary>
        ''' The reaction class
        ''' </summary>
        ''' <returns></returns>
        ''' 
        <XmlArray("class")>
        Public Property [Class] As NamedValue()
        <XmlElement>
        Public Property DBLink As DBLink()

        ''' <summary>
        ''' + (...)
        ''' + m
        ''' + n
        ''' + [nm]-1
        ''' + [nm]+1
        ''' </summary>
        Const polymers$ = "(\(.+?\))|([nm](\s*[+-]\s*[0-9mn]+)? )"

        <XmlNamespaceDeclarations()>
        Public xmlnsImports As XmlSerializerNamespaces

        Public Sub New()
            xmlnsImports = New XmlSerializerNamespaces
            xmlnsImports.Add("KO", OrthologyTerms.Xmlns)
        End Sub

        ''' <summary>
        ''' 从<see cref="Equation"/>属性值字符串创建一个代谢过程的模型
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property ReactionModel As DefaultTypes.Equation
            Get
                Dim rxnStr As String = r.Replace(Equation, polymers, "", RegexICSng)

                Try
                    Dim eq = EquationBuilder.CreateObject(Of DefaultTypes.CompoundSpecieReference, DefaultTypes.Equation)(rxnStr)
                    eq.Id = ID
                    Return eq
                Catch ex As Exception
                    ex = New Exception(rxnStr, ex)
                    Throw ex
                End Try
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return String.Format("[{0}] {1}:  {2}", Enzyme.JoinBy("|"), ID, Definition)
        End Function

        ''' <summary>
        ''' 这个反应过程是否是可逆的代谢反应？
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Reversible As Boolean
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return InStr(Equation, " <=> ") > 0
            End Get
        End Property

        ''' <summary>
        ''' 得到本反应过程对象中的所有的代谢底物的KEGG编号，以便于查询和下载
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetSubstrateCompounds() As String()
            Dim fluxModel = Me.ReactionModel
            Dim allCompounds$() = LinqAPI.Exec(Of String) _
 _
                () <= From csr As DefaultTypes.CompoundSpecieReference
                      In fluxModel.Reactants.AsList + fluxModel.Products
                      Select csr.ID
                      Distinct

            Return allCompounds
        End Function

        ''' <summary>
        ''' 通过查看化合物的编号是否有交集来判断这两个代谢过程是否是应该相连的？
        ''' </summary>
        ''' <param name="[next]"></param>
        ''' <returns></returns>
        Public Function IsConnectWith([next] As Reaction) As Boolean
            Dim a = GetSubstrateCompounds(),
                b = [next].GetSubstrateCompounds

            For Each s As String In a
                If Array.IndexOf(b, s) > -1 Then
                    Return True
                End If
            Next

            Return False
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function LoadXml(handle As String) As Reaction
            Return handle.LoadXml(Of Reaction)(
                preprocess:=Function(text)
                                Return text.Replace("&#x8;", "")
                            End Function
            )
        End Function
    End Class
End Namespace
