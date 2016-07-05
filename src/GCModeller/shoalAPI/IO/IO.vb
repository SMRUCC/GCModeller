#Region "Microsoft.VisualBasic::a0202fde4a4bea23067830c4ab4a57ae, ..\GCModeller\shoalAPI\IO\IO.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
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

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.DocumentFormat.Csv.Extensions
Imports LANS.SystemsBiology.Assembly.NCBI.GenBank.Extensions
Imports Microsoft.VisualBasic.Scripting.ShoalShell
Imports LANS.SystemsBiology.AnalysisTools.ProteinTools.Sanger.Pfam.ProteinDomainArchitecture.MPAlignment
Imports Microsoft.VisualBasic.Scripting.ShoalShell.Runtime.DeviceDriver.DriverHandles
Imports LANS.SystemsBiology.Assembly
Imports LANS.SystemsBiology.Assembly.NCBI.GenBank.TabularFormat
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic
Imports LANS.SystemsBiology.Toolkits.RNA_Seq
Imports LANS.SystemsBiology.SequenceModel
Imports LANS.SystemsBiology.AnalysisTools.ProteinTools.Sanger.Pfam.PfamString
Imports LANS.SystemsBiology.AnalysisTools.ProteinTools.Sanger.Pfam.PfamFastaComponentModels
Imports LANS.SystemsBiology.AnalysisTools.DataVisualization
Imports LANS.SystemsBiology.AnalysisTools.DataVisualization.ChromosomeMap
Imports LANS.SystemsBiology.DatabaseServices.Regprecise
Imports LANS.SystemsBiology.AnalysisTools.DataVisualization.Interaction.Cytoscape
Imports LANS.SystemsBiology.Assembly.Expasy.AnnotationsTool
Imports LANS.SystemsBiology.Assembly.Expasy.Database
Imports LANS.SystemsBiology.Assembly.MetaCyc.File.FileSystem
Imports LANS.SystemsBiology.Assembly.MiST2
Imports LANS.SystemsBiology.SequenceModel.FASTA
Imports Microsoft.VisualBasic.Text
Imports LANS.SystemsBiology.Assembly.NCBI
Imports LANS.SystemsBiology.AnalysisTools.CellularNetwork.PFSNet.DataStructure
Imports LANS.SystemsBiology.Assembly.NCBI.GenBank
Imports LANS.SystemsBiology.AnalysisTools.DataVisualization.Interaction.Cytoscape.CytoscapeGraphView.XGMML

#Const KERNEL_DEBUG = False

''' <summary>
''' This namespace module contains the various biological database I/O (reads and write) operations.
''' </summary>
''' <remarks></remarks>
<[PackageNamespace]("GCModeller.Assembly.File.IO",
                    Description:="This namespace module contains the various biological database I/O (reads and write) operations for the GCModeller.",
                    Publisher:="xie.guigang@gmail.com")>
