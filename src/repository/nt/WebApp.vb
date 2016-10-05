Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.WebCloud.HTTPInternal.AppEngine
Imports SMRUCC.WebCloud.HTTPInternal.AppEngine.APIMethods
Imports SMRUCC.WebCloud.HTTPInternal.AppEngine.APIMethods.Arguments
Imports SMRUCC.WebCloud.HTTPInternal.Platform

<[Namespace]("DATA")>
Public Class RepositoryWebApp : Inherits WebApp

    Public Sub New(main As PlatformEngine)
        MyBase.New(main)
    End Sub

    <[GET](GetType(FastaToken))>
    <ExportAPI("/DATA/search.vbs")>
    Public Function Query(request As HttpRequest, response As HttpResponse) As Boolean

    End Function

    Public Overrides Function Page404() As String
        Return ""
    End Function
End Class
