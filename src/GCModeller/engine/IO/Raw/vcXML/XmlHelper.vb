#Region "Microsoft.VisualBasic::5e9217e2bf03f0ca9f2d7d4345ad03ec, GCModeller\engine\IO\Raw\vcXML\XmlHelper.vb"

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

    '   Total Lines: 25
    '    Code Lines: 18
    ' Comment Lines: 0
    '   Blank Lines: 7
    '     File Size: 724 B


    '     Module XmlHelper
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: getXmlFragment
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports System.Xml
Imports System.Xml.Serialization

Namespace vcXML

    Module XmlHelper

        Friend emptyNamespace As New XmlSerializerNamespaces()

        Sub New()
            emptyNamespace.Add(String.Empty, String.Empty)
        End Sub

        Public Function getXmlFragment(Of T)(obj As T, xmlConfig As XmlWriterSettings) As String
            Dim serializer As New XmlSerializer(GetType(T))
            Dim output As New StringBuilder()
            Dim writer As XmlWriter = XmlWriter.Create(output, xmlConfig)

            Call serializer.Serialize(writer, obj, emptyNamespace)

            Return output.ToString
        End Function
    End Module
End Namespace
