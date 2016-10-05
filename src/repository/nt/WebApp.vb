Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.WebCloud.HTTPInternal.AppEngine
Imports SMRUCC.WebCloud.HTTPInternal.AppEngine.APIMethods
Imports SMRUCC.WebCloud.HTTPInternal.AppEngine.APIMethods.Arguments
Imports SMRUCC.WebCloud.HTTPInternal.Platform

''' <summary>
''' 需要通过命令行预先设置DATA文件夹变量的值
''' </summary>
<[Namespace]("DATA")>
Public Class RepositoryWebApp : Inherits WebApp

    ''' <summary>
    ''' Unable located the DATA directory of the nt database, please specific the variable from commandline by: /@set DATA='DIR_of_database'
    ''' </summary>
    Const DATANotAvaliable$ =
        "Unable located the DATA directory of the nt database, please specific the variable from commandline by: /@set DATA='DIR_of_database'"

    ReadOnly __searchEngine As QueryEngine

    Public Sub New(main As PlatformEngine)
        MyBase.New(main)

        Dim DATA$ = App.GetVariables("DATA").FirstOrDefault

        If Not DATA$.DirectoryExists Then
            Throw New Exception(DATANotAvaliable)
        Else
            Call $"Load database index from {DATA}".__DEBUG_ECHO

            __searchEngine = New QueryEngine()
            __searchEngine.ScanSeqDatabase(DATA$)

            Call "Job Done!".__DEBUG_ECHO
        End If
    End Sub

    Dim __uid As New Uid

    Public Structure QueryTask

        Public query$, break%

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Structure

    <[GET](GetType(FastaToken))>
    <ExportAPI("/DATA/downloads.vbs")>
    Public Function Downloads(request As HttpRequest, response As HttpResponse) As Boolean
        Dim task$ = request.URLParameters("task")
        Dim args As QueryTask = __tasks(task$)

        For Each result In __searchEngine.Search(args.query$)
            Call response.WriteLine(result.GenerateDocument(args.break))
        Next

        Return True
    End Function

    Dim __tasks As New Dictionary(Of String, QueryTask)

    <[POST](GetType(String))>
    <ExportAPI("/DATA/search.vbs")>
    Public Function InvokeQuery(request As HttpPOSTRequest, response As HttpResponse) As Boolean
        Dim query$ = request.POSTData(NameOf(query))
        Dim break% = CInt(Val(request.POSTData(NameOf(break))))
        Dim id$

        SyncLock __uid
            id = (+__uid)
        End SyncLock

        SyncLock __tasks
            __tasks(id) = New QueryTask With {
                .break = break,
                .query = query
            }
        End SyncLock

        Dim url As String = $"./downloads.vbs?task={id$}"

        Call response.Redirect(url)
        Call $"Download task was redirect to {url}".Warning

        Return True
    End Function

    Public Overrides Function Page404() As String
        Return ""
    End Function
End Class
