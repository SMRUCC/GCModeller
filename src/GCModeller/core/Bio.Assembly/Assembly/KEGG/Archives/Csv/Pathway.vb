#Region "Microsoft.VisualBasic::032ecb99bed27a7d657ffa50b7a21617, GCModeller\core\Bio.Assembly\Assembly\KEGG\Archives\Csv\Pathway.vb"

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

    '   Total Lines: 120
    '    Code Lines: 69
    ' Comment Lines: 34
    '   Blank Lines: 17
    '     File Size: 4.51 KB


    '     Class Pathway
    ' 
    '         Properties: [Class], briteID, Category, EntryId, PathwayGenes
    ' 
    '         Function: (+2 Overloads) CreateObjects, GenerateObject, GetPathwayGenes, LoadData
    ' 
    ' 
    ' /********************************************************************************/

#End Region

#If netcore5 = 0 Then
Imports System.Data.Linq.Mapping
#Else
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps
#End If
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Assembly.KEGG.DBGET
Imports SMRUCC.genomics.ComponentModel.Annotation

Namespace Assembly.KEGG.Archives.Csv

    ''' <summary>
    ''' CSV data model for storage the kegg pathway brief information.(用于向Csv文件保存数据的简单格式的代谢途径数据存储对象)
    ''' </summary>
    ''' <remarks></remarks>
    Public Class Pathway : Inherits PathwayBrief
        Implements IKeyValuePairObject(Of String, String())

        Public Overrides Function GetPathwayGenes() As String()
            Return PathwayGenes
        End Function

        ''' <summary>
        ''' Pathway object KEGG database entry id.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Property EntryId As String Implements IKeyValuePairObject(Of String, String()).Key
            Get
                Return MyBase.EntryId
            End Get
            Set(value As String)
                MyBase.EntryId = value
            End Set
        End Property

        ''' <summary>
        ''' Phenotype Class
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Column(Name:="br.Class")> Public Property [Class] As String

        ''' <summary>
        ''' Phenotype Category
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Column(Name:="br.Category")> Public Property Category As String

        ''' <summary>
        ''' The enzyme gene which was involved in this pathway catalysts.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property PathwayGenes As String() Implements IKeyValuePairObject(Of String, String()).Value

        Public Overrides ReadOnly Property briteID As String
            Get
                Return Regex.Match(EntryId, "\d+").Value
            End Get
        End Property

        Public Shared Function GenerateObject(XmlModel As bGetObject.Pathway) As Pathway
            Return New Pathway With {
                .EntryId = XmlModel.EntryId,
                .description = XmlModel.description,
                .PathwayGenes = XmlModel.GetPathwayGenes
            }
        End Function

        ''' <summary>
        ''' 将所下载的代谢途径数据转换为CSV文档保存
        ''' </summary>
        ''' <param name="directory"></param>
        ''' <param name="sp_code">物种名称的三字母简写，例如xcb</param>
        ''' <returns></returns>
        Public Shared Function LoadData(directory$, sp_code$) As Pathway()
            Dim XMLFiles As KEGG.DBGET.bGetObject.Pathway() =
                (ls - l - r - "*.xml" <= directory) _
                .Select(AddressOf SafeLoadXml(Of bGetObject.Pathway)) _
                .ToArray
            Return CreateObjects(XMLFiles, sp_code)
        End Function

        Public Shared Function CreateObjects(source As KEGG.DBGET.bGetObject.Pathway(), spCode As String) As Pathway()
            Dim classDictionary As New SortedDictionary(Of String, DBGET.BriteHEntry.Pathway)
            For Each pwyData In KEGG.DBGET.BriteHEntry.Pathway.LoadFromResource
                Call classDictionary.Add($"{spCode}{pwyData.entry.name }", pwyData)
            Next

            Dim PathwayList As New List(Of Pathway)

            For Each pwyData As bGetObject.Pathway In source
                Dim PathwayObject = Pathway.GenerateObject(pwyData)
                Dim BriteEntry As BriteHEntry.Pathway = classDictionary(PathwayObject.EntryId)

                PathwayObject.Class = BriteEntry.class
                PathwayObject.Category = BriteEntry.category

                PathwayList += PathwayObject
            Next

            Return PathwayList
        End Function

        Public Shared Function CreateObjects(Model As KEGG.Archives.Xml.XmlModel) As Pathway()
            Dim Pathways = Model.GetAllPathways
            Return CreateObjects(Pathways, Model.spCode)
        End Function
    End Class
End Namespace
