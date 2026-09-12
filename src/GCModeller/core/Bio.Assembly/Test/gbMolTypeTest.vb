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

Imports SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF

Module gbMolTypeTest

    ''' <summary>
    ''' 用于测试的genbank数据库文件夹，这些文件夹下的一个gbff文件之中可能会包含有多个复制子记录
    ''' </summary>
    ReadOnly genbankDirs As String() = {
        "K:\pangenome\Cryptococcus_neoformans\genbank",
        "K:\pangenome\Escherichia_coli\genbank"
    }

    ''' <summary>
    ''' 不带参数运行时测试<see cref="genbankDirs"/>之下的全部genbank文件；
    ''' 带参数运行时则只测试命令行参数所给定的若干个gbff文件或者文件夹，用于快速验证指定的数据文件。
    ''' </summary>
    ''' <param name="args"></param>
    Sub Main(args As String())
        If args.Length > 0 Then
            Call Main1(args)
        Else
            Call Main1()
        End If
    End Sub

    ''' <summary>
    ''' 加载目标genbank数据，然后判断并打印出其中的每一个genbank记录所对应的分子类型
    ''' </summary>
    ''' <param name="dirs">
    ''' genbank数据库文件夹或者gbff文件的路径列表，默认为<see cref="genbankDirs"/>
    ''' </param>
    Sub Main1(Optional dirs As String() = Nothing)
        Dim files As New List(Of String)
        Dim summary As New Dictionary(Of GenomeMolType, Integer)

        If dirs Is Nothing Then
            dirs = genbankDirs
        End If

        For Each path As String In dirs
            If System.IO.Directory.Exists(path) Then
                Call files.AddRange(System.IO.Directory.GetFiles(path, "*.gbff").OrderBy(Function(file) file))
            ElseIf System.IO.File.Exists(path) Then
                Call files.Add(path)
            Else
                Call Console.WriteLine($"[missing] {path}")
            End If
        Next

        Call Console.WriteLine($"load {files.Count} genbank files for molecule type test...")
        Call Console.WriteLine()

        For i As Integer = 0 To files.Count - 1
            Dim path As String = files(i)
            Dim records As Integer = 0

            ' 一个gbff文件之中可能会包含有多个genbank记录，例如细菌的染色体基因组与质粒基因组
            For Each gb As File In File.LoadDatabase(path, suppressError:=True)
                records += 1

                Dim evidence As MolTypeEvidence = gb.GetMolTypeEvidence()
                Dim locus As String = If(gb.Locus Is Nothing, "n/a", gb.Locus.AccessionID)
                Dim length As String = If(gb.Locus Is Nothing, "n/a", gb.Locus.Length.ToString)
                Dim definition As String = If(gb.Definition Is Nothing, "", gb.Definition.Value)

                If definition.Length > 60 Then
                    definition = definition.Substring(0, 60) & "..."
                End If

                Call Console.WriteLine($"[{evidence.Type,-15}] {locus,-16} len={length,-10} {definition,-63} <{evidence.Source}: {evidence.Hit}>")

                If Not summary.ContainsKey(evidence.Type) Then
                    summary(evidence.Type) = 0
                End If

                summary(evidence.Type) += 1
            Next

            Call Console.WriteLine($"[{i + 1}/{files.Count}] {System.IO.Path.GetFileName(path)} -> records: {records}")
            Call Console.WriteLine()
        Next

        Call Console.WriteLine("==================== summary ====================")
        Call Console.WriteLine($"{"type",-16}{"records",10}")
        Call Console.WriteLine(New String("-"c, 26))

        For Each type As GenomeMolType In summary.Keys.OrderBy(Function(t) t)
            Call Console.WriteLine($"{type,-16}{summary(type),10}")
        Next

        Pause()
    End Sub
End Module
