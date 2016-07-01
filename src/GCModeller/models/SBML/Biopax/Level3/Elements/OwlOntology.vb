#Region "Microsoft.VisualBasic::980fb195efc468ba224f73c7acfd654f, ..\GCModeller\models\SBML\Biopax\Level3\Elements\OwlOntology.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.DocumentFormat.RDF
Imports Microsoft.VisualBasic.Serialization
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace MetaCyc.Biopax.Level3.Elements

    <XmlType("Ontology")> Public Class owlOntology : Inherits RDFEntity
        <XmlElement("imports")> Public Property owlImports As owlImports

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class

    ''' <summary>
    ''' owl:imports
    ''' </summary>
    <XmlType("imports")> Public Class owlImports : Inherits EntityProperty

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class
End Namespace
