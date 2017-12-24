#Region "Microsoft.VisualBasic::3308db1bc2c78f4432423f634f9d4843, ..\GCModeller\core\Bio.Assembly\Assembly\UniProt\XML\Model\UniprotXML.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
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
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text.Xml
Imports Microsoft.VisualBasic.Text.Xml.Linq

Namespace Assembly.Uniprot.XML

    ''' <summary>
    ''' Download from the uniprot database id mappings result
    ''' </summary>
    <XmlType("uniprot")> Public Class UniProtXML

        Const ns$ = "xmlns=""http://uniprot.org/uniprot"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xsi:schemaLocation=""http://uniprot.org/uniprot http://www.uniprot.org/support/docs/uniprot.xsd"""

        ''' <summary>
        ''' ```xml
        ''' &lt;uniparc xmlns="http://uniprot.org/uniparc" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:schemaLocation="http://uniprot.org/uniparc http://www.uniprot.org/docs/uniparc.xsd" version="2017_03"> 
        ''' ```
        ''' </summary>
        Const uniparc_ns$ = "xmlns=""http://uniprot.org/uniparc"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xsi:schemaLocation=""http://uniprot.org/uniparc http://www.uniprot.org/docs/uniparc.xsd"""

        ''' <summary>
        ''' <see cref="entry.accession"/>可以用作为字典的键名
        ''' </summary>
        ''' <returns></returns>
        <XmlElement("entry")>
        Public Property entries As entry()
        <XmlElement>
        Public Property copyright As String
        <XmlAttribute>
        Public Property version As String

        ''' <summary>
        ''' 从文件系统中的rdf文件加载XML反序列化数据结果
        ''' </summary>
        ''' <param name="path$"></param>
        ''' <returns></returns>
        Public Shared Function Load(path$) As UniProtXML
            Dim xml As String = path.ReadAllText

            If InStr(xml, "<uniparc xmlns=", CompareMethod.Text) > 0 Then
                xml = xml.Replace(UniProtXML.uniparc_ns, Xmlns.DefaultXmlns)
                xml = xml.Replace("<uniparc xmlns", "<uniprot xmlns")
                xml = xml.Replace("</uniparc>", "</uniprot>")
            Else
                xml = xml.Replace(UniProtXML.ns, Xmlns.DefaultXmlns)
            End If

            Dim model As UniProtXML = xml.LoadFromXml(Of UniProtXML)
            Return model
        End Function

        ''' <summary>
        ''' Enumerate all of the data entries in a ultra large size uniprot XML database.
        ''' (使用这个函数来读取超大的uniprot XML数据库)
        ''' </summary>
        ''' <param name="path$"></param>
        ''' <returns></returns>
        Public Shared Function EnumerateEntries(path$) As IEnumerable(Of entry)
            Return path.LoadUltraLargeXMLDataSet(Of entry)(xmlns:="http://uniprot.org/uniprot")
        End Function

        ''' <summary>
        ''' 因为可能会存在一个蛋白质entry对应多个accession的情况，
        ''' 所以这个函数会自动将这些重复的<see cref="entry.accessions"/>进行展开，
        ''' 则取出唯一的accessionID只需要使用表达式
        ''' 
        ''' ```vbnet
        ''' DirectCast(entry, <see cref="InamedValue"/>).Key
        ''' ```
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

            Return CreateTable(source)
        End Function

        Private Shared Function CreateTable(source As entry()) As Dictionary(Of entry)
            Dim groups = From protein As entry
                         In source _
                             .Select(Function(o) o.ShadowCopy) _
                             .IteratesALL
                         Select protein
                         Group protein By DirectCast(protein, INamedValue).Key Into Group
            Dim out As Dictionary(Of entry) =
                groups _
                .Select(Function(g) g.Group.First) _
                .ToDictionary

            Return out
        End Function

        ''' <summary>
        ''' 对<see cref="entries"/>属性使用<see cref="entry.accessions"/>建立索引
        ''' </summary>
        ''' <returns></returns>
        Public Function ToIndexTable() As Dictionary(Of entry)
            Return CreateTable(entries)
        End Function

        Public Overrides Function ToString() As String
            Return GetXml
        End Function
    End Class
End Namespace
