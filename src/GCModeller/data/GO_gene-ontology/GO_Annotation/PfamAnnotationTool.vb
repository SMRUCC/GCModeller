Imports System.IO
Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.Analysis.SequenceTools.HMMER
Imports SMRUCC.genomics.Data.Xfam.Pfam.Pipeline.Database
Imports SMRUCC.genomics.Interops.NCBI.Extensions

Public Module PfamAnnotationTool

    <Extension>
    Public Iterator Function ConvertFromDiamond(diamond As IEnumerable(Of DiamondAnnotation)) As IEnumerable(Of PfamAnnotation)
        For Each hit As DiamondAnnotation In diamond
            Dim pfam = PfamEntryHeader.ParseHeaderTitle(hit.QseqId)

            Yield New PfamAnnotation With {
                .Description = hit.QseqId,
                .BitScore = hit.BitScore,
                .DomainBitScore = hit.BitScore,
                .DomainEnd = hit.SEnd,
                .DomainEValue = hit.EValue,
                .DomainStart = hit.SStart,
                .EValue = hit.EValue,
                .HmmEnd = hit.QEnd,
                .HmmStart = hit.QStart,
                .PfamId = pfam.PfamId,
                .QueryLength = hit.Length,
                .TargetLength = hit.Length,
                .QueryName = hit.SseqId,
                .TargetName = pfam.CommonName
            }
        Next
    End Function

    ''' <summary>
    ''' 从GFF3的Dbxref属性中提取Pfam注释
    ''' 某些GFF文件直接包含Pfam注释，如 Dbxref=PFAM:PF00001,InterPro:IPR000001
    ''' </summary>
    Public Function ExtractPfamFromGFF(gffPath As String) As List(Of PfamAnnotation)
        Dim annotations As New List(Of PfamAnnotation)()

        Using reader As New StreamReader(gffPath)
            Dim line As String
            Do While reader.Peek() >= 0
                line = reader.ReadLine()
                If String.IsNullOrEmpty(line) Then Continue Do
                If line.StartsWith("#") Then Continue Do

                Dim fields As String() = line.Split(ControlChars.Tab)
                If fields.Length < 9 Then Continue Do

                Dim targetName As String = ""
                Dim attrs As String() = fields(8).Split(";"c)
                For Each attr As String In attrs
                    Dim kv As String() = attr.Split(New Char() {"="c}, 2)
                    If kv.Length = 2 Then
                        Dim key As String = kv(0).Trim().ToLower()
                        Dim val As String = kv(1).Trim()
                        If key = "id" OrElse key = "protein_id" Then
                            targetName = val
                        ElseIf key = "dbxref" OrElse key = "db_xref" Then
                            Dim refs As String() = val.Split(","c)
                            For Each r As String In refs
                                r = r.Trim()
                                If r.StartsWith("PFAM:", StringComparison.OrdinalIgnoreCase) Then
                                    Dim pfamId As String = r.Substring(5).Trim()
                                    Dim ann As New PfamAnnotation()
                                    ann.TargetName = targetName
                                    ann.PfamId = pfamId
                                    ann.BitScore = 100.0  ' 默认高比特分
                                    ann.EValue = 0.0000000001   ' 默认低E值
                                    annotations.Add(ann)
                                End If
                            Next
                        End If
                    End If
                Next
            Loop
        End Using

        Return annotations
    End Function

    ' ================================================================
    ' 3. HMMER domtblout 文件解析
    ' ================================================================

    ''' <summary>
    ''' 解析HMMER hmmsearch --domtblout 输出文件
    ''' </summary>
    Public Function ParseHmmsearchDomtblout(domtbloutPath As String) As List(Of PfamAnnotation)
        Dim annotations As New List(Of PfamAnnotation)()

        Using reader As New StreamReader(domtbloutPath)
            Dim line As String
            Do While reader.Peek() >= 0
                line = reader.ReadLine()
                If String.IsNullOrEmpty(line) Then Continue Do
                If line.StartsWith("#") Then Continue Do

                Dim ann As PfamAnnotation = PfamAnnotation.ParseFromDomtblout(line)
                If ann IsNot Nothing Then
                    annotations.Add(ann)
                End If
            Loop
        End Using

        Return annotations
    End Function

    ''' <summary>
    ''' 解析HMMER hmmsearch --tblout 输出文件（简化版）
    ''' </summary>
    Public Function ParseHmmsearchTblout(tbloutPath As String) As List(Of PfamAnnotation)
        Dim annotations As New List(Of PfamAnnotation)()

        Using reader As New StreamReader(tbloutPath)
            Dim line As String
            Do While reader.Peek() >= 0
                line = reader.ReadLine()
                If String.IsNullOrEmpty(line) Then Continue Do
                If line.StartsWith("#") Then Continue Do

                Dim ann As PfamAnnotation = PfamAnnotation.ParseFromTblout(line)
                If ann IsNot Nothing Then
                    annotations.Add(ann)
                End If
            Loop
        End Using

        Return annotations
    End Function
End Module
