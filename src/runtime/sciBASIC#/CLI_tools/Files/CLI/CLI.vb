﻿#Region "Microsoft.VisualBasic::471579dd289fd821308ac51b46c721b8, CLI_tools\Files\CLI\CLI.vb"

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

    ' Module CLI
    ' 
    '     Function: __copyTo, ChangeExt, Copy, Merge, Replace
    '               Utf8_2_Gb2312
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.CommandLine
Imports System.Text
Imports Microsoft.VisualBasic.Text

Module CLI

    <ExportAPI("Change",
               Usage:="Change Ext <*.ext> As <*.ext> [In <DIR> /all]")>
    Public Function ChangeExt(args As CommandLine) As Integer
        Dim fromExt As String = args("ext").Split("."c).Last
        Dim toExt As String = "." & args("as").Split("."c).Last
        Dim fromDIR As String = args.GetValue("in", App.CurrentDirectory)
        Dim method As FileIO.SearchOption =
            If(args.GetBoolean("/all"),
            FileIO.SearchOption.SearchAllSubDirectories,
            FileIO.SearchOption.SearchTopLevelOnly)

        For Each file As String In FileIO.FileSystem.GetFiles(fromDIR, method, $"*.{fromExt}")
            Dim newPath As String = file.TrimSuffix & toExt
            Try
                Call FileIO.FileSystem.MoveFile(file, newPath)
            Catch ex As Exception
                Call App.LogException(New Exception($"{file}  => {newPath}", ex))
            End Try
        Next

        Return 0
    End Function

    <ExportAPI("ReplaceName", Usage:="ReplaceName from <old> to <new> [in <DIR> Limits <extlist> /all]")>
    Public Function Replace(args As CommandLine) As Integer
        Dim from As String = args("from")
        Dim toS As String = args("to")
        Dim fromDIR As String = args.GetValue("in", App.CurrentDirectory)
        Dim lstExt As String() = args.GetValue("limits", "*.*").Split(";"c).ToArray(Function(x) "*." & x.Split("."c).Last)
        Dim method As FileIO.SearchOption =
            If(args.GetBoolean("/all"),
            FileIO.SearchOption.SearchAllSubDirectories,
            FileIO.SearchOption.SearchTopLevelOnly)

        For Each file As String In FileIO.FileSystem.GetFiles(fromDIR, method, lstExt)
            Dim Name As String = IO.Path.GetFileNameWithoutExtension(file)
            Name = Regex.Replace(Name, from, toS, RegexOptions.IgnoreCase)
            Dim newPath As String = fromDIR & $"/{Name}.{file.Split("."c).Last}"
            Try
                Call FileIO.FileSystem.MoveFile(file, newPath)
            Catch ex As Exception
                Call App.LogException(New Exception($"{file}  => {newPath}", ex))
            End Try
        Next

        Return 0
    End Function

    <ExportAPI("/Merge", Usage:="/Merge /source <sourceDIR> [/ext <extList:=*.txt> /out <outFile.txt>]")>
    <Argument("/ext", True, Description:="Each file extension should separated by character ';'.")>
    Public Function Merge(args As CommandLine) As Integer
        Dim inDIR As String = args("/source")
        Dim ext As String = args.GetValue("/ext", "*.txt")
        Dim out As String = args.GetValue("/out", inDIR & ".txt")
        Dim lstExt As String() = ext.Split(";"c)

        Try
            Call FileIO.FileSystem.DeleteFile(out, FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.SendToRecycleBin)
        Catch ex As Exception

        End Try

        Call $"Merged file was saved at {out.ToFileURL}....".__DEBUG_ECHO

        For Each file As String In FileIO.FileSystem.GetFiles(inDIR, FileIO.SearchOption.SearchTopLevelOnly, lstExt)
            Dim txt As String = IO.File.ReadAllText(file)
            Call FileIO.FileSystem.WriteAllText(file:=out, text:=txt & vbCrLf, append:=True)
        Next

        Call "OK!!!".__DEBUG_ECHO

        Return True
    End Function

    <ExportAPI("Copys",
               Usage:="Copys From <DIR> to <DIR> [NameOf <file_name> /no-name /parentName]")>
    <Argument("NameOf", True,
                   Description:="Default is ""*.*"".")>
    Public Function Copy(args As CommandLine) As Integer
        Dim fromDIR As String = args("from")
        Dim copyToDIR As String = args("to")
        Dim name As String = args("nameof")
        Dim noName As Boolean = args.GetBoolean("/no-name")
        Dim ext As String = name.Split("."c).Last
        Dim parentName As Boolean = args.GetBoolean("/parentName")

        Call FileIO.FileSystem.CreateDirectory(copyToDIR)

        If String.IsNullOrEmpty(name) Then
            name = "*.*"
        End If

        For Each file As String In ls - l - r - wildcards(name) <= fromDIR
            Dim dest As String = __copyTo(file, copyToDIR, fromDIR, noName, parentName, ext)
            Try
                Call FileIO.FileSystem.DeleteFile(dest)
            Catch ex As Exception

            End Try
            Try
                Call FileIO.FileSystem.CopyFile(file, dest)
            Catch ex As Exception

            End Try

            Call Console.Write(".")
        Next

        Return 0
    End Function

    Private Function __copyTo(source As String,
                              copyToDIR As String,
                              from As String,
                              noName As Boolean,
                              parentName As Boolean,
                              ext As String) As String

        Dim name As String = If(String.IsNullOrEmpty(ext),
            FileIO.FileSystem.GetFileInfo(source).Name,
            source.BaseName & "." & ext)

        If parentName Then
            name = source.ParentDirName & "-" & name
        End If

        Return copyToDIR & "/" & name
    End Function

    <ExportAPI("/utf8_gbk", 
               Usage:="/utf8_gbk /in <file.txt> [/out <file.txt>]")>
    Public Function Utf8_2_Gb2312(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim out$ = args.GetValue("/out", [in].TrimSuffix & "_gbk.txt")
        Dim text$ = [in].ReadAllText(Encoding.UTF8)
        Return text _
            .SaveTo(out, Encodings.GB2312.CodePage) _
            .CLICode
    End Function
End Module
