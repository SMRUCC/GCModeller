#Region "Microsoft.VisualBasic::b367694a258f4be910856665ad672f5f, core\Bio.Assembly\Assembly\ELIXIR\UniProt\XML\Model\UniprotXML.vb"

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

    '   Total Lines: 163
    '    Code Lines: 102 (62.58%)
    ' Comment Lines: 42 (25.77%)
    '    - Xml Docs: 88.10%
    ' 
    '   Blank Lines: 19 (11.66%)
    '     File Size: 7.83 KB


    '     Class UniProtXML
    ' 
    '         Properties: copyright, entries, version
    ' 
    '         Function: [GetType], CreateTable, (+2 Overloads) EnumerateEntries, Load, LoadDictionary
    '                   LoadXml, ToIndexTable, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text.Xml
Imports Microsoft.VisualBasic.Text.Xml.Linq

Namespace Assembly.Uniprot.XML

    ''' <summary>
    ''' Describes a collection of UniProtKB entries, XML file can be download from the uniprot database id mappings result.
    ''' </summary>
    <XmlType("uniprot", [Namespace]:=UniProtXML.uniprot_xmlns), XmlRoot("uniprot", [Namespace]:=UniProtXML.uniprot_xmlns)>
    Public Class UniProtXML

        Public Const uniprot_xmlns As String = "http://uniprot.org/uniprot"

        Const ns$ = "xmlns=""" & uniprot_xmlns & """ xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xsi:schemaLocation=""http://uniprot.org/uniprot http://www.uniprot.org/support/docs/uniprot.xsd"""

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
        ''' 从文件系统中的rdf文件加载XML反序列化数据结果.(这个函数只适用于小文件的加载操作)
        ''' </summary>
        ''' <param name="path">XML文件路径</param>
        ''' <returns></returns>
        Public Shared Function Load(path As String) As UniProtXML
            Return LoadXml(path.ReadAllText)
        End Function

        Public Shared Function LoadXml(xml As String) As UniProtXML
            If InStr(xml, "<uniparc xmlns=", CompareMethod.Text) > 0 Then
                xml = xml.Replace(UniProtXML.uniparc_ns, Xmlns.DefaultXmlns)
                xml = xml.Replace("<uniparc xmlns", "<uniprot xmlns")
                xml = xml.Replace("</uniparc>", "</uniprot>")
            Else
                xml = xml.Replace(UniProtXML.ns, Xmlns.DefaultXmlns)
            End If

            Return xml.LoadFromXml(Of UniProtXML)
        End Function

        Public Overloads Shared Function [GetType](file As String) As String
            Dim head As String() = file.Peeks.Split(">"c)
            ' 第一个元素为 <?xml version='1.0' encoding='UTF-8'?> 文件申明
            ' 第二个元素则肯定是Xml文件的根节点名称
            ' 判断一下<uniparc是否在第一个位置即可了解数据库的类型
            Dim rootName = head(1).Trim.Split.First.Trim("<"c)
            Return rootName
        End Function

        ''' <summary>
        ''' Enumerate all of the data entries in a ultra large size uniprot XML database.
        ''' (使用这个函数来读取超大的uniprot XML数据库)
        ''' </summary>
        ''' <param name="path">因为uniprot和uniparc这两个数据库的数据结构都是一样的,所以可以使用这个函数来兼容这两个数据库</param>
        ''' <returns></returns>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function EnumerateEntries(path$,
                                                Optional isUniParc As Boolean = False,
                                                Optional ignoreError As Boolean = False,
                                                Optional tqdm As Boolean = False) As IEnumerable(Of entry)
            If isUniParc Then
                Return path.LoadUltraLargeXMLDataSet(Of entry)(xmlns:="http://uniprot.org/uniparc",
                                                               ignoreError:=ignoreError,
                                                               tqdm:=tqdm)
            Else
                Return path.LoadUltraLargeXMLDataSet(Of entry)(xmlns:=uniprot_xmlns,
                                                               ignoreError:=ignoreError,
                                                               tqdm:=tqdm)
            End If
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function EnumerateEntries(files$(),
                                                Optional isUniParc As Boolean = False,
                                                Optional ignoreError As Boolean = False,
                                                Optional tqdm As Boolean = False) As IEnumerable(Of entry)
            Return files _
                .Select(Function(path)
                            Call $"Populate uniprot proteins {path} [{StringFormats.Lanudry(bytes:=path.FileLength)}]".info
                            Return EnumerateEntries(path, isUniParc,
                                                    ignoreError:=ignoreError,
                                                    tqdm:=tqdm)
                        End Function) _
                .IteratesALL
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
