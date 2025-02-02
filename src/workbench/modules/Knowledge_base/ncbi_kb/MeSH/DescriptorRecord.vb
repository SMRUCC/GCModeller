﻿#Region "Microsoft.VisualBasic::4030d747d049c1620c310901f833a1d5, modules\Knowledge_base\ncbi_kb\MeSH\DescriptorRecord.vb"

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

    '   Total Lines: 142
    '    Code Lines: 93 (65.49%)
    ' Comment Lines: 4 (2.82%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 45 (31.69%)
    '     File Size: 3.93 KB


    '     Class DescriptorRecord
    ' 
    '         Properties: AllowableQualifiersList, ConceptList, DateCreated, DateEstablished, DateRevised
    '                     DescriptorClass, DescriptorName, DescriptorUI, HistoryNote, OnlineNote
    '                     PharmacologicalActionList, PreviousIndexingList, PublicMeSHNote, TreeNumberList
    ' 
    '         Function: ToString
    ' 
    '     Class Term
    ' 
    '         Properties: ConceptPreferredTermYN, DateCreated, IsPermutedTermYN, LexicalTag, RecordPreferredTermYN
    '                     TermUI, ThesaurusIDlist
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
    '         Properties: Value
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
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.GCModeller.Workbench.Knowledge_base.NCBI.MeSH.Tree

Namespace MeSH

    ''' <summary>
    ''' the mesh term record
    ''' </summary>
    <XmlRoot(NameOf(DescriptorRecord)), XmlType(NameOf(DescriptorRecord))>
    Public Class DescriptorRecord

        <XmlAttribute>
        Public Property DescriptorClass As Integer

        ''' <summary>
        ''' Descriptor unique id
        ''' </summary>
        ''' <returns></returns>
        Public Property DescriptorUI As String
        Public Property DescriptorName As XmlString
        Public Property DateCreated As XmlDate
        Public Property DateRevised As XmlDate
        Public Property DateEstablished As XmlDate
        Public Property AllowableQualifiersList As AllowableQualifier()
        Public Property HistoryNote As String
        Public Property OnlineNote As String
        Public Property PublicMeSHNote As String
        Public Property PreviousIndexingList As PreviousIndexing()
        Public Property PharmacologicalActionList As PharmacologicalAction()
        Public Property TreeNumberList As TreeNumber()
        Public Property ConceptList As Concept()

        Public Function GetTopClass() As MeshCategory
            Dim top = TreeNumberList.SafeQuery _
                .Select(Function(t) t.Category) _
                .GroupBy(Function(c) c) _
                .OrderByDescending(Function(c) c.Count) _
                .FirstOrDefault

            If top Is Nothing Then
                Return MeshCategory.None
            Else
                Return top.Key
            End If
        End Function

        Public Overrides Function ToString() As String
            Return $"{DescriptorUI} - {DescriptorName} - " & ConceptList.Select(Function(c) c.ScopeNote).JoinBy("; ")
        End Function

    End Class

End Namespace
