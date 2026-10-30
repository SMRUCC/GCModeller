Imports System.Diagnostics
Imports System.IO
Imports System.Text
Imports Microsoft.VisualBasic.ApplicationServices.Debugging
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Data.BioCyc
Imports SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.v2
Imports SMRUCC.genomics.GCModeller.MarkupCompiler.BioCyc

''' <summary>
''' 使用真实的BioCyc数据库(EcoCyc 29.0)进行虚拟细胞模型编译的端到端演示程序。
''' 
''' 运行流程：
''' 
''' 1. 打开BioCyc数据库工作目录；
''' 2. 将数据库之中的全部数据编译为 <see cref="VirtualCell"/> 模型对象；
''' 3. 将模型对象保存为XML模型文件；
''' 4. 重新加载所保存的XML文件，进行往返一致性校验；
''' 5. 使用 <see cref="DebugAudit"/> 记录全部断言的执行结果，并生成测试报告。
''' </summary>
Public Class Program

    ''' <summary>
    ''' 默认的BioCyc数据库位置(EcoCyc 29.0 大肠杆菌K-12)
    ''' </summary>
    Const DefaultDatabase As String = "F:\ecoli\29.0"

    Shared Function Main(args As String()) As Integer
        Dim database As String = args.ElementAtOrDefault(0)
        Dim output As String = args.ElementAtOrDefault(1)
        Dim stopwatch As Stopwatch = Stopwatch.StartNew()

        If database.StringEmpty(, True) Then
            database = DefaultDatabase
        End If
        If output.StringEmpty(, True) Then
            output = resolveOutputDirectory()
        End If

        Call Directory.CreateDirectory(output)

        Call Console.WriteLine($"[demo] database: {database}")
        Call Console.WriteLine($"[demo] output:   {output}")

        If Not Directory.Exists(database) Then
            Call Console.Error.WriteLine($"[demo] the BioCyc database directory does not exists: {database}")
            Return 2
        End If

        ' ------------------------------------------------------------------
        ' 1. 打开BioCyc数据库
        ' ------------------------------------------------------------------
        Dim workspace As Workspace = Workspace.Open(database)

        Call DebugAudit.Mark("open biocyc database", database)
        Call DebugAudit.Check(workspace.checkValid(), "the BioCyc database is valid")
        Call DebugAudit.Check(Not workspace.species Is Nothing, "the organism taxonomy information exists")
        Call DebugAudit.Check(workspace.genes.features.TryCount > 0, "the database contains gene records")
        Call DebugAudit.Check(workspace.compounds.features.TryCount > 0, "the database contains compound records")

        ' ------------------------------------------------------------------
        ' 2. 编译虚拟细胞模型
        ' ------------------------------------------------------------------
        Dim compiler As New v2Compiler(workspace)
        Dim model As VirtualCell = compiler.Compile()

        Call DebugAudit.Mark("compile virtual cell model", stopwatch.Elapsed.ToString())

        stopwatch.Stop()

        Call DebugAudit.Check(Not model Is Nothing, "the virtual cell model has been compiled")

        If model Is Nothing Then
            Call generateReport(output, Nothing, stopwatch.Elapsed, workspace)
            Return 1
        End If

        ' ------------------------------------------------------------------
        ' 3. 保存为XML模型文件
        ' ------------------------------------------------------------------
        Dim modelFile As String = Path.Combine(output, "ecoli_virtualcell.xml")
        Dim saved As Boolean = model.Save(modelFile)

        Call DebugAudit.Mark("save model file", modelFile)
        Call DebugAudit.Check(saved, "the virtual cell model has been saved into the xml model file")
        Call DebugAudit.Check(File.Exists(modelFile), "the xml model file exists")
        Call Console.WriteLine($"[demo] model file: {modelFile} ({New FileInfo(modelFile).Length} bytes)")

        ' ------------------------------------------------------------------
        ' 4. 模型数据结构的科学完整性校验
        ' ------------------------------------------------------------------
        Call checkModel(model)

        ' ------------------------------------------------------------------
        ' 5. XML文件的往返一致性校验
        ' ------------------------------------------------------------------
        callRoundTrip(model, modelFile)

        ' ------------------------------------------------------------------
        ' 6. 生成测试报告
        ' ------------------------------------------------------------------
        Call generateReport(output, model, stopwatch.Elapsed, workspace)

        Call Console.WriteLine($"[demo] assertions failure count: {DebugAudit.FailureCount}")

        Return If(DebugAudit.FailureCount = 0, 0, 1)
    End Function

    ''' <summary>
    ''' 校验模型对象在科学上是否完整：区室、基因组、调控网络、代谢网络、证据以及分类
    ''' </summary>
    ''' <param name="model"></param>
    Private Shared Sub checkModel(model As VirtualCell)
        Dim genome = model.genome
        Dim metabolism = model.metabolismStructure

        ' ---- 物种信息 ----
        Call DebugAudit.Check(Not model.taxonomy Is Nothing, "taxonomy exists")
        Call DebugAudit.Check(Not model.taxonomy.scientificName.StringEmpty(, True), "the scientific name of the species exists")
        Call DebugAudit.Check(Not model.taxonomy.ncbi_taxid.ToString.StringEmpty(, True), "the ncbi taxonomy id exists")
        Call DebugAudit.Check(Equals(model.schemaVersion, "3.0"), "the model file uses the GCMarkup 3.0 schema")

        ' ---- 区室 ----
        Call DebugAudit.Check(Not model.compartments.IsNullOrEmpty, "the compartment table exists")
        Call DebugAudit.Check(model.compartments.TryCount >= 3, "the model defines at least three compartments")
        Call DebugAudit.Check(model.compartments.Any(Function(c) c.kind = CompartmentKind.intracellular), "the cytosol compartment exists")
        Call DebugAudit.Check(model.compartments.Any(Function(c) c.kind = CompartmentKind.periplasm), "the periplasm compartment exists")

        ' ---- 基因组结构 ----
        Call DebugAudit.Check(Not genome Is Nothing, "the genome structure exists")
        Call DebugAudit.Check(Not genome.replicons.IsNullOrEmpty, "the genome contains at least one replicon")
        Call DebugAudit.Check(genome.genomeSize > 0, "the genome size has been calculated")
        Call DebugAudit.Check(genome.gcContent > 0.4 AndAlso genome.gcContent < 0.6, $"the gc content ({genome.gcContent}) is inside the normal range of E. coli")
        Call DebugAudit.Check(genome.GetAllGenes().TryCount > 1000, "the genome contains more than one thousand genes")
        Call DebugAudit.Check(genome.GetAllOperon.TryCount > 1000, "the genome contains more than one thousand transcription units")
        Call DebugAudit.Check(Not genome.proteins.IsNullOrEmpty, "the protein list is not empty")
        Call DebugAudit.Check(genome.proteins.SafeQuery.Count(Function(p) p.molecularWeight > 0) > 0, "the molecular weight of the proteins has been mapped")

        ' ---- 基因与蛋白质的注释 ----
        Dim genes = genome.GetAllGenes().ToArray
        Call DebugAudit.Check(genes.Count(Function(g) Not g.name.StringEmpty(, True)) > 0, "the gene symbol has been mapped")
        Call DebugAudit.Check(genes.Count(Function(g) Not g.dbLinks.IsNullOrEmpty) > 0, "the gene database cross references have been mapped")
        Call DebugAudit.Check(genome.proteins.SafeQuery.Count(Function(p) Not p.goTerms.IsNullOrEmpty) > 0, "the protein GO annotations have been mapped")
        Call DebugAudit.Check(Not genome.proteinFeatures.IsNullOrEmpty, "the protein features have been mapped")

        ' ---- 调控网络 ----
        Call DebugAudit.Check(Not genome.regulations.IsNullOrEmpty, "the transcription regulation network is not empty")
        Call DebugAudit.Check(genome.regulations.SafeQuery.Count(Function(r) Not r.operonId.StringEmpty(, True)) > 0, "the regulation relationship has been linked to the operon")
        Call DebugAudit.Check(Not genome.bindingSites.IsNullOrEmpty, "the dna binding sites have been mapped")
        Call DebugAudit.Check(Not genome.promoters.IsNullOrEmpty, "the promoters have been mapped")
        Call DebugAudit.Check(operonsWithPromoter(genome) > 0, "the promoter/terminator have been attached to the operons")

        ' ---- 代谢网络 ----
        Call DebugAudit.Check(Not metabolism Is Nothing, "the metabolism structure exists")
        Call DebugAudit.Check(Not metabolism.compounds.IsNullOrEmpty, "the compound list is not empty")
        Call DebugAudit.Check(Not metabolism.enzymes.IsNullOrEmpty, "the enzyme list is not empty")
        Call DebugAudit.Check(metabolism.reactions.size > 0, "the reaction list is not empty")
        Call DebugAudit.Check(metabolism.compounds.SafeQuery.Count(Function(c) Not c.formula.StringEmpty(, True)) > 0, "the chemical formula of the compounds has been mapped")
        Call DebugAudit.Check(metabolism.compounds.SafeQuery.Count(Function(c) c.mass > 0) > 0, "the molecular weight of the compounds has been mapped")
        Call DebugAudit.Check(metabolism.compounds.SafeQuery.Count(Function(c) Not c.smiles.StringEmpty(, True)) > 0, "the SMILES structure of the compounds has been mapped")
        Call DebugAudit.Check(metabolism.reactions.AsEnumerable.Count(Function(r) Not r.ec_number.IsNullOrEmpty) > 0, "the EC number of the reactions has been mapped")
        Call DebugAudit.Check(metabolism.reactions.AsEnumerable.Count(Function(r) Not r.gibbs = 0) > 0, "the gibbs free energy of the reactions has been mapped")
        Call DebugAudit.Check(metabolism.reactions.AsEnumerable.Count(Function(r) Not r.pathways.IsNullOrEmpty) > 0, "the reactions have been linked to the pathways")
        Call DebugAudit.Check(metabolism.reactions.AsEnumerable.Count(Function(r) Not r.enzymes.IsNullOrEmpty) > 0, "the reactions have been linked to the enzymes")
        Call DebugAudit.Check(metabolism.reactions.AsEnumerable.Count(Function(r) r.direction <> ReactionDirection.unknown) > 0, "the reaction direction has been mapped")
        Call DebugAudit.Check(Not metabolism.reactions.transportation.IsNullOrEmpty, "the transportation processes have been mapped")
        Call DebugAudit.Check(Not metabolism.maps.IsNullOrEmpty, "the pathway functional categories have been mapped")

        ' ---- 酶动力学 ----
        Call DebugAudit.Check(metabolism.enzymes.SafeQuery.Count(Function(e) Not e.catalysis.IsNullOrEmpty) > 0, "the enzyme kinetics functions have been mapped")
        Call DebugAudit.Check(metabolism.enzymes.SafeQuery.Count(Function(e) Not e.ECNumber.StringEmpty(, True)) > 0, "the EC number of the enzymes has been mapped")

        ' ---- 证据与分类 ----
        Call DebugAudit.Check(Not model.evidence.IsNullOrEmpty, "the publication evidence table is not empty")
        Call DebugAudit.Check(Not model.physiology Is Nothing, "the physiology parameters exist")
    End Sub

    ''' <summary>
    ''' 将模型保存为XML文件之后重新加载，校验关键数据的数量是否保持一致
    ''' </summary>
    ''' <param name="model"></param>
    ''' <param name="modelFile"></param>
    Private Shared Sub callRoundTrip(model As VirtualCell, modelFile As String)
        Dim reload As VirtualCell = Nothing

        Try
            reload = LoadXml(Of VirtualCell)(modelFile)
        Catch ex As Exception
            Call DebugAudit.Check(False, $"the xml model file can not be loaded back: {ex.Message}")
            Return
        End Try

        Call DebugAudit.Mark("reload model file", modelFile)
        Call DebugAudit.Check(Not reload Is Nothing, "the xml model file can be loaded back")

        If reload Is Nothing Then
            Return
        End If

        Call DebugAudit.AreEqual(model.cellular_id, reload.cellular_id, "the cellular id is identical after the round trip")
        Call DebugAudit.AreEqual(model.compartments.TryCount, reload.compartments.TryCount, "the compartment count is identical after the round trip")
        Call DebugAudit.AreEqual(model.genome.replicons.TryCount, reload.genome.replicons.TryCount, "the replicon count is identical after the round trip")
        Call DebugAudit.AreEqual(model.genome.GetAllGenes().TryCount, reload.genome.GetAllGenes().TryCount, "the gene count is identical after the round trip")
        Call DebugAudit.AreEqual(model.genome.regulations.TryCount, reload.genome.regulations.TryCount, "the regulation count is identical after the round trip")
        Call DebugAudit.AreEqual(model.metabolismStructure.compounds.TryCount, reload.metabolismStructure.compounds.TryCount, "the compound count is identical after the round trip")
        Call DebugAudit.AreEqual(model.metabolismStructure.enzymes.TryCount, reload.metabolismStructure.enzymes.TryCount, "the enzyme count is identical after the round trip")
        Call DebugAudit.AreEqual(model.metabolismStructure.reactions.size, reload.metabolismStructure.reactions.size, "the reaction count is identical after the round trip")
        Call DebugAudit.AreEqual(model.evidence.TryCount, reload.evidence.TryCount, "the evidence count is identical after the round trip")
    End Sub

    Private Shared Function operonsWithPromoter(model As Genome) As Integer
        Return model.GetAllOperon.Values.Where(Function(o) Not o.promoter Is Nothing).Count
    End Function

    ''' <summary>
    ''' 输出测试报告以及模型摘要
    ''' </summary>
    ''' <param name="output"></param>
    ''' <param name="model"></param>
    ''' <param name="elapsed"></param>
    ''' <param name="workspace"></param>
    Private Shared Sub generateReport(output As String, model As VirtualCell, elapsed As TimeSpan, workspace As Workspace)
        Dim summary As New StringBuilder

        Call summary.AppendLine("GCModeller :: BioCyc virtual cell model compiler demo")
        Call summary.AppendLine($"compiled: {Now}")
        Call summary.AppendLine($"elapsed: {elapsed}")
        Call summary.AppendLine($"assertions: {DebugAudit.Records.Length}, failures: {DebugAudit.FailureCount}")
        Call summary.AppendLine()
        Call summary.AppendLine($"missing optional data files: {workspace.MissingFiles.JoinBy(", ")}")
        Call summary.AppendLine()

        If Not model Is Nothing Then
            Call summary.AppendLine(VirtualCell.Summary(model))
            Call summary.AppendLine()
            Call summary.AppendLine("mapping statistics:")

            Dim genes = model.genome.GetAllGenes().ToArray
            Dim reactions = model.metabolismStructure.reactions.AsEnumerable.ToArray
            Dim compounds = model.metabolismStructure.compounds.SafeQuery.ToArray

            Call summary.AppendLine($"  genes with symbol:        {genes.Count(Function(g) Not g.name.StringEmpty(, True))}/{genes.Length}")
            Call summary.AppendLine($"  genes with GO terms:      {genes.Count(Function(g) Not g.goTerms.IsNullOrEmpty)}/{genes.Length}")
            Call summary.AppendLine($"  genes with db references: {genes.Count(Function(g) Not g.dbLinks.IsNullOrEmpty)}/{genes.Length}")
            Call summary.AppendLine($"  proteins with weight:     {model.genome.proteins.SafeQuery.Count(Function(p) p.molecularWeight > 0)}/{model.genome.proteins.TryCount}")
            Call summary.AppendLine($"  regulations linked:       {model.genome.regulations.SafeQuery.Count(Function(r) Not r.operonId.StringEmpty(, True))}/{model.genome.regulations.TryCount}")
            Call summary.AppendLine($"  reactions with EC:        {reactions.Count(Function(r) Not r.ec_number.IsNullOrEmpty)}/{reactions.Length}")
            Call summary.AppendLine($"  reactions with pathway:   {reactions.Count(Function(r) Not r.pathways.IsNullOrEmpty)}/{reactions.Length}")
            Call summary.AppendLine($"  reactions with enzyme:    {reactions.Count(Function(r) Not r.enzymes.IsNullOrEmpty)}/{reactions.Length}")
            Call summary.AppendLine($"  reactions with gibbs:     {reactions.Count(Function(r) Not r.gibbs = 0)}/{reactions.Length}")
            Call summary.AppendLine($"  transportation:           {model.metabolismStructure.reactions.transportation.TryCount}")
            Call summary.AppendLine($"  compounds with formula:   {compounds.Count(Function(c) Not c.formula.StringEmpty(, True))}/{compounds.Length}")
            Call summary.AppendLine($"  compounds with weight:    {compounds.Count(Function(c) c.mass > 0)}/{compounds.Length}")
            Call summary.AppendLine($"  evidence records:         {model.evidence.TryCount}")
            Call summary.AppendLine($"  class terms:              {model.classTerms.TryCount}")
        End If

        Dim text As String = summary.ToString()

        Call File.WriteAllText(Path.Combine(output, "summary.txt"), text)
        Call File.WriteAllText(Path.Combine(output, "audit_report.txt"), DebugAudit.Dump(includeRecords:=True))
        Call File.WriteAllText(Path.Combine(output, "audit_report.json"), DebugAudit.ToJson(indent:=True))
        Call Console.WriteLine(text)
    End Sub

    ''' <summary>
    ''' 定位 ``test\output`` 目录：从程序的可执行文件目录向上查找包含 ``test.vbproj`` 的目录
    ''' </summary>
    ''' <returns></returns>
    Private Shared Function resolveOutputDirectory() As String
        Dim dir As New DirectoryInfo(AppContext.BaseDirectory)

        Do While Not dir Is Nothing
            If File.Exists(Path.Combine(dir.FullName, "test.vbproj")) Then
                Return Path.Combine(dir.FullName, "output")
            End If

            dir = dir.Parent
        Loop

        Return Path.Combine(AppContext.BaseDirectory, "output")
    End Function

End Class
