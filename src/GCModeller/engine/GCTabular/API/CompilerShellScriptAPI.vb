#Region "Microsoft.VisualBasic::fc7fd80aafa75ebae5c9bd8aa535dfb6, ..\GCModeller\engine\GCTabular\API\CompilerShellScriptAPI.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.Extensions
Imports SMRUCC.genomics.Data
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.GCTabular.FileStream.XmlFormat
Imports SMRUCC.genomics.Model.Network.STRING

<[Namespace]("GCModeller.Compiler.GCML.Csvx")>
Public Module CompilerShellScriptAPI

    <ExportAPI("model.optimization")>
    Public Function OptimizationModel(Model As FileStream.IO.XmlresxLoader) As IO.File
        Dim Optimization = New Compiler.Components.MetabolismOptimization()
        Dim result = Optimization.Optimization(Model)
        Return result
    End Function

    <ExportAPI("optimization.apply_from_fba")>
    Public Function ApplyFBAOptimization(Model As FileStream.IO.XmlresxLoader,
                                         Result As Generic.IEnumerable(Of Microsoft.VisualBasic.ComponentModel.Collection.Generic.KeyValuePairObject(Of String, Double))) _
        As Integer

        Call Compiler.Components.MetabolismOptimization.ApplyOptimation(Model, Result.ToArray)
        Return 0
    End Function

    ''' <summary>
    ''' <see cref="TCS.SensorInducers.SensorId">Inducer</see>的值可能是CommonName，MetaCyc标识符或者KEGG标识符，这个方法仅仅是生成一个模板数据，用于编译器使用的
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ''' 
    <ExportAPI("sensing_profile.extract")>
    Public Function ExtractSensingProfiles(MiST2 As SMRUCC.genomics.Assembly.MiST2.MiST2) _
        As TCS.SensorInducers()

        Dim IDList As String() = {
            (From protein In MiST2.MajorModules.First.OneComponent Select protein.Identifier).ToArray,
            (From protein In MiST2.MajorModules.First.Chemotaxis Select protein.Identifier).ToArray
        }.ToVector

        Dim Profile = (From strId As String
                       In (From strValue As String In IDList Select strValue Distinct Order By strValue Ascending).ToArray
                       Select New TCS.SensorInducers With
                              {
                                  .SensorId = strId, .Inducers = New String() {}}).ToArray
        Return Profile
    End Function

    <ExportAPI("sensing_profile.save")>
    Public Function SaveProfile(sensingProfiles As TCS.SensorInducers(), save As String) As Boolean
        Return sensingProfiles.SaveTo(save, False)
    End Function

    <ExportAPI("fullfill_operator.create_object")>
    Public Function CreateOperator(xmlFile As String, Metacyc As String) As FullFillModel
        Return New FullFillModel(xmlFile, Metacyc)
    End Function

    <ExportAPI("fullfill_operator.kegg")>
    Public Function Fill_KEGG([operator] As FullFillModel, KEGGCompounds As String, KEGGReactions As String, CARMEN As String) As Integer
        Call [operator].FullFillModel_Kegg(KEGGCompounds, KEGGReactions, CARMEN)
        Return 0
    End Function

    <ExportAPI("fullfill_operator.sabio-rk")>
    Public Function Fill_Sabiork([operator] As FullFillModel, SabiorkCompounds As String, SabiorkKinetics As String, EnzymeModifyKinetics As String, ExpasyMatches As String) As Integer
        Call [operator].FullFillModel_Sabiork(SabiorkCompounds, SabiorkKinetics, EnzymeModifyKinetics, ExpasyMatches.LoadCsv(Of SMRUCC.genomics.Assembly.Expasy.AnnotationsTool.T_EnzymeClass_BLAST_OUT)(False).ToArray)
        Return 0
    End Function

    <ExportAPI("transmembrane.create_object")>
    Public Function TransmembraneMatch(MetaCycAll As String, ExpasyClassCsv As String) As Compiler.Components.AnalysisTransmembraneFlux
        Return New Compiler.Components.AnalysisTransmembraneFlux(MetaCycAll, ExpasyClassCsv)
    End Function

    <ExportAPI("get.loader")>
    Public Function GetModle([operator] As FullFillModel) As FileStream.IO.XmlresxLoader
        Return [operator].ModelLoader
    End Function

    <ExportAPI("cellsystemloader.create_object")>
    Public Function CreateLoader(Model As FileStream.XmlFormat.CellSystemXmlModel) As FileStream.IO.XmlresxLoader
        Return New FileStream.IO.XmlresxLoader(CellSystem:=Model)
    End Function


    <ExportAPI("model.read_from_file")>
    Public Function LoadModel(path As String) As FileStream.IO.XmlresxLoader
        Return New FileStream.IO.XmlresxLoader(path)
    End Function

    <ExportAPI("read.xml.cell_csv")>
    Public Function ReadModelXml(path As String) As FileStream.XmlFormat.CellSystemXmlModel
        Return path.LoadXml(Of FileStream.XmlFormat.CellSystemXmlModel)()
    End Function

    <ExportAPI("analysis_transmembrane")>
    Public Function AnalysisTransmembraneFlux([operator] As Compiler.Components.AnalysisTransmembraneFlux,
                                              FullFilledModel As FileStream.IO.XmlresxLoader,
                                              Expasy As SMRUCC.genomics.Assembly.Expasy.Database.NomenclatureDB) _
        As Integer

        Call [operator].Invoke(FullFilledModel, Expasy)
        Return 0
    End Function

    <ExportAPI("write.fullfill_operator")>
    Public Function WriteModel([operator] As FullFillModel) As Integer
        Call [operator].WriteData()
        Return 0
    End Function

    <ExportAPI("-precompile")>
    Public Function PreCompile(metacyc As String,
                               regprecise_regulator_xml As String,
                               transcript_regulation As String,
                               mist2 As String,
                               mist2_strp_xml As String,
                               stringdb As String) _
 _
        As GCTabular.Compiler.Compiler

        Dim Compiler = New GCTabular.Compiler.Compiler()
        Dim args = New KeyValuePair(Of String, String)() {
 _
            New KeyValuePair(Of String, String)("-metacyc", metacyc),
            New KeyValuePair(Of String, String)("-regprecise_regulator", regprecise_regulator_xml),
            New KeyValuePair(Of String, String)("-export", My.Computer.FileSystem.SpecialDirectories.Temp),
            New KeyValuePair(Of String, String)("-transcript_regulation", transcript_regulation),
            New KeyValuePair(Of String, String)("-mist2", mist2),
            New KeyValuePair(Of String, String)("-mist2_strp", mist2_strp_xml),
            New KeyValuePair(Of String, String)("-string-db", stringdb)
        }
        Dim argvs = Microsoft.VisualBasic.CommandLine.CreateObject(Name:="-precompile", args:=args)
        Call Compiler.PreCompile(argvs)
        Return Compiler
    End Function

    <ExportAPI("Invoke.compile", Info:="invoke the compiler compile method to compile the data into a virtual cell model file.",
        Usage:="invoke.compile compiler $compiler argvs $argv_string",
        Example:="call gcmodeller.compiler invoke.compile compiler $compiler argvs $argv_string")>
    Public Function Compile(compiler As Compiler.Compiler, argvs As String) As CellSystemXmlModel

        Call compiler.Compile(argvs)
        Return compiler.Return
    End Function

    <ExportAPI("write.encrypted_model")>
    Public Function WriteEncryptedModel(Model As FileStream.XmlFormat.CellSystemXmlModel, savefile As String) As Boolean
        Dim XML As FileStream.IO.XmlresxLoader = New FileStream.IO.XmlresxLoader(Model)
        Call XML.Encryption.SaveTo(savefile)
        Return True
    End Function

    <ExportAPI("csv_model_loader.new")>
    Public Function NewXmlLoader() As FileStream.IO.XmlresxLoader
        Return FileStream.IO.XmlresxLoader.CreateObject
    End Function

    <ExportAPI("csv_model.new", Info:="This command is just for the programming debug.")>
    Public Function CreateNewModel() As FileStream.XmlFormat.CellSystemXmlModel
        Return New FileStream.XmlFormat.CellSystemXmlModel
    End Function
End Module
