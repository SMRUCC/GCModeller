Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Terminal.STDIO
Imports LANS.SystemsBiology.GCModeller.ModellingEngine.EngineSystem.Engine
Imports LANS.SystemsBiology.GCModeller.ModellingEngine.EngineSystem
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic

<[PackageNamespace]("GCModeller.EngineHost", Description:="", Url:="", Category:=APICategories.CLI_MAN)>
Public Module CommandLines

    <ExportAPI("run", Info:="",
        Usage:="run -i <model_file> -mysql <mysql_connection_string> [-f <gcml/csv_tabular> -t <time> -metabolism <assembly_path> -expression <assembly_path>]",
        Example:="run -i ~/gc/ecoli.xml -t 1000 -url ""http://localhost:8080/client?user=username%password=password%database=database""")>
    <ParameterInfo("-i", False,
        Description:="This switch value specific the model file that the simulation engine will be load",
        Example:="~/gc/ecoli.xml")>
    <ParameterInfo("-t", True,
        Description:="Optional, This switch specific that the cycle number of this simulation will run, this switch value will override the time value in the loaded model file.",
        Example:="1000")>
    <ParameterInfo("-url", False,
        Description:="Setup the data storage service connection url string.",
        Example:="http://localhost:8080/client?user=username%password=password%database=database")>
    <ParameterInfo("-metabolism", True,
        Description:="N/A - The engine kernel will not load the metabolism module.", Example:="")>
    <ParameterInfo("-expression", True,
        Description:="N/A - The engine kernel will not load the gene expression regulation module.", Example:="")>
    <ParameterInfo("-interval", True,
        Description:="This switch value specific the data commit to the mysql database server time interval, " &
                     "if your compiled model is too large you should consider set up this switch value smaller " &
                     "in order to avoid the unexpected memory out of range exception.")>
    <ParameterInfo("-f", True,
        Description:="This parameter specific the file format of the target input model file, default value is gcml format.")>
    <ParameterInfo("-suppress_warn", True, Description:="T/TRUE/F/FALSE")>
    <ParameterInfo("-suppress_error", True, Description:="T/TRUE/F/FALSE")>
    <ParameterInfo("-suppress_periodic_message", True, Description:="T/TRUE/F/FALSE")>
    Public Function Run(argvs As Microsoft.VisualBasic.CommandLine.CommandLine) As Integer
        Dim Configuration As String

        If argvs.Parameters.Count = 1 AndAlso FileIO.FileSystem.FileExists(argvs.Parameters.First) Then '假若命令行仅有一个参数，且那个参数为一个存在的文件名的话，则认为哪个参数为模型文件名
            '使用默认的配置参数
            Configuration = Settings.SettingsDIR & _VIRTUALCELL_HOST_DEFAULT_CONF
            '重新生成命令行
            Dim new_argvs = CommandLine.TryParse("run")
            Call new_argvs.Add("-i", argvs.Parameters.First)
            Call new_argvs.Add("-f", "gcml")

            argvs = new_argvs
        Else
            Configuration = FileIO.FileSystem.GetParentPath(argvs("-i"))
            Configuration = FileIO.FileSystem.GetFiles(Configuration, FileIO.SearchOption.SearchTopLevelOnly, "*.inf").First
        End If

        Dim Conf As LANS.SystemsBiology.GCModeller.ModellingEngine.EngineSystem.Engine.Configuration.Configurations =
            LANS.SystemsBiology.GCModeller.ModellingEngine.EngineSystem.Engine.Configuration.Configurations.Load(Configuration)

        Return Global.LANS.SystemsBiology.GCModeller.ModellingEngine.EngineSystem.Engine.Run.Invoke(Program.ExternalModuleRegistry, Conf, argvs)
    End Function

    Const _VIRTUALCELL_HOST_DEFAULT_CONF As String = "/virtualcell_host_default.conf.inf"

    <ExportAPI("load.model.csv_tabular", Info:="The csv_tabular format model file is the alternative format of the GCModeller virtual cell modle, as the GCModeller only support the GCML xml file as the modelling data source, so that you should using this command to load the csv_tabular format model file as the GCML format.")>
    Public Function LoadCsv(ModelFile As String, LogFile As Microsoft.VisualBasic.Logging.LogFile, argvs As Microsoft.VisualBasic.CommandLine.CommandLine) _
        As LANS.SystemsBiology.GCModeller.ModellingEngine.EngineSystem.Engine.GCModeller

        Dim Model As LANS.SystemsBiology.GCModeller.ModellingEngine.Assembly.DocumentFormat.GCMarkupLanguage.BacterialModel =
            New LANS.SystemsBiology.GCModeller.ModellingEngine.Assembly.DocumentFormat.CsvTabular.DataModel.CellSystem(ModelFile, LogFile).LoadAction
        Dim EngineSystem As New GCModeller.ModellingEngine.EngineSystem.Engine.GCModeller(Model) With {.args = argvs}
        EngineSystem.ConnectLoggingClient(LogFile)
        Dim KernelModule As New ModellingEngine.EngineSystem.ObjectModels.SubSystem.CellSystem(EngineSystem)
        EngineSystem.LoadKernel(KernelModule)
        EngineSystem.ConnectDataService(New Services.DataAcquisition.ManageSystem(ModellerKernel:=EngineSystem))
        Return EngineSystem
    End Function

    <ExportAPI("Experiment.Whole_Genome_Mutation", Info:="shell parameter is the shoal shell application program file location.")>
    Public Function WholeGenomeMutation(ModelFile As String, Mutation_Factor As Double, shell As String) As Integer
        Dim Configuration = New ModellingEngine.EngineSystem.Engine.Configuration.ConfigReader(ModellingEngine.EngineSystem.Engine.Configuration.Configurations.Load(FileIO.FileSystem.GetFiles(FileIO.FileSystem.GetParentPath(ModelFile), FileIO.SearchOption.SearchTopLevelOnly, "*.inf").First))
        Dim ModelLoader = New LANS.SystemsBiology.GCModeller.ModellingEngine.Assembly.DocumentFormat.CsvTabular.FileStream.IO.XmlresxLoader(ModelFile)
        Dim StorageURL As KeyValuePair(Of GCModeller.ModellingEngine.EngineSystem.Services.DataAcquisition.ManageSystem.DataStorageServiceTypes, String) = Configuration.DataStorageURL

        If StorageURL.Key = LANS.SystemsBiology.GCModeller.ModellingEngine.EngineSystem.Services.DataAcquisition.ManageSystem.DataStorageServiceTypes.MySQL Then
            StorageURL = New KeyValuePair(Of LANS.SystemsBiology.GCModeller.ModellingEngine.EngineSystem.Services.DataAcquisition.ManageSystem.DataStorageServiceTypes, String)(LANS.SystemsBiology.GCModeller.ModellingEngine.EngineSystem.Services.DataAcquisition.ManageSystem.DataStorageServiceTypes.CSV, String.Format("{0}/vcell/", My.Computer.FileSystem.SpecialDirectories.Desktop))
        End If

        Dim Queue As List(Of String) = New List(Of String)

        For Each Gene In ModelLoader.GenomeAnnotiation
            Call New Threading.Thread(Sub() Call InternalRunGeneMutation(Gene.Identifier, "csv_tabular", StorageURL, ModelFile, Queue, Mutation_Factor, shell, "")).Start()
            Call Threading.Thread.Sleep(1000)

            Do While Queue.Count > 8
                Call Threading.Thread.Sleep(1000)
            Loop
        Next

        Return 0
    End Function

    Public ReadOnly Property DefaultConfg As String
        Get
            Return Settings.SettingsDIR & _VIRTUALCELL_HOST_DEFAULT_CONF
        End Get
    End Property

    <ExportAPI("Experiment.Whole_Genome_Mutation2", Info:="shell parameter is the shoal shell application program file location. this command is required the GCML format model file.")>
    Public Function WholeGenomeMutationFromGCML(ModelFile As String, Mutation_Factor As Double, shell As String) As Integer
        Dim Configuration = New ModellingEngine.EngineSystem.Engine.Configuration.ConfigReader(ModellingEngine.EngineSystem.Engine.Configuration.Configurations.Load(DefaultConfg))
        Dim Model = LANS.SystemsBiology.GCModeller.ModellingEngine.Assembly.DocumentFormat.GCMarkupLanguage.BacterialModel.Load(ModelFile)
        Dim StorageURL As KeyValuePair(Of GCModeller.ModellingEngine.EngineSystem.Services.DataAcquisition.ManageSystem.DataStorageServiceTypes, String) = Configuration.DataStorageURL

        If StorageURL.Key = LANS.SystemsBiology.GCModeller.ModellingEngine.EngineSystem.Services.DataAcquisition.ManageSystem.DataStorageServiceTypes.MySQL Then
            StorageURL = New KeyValuePair(Of LANS.SystemsBiology.GCModeller.ModellingEngine.EngineSystem.Services.DataAcquisition.ManageSystem.DataStorageServiceTypes, String)(LANS.SystemsBiology.GCModeller.ModellingEngine.EngineSystem.Services.DataAcquisition.ManageSystem.DataStorageServiceTypes.CSV, String.Format("{0}/vcell/", My.Computer.FileSystem.SpecialDirectories.Desktop))
        End If

        Dim Queue As List(Of String) = New List(Of String)

        For Each Gene In Model.BacteriaGenome.Genes
            Call New Threading.Thread(Sub() Call InternalRunGeneMutation(Gene.AccessionId, "gcml", StorageURL, ModelFile, Queue, Mutation_Factor, shell, conf:=DefaultConfg)).Start()
            Call Threading.Thread.Sleep(1000)

            Do While Queue.Count > 8
                Call Threading.Thread.Sleep(1000)
            Loop
        Next

        Return 0
    End Function

    Private Sub InternalRunGeneMutation(GeneId As String, format As String,
                                               StorageURL As KeyValuePair(Of GCModeller.ModellingEngine.EngineSystem.Services.DataAcquisition.ManageSystem.DataStorageServiceTypes, String),
                                               ModelFile_url As String,
                                               ByRef Queue As List(Of String), Mutation_Factor As Double, shell As String, conf As String)
        Dim ExportDir As String = String.Format("{0}/{1}/", StorageURL.Value, GeneId)
        Do While Queue.Count > 8
            Threading.Thread.Sleep(1000)
        Loop

        Dim Cmdl As String = String.Format("Run -i ""{0}"" -f " & format & " -url ""CSV://{1}/VC_MC/"" -mutation_genes {2}|{3}", ModelFile_url, ExportDir, GeneId, Mutation_Factor)

        If Not String.IsNullOrEmpty(conf) Then
            Cmdl &= String.Format(" -with_configurations ""{0}""", conf)
        End If

        Call Queue.Add(ExportDir)
        Dim FileText As String = String.Format("Imports GCModeller.Engine_Kernel" & vbCrLf & vbCrLf &
                                               "cmdl <- {0}" & vbCrLf &
                                               "Call Run $cmdl", Cmdl)
        Dim TempFile As String = String.Format("{0}/{1}.tmp", Settings.DataCache, GeneId)
        Call FileText.SaveTo(TempFile)
        Cmdl = String.Format("{0} ""{1}""", shell, TempFile)
RETRY:  Call Console.WriteLine(" -------------------->  {0}", Cmdl)
        Try
            Dim i = Microsoft.VisualBasic.Interaction.Shell(Cmdl, AppWinStyle.NormalNoFocus, Wait:=True)
            If i <> 0 Then
                Call Threading.Thread.Sleep(1000 * 10)
                GoTo RETRY
            End If
        Catch ex As Exception
            GoTo RETRY
        End Try

        Call Queue.Remove(ExportDir)
    End Sub
End Module