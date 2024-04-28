#Region "Microsoft.VisualBasic::e565511d058c37574ba2e7396051d37f, G:/GCModeller/src/GCModeller/data/Reactome//Owl/Nodes.vb"

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

    '   Total Lines: 96
    '    Code Lines: 82
    ' Comment Lines: 0
    '   Blank Lines: 14
    '     File Size: 3.80 KB


    '     Class BiochemicalReaction
    ' 
    '         Properties: conversionDirection, displayName, eCNumber, left, participantStoichiometry
    '                     right
    ' 
    '     Class SmallMolecule
    ' 
    '         Properties: cellularLocation, displayName, entityReference, name
    ' 
    '     Class CellularLocationVocabulary
    ' 
    '         Properties: term, xref
    ' 
    '     Class SmallMoleculeReference
    ' 
    '         Properties: name, xref
    ' 
    '     Class Provenance
    ' 
    '         Properties: comment, name
    ' 
    '     Class Catalysis
    ' 
    '         Properties: controlled, controller, controlType
    ' 
    '     Class Protein
    ' 
    '         Properties: cellularLocation, displayName, entityReference, feature, name
    ' 
    '     Class ProteinReference
    ' 
    '         Properties: name, organism
    ' 
    '     Class BioSource
    ' 
    '         Properties: name
    ' 
    '     Class FragmentFeature
    ' 
    '         Properties: featureLocation
    ' 
    '     Class SequenceInterval
    ' 
    '         Properties: sequenceIntervalBegin, sequenceIntervalEnd
    ' 
    '     Class SequenceSite
    ' 
    '         Properties: positionStatus, sequencePosition
    ' 
    '     Class RelationshipTypeVocabulary
    ' 
    '         Properties: term, xref
    ' 
    '     Class Complex
    ' 
    '         Properties: cellularLocation, component, componentStoichiometry, displayName
    ' 
    '     Class Stoichiometry
    ' 
    '         Properties: physicalEntity, stoichiometricCoefficient
    ' 
    '     Class PhysicalEntity
    ' 
    '         Properties: cellularLocation, displayName, memberPhysicalEntity
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Xml.Serialization
Imports SMRUCC.genomics.Data.Reactome.OwlDocument.Abstract
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

Namespace OwlDocument.Nodes

    Public Class BiochemicalReaction : Inherits Node
        Public Property conversionDirection As String
        <XmlElement> Public Property participantStoichiometry As RDFresource()
        <XmlElement> Public Property left As RDFresource()
        <XmlElement> Public Property right As RDFresource()
        Public Property eCNumber As String
        <XmlElement> Public Property displayName As String()
    End Class

    Public Class SmallMolecule : Inherits Node
        Public Property displayName As String
        Public Property name As String
        Public Property cellularLocation As RDFresource
        Public Property entityReference As RDFresource
    End Class
    Public Class CellularLocationVocabulary : Inherits ResourceElement
        Public Property term As String
        <XmlElement> Public Property xref As RDFresource()
    End Class

    Public Class SmallMoleculeReference : Inherits ResourceElement
        <XmlElement> Public Property name As String()
        <XmlElement> Public Property xref As RDFresource()
    End Class

    Public Class Provenance : Inherits ResourceElement
        Public Property name As String
        Public Property comment As String
    End Class

    Public Class Catalysis : Inherits Node
        Public Property controller As RDFresource
        Public Property controlled As RDFresource
        Public Property controlType As String
    End Class
    Public Class Protein : Inherits Node
        Public Property displayName As String
        <XmlElement> Public Property name As String()
        Public Property cellularLocation As RDFresource
        Public Property entityReference As RDFresource
        <XmlElement> Public Property feature As RDFresource()

    End Class
    Public Class ProteinReference : Inherits Node
        Public Property organism As RDFresource
        <XmlElement> Public Property name As String()
    End Class
    Public Class BioSource : Inherits Node
        <XmlElement> Public Property name As String()
    End Class

    Public Class FragmentFeature : Inherits ResourceElement
        Public Property featureLocation As RDFresource
    End Class
    Public Class SequenceInterval : Inherits ResourceElement
        Public Property sequenceIntervalBegin As RDFresource
        Public Property sequenceIntervalEnd As RDFresource
    End Class
    Public Class SequenceSite : Inherits ResourceElement
        Public Property sequencePosition As String
        Public Property positionStatus As String
    End Class

    Public Class RelationshipTypeVocabulary : Inherits ResourceElement
        Public Property term As String
        <XmlElement> Public Property xref As RDFresource()
    End Class

    Public Class Complex : Inherits Node
        <XmlElement> Public Property displayName As String()
        Public Property cellularLocation As RDFresource
        <XmlElement> Public Property componentStoichiometry As RDFresource()
        <XmlElement> Public Property component As RDFresource()

    End Class

    Public Class Stoichiometry : Inherits ResourceElement
        Public Property stoichiometricCoefficient As String
        Public Property physicalEntity As RDFresource
    End Class

    Public Class PhysicalEntity : Inherits Node

        <XmlElement> Public Property memberPhysicalEntity As RDFresource()
        Public Property displayName As String()
        Public Property cellularLocation As RDFresource
    End Class
End Namespace
