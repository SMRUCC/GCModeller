﻿#Region "Microsoft.VisualBasic::3d289d2109b2ceae319f85d5191fb14b, Bio.Repository\KEGG\KEGGOrthology\OrganismModel.vb"

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

    ' Class OrganismModel
    ' 
    '     Properties: genome, organism
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: CreateModel, EnumerateModules, GetGenbankSource, ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.Organism

''' <summary>
''' The <see cref="Pathway"/> repository
''' </summary>
''' 
<XmlType("organism-model", [Namespace]:="http://www.genome.jp/kegg/pathway.html")>
Public Class OrganismModel

    <XmlElement("genome")>
    Public Property organism As OrganismInfo
    ''' <summary>
    ''' 基因组是由代谢途径功能模块所构成的
    ''' </summary>
    ''' <returns></returns>
    <XmlArray("modules", [Namespace]:="http://www.genome.jp/kegg/pathway.html")>
    Public Property genome As Pathway()

    <XmlNamespaceDeclarations()>
    Public xmlns As New XmlSerializerNamespaces

    Sub New()
        Call xmlns.Add("gcmodeller", LICENSE.GCModeller)
        ' Call xmlns.Add("kegg_pathway", "http://www.genome.jp/kegg/pathway.html")
    End Sub

    Public Overrides Function ToString() As String
        Return organism.ToString
    End Function

    ''' <summary>
    ''' Get NCBI genbank reference sequence assembly name
    ''' </summary>
    ''' <returns></returns>
    Public Function GetGenbankSource() As String
        Try
            Return organism _
                .DataSource _
                .Where(Function(d)
                           Return d.name.TextEquals("genbank") OrElse
                                  d.name.TextEquals("RefSeq")
                       End Function) _
                .First _
                .text _
                .Split("/"c) _
                .Last
        Catch ex As Exception
            Call App.LogException(ex)
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' 从KEGG的代谢途径下载文件夹加载零散的文件数据构成这个整体数据模型
    ''' </summary>
    ''' <param name="directory"></param>
    ''' <returns></returns>
    Public Shared Function CreateModel(directory As String) As OrganismModel
        Dim organism As OrganismInfo = (directory & "/kegg.json").LoadJSON(Of OrganismInfo)
        Dim model As Pathway() = (ls - l - r - "*.Xml" <= directory) _
            .Select(AddressOf LoadXml(Of Pathway)) _
            .ToArray

        Return New OrganismModel With {
            .genome = model,
            .organism = organism
        }
    End Function

    Public Shared Function EnumerateModules(handle As String) As IEnumerable(Of Pathway)
        If handle.FileExists AndAlso handle.ExtensionSuffix.TextEquals("Xml") Then
            Return handle.LoadXml(Of OrganismModel).genome
        Else
            Return (ls - l - r - "*.Xml" <= handle) _
                .Select(AddressOf LoadXml(Of Pathway))
        End If
    End Function
End Class
