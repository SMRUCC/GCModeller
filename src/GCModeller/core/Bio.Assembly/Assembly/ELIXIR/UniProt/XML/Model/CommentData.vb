﻿#Region "Microsoft.VisualBasic::3115fd35db05bb9d8374bf33686624c0, G:/GCModeller/src/GCModeller/core/Bio.Assembly//Assembly/ELIXIR/UniProt/XML/Model/CommentData.vb"

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

    '   Total Lines: 68
    '    Code Lines: 53
    ' Comment Lines: 0
    '   Blank Lines: 15
    '     File Size: 2.07 KB


    '     Class comment
    ' 
    '         Properties: [event], dbReferences, evidence, isoforms, physiologicalReaction
    '                     reaction, subcellularLocations, text, type
    ' 
    '         Function: ToString
    ' 
    '     Class reaction
    ' 
    '         Properties: dbReferences, direction, evidence, text
    ' 
    '     Class subcellularLocation
    ' 
    '         Properties: locations, topology
    ' 
    '         Function: ToString
    ' 
    '     Class isoform
    ' 
    '         Properties: id, name, sequence, text
    ' 
    '     Class disease
    ' 
    '         Properties: acronym, dbReference, description, id, name
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Assembly.Uniprot.XML

    Public Class comment

        <XmlAttribute> Public Property type As String
        <XmlAttribute> Public Property evidence As String
        Public Property text As value
        <XmlElement("subcellularLocation")>
        Public Property subcellularLocations As subcellularLocation()
        Public Property [event] As value
        <XmlElement("isoform")>
        Public Property isoforms As isoform()
        <XmlElement("dbReference")>
        Public Property dbReferences As dbReference()
        Public Property reaction As reaction
        Public Property physiologicalReaction As reaction

        Public Overrides Function ToString() As String
            Return text.ToString
        End Function

    End Class

    Public Class reaction

        <XmlAttribute>
        Public Property direction As String
        <XmlAttribute>
        Public Property evidence As Integer
        Public Property text As String
        <XmlElement("dbReference")>
        Public Property dbReferences As dbReference()

    End Class

    Public Class subcellularLocation

        <XmlElement("location")> Public Property locations As value()
        <XmlElement("topology")> Public Property topology As value

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class

    Public Class isoform
        Public Property id As String
        Public Property name As String
        Public Property sequence As value
        Public Property text As value
    End Class

    Public Class disease

        <XmlAttribute>
        Public Property id As String
        Public Property name As String
        Public Property acronym As String
        Public Property description As String
        Public Property dbReference As value

    End Class
End Namespace
