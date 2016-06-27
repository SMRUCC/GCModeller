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