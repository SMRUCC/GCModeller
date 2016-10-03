#Region "Microsoft.VisualBasic::0ee4e9285a9a22ff4054008c727aa5c1, ..\workbench\devenv\Program.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports Microsoft.VisualBasic.ConsoleDevice.STDIO
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData

#Const CONSOLE = 1

<[PackageNamespace]("GCModeller.Workbench",
                    Description:="The ""genome-in-code"" GCModeller virtual cell modelling and simulation workbench environment.",
                    Publisher:="amethyst.asuka@gcmodeller.org",
                    Url:="http://gcmodeller.org",
                    Category:=APICategories.CLI_MAN)>
Module Program

    ''' <summary>
    ''' The profile data of this program.
    ''' </summary>
    ''' <remarks></remarks>
    Public Profile As Microsoft.VisualBasic.ComponentModel.Settings.Settings(Of Settings.File)
    Public Logging As Microsoft.VisualBasic.Logging.LogFile

    Public Dev2Profile As Settings.Programs.IDE

    Public Property IDE As FormMain
    Public Property IDEInstance As IDEInstance

    <ExportAPI("_start()", Info:="Start the new session for the gcmodeller workbench, this command works on windows console environment or required X environment on linux.")>
    Public Sub Main()
        Call Program.Initialize()
        Program.IDE = New FormMain
        Program.IDEInstance = New IDEInstance
        Call IDE.ShowDialog()  'Main thread holding
        Call Program.Exit()
    End Sub

    Public Sub IDEStatueText(s As String, Optional Color As Drawing.Color = Nothing)
        If Color = Nothing Then Color = Drawing.Color.White

        IDE.StatusBar.Text = "  " & s
        IDE.StatusBar.ForeColor = Color
    End Sub

    Public Sub [Exit]()
        Call Out("Saving profile data...")
        Call Profile.Save()
        Call Out("Saving log file...")
        Call Logging.Save(True)
        Call Out("Program exit!")
    End Sub

    Private Sub InitializeProfileData()
        Call Out("Read profile data.")
        Program.Profile = Microsoft.VisualBasic.ComponentModel.Settings.Settings(Of Settings.File).LoadFile(Settings.File.DefaultXmlFile)
        Program.Dev2Profile = Profile.SettingsData.Dev2

        If Dev2Profile.IDE.Language <> Settings.Programs.IDE.Languages.System Then
            System.Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo(Languages(Dev2Profile.IDE.Language - 1))
        End If

        If Program.Dev2Profile.IDE.Location Is Nothing Then
            Program.Dev2Profile.IDE.Location = New Settings.Programs.IDE.IDEConfig.PointF
        End If
        If Program.Dev2Profile.IDE.Size Is Nothing Then
            Program.Dev2Profile.IDE.Size = New Settings.Programs.IDE.IDEConfig.SizeF With {.Width = 1024, .Height = 768}
        End If
    End Sub

    Private Sub ApplyProfileData()
        Call Out("Apply the profile data to client...")
        Call TestDbConnection()
        Call Threading.Thread.Sleep(1500)
    End Sub

    ''' <summary>
    ''' Test the database connection.
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub TestDbConnection()
        Using MySQL As Oracle.LinuxCompatibility.MySQL.MySQL = Profile.SettingsData.MySQL
            Dim r As Double = MySQL.Ping

            If r < 0 Then
                Out("No respond from the database server!", ConsoleColor.Red, LibMySQL, Microsoft.VisualBasic.Logging.MSG_TYPES.ERR)
                Logging.WriteLine(MySQL.GetErrMessageString, LibMySQL, Microsoft.VisualBasic.Logging.MSG_TYPES.ERR)
                Out("Database server is offline!", ConsoleColor.Yellow, Strings.Modeller, Microsoft.VisualBasic.Logging.MSG_TYPES.WRN)
            Else
                Out("Communication latency in {0} ms.", r, LibMySQL, Microsoft.VisualBasic.Logging.MSG_TYPES.INF)
                Out("Good connection to the database server!", ConsoleColor.White, LibMySQL, Microsoft.VisualBasic.Logging.MSG_TYPES.INF)
            End If
        End Using
    End Sub

    Private Sub Initialize()
        Call FileIO.FileSystem.CreateDirectory(Settings.LogDIR)
        Call FileIO.FileSystem.CreateDirectory(AppPath & "/Settings/")
        Call FileIO.FileSystem.CreateDirectory(AppPath & "/plugins/")
        Call FileIO.FileSystem.CreateDirectory(Project)
        Call FileIO.FileSystem.CreateDirectory(Settings.TEMP)

        Logging = New Microsoft.VisualBasic.Logging.LogFile(LogFile)

        Call Out(Format("Created log file at ""%s""", LogFile))

#If CONSOLE Then
        Call DispSplash()
#End If
        Call Out("Start to initialize program...")
        Call Program.InitializeProfileData()

        Using SplashScreen As FormSplash = New FormSplash
            Call (New Threading.Thread(AddressOf SplashScreen.ShowDialog)).Start()
            Call ApplyProfileData()

            SplashScreen.Close()
            Call Out("Program initialize completed!")
        End Using
    End Sub

    Public Sub Out(s As String, Optional Color As ConsoleColor = ConsoleColor.White,
            Optional [Object] As String = Strings.Modeller,
            Optional Type As Microsoft.VisualBasic.Logging.MSG_TYPES = Microsoft.VisualBasic.Logging.MSG_TYPES.INF)

#If CONSOLE Then
        Console.SetCursorPosition(3, 15)
        Console.Write(BlankFlush)
        Console.ForegroundColor = Color
        Console.SetCursorPosition(3, 15)
        Console.Write(s)
#End If
        If Not IDE Is Nothing Then Call IDE.OutputConsole1.Write(data:=s)

        Call Logging.WriteLine(s, [Object], Type)
    End Sub

    Private Sub DispSplash()
        Call Console.Clear()

        Console.Title = "(Genetic Clock Initiative Project) GCModeller Workbench"
        Console.ForegroundColor = ConsoleColor.Cyan
        Console.Write(ConsoleSplash, My.Application.Info.Version.ToString, My.Computer.Info.OSPlatform)
        Console.SetCursorPosition(15, 12)
        Console.ForegroundColor = ConsoleColor.Blue
        Console.WriteLine("http://code.google.com/p/genome-in-code/")
        Console.ResetColor()
        Console.SetCursorPosition(0, 15)
        Console.Write("> ")
    End Sub

    Public ReadOnly ConsoleSplash As String =
<Program>Genetic Clock Initiative Project
GCModeller Workbench/{0}-svn(Subversion 152) ({1}, visualbasic)

MM             MM   MM  MMMMM      MMMMM    MMMMMM   MMMM
MM             MM   MM MMM MMM    MMM MMM  MMM  MMM   MM
MM             MMM  MM MMM        MMM MMM  MMM  MMM   MM
MM             MMMM MM  MMM       MMM      MMM        MM
MM      MMMMM  MM MMMM   MMM   MM MMM      MMM        MM
MM     M   MMM MM  MMM    MMM  MM MMMMMMM  MMM        MM
MM         MMM MM   MM     MMM    MM  MMM  MMM  MMM   MM
MM     MMMMMMM MM   MM MMM MMM    MMM MMM  MMM  MMM   MM
MMMMMM MM  MMM MM   MM  MMMMM      MMMMMM   MMMMMM   MMMM
       MM  MMM        
       MMMMMM
</Program>
End Module

