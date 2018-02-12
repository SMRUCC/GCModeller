#Region "Microsoft.VisualBasic::85b61915ab5765587a8793646e16f115, meme_suite\MEME\Programs\Program.vb"

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

    ' Module Program
    ' 
    '     Function: InvokeMAST, InvokeMeme
    ' 
    '     Sub: Main
    '     Class CLI
    ' 
    '         Function: MAST, MEME
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection

Public Module Program

    Public Sub Main()
        Console.WriteLine("select a program [meme, mast]")
        Dim program As String = Console.ReadLine

        Select Case program.ToLower
            Case "meme"
                Call InvokeMeme()
            Case "mast"
                Call InvokeMAST()
        End Select

        Console.WriteLine("end of process, program exit...")
    End Sub

    Public Function InvokeMAST() As Integer

        Console.WriteLine("input the meme result output data directory below:")
        Dim dir As String
DIR_RE:
        dir = Console.ReadLine
        If Not FileIO.FileSystem.DirectoryExists(dir) Then
            Console.WriteLine("incorrect input location, not exists on the filesystem.")
            GoTo DIR_RE
        End If



        Console.WriteLine("input the mast location:")
        Dim mast As String
MEME_RE:
        mast = Console.ReadLine

        If Not FileIO.FileSystem.FileExists(mast) Then
            Console.WriteLine("mast bin is not exists on the filesystem: {0} , try a again!", mast)
            GoTo MEME_RE
        End If


        Console.WriteLine("input the sequence database file location:")
        Dim Sequence As String
SEQ_RE:
        Sequence = Console.ReadLine

        If Not FileIO.FileSystem.FileExists(Sequence) Then
            Console.WriteLine("target sequence file is not exists on the file system location: {0}", Sequence)
            GoTo SEQ_RE
        End If


        Console.WriteLine("input the sequence search result output directory:")
        Dim OutputLocation As String
OUTPUT_RE:
        OutputLocation = Console.ReadLine
        Call FileIO.FileSystem.CreateDirectory(OutputLocation)




        Console.WriteLine("start to processing....")
        For Each file As String In FileIO.FileSystem.GetFiles(dir, FileIO.SearchOption.SearchAllSubDirectories, "*.xml")
            On Error Resume Next

            Console.WriteLine("process file [{0}]...", file)

            Dim out As String = FileIO.FileSystem.GetParentPath(file).Split(CChar("/")).Last
            out = String.Format("{0}/{1}/", OutputLocation, out).Replace(".fsa", "")
            Dim argum As String = String.Format("{0} {1} -o {2}", file, Sequence, out)
            Dim info = New ProcessStartInfo(mast, argum)
            Dim cmdl As String = String.Format("{0} {1}", mast, argum)
            Call Console.WriteLine("--> {0}", cmdl)
            Process.Start(info)

            Threading.Thread.Sleep(2000)
        Next

        Return 0
    End Function

    Public Function InvokeMeme() As Integer

        Console.WriteLine("input the sequence data directory below:")
        Dim dir As String
DIR_RE:
        dir = Console.ReadLine
        If Not FileIO.FileSystem.DirectoryExists(dir) Then
            Console.WriteLine("un correct input location, not exists on the filesystem.")
            GoTo DIR_RE
        End If



        Console.WriteLine("input the meme location:")
        Dim meme As String
MEME_RE:
        meme = Console.ReadLine

        If Not FileIO.FileSystem.FileExists(meme) Then
            Console.WriteLine("meme bin is not exists on the filesystem: {0} , try a again!", meme)
            GoTo MEME_RE
        End If



        Console.WriteLine("input the motif result output directory:")
        Dim OutputLocation As String
OUTPUT_RE:
        OutputLocation = Console.ReadLine
        Call FileIO.FileSystem.CreateDirectory(OutputLocation)


        For Each pm_dir As String In FileIO.FileSystem.GetDirectories(dir, FileIO.SearchOption.SearchTopLevelOnly)
            Console.WriteLine("start to processing....")
            Dim dirName As String = FileIO.FileSystem.GetDirectoryInfo(pm_dir).Name
            Call FileIO.FileSystem.CreateDirectory(OutputLocation & "/" & dirName)

            For Each file As String In FileIO.FileSystem.GetFiles(pm_dir, FileIO.SearchOption.SearchTopLevelOnly, "*.fsa")
                On Error Resume Next

                Console.WriteLine("process file [{0}]...", file)

                Dim out As String = FileIO.FileSystem.GetName(file)
                out = String.Format("{0}/{1}/{2}/", OutputLocation, dirName, out).Replace(".fsa", "")
                Dim argum As String = String.Format("{0} -dna -mod zoops -o {1}", file, out)
                Dim info = New ProcessStartInfo(meme, argum)
                Dim cmdl As String = String.Format("{0} {1}", meme, argum)
                Call Console.WriteLine("--> {0}", cmdl)
                Process.Start(info)

                Threading.Thread.Sleep(3000)
            Next
        Next



        Return 0
    End Function

    Public Class CLI

        <ExportAPI("-meme")>
        Public Shared Function MEME(args As CommandLine) As Integer
            Return Program.InvokeMeme()
        End Function

        <ExportAPI("-mast")>
        Public Shared Function MAST(args As CommandLine) As Integer
            Return Program.InvokeMAST
        End Function
    End Class
End Module
