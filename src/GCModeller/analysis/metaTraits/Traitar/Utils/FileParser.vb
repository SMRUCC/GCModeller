#Region "Microsoft.VisualBasic::bc3b371ee8abedcde9504006f2d0512b, analysis\Metagenome\MetaFunction\metaTraits\Traitar\Utils\FileParser.vb"

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


    ' Code Statistics:

    '   Total Lines: 586
    '    Code Lines: 403 (68.77%)
    ' Comment Lines: 92 (15.70%)
    '    - Xml Docs: 47.83%
    ' 
    '   Blank Lines: 91 (15.53%)
    '     File Size: 24.92 KB


    '     Module FileParser
    ' 
    '         Function: ConvertFromDiamond, ExtractPfamFromGFF, ParseBiasFile, ParseFasta, ParseFeatsFile
    '                   ParseGFF, ParseHmmsearchDomtblout, ParseHmmsearchTblout, ParseNewick, ParseNewickRecursive
    '                   ParseNonZeroWeightsFile, ParsePfamDescription, ParsePhenotypeTable
    ' 
    ' 
    ' /********************************************************************************/

#End Region

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

Namespace Traitar.Utils

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

