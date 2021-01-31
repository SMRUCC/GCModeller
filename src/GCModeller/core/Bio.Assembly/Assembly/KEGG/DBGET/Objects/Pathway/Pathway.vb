﻿#Region "Microsoft.VisualBasic::7465c8ed35ecb9fe481d32e20d436fe2, core\Bio.Assembly\Assembly\KEGG\DBGET\Objects\Pathway\Pathway.vb"

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

    '     Class Pathway
    ' 
    '         Properties: briteID, compound, disease, drugs, genes
    '                     KOpathway, modules, name, organism, otherDBs
    '                     pathwayMap, references
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: (+2 Overloads) DownloadPage, (+2 Overloads) GetCompoundCollection, GetPathwayGenes, IsContainsCompound, IsContainsGeneObject
    '                   IsContainsModule
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.ComponentModel.Annotation
Imports SMRUCC.genomics.ComponentModel.DBLinkBuilder

Namespace Assembly.KEGG.DBGET.bGetObject

    ''' <summary>
    ''' The kegg pathway annotation data.(这个代谢途径模型是针对某一个物种而言的)
    ''' </summary>
    ''' <remarks></remarks>
    <XmlRoot("KEGG_pathway", Namespace:="http://www.genome.jp/kegg/pathway.html")>
    <XmlType("pathway", Namespace:="http://www.genome.jp/kegg/pathway.html")>
    Public Class Pathway : Inherits PathwayBrief

        ''' <summary>
        ''' The name value of this pathway object
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlAttribute>
        Public Property name As String

        ''' <summary>
        ''' The module entry collection data in this pathway.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property modules As NamedValue()
        ''' <summary>
        ''' The kegg compound entry collection data in this pathway.
        ''' (可以通过这个代谢物的列表得到可以出现在当前的这个代谢途径之中的所有的非酶促反应过程，
        ''' 将整个基因组里面的化合物合并起来则可以得到整个细胞内可能存在的非酶促反应过程) 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property compound As NamedValue()
        Public Property drugs As NamedValue()
        <XmlArray("otherDB", [Namespace]:=LICENSE.GCModeller)>
        Public Property otherDBs As DBLink()
        Public Property pathwayMap As NamedValue

        Public Property genes As NamedValue()
            Get
                Return _genes
            End Get
            Set
                _genes = Value

                If Not Value.IsNullOrEmpty Then
                    _geneTable = Value _
                        .ToDictionary(Function(gene)
                                          Return gene.name.Split(":"c).Last
                                      End Function)
                Else
                    _geneTable = New Dictionary(Of String, NamedValue)
                End If
            End Set
        End Property

        <XmlIgnore>
        Public Overrides ReadOnly Property briteID As String
            Get
                Dim id As String = Regex.Match(EntryId, "\d+").Value

                If String.IsNullOrEmpty(id) Then
                    Return EntryId
                Else
                    Return id
                End If
            End Get
        End Property

        Public Property disease As NamedValue()
        Public Property organism As KeyValuePair
        Public Property KOpathway As NamedValue()

        ''' <summary>
        ''' Reference list of this biological pathway
        ''' </summary>
        ''' <returns></returns>
        Public Property references As Reference()

        Dim _genes As NamedValue()
        Dim _geneTable As New Dictionary(Of String, NamedValue)

        <XmlNamespaceDeclarations()>
        Public xmlns As XmlSerializerNamespaces

        Sub New()
            xmlns = New XmlSerializerNamespaces

            xmlns.Add("gcmodeller", LICENSE.GCModeller)
        End Sub

        Public Function IsContainsCompound(KEGGCompound As String) As Boolean
            If compound.IsNullOrEmpty Then
                Return False
            End If

            Dim find = LinqAPI.DefaultFirst(Of NamedValue)() <=
 _
                From comp As NamedValue
                In compound
                Where String.Equals(comp.name, KEGGCompound)
                Select comp

            Return Not find Is Nothing AndAlso (Not find.name.StringEmpty OrElse Not find.text.StringEmpty)
        End Function

        Public Function IsContainsGeneObject(GeneId As String) As Boolean
            Return _geneTable.ContainsKey(GeneId)
        End Function

        ''' <summary>
        ''' Is current pathway object contains the specific module information?(当前的代谢途径对象是否包含有目标模块信息.)
        ''' </summary>
        ''' <param name="ModuleId"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function IsContainsModule(ModuleId As String) As Boolean
            If modules.IsNullOrEmpty Then
                Return False
            End If
            Dim LQuery As KeyValuePair = LinqAPI.DefaultFirst(Of KeyValuePair) <=
 _
                From [mod] As NamedValue
                In modules
                Where String.Equals([mod].name, ModuleId)
                Select [mod]

            Return Not LQuery Is Nothing
        End Function

        Public Shared Function GetCompoundCollection(source As IEnumerable(Of Pathway)) As String()
            Dim out As New List(Of String)

            For Each pwy As Pathway In source
                If pwy.compound.IsNullOrEmpty Then
                    Continue For
                End If

                out += From met As NamedValue
                       In pwy.compound
                       Select met.name
            Next

            Return LinqAPI.Exec(Of String) <=
 _
                From sId As String
                In out
                Where Not String.IsNullOrEmpty(sId)
                Select sId
                Distinct
                Order By sId Ascending

        End Function

        ''' <summary>
        ''' Imports KEGG compounds from pathways model.
        ''' </summary>
        ''' <param name="ImportsDIR"></param>
        ''' <returns></returns>
        Public Shared Function GetCompoundCollection(ImportsDIR As String) As String()
            Dim LQuery As IEnumerable(Of Pathway) =
 _
                From xml As String
                In ls - l - r - wildcards("*.xml") <= ImportsDIR
                Select xml.LoadXml(Of Pathway)()

            Return GetCompoundCollection(LQuery)
        End Function

        ''' <summary>
        ''' 获取这个代谢途径之中的所有的基因。这个是安全的函数，当出现空值的基因集合的时候函数会返回一个空集合而非空值
        ''' </summary>
        ''' <returns></returns>
        Public Overrides Function GetPathwayGenes() As String()
            If genes.IsNullOrEmpty Then
                Call $"{EntryId} gene set is Null!".__DEBUG_ECHO
                Return {}
            End If

            Dim LQuery As String() = genes _
                .Select(Function(g) g.name.Split(":"c).Last) _
                .ToArray
            Return LQuery
        End Function

        Const PATHWAY_DBGET As String = "http://www.genome.jp/dbget-bin/www_bget?pathway:{0}{1}"

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function DownloadPage(sp As String, entry As String) As Pathway
            Return String.Format(PATHWAY_DBGET, sp, entry).PageParser
        End Function

        ''' <summary>
        ''' 从某一个页面url或者文件路径所指向的网页文件之中解析出模型数据
        ''' </summary>
        ''' <param name="url"></param>
        ''' <returns></returns>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function DownloadPage(url As String) As Pathway
            Return url.PageParser
        End Function
    End Class
End Namespace
