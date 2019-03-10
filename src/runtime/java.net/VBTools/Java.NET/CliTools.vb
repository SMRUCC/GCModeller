Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.CommandLine.Reflection

Module CliTools

    Public Function Main() As Integer
        Return GetType(Global.Java.NET.CliTools).RunCLI(App.CommandLine)
    End Function

    <ExportAPI("--code", Usage:="--code /template <url> [/o <output>]")>
    Public Function GenerateCodeTemplate(argvs As Microsoft.VisualBasic.CommandLine.CommandLine) As Integer
        Dim Output As String = argvs("/o")
        Dim Template As String = argvs("/template")

        If String.IsNullOrEmpty(Template) Then
            Call Console.WriteLine("/template url value can not be null!")
            Return -10
        End If

        If String.IsNullOrEmpty(Output) Then
            Output = $"{FileIO.FileSystem.CurrentDirectory}/{IO.Path.GetFileNameWithoutExtension(Template).Replace("\", "/").Split("/"c).Last}.vb"
        End If

        Dim doc As String = Oracle.Java.CodeGenerator.GenerateCode(Template)
        Return doc.SaveTo(Output, System.Text.Encoding.ASCII)
    End Function

    <ExportAPI("-build", Usage:="-build [/o <out_dir> /updates]")>
    Public Function BuildAllClasses(argvs As CommandLine.CommandLine) As Integer
        Dim EntryUrl As String = "http://docs.oracle.com/javase/7/docs/api/allclasses-noframe.html"
        Dim WebPage As String = EntryUrl.Get_PageContent
        Dim OutDir As String = argvs("/o")
        Dim updates As Boolean = argvs.GetBoolean("/updates")

        If String.IsNullOrEmpty(OutDir) Then
            OutDir = FileIO.FileSystem.CurrentDirectory
            Call Console.WriteLine($"The required out dir is null, using current dir {OutDir} as default!")
        Else
            OutDir = FileIO.FileSystem.GetDirectoryInfo(OutDir).FullName
        End If

        Dim ListEntries = (From m As Match
                           In Regex.Matches(WebPage, "<li><a href.+?>.+?</a></li>", RegexOptions.Singleline)
                           Let entry As String = m.Value
                           Let hrefLink As String = entry.Get_href
                           Let link As String = "http://docs.oracle.com/javase/7/docs/api/" & hrefLink
                           Let Name As String = Regex.Match(entry, "<a.+?>.+?</a>").Value.GetValue
                           Select link, Name, hrefLink).ToArray

        For Each entry In ListEntries
            Dim Path As String = OutDir & "/" & entry.hrefLink.Replace(".html", ".vb")

            Try
                If updates Then
                    GoTo updates
                Else
                    If Not Path.FileExists OrElse FileIO.FileSystem.GetFileInfo(Path).Length = 0 Then
updates:                Dim doc As String = Oracle.Java.CodeGenerator.GenerateCode(url:=entry.link)
                        Call doc.SaveTo(Path, Encoding:=System.Text.Encoding.Unicode)
                    End If

                End If
            Catch ex As Exception
                Call ex.PrintException
                Call FileIO.FileSystem.WriteAllText("./Failure.txt", entry.Name & vbTab & entry.link & vbTab & entry.hrefLink & vbCrLf, append:=True)
            End Try
        Next

        Call Console.WriteLine("[JOB DONE!!!]")

        Return 0
    End Function
End Module
