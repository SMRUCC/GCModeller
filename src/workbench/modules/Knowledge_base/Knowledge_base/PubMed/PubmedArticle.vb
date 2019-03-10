﻿#Region "Microsoft.VisualBasic::abfaf193b09ea2aa38d44f41c230bf8f, Knowledge_base\Knowledge_base\PubMed\PubmedArticle.vb"

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

    ' Class PubmedArticle
    ' 
    '     Properties: MedlineCitation, PubmedData
    ' 
    ' Class MedlineCitation
    ' 
    '     Properties: Article, ChemicalList, CitationSubset, DateCompleted, DateCreated
    '                 DateRevised, MedlineJournalInfo, MeshHeadingList, Owner, PMID
    '                 Status
    ' 
    ' Class MeshHeading
    ' 
    '     Properties: DescriptorName, QualifierName
    ' 
    ' Class PubmedData
    ' 
    '     Properties: ArticleIdList, History, PublicationStatus
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization

Public Class PubmedArticle
    Public Property MedlineCitation As MedlineCitation
    Public Property PubmedData As PubmedData
End Class

Public Class MedlineCitation
    Public Property Status As String
    Public Property Owner As String
    Public Property PMID As PMID
    Public Property DateCreated As PubDate
    Public Property DateCompleted As PubDate
    Public Property DateRevised As PubDate
    Public Property Article As Article
    Public Property MedlineJournalInfo As MedlineJournalInfo
    Public Property ChemicalList As Chemical()
    Public Property CitationSubset As String
    Public Property MeshHeadingList As MeshHeading()
End Class

Public Class MeshHeading
    Public Property DescriptorName As RegisterObject
    <XmlElement("QualifierName")>
    Public Property QualifierName As RegisterObject()
End Class

Public Class PubmedData
    Public Property History As History
    Public Property PublicationStatus As String
    Public Property ArticleIdList As ArticleId()
End Class
