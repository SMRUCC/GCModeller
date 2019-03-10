﻿#Region "Microsoft.VisualBasic::1b1a3fa5bc6c63da4e04a37c65e2b941, GCModeller.Framework.Kernel_Driver\DataServices\ModelBase\API.vb"

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

    '     Module API
    ' 
    '         Function: __add_author, __add_email, __add_publication, __add_reversion, __add_URL
    '                   _set_Comments, _set_DBLinks, _set_Name, _set_Title, set_SpeciesId
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports Microsoft.VisualBasic.Extensions
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData

Namespace LDM

    <Package("GCModeller.Property", Publisher:="GCModeller Virtual Cell System", Description:="")>
    Public Module API

#Region "Shell API"

        <ExportAPI("Write.Name")>
        Public Function _set_Name(model As ModelBaseType, value As String) As ModelBaseType
            model.ModelProperty.Name = value
            Return model
        End Function

        <ExportAPI("Author.Add")>
        Public Function __add_author(model As ModelBaseType, value As String) As ModelBaseType
            model.ModelProperty.Authors.Add(value)
            Return model
        End Function

        <ExportAPI("Write.Comment")>
        Public Function _set_Comments(model As ModelBaseType, value As String) As ModelBaseType
            model.ModelProperty.Comment = value
            Return model
        End Function

        <ExportAPI("Write.Species")>
        Public Function set_SpeciesId(model As ModelBaseType, value As String) As ModelBaseType
            model.ModelProperty.SpecieId = value
            Return model
        End Function

        <ExportAPI("Write.Title")>
        Public Function _set_Title(model As ModelBaseType, value As String) As ModelBaseType
            model.ModelProperty.Title = value
            Return model
        End Function

        <ExportAPI("EMail.Add")>
        Public Function __add_email(model As ModelBaseType, value As String) As ModelBaseType
            model.ModelProperty.Emails.Add(value)
            Return model
        End Function

        <ExportAPI("Reversion.Plus")>
        Public Function __add_reversion(model As ModelBaseType) As ModelBaseType
            model.ModelProperty.Reversion += 1
            Return model
        End Function

        <ExportAPI("Publication.Add")>
        Public Function __add_publication(model As ModelBaseType, value As String) As ModelBaseType
            model.ModelProperty.Publications.Add(value)
            Return model
        End Function

        <ExportAPI("Url.Add")>
        Public Function __add_URL(model As ModelBaseType, value As String) As ModelBaseType
            model.ModelProperty.URLs.Add(value)
            Return model
        End Function

        <ExportAPI("Write.DBLinks")>
        Public Function _set_DBLinks(model As ModelBaseType, value As IEnumerable(Of String)) As ModelBaseType
            model.ModelProperty.DBLinks = value.ToArray
            Return model
        End Function
#End Region
    End Module
End Namespace
