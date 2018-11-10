#Region "Microsoft.VisualBasic::da08378d060808d406c47d77d3ac63fb, IO\GCMarkupLanguage\MetabolismStructure.vb"

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

'     Class VirtualCell
' 
'         Properties: MetabolismStructure, Taxonomy
' 
'         Constructor: (+1 Overloads) Sub New
'         Function: ToString
' 
'     Class MetabolismStructure
' 
'         Properties: Compounds, Pathways, Reactions
' 
'     Class Compound
' 
'         Properties: ID, name, otherNames
' 
'     Class Reaction
' 
'         Properties: Equation, ID, name
' 
'     Class Pathway
' 
'         Properties: Enzymes, ID, name
' 
'     Class Enzyme
' 
'         Properties: catalysis, geneID, KO
' 
'     Class Catalysis
' 
'         Properties: coefficient, comment, Reaction
' 
' 
' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.Metagenomics

Namespace v2

    ''' <summary>
    ''' 虚拟细胞数据模型
    ''' </summary>
    <XmlRoot(NameOf(VirtualCell), [Namespace]:=VirtualCell.GCMarkupLanguage)>
    Public Class VirtualCell : Inherits XmlDataModel

        ''' <summary>
        ''' 物种注释信息
        ''' </summary>
        ''' <returns></returns>
        Public Property Taxonomy As Taxonomy
        ''' <summary>
        ''' 基因组结构模型，包含有基因的列表，以及转录调控网络
        ''' </summary>
        ''' <returns></returns>
        Public Property Genome As Genome

        ''' <summary>
        ''' 代谢组网络结构
        ''' </summary>
        ''' <returns></returns>
        <XmlElement("metabolome", [Namespace]:=GCMarkupLanguage)>
        Public Property MetabolismStructure As MetabolismStructure

        Public Const GCMarkupLanguage$ = "http://CAD_software.gcmodeller.org/XML/schema_revision/GCMarkup_1.0"

        <XmlNamespaceDeclarations()>
        Public xmlns As New XmlSerializerNamespaces

        Sub New()
            Call xmlns.Add("GCModeller", SMRUCC.genomics.LICENSE.GCModeller)
        End Sub

        Public Overrides Function ToString() As String
            Return Taxonomy.ToString
        End Function

    End Class

    <XmlType("metabolome", [Namespace]:=VirtualCell.GCMarkupLanguage)>
    Public Class MetabolismStructure

        <XmlArray("compounds")> Public Property Compounds As Compound()
        <XmlArray("reactions")> Public Property Reactions As Reaction()
        <XmlArray("enzymes")> Public Property Enzymes As Enzyme()

        <XmlArray("pathwayMaps")> Public Property Pathways As Pathway()

    End Class

    <XmlType("compound", [Namespace]:=VirtualCell.GCMarkupLanguage)>
    Public Class Compound : Implements INamedValue

        <XmlAttribute> Public Property ID As String Implements IKeyedEntity(Of String).Key
        <XmlAttribute> Public Property name As String

        <XmlArray>
        Public Property otherNames As String()

    End Class

    <XmlType("reaction", [Namespace]:=VirtualCell.GCMarkupLanguage)>
    Public Class Reaction : Implements INamedValue

        <XmlAttribute> Public Property ID As String Implements IKeyedEntity(Of String).Key
        <XmlAttribute> Public Property name As String

        <XmlText>
        Public Property Equation As String

    End Class

    <XmlType("pathway", [Namespace]:=VirtualCell.GCMarkupLanguage)>
    Public Class Pathway : Implements INamedValue

        <XmlAttribute> Public Property ID As String Implements IKeyedEntity(Of String).Key
        <XmlAttribute> Public Property name As String

        <XmlElement("enzyme")>
        Public Property enzymes As [Property]()

    End Class

    <XmlType("enzyme", [Namespace]:=VirtualCell.GCMarkupLanguage)>
    Public Class Enzyme : Implements INamedValue

        <XmlAttribute> Public Property geneID As String Implements IKeyedEntity(Of String).Key
        <XmlAttribute> Public Property KO As String

        <XmlElement>
        Public Property catalysis As Catalysis()

    End Class

    <XmlType("catalysis", [Namespace]:=VirtualCell.GCMarkupLanguage)>
    Public Class Catalysis

        ''' <summary>
        ''' The reaction id
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property reaction As String
        <XmlAttribute> Public Property coefficient As Double

        <XmlText>
        Public Property comment As String

    End Class
End Namespace
