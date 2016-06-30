Imports System.Text
Imports Microsoft.VisualBasic.Extensions
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData

Namespace LDM

    <[PackageNamespace]("GCModeller.Property", Publisher:="GCModeller Virtual Cell System", Description:="")>
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