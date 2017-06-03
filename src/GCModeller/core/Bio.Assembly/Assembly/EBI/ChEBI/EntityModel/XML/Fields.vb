Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Assembly.EBI.ChEBI.XML

    Public Class CompoundOrigin
        Public Property speciesText As String
        Public Property speciesAccession As String
        Public Property componentText As String
        Public Property componentAccession As String
        Public Property SourceType As String
        Public Property SourceAccession As String

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class

    Public Class OntologyParents
        Public Property chebiName As String
        Public Property chebiId As String
        Public Property type As String
        Public Property status As String
        Public Property cyclicRelationship As Boolean
    End Class

    Public Class DatabaseLinks
        Public Property data As String
        Public Property type As String
    End Class

    Public Class ChemicalStructures
        Public Property [structure] As String
        Public Property type As String
        Public Property dimension As String
        Public Property defaultStructure As String
    End Class

    Public Class Synonyms
        Public Property data As String
        Public Property source As String
        Public Property type As String
    End Class

    Public Class RegistryNumbers

        Public Property data As String
        Public Property source As String
        Public Property type As String

        Public Const Type_Reaxys$ = "Reaxys Registry Number"
        Public Const Type_Beilstein$ = "Beilstein Registry Number"
        Public Const Type_CAS$ = "CAS Registry Number"

    End Class

    Public Class Formulae
        Public Property data As String
        Public Property source As String
    End Class
End Namespace