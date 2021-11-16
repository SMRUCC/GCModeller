#Region "Microsoft.VisualBasic::067316eb4bb56f514aa0547a08a73605, models\GPML\PathVisio\Xml\Components.vb"

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

    '     Class InfoBox
    ' 
    '         Properties: CenterX, CenterY
    ' 
    '     Class Comment
    ' 
    '         Properties: Source, Text
    ' 
    '         Function: ToString
    ' 
    '     Class PublicationXref
    ' 
    '         Properties: id
    ' 
    '     Class Group
    ' 
    '         Properties: GraphId, GroupId, Style
    ' 
    '     Class Xref
    ' 
    '         Properties: Database, ID
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization

Namespace GPML

    Public Class InfoBox
        <XmlAttribute> Public Property CenterX  As  Double
        <XmlAttribute> Public Property CenterY As Double
    End Class

    Public Class Comment

        <XmlAttribute>
        Public Property Source As String

        <XmlText>
        Public Property Text As String

        Public Overrides Function ToString() As String
            Return $"[{Source}] {Text}"
        End Function

    End Class

    Public Class PublicationXref

        <XmlAttribute>
        Public Property id As String
    End Class

    Public Class Group

        <XmlAttribute> Public Property GroupId As String
        <XmlAttribute> Public Property GraphId As String
        <XmlAttribute> Public Property Style As String

    End Class

    Public Class Xref

        <XmlAttribute> Public Property Database As String
        <XmlAttribute> Public Property ID As String

        Public Overrides Function ToString() As String
            If Database.StringEmpty AndAlso ID.StringEmpty Then
                Return "NULL"
            Else
                Return $"[{Database}] {ID}"
            End If
        End Function

    End Class
End Namespace
