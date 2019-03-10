﻿#Region "Microsoft.VisualBasic::e314b56d226fdd24f3d2e9b9e313fc52, Bio.Assembly\Assembly\KEGG\Archives\Csv\Module.vb"

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

    '     Class [Module]
    ' 
    '         Properties: [Class], BriteId, Category, EntryId, Name
    '                     NumberGenes, PathwayGenes, Reactions, Type
    ' 
    '         Function: [Imports], GenerateObject, GetPathwayGenes
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Data.Linq.Mapping
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Text.Xml.Models

Namespace Assembly.KEGG.Archives.Csv

    Public Class [Module] : Inherits ComponentModel.PathwayBrief
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

        Public Overrides ReadOnly Property BriteId As String
            Get
                Return EntryId.Split("_"c).Last
            End Get
        End Property

        Public Shared Function GenerateObject(XmlModel As KEGG.DBGET.bGetObject.Module) As [Module]
            Dim ReactionIdlist As String()

            If XmlModel.Reaction.IsNullOrEmpty Then
                ReactionIdlist = New String() {}
            Else
                ReactionIdlist =
                    LinqAPI.Exec(Of String) <= From rxn As KeyValuePair
                                               In XmlModel.Reaction
                                               Select rxn.Key
                                               Order By Key Ascending
            End If

            Return New [Module] With {
                .EntryId = XmlModel.EntryId,
                .Description = XmlModel.Description,
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
                Dim brMod = defBr(kmod.BriteId)
                kmod.Type = brMod.Class
                kmod.Class = brMod.Category
                kmod.Category = brMod.SubCategory
            Next

            Return mods
        End Function
    End Class
End Namespace
