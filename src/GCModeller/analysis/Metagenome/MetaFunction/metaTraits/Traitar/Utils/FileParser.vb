' ============================================================================
' FileParser.vb - 文件解析工具
'
' 负责解析以下文件格式：
'   1. GFF3文件 - 基因组注释
'   2. FASTA文件 - DNA/蛋白质序列
'   3. HMMER domtblout - Pfam家族注释结果
'   4. 模型文件 - pt2acc.txt, {id}_bias.txt, {id}_feats.txt, {id}_non-zero+weights.txt
'   5. Newick格式 - 系统发育树
' ============================================================================

Imports System.IO
Imports System.Runtime.InteropServices
Imports SMRUCC.genomics.Analysis.SequenceTools.HMMER

Namespace metaTraits.Traitar.Utils

    ''' <summary>
    ''' 文件解析工具类
    ''' </summary>
    Public Module FileParser

        ' ================================================================
        ' 1. GFF3 文件解析
        ' ================================================================

        ''' <summary>
        ''' 解析GFF3文件，提取蛋白/CDS信息
        ''' GFF3格式：9列，制表符分隔
        '''   seqid, source, type, start, end, score, strand, phase, attributes
        ''' </summary>
        Public Function ParseGFF(gffPath As String) As List(Of Models.ProteinSequence)
            Dim proteins As New List(Of Models.ProteinSequence)()

            Using reader As New StreamReader(gffPath)
                Dim line As String
                Do While reader.Peek() >= 0
                    line = reader.ReadLine()
                    If String.IsNullOrEmpty(line) Then Continue Do
                    If line.StartsWith("#") Then Continue Do

                    Dim fields As String() = line.Split(ControlChars.Tab)
                    If fields.Length < 9 Then Continue Do

                    Dim seqType As String = fields(2).ToLower()
                    ' 只处理CDS或gene记录
                    If seqType <> "cds" AndAlso seqType <> "gene" AndAlso seqType <> "protein" Then
                        Continue Do
                    End If

                    Dim protein As New Models.ProteinSequence()
                    protein.SequenceId = fields(0)
                    Integer.TryParse(fields(3), protein.Start)
                    Integer.TryParse(fields(4), protein.End)
                    protein.Strand = fields(6)

                    ' 解析attributes字段
                    Dim attrs As String() = fields(8).Split(";"c)
                    For Each attr As String In attrs
                        Dim kv As String() = attr.Split(New Char() {"="c}, 2)
                        If kv.Length = 2 Then
                            Dim key As String = kv(0).Trim().ToLower()
                            Dim val As String = kv(1).Trim()
                            Select Case key
                                Case "id"
                                    protein.ProteinId = val
                                Case "name", "product"
                                    protein.Product = val
                                Case "protein_id"
                                    If String.IsNullOrEmpty(protein.ProteinId) Then
                                        protein.ProteinId = val
                                    End If
                            End Select
                        End If
                    Next

                    If String.IsNullOrEmpty(protein.ProteinId) Then
                        protein.ProteinId = String.Format("{0}_{1}_{2}_{3}",
                                                          protein.SequenceId, protein.Start, protein.End, protein.Strand)
                    End If

                    proteins.Add(protein)
                Loop
            End Using

            Return proteins
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
        ' 2. FASTA 文件解析
        ' ================================================================

        ''' <summary>
        ''' 解析FASTA文件（DNA或蛋白质）
        ''' </summary>
        Public Function ParseFasta(fastaPath As String) As List(Of Models.ProteinSequence)
            Dim proteins As New List(Of Models.ProteinSequence)()
            Dim current As Models.ProteinSequence = Nothing
            Dim seqBuilder As New System.Text.StringBuilder()

            Using reader As New StreamReader(fastaPath)
                Dim line As String
                Do While reader.Peek() >= 0
                    line = reader.ReadLine()
                    If String.IsNullOrEmpty(line) Then Continue Do

                    If line.StartsWith(">") Then
                        ' 保存前一个序列
                        If current IsNot Nothing Then
                            current.Sequence = seqBuilder.ToString()
                            proteins.Add(current)
                        End If

                        ' 开始新序列
                        current = New Models.ProteinSequence()
                        Dim header As String = line.Substring(1)
                        Dim headerParts As String() = header.Split(ControlChars.Tab)
                        current.ProteinId = headerParts(0).Trim()

                        ' 解析header中的描述
                        If headerParts.Length > 1 Then
                            current.Product = headerParts(1).Trim()
                        End If

                        seqBuilder.Clear()
                    Else
                        seqBuilder.Append(line.Trim())
                    End If
                Loop
            End Using

            ' 保存最后一个序列
            If current IsNot Nothing Then
                current.Sequence = seqBuilder.ToString()
                proteins.Add(current)
            End If

            Return proteins
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

        ' ================================================================
        ' 4. 模型文件解析
        ' ================================================================

        ''' <summary>
        ''' 解析pt2acc.txt文件（表型ID到名称的映射）
        ''' 格式：表型ID  表型名称  类别
        ''' </summary>
        Public Function ParsePhenotypeTable(pt2accPath As String) As Dictionary(Of String, Models.PhenotypeModel)
            Dim phenotypes As New Dictionary(Of String, Models.PhenotypeModel)()

            Using reader As New StreamReader(pt2accPath)
                Dim line As String
                Dim isFirstLine As Boolean = True
                Do While reader.Peek() >= 0
                    line = reader.ReadLine()
                    If String.IsNullOrEmpty(line) Then Continue Do

                    ' 跳过表头
                    If isFirstLine Then
                        isFirstLine = False
                        If line.Contains("accession") OrElse line.Contains("category") Then
                            Continue Do
                        End If
                    End If

                    ' 按空白分割
                    Dim parts As String() = line.Split(New Char() {" "c, ControlChars.Tab},
                                                        StringSplitOptions.RemoveEmptyEntries)
                    If parts.Length < 3 Then Continue Do

                    Dim phenoId As String = parts(0).Trim()
                    Dim phenoName As String = parts(1).Trim()
                    Dim category As String = parts(2).Trim()

                    ' 处理表型名包含空格的情况
                    If parts.Length > 3 Then
                        ' 最后一列是category，中间是name
                        category = parts(parts.Length - 1).Trim()
                        phenoName = String.Join(" ", parts, 1, parts.Length - 2)
                    End If

                    Dim model As New Models.PhenotypeModel()
                    model.PhenotypeId = phenoId
                    model.PhenotypeName = phenoName
                    model.Category = category
                    phenotypes(phenoId) = model
                Loop
            End Using

            Return phenotypes
        End Function

        ''' <summary>
        ''' 解析pf2acc_desc.txt文件（Pfam ID到描述的映射）
        ''' </summary>
        Public Function ParsePfamDescription(pf2accPath As String) As Dictionary(Of String, String)
            Dim descriptions As New Dictionary(Of String, String)()

            Using reader As New StreamReader(pf2accPath)
                Dim line As String
                Dim isFirstLine As Boolean = True
                Do While reader.Peek() >= 0
                    line = reader.ReadLine()
                    If String.IsNullOrEmpty(line) Then Continue Do

                    If isFirstLine Then
                        isFirstLine = False
                        If line.Contains("description") Then Continue Do
                    End If

                    Dim idx As Integer = line.IndexOf(" "c)
                    If idx < 0 Then idx = line.IndexOf(ControlChars.Tab)
                    If idx < 0 Then Continue Do

                    Dim pfamId As String = line.Substring(0, idx).Trim()
                    Dim desc As String = line.Substring(idx + 1).Trim()
                    descriptions(pfamId) = desc
                Loop
            End Using

            Return descriptions
        End Function

        ''' <summary>
        ''' 解析{id}_bias.txt文件（各C值对应的偏置项）
        ''' 格式：C值  偏置
        ''' </summary>
        Public Function ParseBiasFile(biasPath As String) As Dictionary(Of Double, Double)
            Dim biases As New Dictionary(Of Double, Double)()

            Using reader As New StreamReader(biasPath)
                Dim line As String
                Do While reader.Peek() >= 0
                    line = reader.ReadLine()
                    If String.IsNullOrEmpty(line) Then Continue Do

                    Dim parts As String() = line.Split(New Char() {" "c, ControlChars.Tab},
                                                        StringSplitOptions.RemoveEmptyEntries)
                    If parts.Length < 2 Then Continue Do

                    Dim cVal As Double
                    Dim biasVal As Double
                    If Double.TryParse(parts(0), cVal) AndAlso Double.TryParse(parts(1), biasVal) Then
                        biases(cVal) = biasVal
                    End If
                Loop
            End Using

            Return biases
        End Function

        ''' <summary>
        ''' 解析{id}_feats.txt文件（完整特征权重矩阵）
        ''' 格式：第一列为Pfam ID，后续列为各C值对应的权重
        ''' </summary>
        Public Function ParseFeatsFile(featsPath As String,
                                       <Out()> ByRef cValues As List(Of Double)) As Dictionary(Of String, Dictionary(Of Double, Double))
            Dim featWeights As New Dictionary(Of String, Dictionary(Of Double, Double))()
            cValues = New List(Of Double)()

            Using reader As New StreamReader(featsPath)
                Dim line As String
                Dim isFirstLine As Boolean = True
                Dim colHeaders As String() = Nothing

                Do While reader.Peek() >= 0
                    line = reader.ReadLine()
                    If String.IsNullOrEmpty(line) Then Continue Do

                    Dim parts As String() = line.Split(New Char() {" "c, ControlChars.Tab},
                                                        StringSplitOptions.RemoveEmptyEntries)

                    If isFirstLine Then
                        isFirstLine = False
                        ' 第一行是列头：空, 0.5_0, 0.7_0, ...
                        ' 解析C值
                        For i As Integer = 1 To parts.Length - 1
                            Dim header As String = parts(i)
                            ' 格式如 "0.5_0"，取下划线前的部分作为C值
                            Dim [cStr] As String = header
                            Dim underscoreIdx As Integer = header.IndexOf("_"c)
                            If underscoreIdx > 0 Then
                                [cStr] = header.Substring(0, underscoreIdx)
                            End If
                            Dim cVal As Double
                            If Double.TryParse([cStr], cVal) Then
                                cValues.Add(cVal)
                            End If
                        Next
                        colHeaders = parts
                        Continue Do
                    End If

                    If parts.Length < 1 Then Continue Do
                    If colHeaders Is Nothing OrElse cValues.Count = 0 Then Continue Do

                    Dim pfamId As String = parts(0)
                    Dim weights As New Dictionary(Of Double, Double)()

                    For i As Integer = 1 To Math.Min(parts.Length - 1, cValues.Count)
                        Dim w As Double
                        If Double.TryParse(parts(i), w) Then
                            weights(cValues(i - 1)) = w
                        End If
                    Next

                    featWeights(pfamId) = weights
                Loop
            End Using

            Return featWeights
        End Function

        ''' <summary>
        ''' 解析{id}_non-zero+weights.txt文件
        ''' 格式：PfamID  class(+/-)  各C值权重  Pfam_desc  cor
        ''' </summary>
        Public Function ParseNonZeroWeightsFile(filePath As String,
                                                <Out()> ByRef cValues As List(Of Double)) As List(Of Models.KeyFeatureInfo)
            Dim features As New List(Of Models.KeyFeatureInfo)()
            cValues = New List(Of Double)()

            Using reader As New StreamReader(filePath)
                Dim line As String
                Dim isFirstLine As Boolean = True
                Dim numCValues As Integer = 0

                Do While reader.Peek() >= 0
                    line = reader.ReadLine()
                    If String.IsNullOrEmpty(line) Then Continue Do

                    Dim parts As String() = line.Split(New Char() {" "c, ControlChars.Tab},
                                                        StringSplitOptions.RemoveEmptyEntries)

                    If isFirstLine Then
                        isFirstLine = False
                        ' 表头：class, 0.5_0, 0.7_0, ..., Pfam_desc, cor
                        ' 计算C值数量（总列数减去class、Pfam_desc、cor三列）
                        ' 注意第一列是PfamID（空表头）
                        ' 实际列：[空/PfamID, class, C1, C2, ..., Cn, Pfam_desc, cor]
                        ' 所以C值数量 = parts.Length - 4
                        numCValues = parts.Length - 4
                        For i As Integer = 2 To 2 + numCValues - 1
                            Dim header As String = parts(i)
                            Dim [cStr] As String = header
                            Dim underscoreIdx As Integer = header.IndexOf("_"c)
                            If underscoreIdx > 0 Then
                                [cStr] = header.Substring(0, underscoreIdx)
                            End If
                            Dim cVal As Double
                            If Double.TryParse([cStr], cVal) Then
                                cValues.Add(cVal)
                            End If
                        Next
                        Continue Do
                    End If

                    If parts.Length < 4 + numCValues Then Continue Do

                    Dim feat As New Models.KeyFeatureInfo()
                    feat.PfamId = parts(0)
                    feat.FeatureClass = parts(1)

                    ' 解析各C值权重
                    For i As Integer = 0 To numCValues - 1
                        Dim w As Double
                        If Double.TryParse(parts(2 + i), w) Then
                            If i < cValues.Count Then
                                feat.WeightsByC(cValues(i)) = w
                            End If
                        End If
                    Next

                    ' Pfam描述（倒数第二列）
                    feat.Description = parts(parts.Length - 2)

                    ' 皮尔逊相关系数（最后一列）
                    Dim cor As Double
                    If Double.TryParse(parts(parts.Length - 1), cor) Then
                        feat.PearsonCorrelation = cor
                    End If

                    features.Add(feat)
                Loop
            End Using

            Return features
        End Function

        ' ================================================================
        ' 5. Newick 系统发育树解析
        ' ================================================================

        ''' <summary>
        ''' 解析Newick格式系统发育树
        ''' 简化版，仅支持基本结构
        ''' </summary>
        Public Function ParseNewick(newickStr As String) As Models.PhyloTreeNode
            newickStr = newickStr.Trim()
            If newickStr.EndsWith(";") Then
                newickStr = newickStr.Substring(0, newickStr.Length - 1)
            End If

            Dim pos As Integer = 0
            Return ParseNewickRecursive(newickStr, pos)
        End Function

        Private Function ParseNewickRecursive(s As String, ByRef pos As Integer) As Models.PhyloTreeNode
            Dim node As New Models.PhyloTreeNode()

            If pos < s.Length AndAlso s(pos) = "(" Then
                pos += 1  ' 跳过 "("
                Do
                    Dim child As Models.PhyloTreeNode = ParseNewickRecursive(s, pos)
                    child.Parent = node
                    node.Children.Add(child)
                    If pos < s.Length AndAlso s(pos) = "," Then
                        pos += 1  ' 跳过 ","
                    Else
                        Exit Do
                    End If
                Loop

                If pos < s.Length AndAlso s(pos) = ")" Then
                    pos += 1  ' 跳过 ")"
                End If
            End If

            ' 解析节点名
            Dim nameBuilder As New System.Text.StringBuilder()
            Do While pos < s.Length
                Dim c As Char = s(pos)
                If c = ","c OrElse c = ")"c OrElse c = "("c OrElse c = ";"c Then
                    Exit Do
                End If
                nameBuilder.Append(c)
                pos += 1
            Loop

            ' 处理分支长度（如 name:0.123）
            Dim nameStr As String = nameBuilder.ToString()
            Dim colonIdx As Integer = nameStr.IndexOf(":"c)
            If colonIdx >= 0 Then
                node.Name = nameStr.Substring(0, colonIdx).Trim()
                Dim blStr As String = nameStr.Substring(colonIdx + 1).Trim()
                Dim bl As Double
                If Double.TryParse(blStr, bl) Then
                    node.BranchLength = bl
                End If
            Else
                node.Name = nameStr.Trim()
            End If

            Return node
        End Function

    End Module

End Namespace
