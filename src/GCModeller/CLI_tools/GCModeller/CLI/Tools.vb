#Region "Microsoft.VisualBasic::b2477fab1ec92a9c658d0fb20028a919, ..\GCModeller\CLI_tools\GCModeller\CLI\Tools.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
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

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Terminal.STDIO
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.CommandLine

Partial Module CLI

    <ExportAPI("/Located.AppData")>
    Public Function LocatedAppData(args As CommandLine) As Integer
        Call System.Diagnostics.Process.Start(App.ProductSharedDIR.ParentPath)
        Return 0
    End Function

    ''' <summary>
    ''' 列举出所有的GCModeller的CLI工具命令
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("--ls", Info:="Listing all of the available GCModeller CLI tools commands.")>
    Public Function List(args As CommandLine) As Integer
        Dim execs As IEnumerable(Of String) = ls - l - wildcards("*.exe") <= App.HOME
        Dim types = (From exe As String In execs
                     Let def As Type = GetCLIMod(exe)
                     Where Not def Is Nothing
                     Select exe.BaseName,
                         nsDef = GetEntry(def)).ToArray
        Dim exeMAX As Integer = (From x In types Select Len(x.BaseName)).Max + 5

        Call Console.WriteLine("All of the available GCModeller commands were listed below.")
        Call Console.WriteLine("For getting the available function in the GCModeller program, ")
        Call Console.WriteLine("try typing:    <command> ?")
        Call Console.WriteLine("For getting the manual document in the GCModeller program,")
        Call Console.WriteLine("try typing:    <command> man")
        Call Console.WriteLine(vbCrLf)
        Call Console.WriteLine("Listed {0} available GCModeller commands:", types.Length)

        For Each x In types
            Dim exePrint As String = " " & x.BaseName & New String(" "c, exeMAX - Len(x.BaseName))
            Printf("%s%s", exePrint, x.nsDef.Description)
        Next

        Return 0
    End Function

    <ExportAPI("/init.manuals", Usage:="/init.manuals")>
    Public Function InitManuals(args As CommandLine) As Integer
        Dim execs As IEnumerable(Of String) = ls - l - wildcards("*.exe") <= App.HOME
        Dim tools As String() =
            LinqAPI.Exec(Of String) <= From exe As String
                                       In execs
                                       Let def As Type = GetCLIMod(exe)
                                       Where Not def Is Nothing
                                       Select exe
        For Each app As String In tools
            Call New IORedirectFile(app, "man --file").Run()

            Dim path As String = Microsoft.VisualBasic.App.HOME & "/" & app.BaseName & ".md"
            If Not path.FileExists Then
                Continue For
            End If

            Dim md As String = path.ReadAllText
            md = $"---
title: {app.BaseName}
tags: [maunal, tools]
date: {Now.ToString}
---
" & md
            Call md.SaveTo(path)
        Next

        Return 0
    End Function
End Module

