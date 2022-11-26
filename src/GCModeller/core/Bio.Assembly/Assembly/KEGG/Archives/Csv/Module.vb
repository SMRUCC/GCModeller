#Region "Microsoft.VisualBasic::f1422bf711a0478493750cae9d83b900, GCModeller\core\Bio.Assembly\Assembly\KEGG\Archives\Csv\Module.vb"

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

    '   Total Lines: 111
    '    Code Lines: 76
    ' Comment Lines: 19
    '   Blank Lines: 16
    '     File Size: 3.96 KB


    '     Class [Module]
    ' 
    '         Properties: [Class], briteID, Category, EntryId, Name
    '                     NumberGenes, PathwayGenes, Reactions, Type
    ' 
    '         Function: [Imports], GenerateObject, GetPathwayGenes
    ' 
    ' 
    ' /********************************************************************************/

#End Region

#If netcore5 = 0 Then
Imports System.Data.Linq.Mapping
#Else
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps
#End If
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.ComponentModel.Annotation

Namespace Assembly.KEGG.Archives.Csv

    Public Class [Module] : Inherits PathwayBrief
        Implements IKeyValuePairObject(Of String, String())

        Public Overrides Property EntryId As String Implements IKeyValuePairObject(Of String, String()).Key
            Get
                Return MyBase.EntryId
            End Get
            Set(value As String)
                MyBase.EntryId = value
            End Set
        End Property

        <Column(Name:="Num.Genes")> Public ReadOnly Property NumberGenes As Integer
            Get
                Return PathwayGenes.Length
            End Get
        End Property

        Public Property PathwayGenes As String() Implements IKeyValuePairObject(Of String, String()).Value

        Public Overrides Function GetPathwayGenes() As String()
            Return PathwayGenes
        End Function

#Region "Brite Infomation"

        ''' <summary>
        ''' A
        ''' </summary>
        ''' <returns></returns>
        <Column(Name:="br.Type")> Public Property Type As String
        ''' <summary>
        ''' B
        ''' </summary>
        ''' <returns></returns>
        <Column(Name:="br.Class")>
        Public Property [Class] As String
        ''' <summary>
        ''' C
        ''' </summary>
        ''' <returns></returns>
        <Column(Name:="br.Category")>
        Public Property Category As String
#End Region

        Public Property Name As String
        Public Property Reactions As String()

        Public Overrides ReadOnly Property briteID As String
            Get
                Return EntryId.Split("_"c).Last
            End Get
        End Property

        Public Shared Function GenerateObject(XmlModel As KEGG.DBGET.bGetObject.Module) As [Module]
            Dim ReactionIdlist As String()

            If XmlModel.Reaction.IsNullOrEmpty Then
                ReactionIdlist = New String() {}
            Else
                ReactionIdlist = LinqAPI.Exec(Of String) <= From rxn As NamedValue
                                                            In XmlModel.reaction
                                                            Select rxn.name
                                                            Order By name Ascending
            End If

            Return New [Module] With {
                .EntryId = XmlModel.EntryId,
                .description = XmlModel.Description,
                .PathwayGenes = XmlModel.GetPathwayGenes,
                .Name = XmlModel.Name,
                .Reactions = ReactionIdlist
            }
        End Function

        ''' <summary>
        ''' 导出XML文件之中的数据至Csv文件之中
        ''' </summary>
        ''' <typeparam name="TSource"></typeparam>
        ''' <param name="Data"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function [Imports](Of TSource As IEnumerable(Of KEGG.DBGET.bGetObject.Module))(Data As TSource) As [Module]()
            Dim mods = (From kmod As KEGG.DBGET.bGetObject.Module
                        In Data
                        Select GenerateObject(kmod)).ToArray
            Dim defBr = KEGG.DBGET.BriteHEntry.Module.GetDictionary

            For Each kmod As [Module] In mods
                Dim brMod = defBr(kmod.briteID)
                kmod.Type = brMod.Class
                kmod.Class = brMod.Category
                kmod.Category = brMod.SubCategory
            Next

            Return mods
        End Function
    End Class
End Namespace
