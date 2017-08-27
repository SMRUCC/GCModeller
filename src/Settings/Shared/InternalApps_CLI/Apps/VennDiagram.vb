Imports Microsoft.VisualBasic.CommandLine
Imports CLIApp = Microsoft.VisualBasic.CommandLine.InteropService.InteropService

Namespace GCModellerApps

    Public Class VennDiagram : Inherits CLIApp

        Sub New(exe$)
            MyBase._executableAssembly = exe
        End Sub

        ''' <summary>
        ''' Draw the venn diagram from a csv data file, you can specific the diagram drawing options from
        ''' this command switch value. The generated venn dragram will be saved As tiff file format.
        ''' </summary>
        ''' <param name="data$"></param>
        ''' <param name="title$"></param>
        ''' <param name="out$"></param>
        ''' <returns></returns>
        Public Function Draw(data$, Optional title$ = "VennDiagram title", Optional skipFirstID As Boolean = True, Optional out$ = "./vennDiagram.tiff") As Integer
            Dim CLI$ = $".Draw -i {data.CLIPath} -t {title.CLIToken} -o {out.CLIPath}"

            If skipFirstID Then
                CLI &= " /First.ID.Skip"
            End If

            Return RunDotNetApp(args:=CLI).Run
        End Function
    End Class
End Namespace
