#Region "Microsoft.VisualBasic::1076428e2dfcb5718997fa9fc1e92cd0, GCModeller\engine\CompilerServices\ModelBase\Property.vb"

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

    '   Total Lines: 36
    '    Code Lines: 27
    ' Comment Lines: 3
    '   Blank Lines: 6
    '     File Size: 1.24 KB


    ' Class [Property]
    ' 
    '     Properties: authors, comment, compiled, DBLinks, Emails
    '                 guid, name, publications, reversion, specieId
    '                 title, URLs
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization

''' <summary>
''' the property about the current virtual cell model
''' </summary>
Public Class [Property]

    <XmlAttribute> Public Property name As String
    <XmlAttribute> Public Property compiled As Date
    <XmlAttribute> Public Property reversion As Integer

    <XmlElement> Public Property guid As String
    <XmlElement> Public Property specieId As String
    <XmlElement> Public Property title As String
    <XmlElement> Public Property Emails As List(Of String)
    <XmlElement> Public Property authors As List(Of String)
    <XmlElement> Public Property comment As String
    <XmlElement> Public Property publications As List(Of String)
    <XmlElement> Public Property URLs As List(Of String)

    Public Property DBLinks As String()

    Sub New()
        authors = New List(Of String) From {Environment.MachineName}
        compiled = Now.ToString
        Emails = New List(Of String)
        guid = System.Guid.NewGuid.ToString
        publications = New List(Of String)
        URLs = New List(Of String) From {"https://gcmodeller.org/"}
        DBLinks = New String() {}
    End Sub

    Public Overrides Function ToString() As String
        Return specieId
    End Function
End Class
