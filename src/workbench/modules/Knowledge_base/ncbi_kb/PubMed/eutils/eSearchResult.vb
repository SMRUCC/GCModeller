#Region "Microsoft.VisualBasic::2ad607b3a6d782252f55a02b694bc455, modules\Knowledge_base\Knowledge_base\PubMed\eutils\eSearchResult.vb"

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

    '   Total Lines: 58
    '    Code Lines: 48
    ' Comment Lines: 0
    '   Blank Lines: 10
    '     File Size: 1.75 KB


    ' Class eSearchResult
    ' 
    '     Properties: Count, IdList, QueryTranslation, RetMax, RetStart
    '                 TranslationSet, TranslationStack
    ' 
    ' Class IdList
    ' 
    '     Properties: Id
    ' 
    '     Function: GenericEnumerator, GetEnumerator, ToString
    ' 
    ' Class TranslationSet
    ' 
    '     Properties: Translation
    ' 
    ' Class Translation
    ' 
    '     Properties: [To], From
    ' 
    ' Class TranslationStack
    ' 
    '     Properties: OP, TermSet
    ' 
    ' Class TermSet
    ' 
    '     Properties: Count, Explode, Field, Term
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON

Public Class eSearchResult
    Public Property Count As Integer
    Public Property RetMax As Integer
    Public Property RetStart As Integer
    Public Property IdList As IdList
    Public Property TranslationSet As TranslationSet
    Public Property TranslationStack As TranslationStack
    Public Property QueryTranslation As String
End Class

Public Class IdList : Implements Enumeration(Of String)

    <XmlElement("Id")> Public Property Id As String()

    Public Overrides Function ToString() As String
        Return Id.GetJson
    End Function

    Public Iterator Function GenericEnumerator() As IEnumerator(Of String) Implements Enumeration(Of String).GenericEnumerator
        If Not Id Is Nothing Then
            For Each id As String In Me.Id
                Yield id
            Next
        End If
    End Function

    Public Iterator Function GetEnumerator() As IEnumerator Implements Enumeration(Of String).GetEnumerator
        Yield GenericEnumerator()
    End Function
End Class

Public Class TranslationSet
    <XmlElement("Translation")>
    Public Property Translation As Translation()
End Class

Public Class Translation
    Public Property From As String
    Public Property [To] As String
End Class

Public Class TranslationStack
    <XmlElement("TermSet")>
    Public Property TermSet As TermSet()
    <XmlElement("OP")>
    Public Property OP As String()
End Class

Public Class TermSet
    Public Property Term As String
    Public Property Field As String
    Public Property Count As Integer
    Public Property Explode As String
End Class
