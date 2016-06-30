Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.DocumentFormat.Csv.Extensions
Imports Microsoft.VisualBasic.Scripting
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Scripting.ShoalShell.Runtime.DeviceDriver.DriverHandles
Imports Microsoft.VisualBasic.Linq
Imports System.Text.RegularExpressions
Imports LANS.SystemsBiology.Assembly.KEGG.DBGET.BriteHEntry
Imports Microsoft.VisualBasic.DocumentFormat.Csv
Imports LANS.SystemsBiology.Assembly.KEGG.DBGET
Imports LANS.SystemsBiology.Assembly.KEGG
Imports LANS.SystemsBiology.Assembly.KEGG.DBGET.LinkDB
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Language

<[PackageNamespace]("KEGG", Description:="The KEGG database assembly data file operations.", Publisher:="xie.guigang@gmail.com")>
Public Module KEGG

    <ExportAPI("Read.Xml.KEGG.PathwayInfo")>
    Public Function ReadXmlModel(path As String) As Archives.Xml.XmlModel
        Return path.LoadXml(Of Archives.Xml.XmlModel)()
    End Function

    <IO_DeviceHandle(GetType(Archives.Xml.XmlModel))>
    <ExportAPI("Write.Xml.KEGG.PathwayInfo")>
    Public Function WriteXmlModel(data As Archives.Xml.XmlModel, saveXml As String) As Boolean
        Return data.GetXml.SaveTo(saveXml)
    End Function

    <ExportAPI("Compile.Pathway.Archives")>
    Public Function CompilePathwayInfo(pathways As String, modules As String, reactions As String, <Parameter("sp")> speciesCode As String) As Archives.Xml.XmlModel
        Return Archives.Xml.Compile(
            KEGGPathways:=pathways,
            KEGGModules:=modules,
            KEGGReactions:=reactions,
            speciesCode:=speciesCode)
    End Function

    <ExportAPI("Extract.KEGG_Modules")>
    Public Function ExportModuleCsv(importsDIR As String) As Archives.Csv.Module()
        Dim XmlFiles = (From path As String
                        In FileIO.FileSystem.GetFiles(importsDIR, FileIO.SearchOption.SearchAllSubDirectories, "*.xml")
                        Select path.LoadXml(Of bGetObject.Module)()).ToArray
        Return Archives.Csv.Module.Imports(XmlFiles)
    End Function

    <IO_DeviceHandle(GetType(IEnumerable(Of Archives.Csv.Module)))>
    <ExportAPI("Write.Csv.KEGG_Modules")>
    Public Function SaveKEGGModules(data As IEnumerable(Of Archives.Csv.Module), saveCsv As String) As Boolean
        Return data.SaveTo(saveCsv, False)
    End Function

    <ExportAPI("Read.Csv.KEGG_Modules")>
    Public Function ReadKeggModulesCSV(path As String) As Archives.Csv.Module()
        Return path.LoadCsv(Of Archives.Csv.Module)(False).ToArray
    End Function

    <ExportAPI("Export.Compounds")>
    Public Function ExportCompounds(xmlDIR As String, saveFile As String) As bGetObject.Compound()
        Dim LQuery = (From path As String
                      In FileIO.FileSystem.GetFiles(xmlDIR, FileIO.SearchOption.SearchTopLevelOnly, "*.xml")
                      Select path.LoadXml(Of bGetObject.Compound)()).ToArray
        Call LQuery.SaveTo(saveFile, False)
        Return LQuery
    End Function

    <ExportAPI("Export.Reactions")>
    Public Function ExportReactions(xmlDIR As String, saveCsv As String) As bGetObject.Reaction()
        Dim LQuery = (From path As String
                      In FileIO.FileSystem.GetFiles(xmlDIR, FileIO.SearchOption.SearchTopLevelOnly, "*.xml")
                      Select path.LoadXml(Of bGetObject.Reaction)()).ToArray
        Call LQuery.SaveTo(saveCsv, False)
        Return LQuery
    End Function

    <ExportAPI("Load.KEGG_Reactions")>
    Public Function LoadKEGGReactions(dir As String) As LANS.SystemsBiology.Assembly.KEGG.DBGET.bGetObject.Reaction()
        Dim LQuery = (From path As String
                      In FileIO.FileSystem.GetFiles(dir, FileIO.SearchOption.SearchAllSubDirectories, "*.xml")
                      Let reaction = path.LoadXml(Of LANS.SystemsBiology.Assembly.KEGG.DBGET.bGetObject.Reaction)()
                      Where Not reaction Is Nothing
                      Select reaction).ToArray
        Return LQuery
    End Function

    <ExportAPI("KEGG_Pathway.Downloads")>
    Public Function DownloadKeggPathways(species As String, <MetaData.Parameter("Dir.Save")> exportDir As String) As Integer
        Return LANS.SystemsBiology.Assembly.KEGG.DBGET.bGetObject.Pathway.Download(species, exportDir)
    End Function

    <InputDeviceHandle("KEGG.PathwayCollection")>
    <ExportAPI("KEGG_Pathways.Load")>
    Public Function LoadKeggPathways(<MetaData.Parameter("Source.DIR")> Dir As String) As LANS.SystemsBiology.Assembly.KEGG.DBGET.bGetObject.Pathway()
        Return (From Path As String
                In FileIO.FileSystem.GetFiles(Dir, FileIO.SearchOption.SearchAllSubDirectories, "*.xml")
                Select Path.LoadXml(Of bGetObject.Pathway)()).ToArray
    End Function

    <ExportAPI("Load.KEGG_Modules")>
    Public Function LoadKEGGModulesFromDir(dir As String) As LANS.SystemsBiology.Assembly.KEGG.DBGET.bGetObject.Module()
        Dim LQuery = (From path As String
                      In FileIO.FileSystem.GetFiles(dir, FileIO.SearchOption.SearchAllSubDirectories, "*.xml")
                      Select path.LoadXml(Of LANS.SystemsBiology.Assembly.KEGG.DBGET.bGetObject.Module)()).ToArray
        Return LQuery
    End Function

    <ExportAPI("KEGG_Pathway.Imports.Xml")>
    Public Function ConvertXml([imports] As String, <Parameter("sp.Code", "The KEGG 3 letters species brief code.")> spCode As String) As Archives.Csv.Pathway()
        Return Archives.Csv.Pathway.LoadData([imports], spCode.ToLower)
    End Function

    <IO_DeviceHandle(GetType(Generic.IEnumerable(Of LANS.SystemsBiology.Assembly.KEGG.Archives.Csv.Pathway)))>
    <ExportAPI("Write.Csv.KEGG_Pathways")>
    Public Function SavePathways(data As IEnumerable(Of Archives.Csv.Pathway), saveCsv As String) As Boolean
        Return data.SaveTo(saveCsv, False, encoding:=System.Text.Encoding.ASCII)
    End Function

    <ExportAPI("KEGG_Module.Downloads")>
    Public Function DownloadKeggModules(<Parameter("sp", "KEGG organism species brief code.")> species As String,
                                        <Parameter("Dir.Save")> saveDir As String) As Integer
        Call Modules.Download(species, saveDir)
        Return 0
    End Function

    <ExportAPI("KEGG_Module.Downloads.With.Category")>
    Public Function DownloadKEGGModules2(<Parameter("sp", "KEGG organism species brief code.")> species As String,
                                         <Parameter("Dir.Save")> saveDir As String) As Integer
        Return [Module].DownloadModules(species, saveDir)
    End Function

    <ExportAPI("Read.Csv.KEGG_Pathways")>
    Public Function ReadKEGGPathwaysCsv(path As String) As Archives.Csv.Pathway()
        Return path.LoadCsv(Of Archives.Csv.Pathway)(False).ToArray
    End Function

    <ExportAPI("Imports.Compounds.From.KEGG_Pathways")>
    Public Function GetCompoundIdlist(<MetaData.Parameter("Dir.Imports")> importsDir As String) As String()
        Return bGetObject.Pathway.GetCompoundCollection(importsDir)
    End Function

    <ExportAPI("Downloads.KEGG_Compounds")>
    Public Function DownloadKEGGCompounds(<Parameter("List.ID")> idList As Generic.IEnumerable(Of String),
                                          <Parameter("Dir.Save")> saveTo As String) As Integer
        Return bGetObject.Compound.FetchTo(idList.ToArray, saveTo)
    End Function

    <ExportAPI("Downloads.KEGG_Compounds")>
    Public Function DownloadKEGGCompound(Optional exportDIR As String = "", Optional organized As Boolean = True) As Integer
        If String.IsNullOrEmpty(exportDIR) Then
            exportDIR = GCModeller.FileSystem.KEGG.GetCompounds
        End If
        Return Compound.DownloadFromResource(exportDIR, DirectoryOrganized:=organized)
    End Function

    <ExportAPI("Reaction.idList.Imports.From.Modules")>
    Public Function GetReactionListFromModules([imports] As String) As String()
        Dim modColl = (From file As String
                       In FileIO.FileSystem.GetFiles([imports], FileIO.SearchOption.SearchAllSubDirectories, "*.xml")
                       Select file.LoadXml(Of bGetObject.Module)()).ToArray
        Return bGetObject.Module.GetKEGGReactionIdlist(modColl)
    End Function

    <ExportAPI("Downloads.KEGG_Reactions")>
    Public Function DownloadKEGGReactions(lstId As IEnumerable(Of String), saveDIR As String) As Integer
        Return bGetObject.Reaction.FetchTo(lstId.ToArray, saveDIR)
    End Function

    <ExportAPI("Downloads.KEGG_Reactions")>
    Public Function DownloadKEGGReactions(Optional EXPORT As String = "", <Parameter("Is.Organized")> Optional organized As Boolean = True) As Integer
        If String.IsNullOrEmpty(EXPORT) Then
            EXPORT = GCModeller.FileSystem.KEGG.GetReactions
        End If
        Return EnzymaticReaction.DownloadReactions(EXPORT, DirectoryOrganized:=organized)
    End Function

    <ExportAPI("Modules.Statics")>
    Public Function ModulesStatics(DIR As String) As DocumentStream.File
        Dim Modules As String() =
            FileIO.FileSystem.GetFiles(DIR, FileIO.SearchOption.SearchAllSubDirectories, "*.xml") _
                .ToArray(Function(file) _
                             Regex.Match(System.IO.Path.GetFileNameWithoutExtension(file),
                                         "M\d+",
                                         RegexOptions.IgnoreCase).Value.ToUpper)
        Dim lstModuleEntry = [Module].LoadFromResource
    End Function

