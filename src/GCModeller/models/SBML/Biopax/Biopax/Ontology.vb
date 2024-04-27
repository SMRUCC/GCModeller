#Region "Microsoft.VisualBasic::a419be799859b231fefc0c62825a58b4, G:/GCModeller/src/GCModeller/models/SBML/Biopax//Biopax/Ontology.vb"

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

    '   Total Lines: 12
    '    Code Lines: 8
    ' Comment Lines: 0
    '   Blank Lines: 4
    '     File Size: 364 B


    ' Class OwlOntology
    ' 
    '     Properties: [imports]
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.MIME.application.rdf_xml

<XmlType("Ontology", [Namespace]:=OwlOntology.owl)>
Public Class OwlOntology : Inherits RDFEntity

    Public Const owl As String = "http://www.w3.org/2002/07/owl#"

    <XmlElement("imports", [Namespace]:=owl)>
    Public Property [imports] As Resource

End Class
