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
Imports SMRUCC.genomics.Metagenomics

Namespace v2

    <XmlType(NameOf(VirtualCell), [Namespace]:=SMRUCC.genomics.LICENSE.GCModeller)>
    Public Class VirtualCell : Inherits XmlDataModel

        Public Property Taxonomy As Taxonomy
        Public Property MetabolismStructure As MetabolismStructure

        <XmlNamespaceDeclarations()>
        Public xmlns As New XmlSerializerNamespaces

        Sub New()
            Call xmlns.Add("GCModeller", LICENSE.GCModeller)
        End Sub

        Public Overrides Function ToString() As String
            Return Taxonomy.ToString
        End Function

    End Class

    Public Class MetabolismStructure

        Public Property Compounds As Compound()
        Public Property Reactions As Reaction()
        Public Property Pathways As Pathway()

    End Class

    Public Class Compound : Implements INamedValue

        <XmlAttribute> Public Property ID As String Implements IKeyedEntity(Of String).Key
        <XmlAttribute> Public Property name As String

        <XmlArray>
        Public Property otherNames As String()

    End Class

    Public Class Reaction : Implements INamedValue

        <XmlAttribute> Public Property ID As String Implements IKeyedEntity(Of String).Key
        <XmlAttribute> Public Property name As String

        <XmlText>
        Public Property Equation As String

    End Class

    Public Class Pathway : Implements INamedValue

        <XmlAttribute> Public Property ID As String Implements IKeyedEntity(Of String).Key
        <XmlAttribute> Public Property name As String

        Public Property Enzymes As Enzyme()

    End Class

    Public Class Enzyme : Implements INamedValue

        Public Property geneID As String Implements IKeyedEntity(Of String).Key
        Public Property KO As String
        Public Property catalysis As Catalysis()

    End Class

    Public Class Catalysis

        ''' <summary>
        ''' The reaction id
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property Reaction As String
        <XmlAttribute> Public Property coefficient As Double

        <XmlText>
        Public Property comment As String

    End Class
End Namespace
