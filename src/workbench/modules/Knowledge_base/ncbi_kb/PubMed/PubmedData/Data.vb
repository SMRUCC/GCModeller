#Region "Microsoft.VisualBasic::a80fe60307ee3cf8781e8e4da836e5ad, modules\Knowledge_base\ncbi_kb\PubMed\PubmedData\Data.vb"

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

    '   Total Lines: 27
    '    Code Lines: 19 (70.37%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 8 (29.63%)
    '     File Size: 659 B


    '     Class History
    ' 
    '         Properties: PubMedPubDate
    ' 
    '         Function: ToString
    ' 
    '     Class ArticleId
    ' 
    '         Properties: ID, IdType
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Linq

Namespace PubMed

    Public Class History

        <XmlElement("PubMedPubDate")> Public Property PubMedPubDate As PubDate()

        Public Overrides Function ToString() As String
            Return PubMedPubDate.SafeQuery.JoinBy(" -> ")
        End Function

    End Class

    Public Class ArticleId

        <XmlAttribute>
        Public Property IdType As String
        <XmlText>
        Public Property ID As String

        Sub New()
        End Sub

        Sub New(aid As String, type As String)
            ID = aid
            IdType = type
        End Sub

        Public Overrides Function ToString() As String
            Return $"{IdType}: {ID}"
        End Function
    End Class
End Namespace
