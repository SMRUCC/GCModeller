#Region "Microsoft.VisualBasic::69872b985299863ee1d97d2378f28df3, ..\GCModeller\models\SBML\Biopax\Level3\Elements\SmallMolecule.vb"

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
Imports SMRUCC.genomics.Model.Biopax.EntityProperties

<XmlType("SmallMolecule")>
Public Class SmallMolecule : Inherits RDFEntity

    Public Property standardName As standardName
    Public Property displayName As displayName

    <XmlElement("name")> Public Property names As name()
    Public Property cellularLocation As cellularLocation
    Public Property entityReference As entityReference
    <XmlElement("xref")> Public Property xrefs As xref()
    Public Property dataSource As dataSource

End Class


Public Class BiochemicalReaction : Inherits RDFEntity
    Public Property conversionDirection As conversionDirection
    <XmlElement> Public Property left As left()
    <XmlElement> Public Property right As right()
    Public Property eCNumber As eCNumber
    Public Property displayName As displayName
    <XmlElement> Public Property xref As xref()
    Public Property dataSource As dataSource
End Class

Public Class CellularLocationVocabulary : Inherits RDFEntity
    Public Property term As term
    <XmlElement> Public Property xref As xref()
End Class

Public Class ChemicalStructure : Inherits RDFEntity
    Public Property structureFormat As structureFormat
    Public Property structureData As structureData
End Class

Public Class SmallMoleculeReference : Inherits RDFEntity
    <XmlElement> Public Property name As name()
    <XmlElement> Public Property xref As xref()
    Public Property molecularWeight As molecularWeight
End Class
Public Class UnificationXref : Inherits RDFEntity
    Public Property db As db
    Public Property id As id
    Public Property idVersion As idVersion
End Class
Public Class Provenance : Inherits RDFEntity
    Public Property name As name
End Class


Public Class Complex : Inherits RDFEntity
    Public Property displayName As displayName
    Public Property cellularLocation As cellularLocation
    Public Property componentStoichiometry As componentStoichiometry
    <XmlElement> Public Property xref As xref()
    Public Property dataSource As dataSource
End Class



Public Class Protein : Inherits RDFEntity
    Public Property displayName As displayName
    Public Property cellularLocation As cellularLocation
    Public Property entityReference As entityReference
    Public Property feature As feature
    <XmlElement> Public Property xref As xref()
    Public Property dataSource As dataSource
End Class
Public Class ProteinReference : Inherits RDFEntity
    Public Property organism As organism
    <XmlElement> Public Property name As name()
    <XmlElement> Public Property xref As xref()
End Class
Public Class BioSource : Inherits RDFEntity
    Public Property name As name
    <XmlElement> Public Property xref As xref()
End Class


Public Class FragmentFeature : Inherits RDFEntity
    Public Property featureLocation As featureLocation
End Class
Public Class SequenceInterval : Inherits RDFEntity
    Public Property sequenceIntervalBegin As sequenceIntervalBegin
    Public Property sequenceIntervalEnd As sequenceIntervalEnd
End Class
Public Class SequenceSite : Inherits RDFEntity
    Public Property sequencePosition As sequencePosition
    Public Property positionStatus As positionStatus
End Class

Public Class Stoichiometry : Inherits RDFEntity
    Public Property stoichiometricCoefficient As stoichiometricCoefficient
    Public Property physicalEntity As PhysicalEntity
End Class



Public Class Catalysis : Inherits RDFEntity
    Public Property controller As controller
    Public Property controlled As controlled
    Public Property controlType As controlType
    <XmlElement> Public Property xref As xref()
    Public Property dataSource As dataSource
End Class
Public Class RelationshipXref : Inherits RDFEntity
    Public Property db As db
    Public Property id As id
    Public Property relationshipType As relationshipType
End Class
Public Class RelationshipTypeVocabulary : Inherits RDFEntity
    Public Property term As term
    Public Property xref As xref
End Class


Public Class PublicationXref : Inherits RDFEntity
    Public Property id As id
    Public Property db As db
    Public Property year As year
    Public Property title As title
    <XmlElement> Public Property author As author()
    Public Property source As source
End Class



Public Class PhysicalEntity : Inherits RDFEntity
    <XmlElement> Public Property displayName As displayName
    <XmlElement> Public Property cellularLocation As cellularLocation
    <XmlElement> Public Property xref As xref()
    <XmlElement> Public Property dataSource As dataSource
End Class
