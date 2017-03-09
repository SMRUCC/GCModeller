#Region "Microsoft.VisualBasic::97c9708153190eb6f7853a38cf8c657f, ..\GCModeller\core\Bio.Assembly\Assembly\Uniprot\UniprotXML.vb"

' Author:
' 
'       asuka (amethyst.asuka@gcmodeller.org)
'       xieguigang (xie.guigang@live.com)
'       xie (genetics@smrucc.org)
' 
' Copyright (c) 2016 GPL3 Licensed
' 
' 
' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text.Xml

Namespace Assembly.Uniprot.XML

    <XmlType("uniprot")> Public Class UniprotXML

        Const ns$ = "xmlns=""http://uniprot.org/uniprot"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xsi:schemaLocation=""http://uniprot.org/uniprot http://www.uniprot.org/support/docs/uniprot.xsd"""

        ''' <summary>
        ''' <see cref="entry.accession"/>可以用作为字典的键名
        ''' </summary>
        ''' <returns></returns>
        <XmlElement("entry")>
        Public Property entries As entry()
        <XmlElement>
        Public Property copyright As String

        Public Shared Function Load(path$) As UniprotXML
            Dim xml As String = path.ReadAllText.Replace(UniprotXML.ns, Xmlns.DefaultXmlns)
            Dim model As UniprotXML = xml.LoadFromXml(Of UniprotXML)
            Return model
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="handle$">file or directory</param>
        ''' <returns></returns>
        Public Shared Function LoadDictionary(handle$) As Dictionary(Of entry)
            Dim source As entry()

            If handle.FileExists(True) Then
                source = Load(handle).entries
            Else
                source =
                    (ls - l - r - "*.xml" <= handle) _
                    .Select(AddressOf Load) _
                    .Select(Function(xml) xml.entries) _
                    .IteratesALL _
                    .ToArray
            End If

            Dim groups = From protein As entry
                         In source _
                             .Select(Function(o) o.ShadowCopy) _
                             .IteratesALL
                         Select protein
                         Group protein By DirectCast(protein, INamedValue).Key Into Group
            Dim out As Dictionary(Of entry) = groups _
                .Select(Function(g) g.Group.First) _
                .ToDictionary

            Return out
        End Function

        Public Overrides Function ToString() As String
            Return GetXml
        End Function
    End Class
End Namespace
