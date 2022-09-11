#Region "Microsoft.VisualBasic::74ee32f975c80d25167c460d5dd95869, GCModeller\core\Bio.Assembly\Assembly\ELIXIR\UniProt\XML\Model\entry.vb"

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

    '   Total Lines: 126
    '    Code Lines: 79
    ' Comment Lines: 33
    '   Blank Lines: 14
    '     File Size: 4.85 KB


    '     Class entry
    ' 
    '         Properties: accession, accessions, CommentList, comments, created
    '                     dataset, dbReferences, features, gene, keywords
    '                     modified, name, organism, protein, proteinExistence
    '                     references, sequence, version, xrefs
    ' 
    '         Function: ShadowCopy, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Assembly.Uniprot.XML

    ''' <summary>
    ''' Describes a UniProtKB entry.
    ''' (因为<see cref="accessions"/>可能会出现多个值，所以会需要使用
    ''' <see cref="entry.ShadowCopy()"/>函数来解决实体多态的问题。
    ''' 经过shadow copy之后可以使用主键<see cref="accession"/>来创建字典)
    ''' </summary>
    ''' 
    Public Class entry : Implements INamedValue

        <XmlAttribute> Public Property dataset As String
        <XmlAttribute> Public Property created As String
        <XmlAttribute> Public Property modified As String
        <XmlAttribute> Public Property version As String

        ''' <summary>
        ''' shadow copy之后的唯一标识符
        ''' </summary>
        ''' <returns></returns>
        <XmlIgnore>
        Private Property accession As String Implements INamedValue.Key

        ''' <summary>
        ''' 蛋白质的唯一标识符，可以用作为字典的键名
        ''' </summary>
        ''' <returns></returns>
        <XmlElement("accession")>
        Public Property accessions As String()
        Public Property name As String
        Public Property protein As protein
        <XmlElement("feature")>
        Public Property features As feature()
        Public Property gene As gene
        Public Property proteinExistence As value
        Public Property organism As organism

        ''' <summary>
        ''' 当前的这个蛋白质的蛋白序列，在Uniprot数据库之中，蛋白记录只有蛋白序列
        ''' 没有核酸序列，如果需要核酸序列，则会需要通过accession编号从nt库之中
        ''' 提取出来
        ''' </summary>
        ''' <returns></returns>
        Public Property sequence As sequence

        <XmlElement("keyword")> Public Property keywords As value()
        <XmlElement("comment")> Public Property comments As comment()
            Get
                Return CommentList.Values.ToVector
            End Get
            Set(value As comment())
                If value Is Nothing Then
                    _CommentList = New Dictionary(Of String, comment())
                Else
                    _CommentList = value _
                        .OrderBy(Function(c) c.type) _
                        .GroupBy(Function(c) c.type) _
                        .ToDictionary(Function(t) t.Key,
                                      Function(v)
                                          Return v.ToArray
                                      End Function)
                End If
            End Set
        End Property

        ''' <summary>
        ''' Dictionary table can be read from <see cref="xrefs"/> property
        ''' </summary>
        ''' <returns></returns>
        <XmlElement("dbReference")> Public Property dbReferences As dbReference()
            Get
                Return xrefs.Values.ToVector
            End Get
            Set(value As dbReference())
                If value Is Nothing Then
                    _xrefs = New Dictionary(Of String, dbReference())
                    Return
                End If

                _xrefs = value _
                    .OrderBy(Function(ref) ref.type) _
                    .GroupBy(Function(ref) ref.type) _
                    .ToDictionary(Function(t) t.Key,
                                  Function(v)
                                      Return v.ToArray
                                  End Function)
            End Set
        End Property

        <XmlElement("reference")> Public Property references As reference()
        <XmlIgnore>
        Public ReadOnly Property CommentList As Dictionary(Of String, comment())
        ''' <summary>
        ''' <see cref="dbReferences"/> table
        ''' </summary>
        ''' <returns></returns>
        <XmlIgnore>
        Public ReadOnly Property xrefs As Dictionary(Of String, dbReference())

        Public Overrides Function ToString() As String
            Return accessions.GetJson
        End Function

        ''' <summary>
        ''' 这个是处理有多个accession的情况
        ''' </summary>
        ''' <returns></returns>
        Public Function ShadowCopy() As entry()
            Dim list As New List(Of entry)

            For Each accID As String In accessions
                Dim o As entry = TryCast(Me.MemberwiseClone, entry)
                o.accession = accID
                list += o
            Next

            Return list
        End Function
    End Class
End Namespace
