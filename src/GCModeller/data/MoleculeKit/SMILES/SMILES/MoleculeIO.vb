Imports SMRUCC.genomics.Molecule.SMILES.Graph
Imports SMRUCC.genomics.Molecule.SMILES.InchiSyntax
Imports SMRUCC.genomics.Molecule.SMILES.SmilesSyntax

''' <summary>
''' 结构式字符串的统一入口：自动识别 SMILES / InChI，解析为同一个分子图对象。
''' </summary>
''' <remarks>
''' <code>
''' Dim mol As MoleculeGraph = MoleculeIO.Parse("c1ccccc1O")
''' mol.Layout2D()
''' Dim smiles$ = MoleculeIO.WriteSmiles(mol)
''' </code>
''' </remarks>
Public Module MoleculeIO

    ''' <summary>
    ''' 解析结构式字符串（默认自动识别格式）。
    ''' </summary>
    ''' <param name="text">SMILES 或 InChI 文本。</param>
    ''' <param name="format">显式指定的格式；<see cref="MoleculeFormat.Unknown"/> 表示自动识别。</param>
    ''' <returns>统一的分子图对象。</returns>
    ''' <exception cref="MoleculeParseException">格式非法时抛出。</exception>
    Public Function Parse(text As String, Optional format As MoleculeFormat = MoleculeFormat.Unknown) As MoleculeGraph
        If format = MoleculeFormat.Unknown Then format = DetectFormat(text)

        Select Case format
            Case MoleculeFormat.InChI
                Return InChIBuilder.Parse(text)
            Case MoleculeFormat.Smiles
                Return SmilesParser.Parse(text)
            Case Else
                Throw New MoleculeParseException("unable to detect the molecule format", 0, text)
        End Select
    End Function

    ''' <summary>
    ''' 解析 SMILES 字符串。
    ''' </summary>
    ''' <param name="smiles">SMILES 文本。</param>
    Public Function ParseSmiles(smiles As String) As MoleculeGraph
        Return SmilesParser.Parse(smiles)
    End Function

    ''' <summary>
    ''' 解析 InChI 字符串。
    ''' </summary>
    ''' <param name="inchi">InChI 文本。</param>
    Public Function ParseInChI(inchi As String) As MoleculeGraph
        Return InChIBuilder.Parse(inchi)
    End Function

    ''' <summary>
    ''' 从文件读取并解析结构式字符串（每行一条时只取第一行有效内容）。
    ''' </summary>
    ''' <param name="path">文本文件路径。</param>
    ''' <param name="format">显式指定的格式；默认按文件内容自动识别。</param>
    Public Function ParseFile(path As String, Optional format As MoleculeFormat = MoleculeFormat.Unknown) As MoleculeGraph
        If Not IO.File.Exists(path) Then
            Throw New IO.FileNotFoundException($"molecule source file not found: {path}", path)
        End If

        Dim text As String = IO.File.ReadAllText(path).Trim

        If text.Contains(vbCrLf) OrElse text.Contains(vbLf) Then
            text = text.LineTokens.
                Select(Function(line) line.Trim).
                First(Function(line) Not String.IsNullOrEmpty(line))
        End If

        Return Parse(text, format)
    End Function

    ''' <summary>
    ''' 尝试解析，失败时返回 <c>False</c> 而不抛异常。
    ''' </summary>
    ''' <param name="text">SMILES 或 InChI 文本。</param>
    ''' <param name="graph">解析成功时的分子图。</param>
    ''' <param name="format">显式指定的格式；默认自动识别。</param>
    ''' <param name="[error]">失败原因。</param>
    Public Function TryParse(text As String,
                             ByRef graph As MoleculeGraph,
                             Optional format As MoleculeFormat = MoleculeFormat.Unknown,
                             Optional ByRef [error] As String = Nothing) As Boolean
        Try
            graph = Parse(text, format)
            [error] = Nothing

            Return True
        Catch ex As Exception
            graph = Nothing
            [error] = ex.Message

            Return False
        End Try
    End Function

    ''' <summary>
    ''' 依据文本前缀判断结构式格式。
    ''' </summary>
    ''' <param name="text">待判断的文本。</param>
    Public Function DetectFormat(text As String) As MoleculeFormat
        If String.IsNullOrWhiteSpace(text) Then Return MoleculeFormat.Unknown

        If InChIReader.LooksLikeInChI(text) Then Return MoleculeFormat.InChI

        Return MoleculeFormat.Smiles
    End Function

    ''' <summary>
    ''' 把分子图写回 SMILES 字符串（Kekulé 形式）。
    ''' </summary>
    ''' <param name="graph">待写出的分子图。</param>
    Public Function WriteSmiles(graph As MoleculeGraph) As String
        Return SmilesWriter.Write(graph)
    End Function

    ''' <summary>
    ''' 生成分子摘要（分子式、原子/键/环数、分子量与告警条数），用于日志与测试报告。
    ''' </summary>
    ''' <param name="graph">目标分子图。</param>
    Public Function Describe(graph As MoleculeGraph) As String
        If graph Is Nothing Then Return "<nothing>"

        Return $"{graph.MolecularFormula()} | atoms={graph.AtomCount} bonds={graph.BondCount} " &
               $"rings={graph.Rings().Count} MW={graph.MolecularWeight():F2} warnings={graph.Warnings.Count}"
    End Function

End Module
