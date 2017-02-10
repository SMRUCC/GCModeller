Imports Microsoft.VisualBasic.CommandLine

Namespace GCModellerApps

    Public Class VennDiagram : Inherits InteropService

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
        Public Function Draw(data$, Optional title$ = "VennDiagram title", Optional out$ = "./vennDiagram.tiff") As Integer
            Dim CLI$ = $".Draw -i {data.CLIPath} -t {title.CLIToken} -o {out.CLIPath}"
            Return RunDotNetApp(args:=CLI).Run
        End Function
    End Class
End Namespace
