#Region "Microsoft.VisualBasic::660da1239ce8c561b66a593f690d571b, core\Bio.Assembly\Assembly\MetaCyc\File\AttributeValue DataFile\DataFiles\Slots\ObjectBase\Object.vb"

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

    '     Class [Object]
    ' 
    '         Properties: AbbrevName, Citations, Comment, CommonName, DBLinks
    '                     DBLinksMgr, Identifier, Names, Synonyms, Table
    '                     Types
    ' 
    '         Function: GetDBLinks
    '         Enum Tables
    ' 
    '             dnabindsites, enzrxns, genes, pathways, promoters
    '             proteinfeatures, proteins, protligandcplxes, pubs, reactions
    '             regulation, regulons, species, terminators, transunits
    '             trna
    ' 
    ' 
    ' 
    '  
    ' 
    '     Properties: Item
    ' 
    '     Constructor: (+2 Overloads) Sub New
    ' 
    '     Function: Exists, StringQuery, ToString
    ' 
    '     Sub: [TypeCast], CopyTo
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text.RegularExpressions
Imports System.Xml.Serialization
Imports SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.Language

Namespace Assembly.MetaCyc.File.DataFiles.Slots

    ''' <summary>
    ''' The object type is the base type of the objects definition both in the namespace PGDB.DataFile and PGDB.Schemas 
    ''' </summary>
    ''' <remarks></remarks>
    <XmlType("MetaCyc-Slot-Object")>
    Public Class [Object] : Implements INamedValue

        <MetaCycField(Name:="UNIQUE-ID")> <XmlAttribute>
        Public Overridable Property Identifier As String Implements INamedValue.Key

        ''' <summary>
        ''' (Common-Name) This slot defines the primary name by which an object is known 
        ''' to scientists -- a widely used and familiar name (in some cases arbitrary 
        ''' choices must be made). This field can have only one value; that value must 
        ''' be a string.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <MetaCycField()> <XmlAttribute> Public Overridable Property CommonName As String
        ''' <summary>
        ''' (Abbrev-Name) This slot stores an abbreviated name for an object. It is used in 
        ''' some displays.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <MetaCycField()> <XmlAttribute> Public Overridable Property AbbrevName As String

        ''' <summary>
        ''' (Synonyms) This field defines one or more secondary names for an object -- names 
        ''' that a scientist might attempt to use to retrieve the object. These names may be 
        ''' out of date or ambiguous, but are used to facilitate retrieval -- the Synonyms 
        ''' should include any name that you might use to try to retrieve an object. In a 
        ''' sense, the name "Synonyms" is misleading because the names listed in this slot may 
        ''' not be exactly synonymous with the preferred name of the object.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <MetaCycField(Type:=MetaCycField.Types.TStr)> <XmlElement> Public Overridable Property Synonyms As String()
        <XmlElement> Public Overridable Property Names As List(Of String)
        ''' <summary>
        ''' (Comment) The Comment slot stores a general comment about the object that contains 
        ''' the slot. The comment should always be enclosed in double quotes.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <MetaCycField()> <XmlElement> Public Overridable Property Comment As String
        ''' <summary>
        ''' (Citations) This slot lists general citations pertaining to the object containing 
        ''' the slot. Citations may or may not have evidence codes attached to them. Each value 
        ''' of the slot is a string of the form 
        ''' [reference-ID] or 
        ''' [reference-id:evidence-code:timestamp:curator:probability:with]
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <MetaCycField(Type:=MetaCycField.Types.TStr)> <XmlElement> Public Overridable Property Citations As List(Of String)

        ''' <summary>
        ''' The TYPES enumerate values in each object.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <MetaCycField(Type:=MetaCycField.Types.TStr)> <XmlArray> Public Overridable Property Types As List(Of String)
        <MetaCycField(Type:=MetaCycField.Types.TStr, Name:="DBLINKS")> Public Overridable Property DBLinks As String()
            Get
                Return _DBLinks.DBLinks
            End Get
            Set(value As String())
                _DBLinks = Schema.DBLinkManager.CreateFromMetaCycFormat(value)
            End Set
        End Property

        Protected _innerHash As Dictionary(Of String, String())
        Protected _DBLinks As Schema.DBLinkManager

        Public ReadOnly Property DBLinksMgr As Schema.DBLinkManager
            Get
                Return _DBLinks
            End Get
        End Property

        ''' <summary>
        ''' (解析Unique-Id字段的值所需要的正则表达式)
        ''' </summary>
        ''' <remarks></remarks>
        Public Const UNIQUE_ID_REGX As String = "[0-9A-Z]+([-][0-9A-Z]*)*"

        ''' <summary>
        ''' 当前的对象所属的表对象
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlIgnore> Public Overridable ReadOnly Property Table As [Object].Tables
            Get
                Return Tables.classes
            End Get
        End Property

        Public Function GetDBLinks() As Schema.DBLinkManager
            Return _DBLinks
        End Function

        Public Enum Tables
            bindrxns = 0
            classes = -1
            compounds = 1
            dnabindsites
            enzrxns
            genes
            pathways
            promoters
            proteinfeatures
            proteins
            protligandcplxes
            pubs
            reactions
            regulation
            regulons
            species
            terminators
            transunits
            trna
        End Enum

        ''' <summary>
        ''' 使用关键词查询<see cref="[Object]._innerHash"></see>字典对象
        ''' </summary>
        ''' <param name="Key"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function StringQuery(Key As String, Optional emptyNull As Boolean = False) As List(Of String)
            If Not _innerHash.ContainsKey(Key) Then
                Dim QueryString As String = Key.ToUpper
                If Not _innerHash.ContainsKey(QueryString) Then
                    Return If(emptyNull, New List(Of String), Nothing)
                Else
                    Return _innerHash(QueryString).AsList
                End If
            Else
                Return _innerHash(Key).AsList
            End If
        End Function

        Public Sub CopyTo(Of T As [Object])(ByRef target As T)
            With target
                .AbbrevName = AbbrevName
                .Citations = Citations
                .Comment = Comment
                .CommonName = CommonName
                .Names = Names
                ._innerHash = _innerHash
                .Synonyms = Synonyms
                .Identifier = Identifier
                .Types = Types
            End With
        End Sub

        ''' <summary>
        ''' 查询某一个键名是否存在于这个对象之中
        ''' </summary>
        ''' <param name="KeyName">键名</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Exists(KeyName As String) As Boolean
            Return _innerHash.ContainsKey(KeyName) OrElse
                _innerHash.ContainsKey(KeyName.ToUpper)
        End Function

        Public ReadOnly Property Item(Name As String) As String()
            Get
                Return StringQuery(Name).ToArray
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return Identifier
        End Function

        Sub New()
        End Sub

        Sub New(lst As Dictionary(Of String, String()))
            _innerHash = lst
        End Sub

        ''' <summary>
        ''' 基类至派生类的转换
        ''' </summary>
        ''' <param name="target">数据源，基类</param>
        ''' <param name="ToType">转换至的目标类型</param> 
        ''' <typeparam name="T">类型约束</typeparam> 
        ''' <remarks></remarks>
        Public Shared Sub [TypeCast](Of T As [Object])(target As [Object], ByRef ToType As T)
            With ToType
                .AbbrevName = target.AbbrevName
                .Citations = target.Citations
                .Comment = target.Comment
                .CommonName = target.CommonName
                .Names = target.Names
                ._innerHash = target._innerHash
                .Synonyms = target.Synonyms
                .Identifier = target.Identifier
                .Types = target.Types
            End With
        End Sub
    End Class
End Namespace
