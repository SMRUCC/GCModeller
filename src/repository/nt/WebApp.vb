#Region "Microsoft.VisualBasic::6468109ec0c55e00d4abcb5a2c57ea94, nt\WebApp.vb"

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

    ' Class RepositoryWebApp
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: Downloads, InvokeQuery
    '     Structure QueryTask
    ' 
    '         Function: ToString
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.WebCloud.HTTPInternal.AppEngine
Imports SMRUCC.WebCloud.HTTPInternal.AppEngine.APIMethods
Imports SMRUCC.WebCloud.HTTPInternal.Core
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

        Dim DATA$ = App.GetVariable("DATA")

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

    <[GET](GetType(FastaSeq))>
    <ExportAPI("/DATA/Download.fasta")>
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
            id = (++__uid).FormatZero("00000")
        End SyncLock

        SyncLock __tasks
            __tasks(id) = New QueryTask With {
                .break = break,
                .query = query
            }
        End SyncLock

        Dim url As String = $"./Download.fasta?task={id$}"

        Call response.Redirect(url)
        Call $"Download task was redirect to {url}".Warning

        Return True
    End Function
End Class
