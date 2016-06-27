Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData

<PackageNamespace("GCModeller.Configuration.CLI",
                  Category:=APICategories.CLI_MAN,
                  Url:="http://gcmodeller.org",
                  Description:="GCModeller configuration console.",
                  Publisher:="xie.guigang@gcmodeller.org",
                  Revision:=215)>
Public Module CLI

    Sub New()
        Settings.Session.Initialize()
    End Sub

    <ExportAPI("Set", Info:="Setting up the configuration data node.",
               Usage:="Set <varName> <value>",
               Example:="Set java /usr/lib/java/java.bin")>
    Public Function [Set](args As CommandLine) As Integer
        Using Settings = Global.GCModeller.Configuration.Settings.Session.ProfileData
            Dim params As String() = args.Parameters
            Dim x As String = params(0)
            Dim Value As String = params(1)

            Call Settings.Set(x, Value)
        End Using
        Return 0
    End Function

    <ExportAPI("var", Info:="Gets the settings value.",
               Usage:="var [varName]",
               Example:="")>
    <ParameterInfo("[VarName]", True,
                   Description:="If this value is null, then the program will prints all of the variables in the gcmodeller config file or when the variable is presents in the database, only the config value of the specific variable will be display.")>
    Public Function Var(args As CommandLine) As Integer
        Using Settings = Global.GCModeller.Configuration.Settings.Session.ProfileData
            If args.Parameters.Length = 0 Then 'list all setting items
                Call Console.WriteLine(Settings.View)
            Else
                Dim x As String = args.Parameters.First
                Call Console.WriteLine(Settings.View(x))
            End If
        End Using

        Return 0
    End Function
End Module
