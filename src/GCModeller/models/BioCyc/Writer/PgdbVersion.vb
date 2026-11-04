Imports System.Globalization
Imports System.IO
Imports System.Text

Namespace Writer

    ''' <summary>
    ''' BioCyc PGDB 根目录之中的 ``version.dat`` 文件。
    ''' 
    ''' 该文件是以 TAB 分隔的属性-值文件(与 ``*.dat`` 的 ``A - B`` 格式不同)：
    ''' 
    ''' ```
    ''' ORGID	HSA
    ''' ORGANISM	Homo sapiens
    ''' VERSION	1.0
    ''' RELEASE-DATE	Wed Nov 4, 2026
    ''' ```
    ''' </summary>
    Public Class PgdbVersion

        ''' <summary>
        ''' BioCyc 数据库的短名称，例如 ``HSA``/``ECOLI``
        ''' </summary>
        ''' <returns></returns>
        Public Property orgId As String

        ''' <summary>
        ''' 物种的完整名称
        ''' </summary>
        ''' <returns></returns>
        Public Property organism As String

        ''' <summary>
        ''' 数据库版本号
        ''' </summary>
        ''' <returns></returns>
        Public Property version As String = "1.0"

        Public Property releaseDate As Date = Date.Now

        ''' <summary>
        ''' 写出到指定目录下面的 ``version.dat``
        ''' </summary>
        ''' <param name="dataDir"></param>
        Public Sub SaveDir(dataDir As String)
            Call Save(IO.Path.Combine(dataDir, "version.dat"))
        End Sub

        ''' <summary>
        ''' 写出到指定的文件路径
        ''' </summary>
        ''' <param name="path"></param>
        Public Sub Save(path As String)
            Dim dir As String = IO.Path.GetDirectoryName(path)

            If Not String.IsNullOrEmpty(dir) Then
                Call Directory.CreateDirectory(dir)
            End If

            Using writer As New StreamWriter(path, append:=False, encoding:=New UTF8Encoding(encoderShouldEmitUTF8Identifier:=False))
                Call writer.WriteLine(";;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;")
                Call writer.WriteLine(";; Lines starting with ; are comment lines. Each non-comment line is of the form:")
                Call writer.WriteLine(";; ATTRIBUTE" & vbTab & "value")
                Call writer.WriteLine(";; The separator character is the Tab character.")
                Call writer.WriteLine(";; Valid attributes are:")
                Call writer.WriteLine(";;")
                Call writer.WriteLine(";; ORGID [BioCyc database unique id for the organism]")
                Call writer.WriteLine(";; ORGANISM [full species and strain name]")
                Call writer.WriteLine(";; VERSION [current database version of the organism]")
                Call writer.WriteLine(";; RELEASE-DATE [flat file creation date]")
                Call writer.WriteLine(";;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;")
                Call writer.WriteLine($"ORGID{vbTab}{orgId}")
                Call writer.WriteLine($"ORGANISM{vbTab}{organism}")
                Call writer.WriteLine($"VERSION{vbTab}{version}")
                Call writer.WriteLine($"RELEASE-DATE{vbTab}{releaseDate.ToString("ddd MMM d, yyyy", CultureInfo.InvariantCulture)}")
            End Using
        End Sub
    End Class

End Namespace
