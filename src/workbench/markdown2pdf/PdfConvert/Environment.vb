Imports System.Configuration
Imports System.IO
Imports Microsoft.VisualBasic.Language.Default
Imports WkHtmlToPdf.Arguments
Imports ProgramFiles = Microsoft.VisualBasic.FileIO.Path.ProgramPathSearchTool

Module InternalEnvironment

    Public ReadOnly Property Environment As [Default](Of PdfConvertEnvironment)

    Public Const wkhtmltopdf$ = "wkhtmltopdf.exe"
    Public Const wkhtmltopdfInstall$ = "wkhtmltopdf\wkhtmltopdf.exe"

    Sub New()
        Dim isDebugMode As Boolean = False
#If DEBUG Then
        isDebugMode = True
#End If
        Environment = New PdfConvertEnvironment With {
            .TempFolderPath = Path.GetTempPath(),
            .WkHtmlToPdfPath = GetWkhtmlToPdfExeLocation(),
            .Timeout = 60000,
            .Debug = isDebugMode
        }
    End Sub

    Private Function GetWkhtmlToPdfExeLocation() As String
        Dim customPath As String = ConfigurationManager.AppSettings("wkhtmltopdf:path")
        Dim search As New ProgramFiles() With {
            .CustomDirectories = {
                customPath, App.HOME
            }
        }
        Dim file As String = search _
            .FindProgram("wkhtmltopdf", includeDll:=False) _
            .FirstOrDefault

        If Not file.StringEmpty Then
            Return file
        End If

        For Each dir As String In ProgramFiles.SearchDirectory("wkhtmltopdf")
            For Each exeFile As String In ProgramFiles.SearchProgram(dir, "wkhtmltopdf", includeDll:=False)
                Return exeFile
            Next
        Next

        Throw New FileNotFoundException("Progream ""wkhtmltopdf"" is not installed!")
    End Function
End Module
