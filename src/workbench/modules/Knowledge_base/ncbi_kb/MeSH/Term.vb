#Region "Microsoft.VisualBasic::679cb104899e0baecd31a67235ca9e1b, modules\Knowledge_base\ncbi_kb\MeSH\Term.vb"

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

    '   Total Lines: 136
    '    Code Lines: 91 (66.91%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 45 (33.09%)
    '     File Size: 3.62 KB


    '     Class Term
    ' 
    '         Properties: ConceptPreferredTermYN, DateCreated, IsPermutedTermYN, LexicalTag, RecordPreferredTermYN
    '                     TermUI, ThesaurusIDlist
    ' 
    '         Function: ToString
    ' 
    '     Class ThesaurusID
    ' 
    '         Properties: Value
    ' 
    '         Function: ToString
    ' 
    '     Class Concept
    ' 
    '         Properties: CASN1Name, ConceptName, ConceptRelationList, ConceptUI, PreferredConceptYN
    '                     RegistryNumber, RelatedRegistryNumberList, ScopeNote, TermList
    ' 
    '         Function: ToString
    ' 
    '     Class ConceptRelation
    ' 
    '         Properties: Concept1UI, Concept2UI, RelationName
    ' 
    '     Class RelatedRegistryNumber
    ' 
    '         Properties: Value
    ' 
    '         Function: ToString
    ' 
    '     Class TreeNumber
    ' 
    '         Properties: Category, Value
    ' 
    '         Function: ToString
    ' 
    '     Class PharmacologicalAction
    ' 
    '         Properties: DescriptorReferredTo
    ' 
    '     Class DescriptorReferredTo
    ' 
    '         Properties: DescriptorName, DescriptorUI
    ' 
    '         Function: ToString
    ' 
    '     Class PreviousIndexing
    ' 
    '         Properties: Value
    ' 
    '         Function: ToString
    ' 
    '     Class AllowableQualifier
    ' 
    '         Properties: Abbreviation, QualifierReferredTo
    ' 
    '     Class QualifierReferredTo
    ' 
    '         Properties: QualifierName, QualifierUI
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

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
