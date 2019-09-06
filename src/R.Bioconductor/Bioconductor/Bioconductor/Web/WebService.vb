#Region "Microsoft.VisualBasic::7a01a281bd93e474c59c65fc27f8852e, Bioconductor\Bioconductor\Web\WebService.vb"

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

    '     Class WebService
    ' 
    '         Properties: AnnotationData, ExperimentData, Repository, Softwares, Version
    ' 
    '         Constructor: (+2 Overloads) Sub New
    ' 
    '         Function: [Default], __init, DownloadList, ToString
    ' 
    '         Sub: __downloadUpdates
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Terminal.STDIO
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.R.CRAN.Bioconductor.Web.Packages

Namespace Web

    Public Class WebService

        Public Const BIOCVIEWS_INSTALL As String = "http://master.bioconductor.org/install/"

        Public ReadOnly Property Version As Version
            Get
                Return __repository.version
            End Get
        End Property

        Public ReadOnly Property Softwares As Package()
            Get
                Return __repository.softwares
            End Get
        End Property
        Public ReadOnly Property AnnotationData As Package()
            Get
                Return __repository.annotation
            End Get
        End Property
        Public ReadOnly Property ExperimentData As Package()
            Get
                Return __repository.experiment
            End Get
        End Property

        ReadOnly __repository As Repository

        Public ReadOnly Property Repository As Repository
            Get
                Return __repository
            End Get
        End Property

        Sub New()
            __repository = New Repository With {
                .version = Web.Version.GetVersion
            }
            Call __init(AddressOf __downloadUpdates)
            Call __repository.Save(Repository.DefaultFile, Encodings.ASCII)
            Call " DONE!".__DEBUG_ECHO
        End Sub

        Sub New(cache As Repository)
            __repository = cache
            Call __init(Nothing)
        End Sub

        Public Overrides Function ToString() As String
            Return Version.ToString
        End Function

        Private Sub __downloadUpdates()
            Dim ver As String = Me.Version.BiocLite

            __repository.softwares = DownloadList(url:=String.Format(SOFTWARE_PACKAGES, ver), cat:=BiocTypes.bioc)
            __repository.annotation = DownloadList(url:=String.Format(ANNOTATION_DATA_PACKAGES, ver), cat:=BiocTypes.annotation)
            __repository.experiment = DownloadList(url:=String.Format(EXPERIMENT_DATA_PACKAGES, ver), cat:=BiocTypes.experiment)
        End Sub

        Private Function __init(inits As Action) As Integer
            printf("Initialize Bioconductor web api.....")
            printf("Retrieve package information for biocLite.R....")

            If Not inits Is Nothing Then
                Call inits()
            End If

            printf("Get %s biocLite packages...", Softwares.Length)
            printf("Get %s annotation data packages...", AnnotationData.Length)
            printf("Get %s experiment data packages...", ExperimentData.Length)

            Return 0
        End Function

        Const SOFTWARE_PACKAGES As String = "http://master.bioconductor.org/packages/{0}/bioc/"
        Const ANNOTATION_DATA_PACKAGES As String = "http://master.bioconductor.org/packages/{0}/data/annotation/"
        Const EXPERIMENT_DATA_PACKAGES As String = "http://master.bioconductor.org/packages/{0}/data/experiment/"

        Const ROW_FORMAT_REGEX As String = "<tr class=""row_(odd|even)"">.+?</tr>"

        Public Shared Function DownloadList(url As String, cat As BiocTypes) As Package()
            Dim pageHTML As String = url.GET
            Dim ms As String() = Regex.Matches(pageHTML, ROW_FORMAT_REGEX, RegexOptions.Singleline + RegexOptions.IgnoreCase).ToArray
            Dim packages As Package() = ms.Select(Function(token) Package.Parser(token, cat)).ToArray
            Return packages
        End Function

        Public Shared Function [Default]() As WebService
            Dim cache As Repository = Repository.LoadDefault
            Return New WebService(cache)
        End Function
    End Class
End Namespace
