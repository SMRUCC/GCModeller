Imports System.Xml.Serialization
Imports SMRUCC.genomics.GCModeller.Workbench.Knowledge_base.NCBI.MeSH.Tree

Namespace MeSH

    Public Class Term : Inherits XmlString

        <XmlAttribute> Public Property ConceptPreferredTermYN As String
        <XmlAttribute> Public Property IsPermutedTermYN As String
        <XmlAttribute> Public Property LexicalTag As String
        <XmlAttribute> Public Property RecordPreferredTermYN As String

        Public Property TermUI As String
        Public Property DateCreated As XmlDate
        Public Property ThesaurusIDlist As ThesaurusID()

        Public Overrides Function ToString() As String
            Return $"[{TermUI}] {MyBase.ToString}"
        End Function

    End Class

    Public Class ThesaurusID

        <XmlText>
        Public Property Value As String

        Public Overrides Function ToString() As String
            Return Value
        End Function

    End Class

    Public Class Concept

        <XmlAttribute>
        Public Property PreferredConceptYN As String
        Public Property ConceptUI As String
        Public Property ConceptName As XmlString
        Public Property RegistryNumber As String
        Public Property ScopeNote As String
        Public Property RelatedRegistryNumberList As RelatedRegistryNumber()
        Public Property ConceptRelationList As ConceptRelation()
        Public Property CASN1Name As String
        Public Property TermList As Term()

        Public Overrides Function ToString() As String
            Return ScopeNote
        End Function

    End Class

    Public Class ConceptRelation

        <XmlAttribute>
        Public Property RelationName As String

        Public Property Concept1UI As String
        Public Property Concept2UI As String

    End Class

    Public Class RelatedRegistryNumber

        <XmlText>
        Public Property Value As String

        Public Overrides Function ToString() As String
            Return Value
        End Function

    End Class

    Public Class TreeNumber

        <XmlText>
        Public Property Value As String

        Public ReadOnly Property Category As MeshCategory
            Get
                Return MeSH.Tree.Term.GetClass(Value)
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return $"({Category.Description}) " & Value
        End Function

    End Class

    Public Class PharmacologicalAction

        Public Property DescriptorReferredTo As DescriptorReferredTo

    End Class

    Public Class DescriptorReferredTo

        Public Property DescriptorUI As String
        Public Property DescriptorName As XmlString

        Public Overrides Function ToString() As String
            Return $"[{DescriptorUI}] {DescriptorName}"
        End Function

    End Class

    Public Class PreviousIndexing

        <XmlText>
        Public Property Value As String

        Public Overrides Function ToString() As String
            Return Value
        End Function

    End Class

    Public Class AllowableQualifier

        Public Property QualifierReferredTo As QualifierReferredTo
        Public Property Abbreviation As String

    End Class

    Public Class QualifierReferredTo

        Public Property QualifierUI As String
        Public Property QualifierName As XmlString

        Public Overrides Function ToString() As String
            Return $"[{QualifierUI}] {QualifierName}"
        End Function

    End Class
End Namespace