Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.DocumentFormat.Csv.Extensions
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.DatabaseServices.Regprecise

Namespace ShellScriptAPI

    <[PackageNamespace]("Sabio-rk", Category:=APICategories.ResearchTools, Publisher:="xie.guigang@gmail.com")>
    Public Module SabiorkKinetics

        <ExportAPI("Load.From", Info:="Load sabio-rk database from a specific data directory.")>
        Public Function LoadData(dir As String) As SMRUCC.genomics.DatabaseServices.SabiorkKineticLaws.SABIORK()
            Dim LQuery = (From strPath As String
                            In FileIO.FileSystem.GetFiles(dir, FileIO.SearchOption.SearchTopLevelOnly, "*.sbml").AsParallel
                          Where FileIO.FileSystem.GetFileInfo(strPath).Length > 0
                          Select SabiorkKineticLaws.SBMLParser.kineticLawModel.LoadDocument(strPath)).ToArray 'Read sbml file document from the filesystem
            Call Console.WriteLine("{0} sabio-rk data was loaded from filesystem!", LQuery.Count)
            Return LQuery
        End Function

        <ExportAPI("Export")>
        Public Function ExportDatabase(data As SabiorkKineticLaws.SABIORK(),
                                       <Parameter("DIR.Export", "The directory for export the loaded sabio-rk database as assembly file.")> exportDIR As String) As Integer
            Call $"Export sabio-rk database to ""{exportDIR}""".__DEBUG_ECHO
            Call "Start to proceeding ......".__DEBUG_ECHO
            Call SabiorkKineticLaws.ExportDatabase(data, exportDIR)
            Call "Job done!".__DEBUG_ECHO
            Return 0
        End Function
    End Module

    <[PackageNamespace]("MiST2.STrP_Network", Category:=APICategories.ResearchTools, Publisher:="xie.guigang@gmail.com")>
    Public Module MisT2

        <ExportAPI("Assemble.STrP",
                   Info:="Assemble the signal transduction network from string-db protein interaction data and MiST2 annotations.")>
        Public Function AssemblySignalTransductionNetwork(stringDB As IEnumerable(Of StringDB.SimpleCsv.PitrNode),
                                                          MiST2 As String,
                                                          Regulators As IEnumerable(Of RegpreciseMPBBH)) As StringDB.StrPNet.Network
            Dim Assembler As New StringDB.StrPNet.Assembler(stringDB, MiST2, Regulators)
            Return Assembler.CompileAssembly()
        End Function

        <ExportAPI("Read.Mappings.Effector", Info:="Load the effector mapping data from the database.")>
        Public Function ReadEffectorMap(Path As String) As StringDB.StrPNet.EffectorMap()
            Return Path.LoadCsv(Of StringDB.StrPNet.EffectorMap)(False).ToArray
        End Function

        <ExportAPI("Assemble.STrP_With_MetaCyc", Info:="Build the signal transduction network with MetaCyc compound as the effector mappings.")>
        Public Function AssemblySignalTransductionNetwork(stringDB As StringDB.SimpleCsv.PitrNode(),
                                                          MiST2 As String,
                                                          Regulators As DatabaseServices.Regprecise.RegpreciseMPBBH(),
                                                          Mapping As StringDB.StrPNet.EffectorMap()) As StringDB.StrPNet.Network
            Dim Assembler As New SMRUCC.genomics.DatabaseServices.StringDB.StrPNet.Assembler(stringDB, MiST2, Regulators)
            Return Assembler.CompileAssembly(Mapping)
        End Function

        <ExportAPI("Write.Xml.STrP", Info:="Save the signal transduction network model file.")>
        Public Function SaveNetwork(Network As StringDB.StrPNet.Network, <Parameter("Save.Path")> file As String) As Boolean
            Return Network.GetXml.SaveTo(file)
        End Function

        <ExportAPI("String-Db.Network.Load", Info:="Load the string-db interaction network model from database.")>
        Public Function LoadStringNetwork(Path As String) As StringDB.SimpleCsv.PitrNode()
            Return Path.LoadXml(Of SMRUCC.genomics.DatabaseServices.StringDB.SimpleCsv.Network).Nodes
        End Function

        <ExportAPI("Load.RegpreciseRegulator", Info:="Load the regulators which was mapped from the regprecise database.")>
        Public Function LoadRegpreciseRegulators(<Parameter("Map.Csv", "The bbh map data from the annotated bacterial genome and the regprecise database.")>
                                                 Csv As String) As DatabaseServices.Regprecise.RegpreciseMPBBH()
            Return Csv.LoadCsv(Of DatabaseServices.Regprecise.RegpreciseMPBBH)(False).ToArray
        End Function
    End Module
End Namespace
