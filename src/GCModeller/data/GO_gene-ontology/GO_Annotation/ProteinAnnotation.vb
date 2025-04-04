﻿#Region "Microsoft.VisualBasic::397c66ae394310cabb7e4d1763ca9eb3, data\GO_gene-ontology\GO_Annotation\ProteinAnnotation.vb"

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

    '   Total Lines: 51
    '    Code Lines: 43 (84.31%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 8 (15.69%)
    '     File Size: 1.85 KB


    ' Class ProteinAnnotation
    ' 
    '     Properties: description, GO_terms, proteinID
    ' 
    '     Function: ToString
    ' 
    ' Class AnnotationClusters
    ' 
    '     Properties: proteins, size
    ' 
    '     Function: GenericEnumerator, ToAnnotationTable
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.Framework.IO
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Text.Xml.Models

Public Class ProteinAnnotation

    <XmlAttribute("id")>
    Public Property proteinID As String
    Public Property description As String
    Public Property GO_terms As XmlList(Of NamedValue)

    Public Overrides Function ToString() As String
        Return $"Dim {proteinID} As [{description}] = {GO_terms.AsEnumerable.Select(Function(term) term.name).GetJson}"
    End Function
End Class

Public Class AnnotationClusters : Inherits XmlDataModel
    Implements IList(Of ProteinAnnotation)

    <XmlElement>
    Public Property proteins As ProteinAnnotation()

    <XmlAttribute>
    Public ReadOnly Property size As Integer Implements IList(Of ProteinAnnotation).size
        Get
            Return proteins.Length
        End Get
    End Property

    Public Iterator Function ToAnnotationTable() As IEnumerable(Of EntityObject)
        For Each protein As ProteinAnnotation In proteins
            Yield New EntityObject With {
                .ID = protein.proteinID,
                .Properties = New Dictionary(Of String, String) From {
                    {"fullName", protein.description},
                    {"GO", protein.GO_terms.AsEnumerable.Keys.JoinBy("; ")}
                }
            }
        Next
    End Function

    Public Iterator Function GenericEnumerator() As IEnumerator(Of ProteinAnnotation) Implements Enumeration(Of ProteinAnnotation).GenericEnumerator
        For Each protein In proteins
            Yield protein
        Next
    End Function
End Class
