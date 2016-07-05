#Region "Microsoft.VisualBasic::f453a21b8fc9c88eb5764ce8d3f19c06, ..\GCModeller\models\SBML\Biopax\Level3\Schema\FunctionalProperty.vb"

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

Namespace Level3.Schema

    ''' <summary>
    ''' Absolute location as defined by the referenced sequence database record. E.g. an operon 
    ''' has a absolute region on the DNA molecule referenced by the UnificationXref.
    ''' </summary>
    Public Class FunctionalProperty : Inherits RDFEntity

        Public Property type As RDFProperty
        Public Property domain As domain

    End Class

    Public Class domain : Inherits RDFProperty
        Public Property [Class] As [Class]
    End Class

    ''' <summary>
    ''' Definition: Imposes ordering on a step in a biochemical pathway. 
    ''' 
    ''' Retionale: A biochemical reaction can be reversible by itself, 
    ''' but can be physiologically directed In the context Of a pathway, 
    ''' For instance due To flux Of reactants And products. 
    ''' 
    ''' Usage: Only one conversion interaction can be ordered at a time, 
    ''' but multiple catalysis Or modulation instances can be part Of 
    ''' one Step.
    ''' </summary>
    Public Class [Class] : Inherits RDFEntity
        Public Property unionOf As unionOf
        ''' <summary>
        ''' rdfs:subClassOf
        ''' </summary>
        ''' <returns></returns>
        <XmlElement> Public Property subClassOf As subClassOf()
        ''' <summary>
        ''' owl:disjointWith
        ''' </summary>
        ''' <returns></returns>
        Public Property disjointWith As RDFProperty
    End Class

    Public Class subClassOf : Inherits RDFEntity
        Public Property Restriction As Restriction
    End Class

    Public Class Restriction : Inherits RDFEntity
        Public Property onProperty As RDFProperty
        Public Property allValuesFrom As RDFProperty

    End Class

    Public Class unionOf : Inherits RDFProperty
        <XmlElement> Public Property [Class] As RDFProperty()
    End Class

    ''' <summary>
    ''' A binding feature represents a &quot;half&quot; of the bond between two entities. 
    ''' This property points to another binding feature which represents the other half. 
    ''' The bond can be covalent or non-covalent.
    ''' </summary>
    Public Class InverseFunctionalProperty : Inherits RDFEntity
        <XmlElement> Public Property type As RDFProperty()
        Public Property domain As domain
        Public Property inverseOf As RDFProperty

    End Class
End Namespace
