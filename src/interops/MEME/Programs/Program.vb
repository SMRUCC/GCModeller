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
        Public Shared Function MEME(args As CommandLine.CommandLine) As Integer
            Return Program.InvokeMeme()
        End Function

        <ExportAPI("-mast")>
        Public Shared Function MAST(args As CommandLine.CommandLine) As Integer
            Return Program.InvokeMAST
        End Function
    End Class
End Module