#Region "Microsoft.VisualBasic::e42f3fa1bc9b9894d775e2738ab6cf51, engine\vcellkit\Modeller\Editor.vb"

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

    '   Total Lines: 110
    '    Code Lines: 59 (53.64%)
    ' Comment Lines: 39 (35.45%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 12 (10.91%)
    '     File Size: 3.68 KB


    ' Module EditorAPI
    ' 
    '     Function: __add_author, __add_email, __add_publication, __add_reversion, __add_URL
    '               _set_DBLinks, _set_Name, _set_Title, set_SpeciesId, setComments
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Extensions
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.GCModeller.CompilerServices
Imports SMRUCC.Rsharp.Runtime.Interop

''' <summary>
''' Edit the model metadata
''' </summary>
<Package("property_edit", Publisher:="GCModeller Virtual Cell System")>
<RTypeExport("properties", GetType([Property]))>
Module EditorAPI

    ''' <summary>
    ''' set model name
    ''' </summary>
    ''' <param name="model"></param>
    ''' <param name="value"></param>
    ''' <returns></returns>
    <ExportAPI("write.name")>
    Public Function _set_Name(model As ModelBaseType, value As String) As ModelBaseType
        model.properties.name = value
        Return model
    End Function

    ''' <summary>
    ''' add author into model
    ''' </summary>
    ''' <param name="model"></param>
    ''' <param name="value"></param>
    ''' <returns></returns>
    <ExportAPI("add_author")>
    Public Function __add_author(model As ModelBaseType, value As String) As ModelBaseType
        model.properties.authors.Add(value)
        Return model
    End Function

    ''' <summary>
    ''' write comment text into the model
    ''' </summary>
    ''' <param name="model"></param>
    ''' <param name="value"></param>
    ''' <returns></returns>
    <ExportAPI("write.comment")>
    Public Function setComments(model As ModelBaseType, value As String) As ModelBaseType
        model.properties.comment = value
        Return model
    End Function

    ''' <summary>
    ''' write organism species information into the model
    ''' </summary>
    ''' <param name="model"></param>
    ''' <param name="value"></param>
    ''' <returns></returns>
    <ExportAPI("write.species")>
    Public Function set_SpeciesId(model As ModelBaseType, value As String) As ModelBaseType
        model.properties.specieId = value
        Return model
    End Function

    ''' <summary>
    ''' write the data title into the model
    ''' </summary>
    ''' <param name="model"></param>
    ''' <param name="value"></param>
    ''' <returns></returns>
    <ExportAPI("write.title")>
    Public Function _set_Title(model As ModelBaseType, value As String) As ModelBaseType
        model.properties.title = value
        Return model
    End Function

    ''' <summary>
    ''' add e-mail information about the author inside model
    ''' </summary>
    ''' <param name="model"></param>
    ''' <param name="value"></param>
    ''' <returns></returns>
    <ExportAPI("add_email")>
    Public Function __add_email(model As ModelBaseType, value As String) As ModelBaseType
        model.properties.Emails.Add(value)
        Return model
    End Function

    <ExportAPI("Reversion.Plus")>
    Public Function __add_reversion(model As ModelBaseType) As ModelBaseType
        model.properties.reversion += 1
        Return model
    End Function

    <ExportAPI("Publication.Add")>
    Public Function __add_publication(model As ModelBaseType, value As String) As ModelBaseType
        model.properties.publications.Add(value)
        Return model
    End Function

    <ExportAPI("Url.Add")>
    Public Function __add_URL(model As ModelBaseType, value As String) As ModelBaseType
        model.properties.URLs.Add(value)
        Return model
    End Function

    <ExportAPI("Write.DBLinks")>
    Public Function _set_DBLinks(model As ModelBaseType, value As IEnumerable(Of String)) As ModelBaseType
        model.properties.DBLinks = value.ToArray
        Return model
    End Function

End Module