Public Module IO

    <InputDeviceHandle("MotifScan.LDM")>
    <ExportAPI("Read.Xml.MotifScanModel")>
    Public Function ReadRegpreciseAnnotationModel(path As String) As LANS.SystemsBiology.AnalysisTools.NBCR.Extensions.MEME_Suite.Analysis.MotifScans.AnnotationModel
        Return path.LoadXml(Of LANS.SystemsBiology.AnalysisTools.NBCR.Extensions.MEME_Suite.Analysis.MotifScans.AnnotationModel)
    End Function

    <ExportAPI("Read.Csv.VirtualFootprints")>
    <InputDeviceHandle("Virtual.Footprint")>
    Public Function ReadVritualFootprint(path As String) As LANS.SystemsBiology.AnalysisTools.NBCR.Extensions.MEME_Suite.Analysis.GenomeMotifFootPrints.VirtualFootprints()
        Return path.LoadCsv(Of LANS.SystemsBiology.AnalysisTools.NBCR.Extensions.MEME_Suite.Analysis.GenomeMotifFootPrints.VirtualFootprints)(False).ToArray
    End Function

    <ExportAPI("Read.Csv.RegpreciseFootprints", Info:="This data contains the regulation relationship between the TF and its downstream regulated genes.")>
    <InputDeviceHandle("Regprecise.Footprint")>
    Public Function ReadRegpreciseFootprint(path As String) As LANS.SystemsBiology.AnalysisTools.NBCR.Extensions.MEME_Suite.Analysis.GenomeMotifFootPrints.PredictedRegulationFootprint()
        Return path.LoadCsv(Of LANS.SystemsBiology.AnalysisTools.NBCR.Extensions.MEME_Suite.Analysis.GenomeMotifFootPrints.PredictedRegulationFootprint)(False).ToArray
    End Function

    <InputDeviceHandle("reader")>
    <ExportAPI("reader.create")>
    Public Function CreateReader(Sequence As I_PolymerSequenceModel) As LANS.SystemsBiology.SequenceModel.NucleotideModels.SegmentReader
        Return New LANS.SystemsBiology.SequenceModel.NucleotideModels.SegmentReader(NucleicAcid:=Sequence.SequenceData)
    End Function

    <InputDeviceHandle("bbh")>
    Public Function ReadBBh(path As String) As LANS.SystemsBiology.NCBI.Extensions.LocalBLAST.Application.BBH.BiDirectionalBesthit()
        Return path.LoadCsv(Of LANS.SystemsBiology.NCBI.Extensions.LocalBLAST.Application.BBH.BiDirectionalBesthit)(False).ToArray
    End Function

    <IO_DeviceHandle(GetType(LANS.SystemsBiology.DatabaseServices.ComparativeGenomics.AnnotationTools.Reports.GenomeAnnotations))>
    <ExportAPI("write.xml.anno")>
    Public Function WriteAnnotationResult(result As LANS.SystemsBiology.DatabaseServices.ComparativeGenomics.AnnotationTools.Reports.GenomeAnnotations, saveto As String) As Boolean
        Return result.GetXml.SaveTo(saveto)
    End Function

    <InputDeviceHandle("besthit")>
    <ExportAPI("read.csv.besthit")>
    Public Function LoadBesthit(path As String) As LANS.SystemsBiology.NCBI.Extensions.LocalBLAST.Application.BBH.BestHit()
        Return path.LoadCsv(Of LANS.SystemsBiology.NCBI.Extensions.LocalBLAST.Application.BBH.BestHit)(False).ToArray
    End Function

    <InputDeviceHandle("kegg.refmap")>
    <ExportAPI("Read.Xml.KEGG.refMap")>
    Public Function LoadRefmap(path As String) As LANS.SystemsBiology.Assembly.KEGG.DBGET.ReferenceMap.ReferenceMapData
        Return path.LoadXml(Of LANS.SystemsBiology.Assembly.KEGG.DBGET.ReferenceMap.ReferenceMapData)()
    End Function

    <ExportAPI("Sequence", Info:="Gets the sequence data from the target fasta object.")>
    Public Function GetSequenceData(sequence As LANS.SystemsBiology.SequenceModel.FASTA.FastaToken) As String
        Return sequence.SequenceData
    End Function

    <InputDeviceHandle("PDB")>
    <ExportAPI("Read.PDB")>
    Public Function ReadPDB(path As String) As LANS.SystemsBiology.Assembly.RCSB.PDB.PDB
        Return LANS.SystemsBiology.Assembly.RCSB.PDB.PDB.Load(path)
    End Function

    <InputDeviceHandle("Fasta.Token")>
    <ExportAPI("Read.Fasta.Token")>
    Public Function ReadFastaObject(path As String) As LANS.SystemsBiology.SequenceModel.FASTA.FastaToken
        Return LANS.SystemsBiology.SequenceModel.FASTA.FastaToken.Load(path)
    End Function

    <IO_DeviceHandle(GetType(LANS.SystemsBiology.SequenceModel.FASTA.FastaToken))>
    Public Function WriteFastaObject(Fasta As LANS.SystemsBiology.SequenceModel.FASTA.FastaToken, SaveTo As String) As Boolean
        Return Fasta.SaveTo(SaveTo)
    End Function

    <IO_DeviceHandle(GetType(LANS.SystemsBiology.Assembly.DOOR.DOOR))>
    Public Function WriteDoor(Data As LANS.SystemsBiology.Assembly.DOOR.DOOR, SaveTo As String) As Boolean
        Return Data.Save(SaveTo)
    End Function

    <InputDeviceHandle("Uniprot")>
    <ExportAPI("Read.Fasta.Uniprot")>
    Public Function LoadUniprotFasta(path As String) As LANS.SystemsBiology.Assembly.Uniprot.UniprotFasta()
        Return LANS.SystemsBiology.Assembly.Uniprot.UniprotFasta.LoadFasta(path)
    End Function

    <IO_DeviceHandle(GetType(Generic.IEnumerable(Of LANS.SystemsBiology.Assembly.Uniprot.UniprotFasta)))>
    <ExportAPI("Write.Fasta.Uniprot")>
    Public Function WriteUniprotFasta(data As Generic.IEnumerable(Of LANS.SystemsBiology.Assembly.Uniprot.UniprotFasta), saveto As String) As Boolean
        Return CType(data, LANS.SystemsBiology.SequenceModel.FASTA.FastaFile).Save(Path:=saveto)
    End Function

    <InputDeviceHandle("ExpasyAnno")>
    <ExportAPI("Read.Csv.ExpasyAnno")>
    Public Function LoadExpasyAnnotations(path As String) As LANS.SystemsBiology.Assembly.Expasy.AnnotationsTool.EnzymeClass()
        Return path.LoadCsv(Of LANS.SystemsBiology.Assembly.Expasy.AnnotationsTool.EnzymeClass)(False).ToArray
    End Function

    <IO_DeviceHandle(GetType(Generic.IEnumerable(Of LANS.SystemsBiology.Assembly.Expasy.AnnotationsTool.EnzymeClass)))>
    <ExportAPI("Write.Csv.ExpasyAnno")>
    Public Function WriteExpasyAnnotations(data As Generic.IEnumerable(Of LANS.SystemsBiology.Assembly.Expasy.AnnotationsTool.EnzymeClass), saveto As String) As Boolean
        Return data.SaveTo(saveto, False)
    End Function

    ''' <summary> 
    ''' 将没有序列的fasta对象删除
    ''' </summary>
    ''' <param name="FASTA"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ''' 
    <ExportAPI("Fasta.TrimNull")>
    Public Function TrimNull(FASTA As FastaFile) As FastaFile
        Dim LQuery = (From fa In FASTA.AsParallel Where Not String.IsNullOrEmpty(fa.SequenceData) AndAlso fa.Length >= 6 Select fa).ToArray
        Dim Grouped = (From item In LQuery Let sig = (item.Attributes(1) & item.SequenceData).ToUpper Select item, sig Group By sig Into Group).ToArray
        LQuery = (From item In Grouped Select item.Group.First.item).ToArray
        Return CType(LQuery, LANS.SystemsBiology.SequenceModel.FASTA.FastaFile)
    End Function

    <ExportAPI("Fasta.GrepTitle")>
    Public Function GrepFasta(data As IEnumerable(Of FastaToken), method As TextGrepScriptEngine) As FastaFile
        Dim LQuery As FastaToken() = (From fa As FastaToken In data Select fa.GrepTitle(method.Method)).ToArray
        Return CType(LQuery, FastaFile)
    End Function

    <InputDeviceHandle("GrepScript")>
    <ExportAPI("Grep_Script.Create")>
    Public Function CreateScript(script As String) As TextGrepScriptEngine
        Return TextGrepScriptEngine.Compile(script)
    End Function

    <ExportAPI("CDD.Build")>
    Public Function BuildCDD(DIR As String, EXPORT As String) As Boolean
        Try
            Call CDD.DbFile.BuildDb(DIR, EXPORT)
            Return True
        Catch ex As Exception
            ex = New Exception($"{NameOf(DIR)} = {DIR}", ex)
            ex = New Exception($"{NameOf(EXPORT)} = {EXPORT}", ex)
            Call App.LogException(ex)
            Call ex.PrintException
            Return False
        End Try
    End Function

    <IO_DeviceHandle(GetType(IEnumerable(Of MPCsvArchive)))>
    Public Function WriteMPAlignmentOutput(data As IEnumerable(Of MPCsvArchive), saveto As String) As Boolean
        Return data.SaveTo(saveto, False)
    End Function

    <IO_DeviceHandle(GetType(Generic.IEnumerable(Of LANS.SystemsBiology.AnalysisTools.NBCR.Extensions.MEME_Suite.Analysis.GenomeMotifFootPrints.PredictedRegulationFootprint)))>
    Public Function WriteFootprintData(data As Generic.IEnumerable(Of LANS.SystemsBiology.AnalysisTools.NBCR.Extensions.MEME_Suite.Analysis.GenomeMotifFootPrints.PredictedRegulationFootprint), saveto As String) As Boolean
        Return data.SaveTo(saveto, False)
    End Function

    <IO_DeviceHandle(GetType(Generic.IEnumerable(Of LANS.SystemsBiology.AnalysisTools.NBCR.Extensions.MEME_Suite.Analysis.GenomeMotifFootPrints.VirtualFootprints)))>
    Public Function WriteFootprintData(data As Generic.IEnumerable(Of LANS.SystemsBiology.AnalysisTools.NBCR.Extensions.MEME_Suite.Analysis.GenomeMotifFootPrints.VirtualFootprints), saveto As String) As Boolean
        Return data.SaveTo(saveto, False)
    End Function

    <ExportAPI("Write.Csv.Besthits")>
    <IO_DeviceHandle(GetType(Generic.IEnumerable(Of NCBI.Extensions.LocalBLAST.Application.BBH.BestHit)))>
    Public Function WriteBesthit(data As Generic.IEnumerable(Of NCBI.Extensions.LocalBLAST.Application.BBH.BestHit), saveto As String) As Boolean
        Return data.SaveTo(saveto, False)
    End Function

    <ExportAPI("Write.Csv.BBH")>
    <IO_DeviceHandle(GetType(Generic.IEnumerable(Of LANS.SystemsBiology.NCBI.Extensions.LocalBLAST.Application.BBH.BiDirectionalBesthit)))>
    Public Function WriteBesthit(data As Generic.IEnumerable(Of LANS.SystemsBiology.NCBI.Extensions.LocalBLAST.Application.BBH.BiDirectionalBesthit), saveto As String) As Boolean
        Return data.SaveTo(saveto, False)
    End Function

    <IO_DeviceHandle(GetType(Generic.IEnumerable(Of LANS.SystemsBiology.NCBI.Extensions.LocalBLAST.Application.RpsBLAST.MyvaCOG)))>
    Public Function WriteMyvaCog(data As Generic.IEnumerable(Of LANS.SystemsBiology.NCBI.Extensions.LocalBLAST.Application.RpsBLAST.MyvaCOG), saveto As String) As Boolean
        Return data.SaveTo(saveto, False)
    End Function

    <InputDeviceHandle("MyvaCOG")>
    <ExportAPI("Read.Csv.MyvaCOG")>
    Public Function ReadMyvaCog(Path As String) As LANS.SystemsBiology.NCBI.Extensions.LocalBLAST.Application.RpsBLAST.MyvaCOG()
        Return Path.LoadCsv(Of LANS.SystemsBiology.NCBI.Extensions.LocalBLAST.Application.RpsBLAST.MyvaCOG)(False).ToArray
    End Function

    <IO_DeviceHandle(GetType(PccMatrix))>
    Public Function WritePccMAT(data As PccMatrix, saveto As String) As Boolean
        Return data.SaveTo(saveto)
    End Function

    <IO_DeviceHandle(GetType(KeyValuePair(Of Drawing.Imaging.ImageFormat, Drawing.Bitmap())))>
    <ExportAPI("Write.Image.ChromosomeMap_resource")>
    Public Function Save_ChromosomeMap(resource As KeyValuePair(Of Drawing.Imaging.ImageFormat, Drawing.Bitmap()), export As String) As Boolean
        Dim i As Integer = 0

        Call FileIO.FileSystem.CreateDirectory(export)

        For Each bmp In resource.Value
            i += 1
            Call bmp.Save(String.Format("{0}/ChromosomeMap_Drawing_data.resources__{1}.bmp", export, i), resource.Key)
        Next
        Return i
    End Function

    <InputDeviceHandle("ChromosomeMap.Config")>
    <ExportAPI("Read.TXT.Drawing_Config")>
    Public Function LoadChromosomeMapConfig(Path As String) As ChromosomeMap.Configurations
        Return ChromesomeMapAPI.LoadConfig(Path)
    End Function

    <InputDeviceHandle("regprecise")>
    Public Function ReadRegpreciseXml(path As String) As TranscriptionFactors
        Return path.LoadXml(Of TranscriptionFactors)()
    End Function

    <ExportAPI("write.csv.regprecise_bh")>
    <IO_DeviceHandle(GetType(Generic.IEnumerable(Of RegpreciseMPBBH)))>
    Public Function WriteResult(data As Generic.IEnumerable(Of RegpreciseMPBBH), saveto As String) As Boolean
        Return data.SaveTo(saveto, False)
    End Function

    <IO_DeviceHandle(GetType(Generic.IEnumerable(Of BiDirectionalBesthit)))>
    Public Function WriteResult(data As Generic.IEnumerable(Of BiDirectionalBesthit), saveto As String) As Boolean
        Return data.SaveTo(saveto, False)
    End Function

    <ExportAPI("pfam_alignment_output.new")>
    Public Function CreateAlignmentOutput() As List(Of AlignmentOutput)
        Return New List(Of AlignmentOutput)
    End Function

    <IO_DeviceHandle(GetType(IEnumerable(Of AlignmentOutput)))>
    <ExportAPI("write.txt.pfam_alignment_output")>
    Public Function SaveAlignmentOutput(output As IEnumerable(Of AlignmentOutput), saveto As String) As Boolean
        Dim ChunkBuffer As String = String.Join(vbCrLf, (From item In output Let strValue As String = item.ToString Select strValue).ToArray)
        Return ChunkBuffer.SaveTo(saveto, System.Text.Encoding.ASCII)
    End Function

    <ExportAPI("pfam_alignment_output.add_item")>
    Public Function AddOutputItem(list As List(Of AlignmentOutput), item As AlignmentOutput) As Integer
        Call list.Add(item)
        Return 0
    End Function

    <OutputDeviceHandle(GetType(AlignmentOutput))>
    Public Function DisplayPfamScore(score As AlignmentOutput) As String
        Dim s As String = score.ToString
        Call Console.WriteLine(s)
        Return s
    End Function

    <InputDeviceHandle("pfam_string")>
    <ExportAPI("read.csv.pfam_string")>
    Public Function ReadPfamString(path As String) As PfamString()
        Return path.LoadCsv(Of PfamString)(False).ToArray
    End Function

    <IO_DeviceHandle(GetType(IEnumerable(Of PfamString)))>
    <ExportAPI("write.csv.pfam_string")>
    Public Function WritePfamString(data As IEnumerable(Of PfamString), path As String) As Boolean
        Return data.SaveTo(path, False)
    End Function

    <InputDeviceHandle("pfam_a")>
    <ExportAPI("read.fasta.pfam")>
    Public Function ReadPfam(path As String) As PfamFasta()
        Dim FastaFile As FASTA.FastaFile = FASTA.FastaFile.Read(path)
        Return (From FastaObject As FASTA.FastaToken
                In FastaFile.AsParallel
                Select PfamFasta.CreateObject(FastaObject)).ToArray
    End Function

    <IO_DeviceHandle(GetType(IEnumerable(Of PfamCsvRow)))>
    <ExportAPI("write.csv.pfam")>
    Public Function WritePfamCsv(data As IEnumerable(Of PfamCsvRow), path As String) As Boolean
        Return data.SaveTo(path, False)
    End Function

    <IO_DeviceHandle(GetType(IEnumerable(Of PFSNetResultOut)))>
    <ExportAPI("write.csv.pfsnet_result")>
    Public Function WritePfsNETResult(data As IEnumerable(Of PFSNetResultOut), saveto As String) As Boolean
        Throw New NotImplementedException
    End Function

    <IO_DeviceHandle(GetType(PFSNetResultOut))>
    <ExportAPI("write.xml.pfsnet_result")>
    Public Function WritePfsNETResult(data As PFSNetResultOut, saveto As String) As Boolean
        Return data.GetXml.SaveTo(saveto)
    End Function

    <InputDeviceHandle("pfam_csv")>
    <ExportAPI("pfam.fasta_2_pfam.csv")>
    Public Function Convert2PfamCsv(data As IEnumerable(Of PfamFasta)) As PfamCsvRow()
        Return PfamFasta.CreateCsvArchive(data)
    End Function

    <IO_DeviceHandle(GetType(IEnumerable(Of PfamFasta)))>
    <ExportAPI("write.fasta.pfam")>
    Public Function WritePfamFasta(data As IEnumerable(Of PfamFasta), path As String) As Boolean
        Return FASTA.FastaFile.SaveData(data, path)
    End Function

    <InputDeviceHandle("door")>
    <ExportAPI("read.door_file")>
    Public Function ReadDoor(path As String) As DOOR.DOOR
        Return DOOR.Load(path)
    End Function

    <InputDeviceHandle("mist2")>
    <ExportAPI("read.mist2")>
    Public Function ReadMiST2(file As String) As MiST2
        Return file.LoadXml(Of MiST2)()
    End Function

    <InputDeviceHandle("pgdb")>
    <ExportAPI("read.pgdb")>
    Public Function LoadPGDB(dir As String) As DatabaseLoadder
        Return DatabaseLoadder.CreateInstance(dir, Explicit:=False)
    End Function

    <InputDeviceHandle("expasy")>
    <ExportAPI("read.expasy")>
    Public Function LoadExpasy(path As String) As NomenclatureDB
        Return NomenclatureDB.CreateObject(path)
    End Function

    <IO_DeviceHandle(GetType(IEnumerable(Of T_EnzymeClass_BLAST_OUT)))>
    <ExportAPI("write.csv.enzyme_class")>
    Public Function WriteEnzymeClassData(data As IEnumerable(Of T_EnzymeClass_BLAST_OUT), path As String) As Boolean
        Return data.SaveTo(path, False)
    End Function

    <IO_DeviceHandle(GetType(Graph))>
    <ExportAPI("Write.Xml.Cytoscape")>
    Public Function WriteCytoscapeModelView(cysModel As Graph, SaveTo As String) As Boolean
        Return cysModel.Save(SaveTo)
    End Function

    <InputDeviceHandle("cytoscape")>
    <ExportAPI("read.xml.cytoscape")>
    Public Function ReadCytoscapeModelView(path As String) As Graph
        Return Graph.Load(path)
    End Function

    <InputDeviceHandle("PTT")>
    <ExportAPI("Read.PTT", Info:="Load the ptt object from a text file with specific path.")>
    Public Function ReadPTT(<Parameter("PTT.Path")> File As String) As PTT
        Dim PTT = TabularFormat.PTT.Load(File)
        Return PTT
    End Function

    <InputDeviceHandle("PTT.Database")>
    <ExportAPI("Load.PTT.Database")>
    Public Function LoadPtt(<Parameter("Dir.PTT", "Directory path which contains all of the bacteria its genome information like *.ptt, *.rnt, *.fna, *.gbk")>
                            DIR As String) As PTTDbLoader
        Return New PTTDbLoader(DIR)
    End Function

    <InputDeviceHandle("genome_sequence")>
    <ExportAPI("get.genome_sequence")>
    Public Function GetGenomeSequence(PTT As PTTDbLoader) As NucleotideModels.NucleicAcid
        Return NucleotideModels.NucleicAcid.CreateObject(PTT.GenomeFasta.SequenceData)
    End Function

    ''' <summary>
    ''' 文件的默认编码格式是ASCII
    ''' </summary>
    ''' <param name="fasta"></param>
    ''' <param name="saveto"></param>
    ''' <returns></returns>
    <IO_DeviceHandle(GetType(FastaFile))>
    <ExportAPI("Write.Fasta", Info:="Please notice that the default encoding format of this save method is ASCII.")>
    Public Function SaveFasta(fasta As FastaFile, saveto As String) As Boolean
        Return fasta.Save(saveto, System.Text.Encoding.ASCII)
    End Function

    <ExportAPI("Write.Fasta", Info:="You can using this method to specific the saved encoding format of the fasta file.")>
    Public Function SaveFasta(Fasta As IEnumerable(Of FASTA.FastaToken), SaveTo As String, encoding As System.Text.Encoding) As Boolean
        Return CType(Fasta.ToArray, FastaFile).Save(SaveTo, encoding)
    End Function

    <InputDeviceHandle("gbk")>
    <ExportAPI("read.gbff")>
    Public Function ReadGbk(path As String) As GBFF.File
        Return GBFF.File.Read(path)
    End Function

    <InputDeviceHandle("genbank")>
    <ExportAPI("read.genbank")>
    Public Function LoadGenbank(Path As String) As GBFF.File
        Return GBFF.File.Load(Path)
    End Function

    <InputDeviceHandle("translations")>
    <ExportAPI("gbk.export_proteins")>
    Public Function ExportProteins(gbk As GBFF.File) As FastaFile
        Return gbk.ExportProteins_Short
    End Function

    <InputDeviceHandle("fasta")>
    <ExportAPI("read.fasta")>
    Public Function ReadFastaFile(path As String) As FastaFile
        Return FastaFile.Read(path)
    End Function

    <InputDeviceHandle("gene_ids")>
    <ExportAPI("get.idlist")>
    Public Function GetIdList(FastaFile As FastaFile) As String()
        Return (From FastaObject In FastaFile Select FastaObject.Attributes.First.Split.First).ToArray
    End Function

    <ExportAPI("read.chebi.names_table")>
    Public Function ReadChEBINamesTable(path As String) As LANS.SystemsBiology.Assembly.EBI.ChEBI.Database.IO.StreamProviders.Tsv.Tables.Names()
        Return LANS.SystemsBiology.Assembly.EBI.ChEBI.Database.IO.StreamProviders.Tsv.FileIO.Load(Of LANS.SystemsBiology.Assembly.EBI.ChEBI.Database.IO.StreamProviders.Tsv.Tables.Names)(path)
    End Function

    ''' <summary>
    ''' Merge the fasta files in a directory into a total fasta file.(将目标文件夹之中的文件合并至一个Fasta文件之中)
    ''' </summary>
    ''' <param name="dir"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <ExportAPI("Fasta.Merge", Info:="Merge the fasta file which contains in one directory into a big fasta file.")>
    Public Function MergeFasta(<Parameter("Dir.Merge.Source", "The source directory which contains the fasta file data.")> dir As String) As LANS.SystemsBiology.SequenceModel.FASTA.FastaFile
        Dim LQuery = (From File As String
                      In FileIO.FileSystem.GetFiles(dir, FileIO.SearchOption.SearchAllSubDirectories, "*.fasta", "*.fsa", "*.fa").AsParallel
                      Let FastaData = LANS.SystemsBiology.SequenceModel.FASTA.FastaFile.Read(File)
                      Where Not FastaData.IsNullOrEmpty
                      Select FastaData.ToArray).ToArray.MatrixToVector
        Dim FastaFile As LANS.SystemsBiology.SequenceModel.FASTA.FastaFile =
            CType((From FastaObject As LANS.SystemsBiology.SequenceModel.FASTA.FastaToken
                   In LQuery.AsParallel
                   Where Not (FastaObject.Attributes.IsNullOrEmpty OrElse FastaObject.Length = 0)
                   Select FastaObject
                   Order By FastaObject.Attributes.First Ascending).ToArray, LANS.SystemsBiology.SequenceModel.FASTA.FastaFile)
        Return FastaFile
    End Function

    <ExportAPI("length", Info:="Get the length of the fasta sequence.")>
    Public Function Len(FastaObject As LANS.SystemsBiology.SequenceModel.FASTA.FastaToken) As Integer
        Return FastaObject.Length
    End Function

#If Not KERNEL_DEBUG Then

    <ExportAPI("Read.DbXref.ToGo")>
    Public Function LoadToGoDocument(Path As String, DbXref As String) As GeneOntology.ToGo()
        Return GeneOntology.ToGo.LoadDocument(Path, DbXrefHead:=DbXref)
    End Function

    <IO_DeviceHandle(GetType(Generic.IEnumerable(Of GeneOntology.ToGo)))>
    <ExportAPI("Write.Csv.DbXref.ToGo")>
    Public Function WriteToGoAsCsv(DbXref As Generic.IEnumerable(Of GeneOntology.ToGo), <Parameter("Path.Save")> SaveTo As String) As Boolean
        Return DbXref.SaveTo(SaveTo, False)
    End Function

    <InputDeviceHandle("GO")>
    <ExportAPI("Read.Obo")>
    Public Function ReadOBOFile(path As String) As GeneOntology.AnnotationFile
        Return GeneOntology.AnnotationFile.LoadDocument(path)
    End Function

    <IO_DeviceHandle(GetType(GeneOntology.AnnotationFile))>
    <ExportAPI("Write.Obo")>
    Public Function SaveOBOFile(data As GeneOntology.AnnotationFile, saveto As String) As Boolean
        Return data.Save(saveto)
    End Function

    <ExportAPI("Write.Csv.GO.OBO", Info:="Save the go.obo annotation data as a csv excel data table file.")>
    Public Function WriteOboAsCsv(data As GeneOntology.AnnotationFile, <Parameter("Path.Save")> SavePath As String) As Boolean
        Return data.Terms.SaveTo(SavePath, False)
    End Function

    <InputDeviceHandle("gaf")>
    <ExportAPI("Read.Gaf")>
    Public Function ReadGAF(path As String) As GeneOntology.GAF()
        Return GeneOntology.GAF.Load(path)
    End Function

    <IO_DeviceHandle(GetType(Generic.IEnumerable(Of GeneOntology.GAF)))>
    <ExportAPI("Write.Gaf")>
    Public Function WriteGAF(data As Generic.IEnumerable(Of GeneOntology.GAF), saveto As String) As Boolean
        Return GeneOntology.GAF.Save(data, saveto)
    End Function
#End If
End Module
