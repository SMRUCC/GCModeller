#Region "Microsoft.VisualBasic::c4c71d7a70174d45d598dca8bad70956, modules\Knowledge_base\Knowledge_base\PubMed\PubmedData\Data.vb"

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

    '   Total Lines: 20
    '    Code Lines: 15
    ' Comment Lines: 0
    '   Blank Lines: 5
    '     File Size: 480 B


    '     Class History
    ' 
    '         Properties: PubMedPubDate
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

Namespace PubMed

    Public Class History

        <XmlElement("PubMedPubDate")> Public Property PubMedPubDate As PubDate()
    End Class

    Public Class ArticleId
        <XmlAttribute>
        Public Property IdType As String
        <XmlText>
        Public Property ID As String

        Public Overrides Function ToString() As String
            Return $"{IdType}: {ID}"
        End Function
    End Class
End Namespace
