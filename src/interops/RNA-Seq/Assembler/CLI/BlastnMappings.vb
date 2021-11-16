#Region "Microsoft.VisualBasic::055e76b9d990952a9a1bae9a193d30c6, RNA-Seq\Assembler\CLI\BlastnMappings.vb"

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
    '     Function: CollectionStatics, MergeMappings, TrimMappings
    ' 
    ' /********************************************************************************/

#End Region

Imports LANS.SystemsBiology.NCBI.Extensions.LocalBLAST.Application
Imports LANS.SystemsBiology.SequenceModel.FASTA
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.DocumentFormat.Csv
Imports Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.Linq
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting

Partial Module CLI

    <ExportAPI("/Merge.Mappings",
           Usage:="/Merge.Mappings /in <blastn.maps.Csv.DIR> [/out <out.csv> /trim /full /perfect /unique /identities <0.9>]")>
    Public Function MergeMappings(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim trim As Boolean = args.GetBoolean("/trim")
        Dim full As Boolean = args.GetBoolean("/full")
        Dim perfect As Boolean = args.GetBoolean("/perfect")
        Dim identities As Double = args.GetValue("/identities", 0.9)
        Dim unique As Boolean = args.GetBoolean("/unique")
        Dim out As String = args.GetValue("/out", [in].TrimFileExt & ".MergeMappings.Csv")
        Dim test As Func(Of BlastnMapping, Boolean)

        If trim Then
            out = out.TrimFileExt & "-Trim"

            If full Then
                out &= ".Full"
            End If
            If perfect Then
                out &= ".Perfect"
            End If
            If unique Then
                out &= ".unique"
            End If

            out &= $".identities={identities}.Csv"
            test = MapsAPI.Where(full, perfect, identities)
        Else
            test = Function(x) True
        End If

        Dim source As IEnumerable(Of BlastnMapping) = (ls - l - r - wildcards("*.Csv") <= [in]).IteratesAll(Of BlastnMapping)
        Dim result As BlastnMapping() =
            LinqAPI.Exec(Of BlastnMapping) <= From x As BlastnMapping
                                              In source'.AsParallel
                                              Where True = test(x)
                                              Select x
        Return result.SaveTo(out).CLICode
    End Function

    <ExportAPI("/Trim.Mappings",
               Info:="Removes the not-unique reads and not perfect aligned reads from the mapping result.",
               Usage:="/Trim.Mappings /in <blastn.maps.Csv.DIR> [/out <out.csv> /full /perfect /unique /identities <0.9> /andalso]")>
    <ParameterInfo("/in", False,
                   Description:="Mappings result on the reference genome.")>
    <ParameterInfo("/out", True,
                   Description:="The excel sheet file location for saving the result.")>
    Public Function TrimMappings(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim full As Boolean = args.GetBoolean("/full")
        Dim perfect As Boolean = args.GetBoolean("/perfect")
        Dim identities As Double = args.GetValue("/identities", 0.9)
        Dim unique As Boolean = args.GetBoolean("/unique")
        Dim out As String = args.GetValue("/out", [in].TrimFileExt & "-TrimMappings.Csv")
        Dim test As Func(Of BlastnMapping, Boolean)
        Dim andalsoL As Boolean = args.GetBoolean("/andalso")

        If full Then
            out &= ".Full"
        End If
        If perfect Then
            out &= ".Perfect"
        End If
        If unique Then
            out &= ".unique"
        End If

        out &= $".identities={identities}.Csv"
        test = MapsAPI.Where(full,
                             perfect,
                             unique,
                             identities,
                             logics:=If(andalsoL,
                             Logics.AndAlso,
                             Logics.OrElse))

        Dim mappings As New DataStream([in])  ' 读取测序数据的mapping结果

        Using IO As New WriteStream(Of BlastnMapping)(out) ' NCBI 开源测序软件包
            Dim handle As Action(Of BlastnMapping()) =
                Sub(source)
                    Dim n As Integer = source.Length
                    source = source.Where(test).ToArray   ' 只筛选出unique和perfect的reads
                    Call IO.Flush(source)   ' 数据块写入文件
                    Call $"   {n} -> {source.Length}".__DEBUG_ECHO
                End Sub
            Call mappings.ForEachBlock(
                invoke:=handle,
                blockSize:=12800)  ' 默认的数据分块大小为 500MB

            Return 0
        End Using
    End Function

    ''' <summary>
    ''' Excepts, Intersect, None. 交集，两个都Mapping不上
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/Mappings.Statics",
               Usage:="/Mappings.Statics /a <blastnMappings.Csv> /b <blastnMappings.Csv> /fa <a.fasta> [/fb <b.fasta> /out <out.Csv.DIR>]")>
    Public Function CollectionStatics(args As CommandLine) As Integer
        Dim a As String = args - "/a"
        Dim b As String = args - "/b"
        Dim out As String = args.GetValue("/out", a.TrimFileExt & "-" & b.BaseName & "/")
        Dim aSet As Dictionary(Of String, BlastnMapping()) = (From x As BlastnMapping In a.LoadCsv(Of BlastnMapping) Select x Group x By x.ReadQuery Into Group).ToDictionary(Function(x) x.ReadQuery, Function(x) x.Group.ToArray)
        Dim bSet As Dictionary(Of String, BlastnMapping()) = (From x As BlastnMapping In b.LoadCsv(Of BlastnMapping) Select x Group x By x.ReadQuery Into Group).ToDictionary(Function(x) x.ReadQuery, Function(x) x.Group.ToArray)

        ' 求两个都有的
        Using intersect As New WriteStream(Of BlastnMapping)(out & "/intersect.Csv")
            For Each x As BlastnMapping In (aSet.Values.MatrixToList + bSet.Values.MatrixAsIterator)
                If aSet.ContainsKey(x.ReadQuery) AndAlso bSet.ContainsKey(x.ReadQuery) Then
                    Call intersect.Flush(x)
                End If
            Next
        End Using

        ' 求两个都没有的
        Dim fa As New StreamIterator(args - "/fa")
        Dim source As IEnumerable(Of FastaToken)
        If Not args("/fb").FileExists Then
            source = fa.ReadStream
        Else
            source = fa.ReadStream.JoinAsIterator(New StreamIterator(args - "/fb").ReadStream)
        End If

        Dim NoMaps As New List(Of FastaToken)

        For Each fasta As FastaToken In source
            Dim name As String = fasta.Title

            If Not (aSet.ContainsKey(name) OrElse bSet.ContainsKey(name)) Then
                NoMaps += fasta
            End If
        Next

        Call New FastaFile(NoMaps).Save(-1, out & "/NoMaps.fasta", Encodings.ASCII)

        Return 0
    End Function
End Module
