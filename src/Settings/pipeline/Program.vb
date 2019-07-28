Imports System.ComponentModel
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.InteropService.SharedORM
Imports Microsoft.VisualBasic.CommandLine.Reflection

<CLI> Module Program

    Public Function Main() As Integer
        Return GetType(Program).RunCLI(App.CommandLine)
    End Function

    <ExportAPI("/dispose")>
    <Usage("/dispose /resource <resource_name>")>
    <Description("Delete an exists memory mapping file resource.")>
    Public Function Dispose(args As CommandLine) As Integer

    End Function

    <ExportAPI("/register")>
    <Usage("/register /resource <resource_name> /size <size_in_bytes> /type <meta_base64>")>
    <Description("Allocate a new memory mapping file resource for save the temp data for cli pipeline scripting")>
    Public Function Register(args As CommandLine) As Integer
        Dim name$ = args <= "/resource"
        Dim size& = args <= "/size"
        Dim type$ = args <= "/type"



        Return 0
    End Function
End Module