#Region "XML模型文件更新函数"

    <ExportAPI("kegg.general.updates")>
    Public Function KEGGGeneralUpdates(source As String, export As String) As Boolean
        Dim Load = (From path In source.LoadSourceEntryList({"*.xml"}) Select ID = path.Key, XML = FileIO.FileSystem.ReadAllText(path.Value)).ToArray

        For Each xmlFile In Load
            Call xmlFile.XML.Replace("<KeyValuePair Key=", "<DictionaryEntry Key=").SaveTo(export & "/" & xmlFile.ID & ".xml")
        Next

        Return True
    End Function

    ''' <summary>
    ''' 由于在修改了对象类型的XML元数据定义之后，原先的XML文件的结构已经无法对应于当前的数据定义了，则可以尝试使用本方法进行XML文件数据格式的更新
    ''' </summary>
    ''' <param name="source"></param>
    ''' <param name="export"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <ExportAPI("kegg.pathway.xml.schema.updates")>
    Public Function KEGGPathwaysXMLUpdatesTool(source As String, export As String) As Boolean
        Dim LQuery = (From path In source.LoadSourceEntryList({"*.xml"}).AsParallel
                      Let obj = path.Value.LoadXml(Of LANS.SystemsBiology.Assembly.KEGG.DBGET.bGetObject.Pathway)()
                      Where Not obj Is Nothing
                      Select ID = path.Key, Xml = FileIO.FileSystem.ReadAllText(path.Value), obj).ToArray
        '更新一些比较重要的值
        Dim UPDATES = (From item In LQuery Select InternalUpdates(item.obj, item.Xml)).ToArray
        For Each Item In UPDATES
            Call Item.GetXml.SaveTo(export & "/" & Item.EntryId & ".xml")
        Next

        Return True
    End Function

    Private Function InternalUpdates(pwy As LANS.SystemsBiology.Assembly.KEGG.DBGET.bGetObject.Pathway, Xml As String) As LANS.SystemsBiology.Assembly.KEGG.DBGET.bGetObject.Pathway
        'Dim ENTRY_ID As String = Regex.Match(Xml, "R\d+").Value
        ' pwy.EntryId = ENTRY_ID

        Dim Compounds = Regex.Match(Xml, "<Compound>.+?</Compound>", RegexOptions.Singleline).Value
        Dim Modules = Regex.Match(Xml, "<Modules>.+?</Modules>", RegexOptions.Singleline).Value
        Dim Genes = Regex.Match(Xml, "<Genes>.+?</Genes>", RegexOptions.Singleline).Value

        pwy.Compound = InternalEntryTryParser(Compounds)
        pwy.Modules = InternalEntryTryParser(Modules)
        pwy.Genes = InternalEntryTryParser(Genes)

        Return pwy
    End Function

    ''' <summary>
    ''' 由于在修改了对象类型的XML元数据定义之后，原先的XML文件的结构已经无法对应于当前的数据定义了，则可以尝试使用本方法进行XML文件数据格式的更新
    ''' </summary>
    ''' <param name="source"></param>
    ''' <param name="export"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <ExportAPI("kegg.reaction.xml.schema.updates")>
    Public Function KEGGReactionsXMLUpdatesTool(source As String, export As String) As Boolean
        Dim LQuery = (From path In source.LoadSourceEntryList({"*.xml"}).AsParallel
                      Let obj = path.Value.LoadXml(Of LANS.SystemsBiology.Assembly.KEGG.DBGET.bGetObject.Reaction)()
                      Where Not obj Is Nothing
                      Select ID = path.Key, Xml = FileIO.FileSystem.ReadAllText(path.Value), obj).ToArray
        '更新一些比较重要的值
        Dim UPDATES = (From item In LQuery Select InternalUpdates(item.obj, item.Xml)).ToArray
        For Each Item In UPDATES
            Call Item.GetXml.SaveTo(export & "/" & Item.Entry & ".xml")
        Next

        Return True
    End Function

    Private Function InternalUpdates(rxn As LANS.SystemsBiology.Assembly.KEGG.DBGET.bGetObject.Reaction, Xml As String) As LANS.SystemsBiology.Assembly.KEGG.DBGET.bGetObject.Reaction
        Dim ENTRY_ID As String = Regex.Match(Xml, "R\d+").Value
        Dim EC = Regex.Match(Xml, "<EnzymeClass>.+?</EnzymeClass>").Value
        If Not String.IsNullOrEmpty(EC) Then rxn.ECNum = {EC.GetValue}
        rxn.Entry = ENTRY_ID

        Dim Pathways = Regex.Match(Xml, "<Pathway>.+?</Pathway>", RegexOptions.Singleline).Value
        Dim Modules = Regex.Match(Xml, "<Module>.+?</Module>", RegexOptions.Singleline).Value

        rxn.Pathway = InternalEntryTryParser(Pathways)
        rxn.Module = InternalEntryTryParser(Modules)

        Return rxn
    End Function

    Private Function InternalEntryTryParser(str As String) As KeyValuePair()
        Dim p = (From m As Match In Regex.Matches(str, "Key=.+? Value=.+? />") Select m.Value).ToArray
        Dim buf As KeyValuePair() = Nothing

        If Not p.IsNullOrEmpty Then
            buf = LinqAPI.Exec(Of KeyValuePair) <= From s As String
                                                   In p
                                                   Let tokens As String() = (From m As Match
                                                                             In Regex.Matches(s, "="".+?""")
                                                                             Let ss = m.Value
                                                                             Select Mid(ss, 3, ss.Length - 3)).ToArray
                                                   Select New KeyValuePair With {
                                                       .Key = tokens(0),
                                                       .Value = tokens.Last
                                                   }
        End If

        Return buf
    End Function

#End Region
End Module
