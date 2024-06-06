﻿#Region "Microsoft.VisualBasic::8de568650573d2af2e10150bcec748b1, models\SBML\Biopax\Level3\Elements\SmallMolecule.vb"

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

    '   Total Lines: 170
    '    Code Lines: 137 (80.59%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 33 (19.41%)
    '     File Size: 5.73 KB


    ' Class SmallMolecule
    ' 
    '     Properties: dataSource, standardName, xrefs
    ' 
    ' Class BiochemicalReaction
    ' 
    '     Properties: conversionDirection, dataSource, displayName, eCNumber, left
    '                 name, participantStoichiometry, right, spontaneous, xref
    ' 
    ' Class spontaneous
    ' 
    ' 
    ' 
    ' Class CellularLocationVocabulary
    ' 
    '     Properties: term, xref
    ' 
    ' Class ChemicalStructure
    ' 
    '     Properties: structureData, structureFormat
    ' 
    ' Class SmallMoleculeReference
    ' 
    '     Properties: [structure], chemicalFormula, molecularWeight
    ' 
    ' Class MoleculeReference
    ' 
    '     Properties: displayName, name, xref
    ' 
    ' Class UnificationXref
    ' 
    '     Properties: db, id, idVersion
    ' 
    ' Class Provenance
    ' 
    '     Properties: name
    ' 
    ' Class Complex
    ' 
    '     Properties: component, componentStoichiometry, dataSource, xref
    ' 
    ' Class Protein
    ' 
    '     Properties: dataSource, feature, xref
    ' 
    ' Class ProteinReference
    ' 
    '     Properties: organism
    ' 
    ' Class BioSource
    ' 
    '     Properties: displayName, name, xref
    ' 
    ' Class FragmentFeature
    ' 
    '     Properties: featureLocation
    ' 
    ' Class SequenceInterval
    ' 
    '     Properties: sequenceIntervalBegin, sequenceIntervalEnd
    ' 
    ' Class SequenceSite
    ' 
    '     Properties: positionStatus, sequencePosition
    ' 
    ' Class Stoichiometry
    ' 
    '     Properties: physicalEntity, stoichiometricCoefficient
    ' 
    '     Function: ToString
    ' 
    ' Class Molecule
    ' 
    '     Properties: cellularLocation, displayName, entityReference, name
    ' 
    '     Function: GetEntityResourceId
    ' 
    ' Class Catalysis
    ' 
    '     Properties: controlled, controller, controlType, dataSource, displayName
    '                 xref
    ' 
    ' Class RelationshipXref
    ' 
    '     Properties: db, id, relationshipType
    ' 
    ' Class RelationshipTypeVocabulary
    ' 
    '     Properties: term, xref
    ' 
    ' Class PublicationXref
    ' 
    '     Properties: author, db, id, source, title
    '                 year
    ' 
    ' Class PhysicalEntity
    ' 
    '     Properties: cellularLocation, dataSource, displayName, memberPhysicalEntity, name
    '                 xref
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.MIME.application.rdf_xml
Imports SMRUCC.genomics.Model.Biopax.EntityProperties

<XmlType("SmallMolecule")>
Public Class SmallMolecule : Inherits Molecule

    Public Property standardName As standardName
    <XmlElement("xref")>
    Public Property xrefs As xref()
    Public Property dataSource As dataSource

End Class


Public Class BiochemicalReaction : Inherits RDFEntity
    Public Property conversionDirection As conversionDirection
    <XmlElement> Public Property left As EntityProperty()
    <XmlElement> Public Property right As EntityProperty()
    Public Property eCNumber As eCNumber
    Public Property displayName As displayName
    <XmlElement> Public Property xref As xref()
    Public Property dataSource As dataSource
    <XmlElement>
    Public Property participantStoichiometry As participantStoichiometry()
    Public Property spontaneous As spontaneous
    Public Property name As name
End Class

Public Class spontaneous : Inherits EntityProperty

End Class

Public Class CellularLocationVocabulary : Inherits RDFEntity
    Public Property term As term
    <XmlElement> Public Property xref As xref()
End Class

Public Class ChemicalStructure : Inherits RDFEntity
    Public Property structureFormat As structureFormat
    Public Property structureData As structureData
End Class

<XmlType("SmallMoleculeReference", [Namespace]:=Level3.File.bp)>
Public Class SmallMoleculeReference : Inherits MoleculeReference
    Public Property molecularWeight As molecularWeight
    Public Property chemicalFormula As chemicalFormula
    Public Property [structure] As [structure]
End Class

Public MustInherit Class MoleculeReference : Inherits RDFEntity

    Public Property displayName As displayName

    <XmlElement> Public Property name As name()
    <XmlElement> Public Property xref As xref()
End Class

Public Class UnificationXref : Inherits RDFEntity
    Public Property db As db
    Public Property id As id
    Public Property idVersion As idVersion
End Class
Public Class Provenance : Inherits RDFEntity
    Public Property name As name
End Class


Public Class Complex : Inherits Molecule

    <XmlElement> Public Property componentStoichiometry As componentStoichiometry()
    <XmlElement> Public Property xref As xref()
    Public Property dataSource As dataSource
    <XmlElement> Public Property component As component()

End Class

Public Class Protein : Inherits Molecule

    Public Property feature As feature
    <XmlElement>
    Public Property xref As xref()
    Public Property dataSource As dataSource
End Class

Public Class ProteinReference : Inherits MoleculeReference
    Public Property organism As organism
End Class

Public Class BioSource : Inherits RDFEntity
    Public Property name As name
    Public Property displayName As displayName
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
    Public Property physicalEntity As EntityProperty

    Public Overrides Function ToString() As String
        Return $"{RDFId} [{physicalEntity}]"
    End Function
End Class

Public MustInherit Class Molecule : Inherits RDFEntity

    Public Property displayName As displayName
    <XmlElement>
    Public Property name As name()
    Public Property cellularLocation As cellularLocation
    Public Property entityReference As entityReference

    Public Function GetEntityResourceId() As String
        If entityReference Is Nothing OrElse entityReference.resource Is Nothing Then
            Return ""
        Else
            Return entityReference.resource
        End If
    End Function
End Class

Public Class Catalysis : Inherits RDFEntity
    Public Property controller As controller
    Public Property controlled As controlled
    Public Property controlType As controlType
    <XmlElement> Public Property xref As xref()
    Public Property dataSource As dataSource
    Public Property displayName As displayName
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
    <XmlElement> Public Property name As name()
    <XmlElement> Public Property displayName As displayName
    <XmlElement> Public Property cellularLocation As cellularLocation
    <XmlElement> Public Property xref As xref()
    <XmlElement> Public Property dataSource As dataSource
    <XmlElement> Public Property memberPhysicalEntity As memberPhysicalEntity()
End Class
