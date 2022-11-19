#Region "Microsoft.VisualBasic::b17e3fff6a4ed9afb4177e39e3a7266c, GCModeller\models\SBML\Biopax\Level2\Elements\Ontology.vb"

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

    '   Total Lines: 61
    '    Code Lines: 51
    ' Comment Lines: 0
    '   Blank Lines: 10
    '     File Size: 1.95 KB


    ' Class Ontology
    ' 
    '     Properties: Comment
    '     Class [Imports]
    ' 
    '         Properties: Resource
    ' 
    ' 
    ' 
    ' Class PhysicalEntityParticipant
    ' 
    '     Properties: CellularLocation, PhysicalEntity, Stoichiometric
    '     Class PhysicalEntity_
    ' 
    '         Properties: Protein
    '         Class Protein_
    ' 
    '             Properties: Comment, Id, Name, Organism, Synonyms
    '                         Xref
    '             Class Organism_
    ' 
    '                 Properties: BioSource
    '                 Class BioSource_
    ' 
    '                     Properties: Id, Name, TaxonXref
    '                     Class TaxonXref_
    ' 
    '                         Properties: UnificationXref
    ' 
    ' 
    ' 
    ' 
    ' 
    ' 
    ' 
    ' 
    ' 
    '     Class CellularLocation_
    ' 
    '         Properties: OpenControlledVocabulary
    '         Class OpenControlledVocabulary_
    ' 
    '             Properties: Id, Term, Xref
    ' 
    ' 
    ' 
    ' 
    ' 
    '     Class Xref
    ' 
    '         Properties: UnificationXref
    '         Class UnificationXref_
    ' 
    '             Properties: attrId, Db, Id
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Public Class Ontology
    Public Class [Imports]
        Public Property Resource As String
    End Class

    Public Property Comment As String
End Class

Public Class PhysicalEntityParticipant
    Public Property Stoichiometric As Integer

    Public Property PhysicalEntity As PhysicalEntity_
    Public Property CellularLocation As CellularLocation_

    Public Class PhysicalEntity_
        Public Property Protein As Protein_

        Public Class Protein_
            Public Property Id As String
            Public Property Xref As CommonElements.Xref
            Public Property Synonyms As String()
            Public Property Organism As Organism_
            Public Class Organism_
                Public Property BioSource As BioSource_

                Public Class BioSource_
                    Public Property Id As String
                    Public Property TaxonXref As TaxonXref_
                    Public Class TaxonXref_
                        Public Property UnificationXref As CommonElements.Xref.UnificationXref_
                    End Class
                    Public Property Name As String
                End Class
            End Class
            Public Property Name As String
            Public Property Comment As String

        End Class
    End Class
    Public Class CellularLocation_
        Public Property OpenControlledVocabulary As OpenControlledVocabulary_

        Public Class OpenControlledVocabulary_
            Public Property Id As String
            Public Property Xref As CommonElements.Xref
            Public Property Term As String
        End Class
    End Class
End Class

Namespace CommonElements
    Public Class Xref
        Public Property UnificationXref As UnificationXref_

        Public Class UnificationXref_
            Public Property attrId As String
            Public Property Id As String
            Public Property Db As String
        End Class
    End Class
End Namespace
