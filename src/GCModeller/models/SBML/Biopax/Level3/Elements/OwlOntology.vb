#Region "Microsoft.VisualBasic::f9cb020d4a1ccd4d3b0b1967d2e2e410, GCModeller\models\SBML\Biopax\Level3\Elements\OwlOntology.vb"

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

    '   Total Lines: 24
    '    Code Lines: 16
    ' Comment Lines: 3
    '   Blank Lines: 5
    '     File Size: 736 B


    '     Class owlOntology
    ' 
    '         Properties: owlImports
    ' 
    '         Function: ToString
    ' 
    '     Class owlImports
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.MIME.application.rdf_xml
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
