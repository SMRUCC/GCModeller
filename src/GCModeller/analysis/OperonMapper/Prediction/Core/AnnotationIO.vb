' ============================================================================
' AnnotationIO.vb — GFF3 / PTT 基因注释解析 + 同源映射 + 功能注释
' ----------------------------------------------------------------------------
' GFF3：type 取 CDS/gene；attributes 取 ID=、locus_tag=、Name=（首个非空）。
' PTT：Protein Table（位置..位置 +/-，Gene，Synonym 列）。
' 同源映射 TSV：query_gene <TAB> subject_gene <TAB> ref_genome [TAB bitscore]
'   ——可由 MiniBlast blastp / OrthoFinder 等预生成。
' 功能注释 TSV：gene <TAB> category（COG 单字母 / KEGG 通路 / Pfam 均可）。
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports System.Globalization
Imports System.IO

Namespace OperonPredictor.Core

    Public Module AnnotationIO

        ''' <summary>GFF3 解析（CDS/gene 记录）</summary>
        Public Function ReadGff(path As String, defaultContig As String) As List(Of Gene)
            Dim genes As New List(Of Gene)()
            Dim seen As New HashSet(Of String)()
            For Each raw In File.ReadLines(path)
                If raw.StartsWith("#"c) Then Continue For
                Dim cols = raw.Split(ControlChars.Tab)
                If cols.Length < 8 Then Continue For
                Dim ftype = cols(2).Trim()
                If ftype <> "CDS" AndAlso ftype <> "gene" Then Continue For
                Dim contig = If(cols(0).Trim().Length > 0, cols(0).Trim(), defaultContig)
                Dim startP = Integer.Parse(cols(3).Trim(), CultureInfo.InvariantCulture)
                Dim endP = Integer.Parse(cols(4).Trim(), CultureInfo.InvariantCulture)
                Dim strand = cols(6).Trim()
                If strand.Length = 0 Then strand = "+"
                ' attributes：locus_tag 优先，其次 ID，再次 Name
                Dim attrs = If(cols.Length > 8, cols(8), "")
                Dim gid As String = ""
                If attrs IsNot Nothing Then
                    For Each key In New String() {"locus_tag", "ID", "Name"}
                        Dim tag = key & "="
                        Dim p2 = attrs.IndexOf(tag, StringComparison.Ordinal)
                        If p2 >= 0 Then
                            Dim rest = attrs.Substring(p2 + tag.Length)
                            Dim semi = rest.IndexOf(";"c)
                            gid = If(semi >= 0, rest.Substring(0, semi), rest).Trim()
                            Exit For
                        End If
                    Next
                End If
                If gid.Length = 0 Then gid = $"{contig}_{startP}"
                Dim dupKey = contig & ":" & startP & ":" & endP & ":" & strand & ":" & ftype
                If seen.Contains(dupKey) Then Continue For     ' CDS 与 gene 重复位置
                seen.Add(dupKey)
                genes.Add(New Gene With {
                    .Id = gid, .Contig = contig,
                    .StartMin = Math.Min(startP, endP), .EndMax = Math.Max(startP, endP),
                    .Strand = If(strand(0) = "-"c, "-"c, "+"c), .Name = gid})
            Next
            Return genes
        End Function

        ''' <summary>PTT 解析（NCBI 蛋白表）</summary>
        Public Function ReadPtt(path As String, contigName As String) As List(Of Gene)
            Dim genes As New List(Of Gene)()
            Dim lineNo As Int32 = 0
            For Each raw In File.ReadLines(path)
                lineNo += 1
                If lineNo <= 3 Then Continue For         ' 头部
                If raw.Trim().Length = 0 Then Continue For
                Dim cols = raw.Split(ControlChars.Tab)
                If cols.Length < 5 Then Continue For
                Dim loc = cols(0).Trim()
                Dim lp = loc.IndexOf("."c)
                If lp < 0 Then Continue For
                Dim rr = loc.Substring(0, lp).Split("."c)
                If rr.Length < 2 Then Continue For
                Dim startP As Int32 = 0
                Dim endP As Int32 = 0
                If Not Integer.TryParse(rr(0).Trim(), startP) OrElse
                   Not Integer.TryParse(rr(1).Trim(), endP) Then Continue For
                Dim strand = cols(1).Trim()
                Dim geneName = cols(4).Trim()
                If geneName.Length = 0 AndAlso cols.Length > 5 Then geneName = cols(5).Trim()
                If geneName.Length = 0 Then geneName = $"gene_{startP}"
                genes.Add(New Gene With {
                    .Id = geneName, .Contig = contigName,
                    .StartMin = Math.Min(startP, endP), .EndMax = Math.Max(startP, endP),
                    .Strand = If(strand.StartsWith("-"c), "-"c, "+"c), .Name = geneName})
            Next
            Return genes
        End Function

        ''' <summary>
        ''' 同源映射：query → (refGenome → (subject, score))。同一 ref 取最高分。
        ''' </summary>
        Public Function ReadHomology(path As String) As Dictionary(Of String, Dictionary(Of String, Tuple(Of String, Double)))
            Dim result As New Dictionary(Of String, Dictionary(Of String, Tuple(Of String, Double)))()
            For Each raw In File.ReadLines(path)
                Dim line = raw.TrimEnd(Convert.ToChar(13), Convert.ToChar(10))
                If line.Length = 0 OrElse line.StartsWith("#"c) Then Continue For
                Dim cols = line.Split(ControlChars.Tab)
                If cols.Length < 3 Then Continue For
                Dim q = cols(0).Trim()
                Dim s = cols(1).Trim()
                Dim refId = cols(2).Trim()
                Dim score As Double = 0
                If cols.Length >= 4 Then
                    Double.TryParse(cols(3).Trim(), NumberStyles.Float,
                                    CultureInfo.InvariantCulture, score)
                End If
                If q.Length = 0 OrElse refId.Length = 0 Then Continue For
                If Not result.ContainsKey(q) Then result(q) = New Dictionary(Of String, Tuple(Of String, Double))()
                Dim perRef = result(q)
                If Not perRef.ContainsKey(refId) OrElse perRef(refId).Item2 < score Then
                    perRef(refId) = Tuple.Create(s, score)
                End If
            Next
            Return result
        End Function

        ''' <summary>参考基因组注释：refId → 基因列表（保守对邻接判定用）</summary>
        Public Function ReadReferenceGffs(specs As List(Of Tuple(Of String, String))) As Dictionary(Of String, List(Of Gene))
            Dim result As New Dictionary(Of String, List(Of Gene))()
            For Each spec In specs
                result(spec.Item2) = ReadGff(spec.Item1, spec.Item2)
            Next
            Return result
        End Function

        ''' <summary>功能注释：gene → category</summary>
        Public Function ReadFunctions(path As String) As Dictionary(Of String, String)
            Dim result As New Dictionary(Of String, String)()
            For Each raw In File.ReadLines(path)
                Dim line = raw.TrimEnd(Convert.ToChar(13), Convert.ToChar(10))
                If line.Length = 0 OrElse line.StartsWith("#"c) Then Continue For
                Dim cols = line.Split(ControlChars.Tab)
                If cols.Length < 2 Then Continue For
                result(cols(0).Trim()) = cols(1).Trim()
            Next
            Return result
        End Function

        ''' <summary>读 FASTA（单/多 contig，键 = contig 名）</summary>
        Public Function ReadFasta(path As String) As Dictionary(Of String, String)
            Dim result As New Dictionary(Of String, System.Text.StringBuilder)()
            Dim cur As System.Text.StringBuilder = Nothing
            Dim curName As String = ""
            For Each raw In File.ReadLines(path)
                Dim line = raw.TrimEnd(Convert.ToChar(13), Convert.ToChar(10))
                If line.StartsWith(">"c) Then
                    If cur IsNot Nothing Then result(curName) = cur
                    Dim header = line.Substring(1).Trim()
                    Dim sp = header.IndexOf(" "c)
                    curName = If(sp < 0, header, header.Substring(0, sp))
                    cur = New System.Text.StringBuilder()
                ElseIf line.Length > 0 AndAlso cur IsNot Nothing Then
                    cur.Append(line.Trim())
                End If
            Next
            If cur IsNot Nothing Then result(curName) = cur
            Dim outDict As New Dictionary(Of String, String)()
            For Each kv In result
                outDict(kv.Key) = kv.Value.ToString().ToUpperInvariant()
            Next
            Return outDict
        End Function

    End Module

End Namespace
