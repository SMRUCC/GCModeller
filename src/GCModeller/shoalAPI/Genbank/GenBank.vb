#Region "Microsoft.VisualBasic::4f8a4daefadff9d76e3e98e47522735a, ..\GCModeller\shoalAPI\Genbank\GenBank.vb"

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

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.Reflection.Reflector
Imports Microsoft.VisualBasic.DocumentFormat.Csv.Extensions
Imports LANS.SystemsBiology.Assembly.NCBI.GenBank.gbExportService
Imports Microsoft.VisualBasic.Scripting
Imports Microsoft.VisualBasic.Scripting.ShoalShell.Runtime.DeviceDriver.DriverHandles
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Text
Imports Microsoft.VisualBasic.Linq
Imports LANS.SystemsBiology.Assembly.NCBI.GenBank.GBFF.Keywords.FEATURES
Imports LANS.SystemsBiology.SequenceModel.FASTA
Imports Microsoft.VisualBasic.Text.Similarity
Imports Microsoft.VisualBasic
Imports LANS.SystemsBiology.Assembly.NCBI.GenBank.TabularFormat
Imports LANS.SystemsBiology.Assembly.NCBI.GenBank
Imports LANS.SystemsBiology.Assembly.NCBI.GenBank.TabularFormat.ComponentModels
Imports LANS.SystemsBiology.Assembly.NCBI.GenBank.CsvExports
Imports LANS.SystemsBiology.SequenceModel
Imports Microsoft.VisualBasic.DocumentFormat.Csv
Imports Microsoft.VisualBasic.Language

<[PackageNamespace]("NCBI.Genbank",
                    Category:=APICategories.UtilityTools,
                    Description:="Utility tools for NCBI genbank file IO and data export operations.",
                    Publisher:="xie.guigang@live.com")>
Public Module GenBank

    <ExportAPI("Source.Copy")>
    Public Function Copy(Source As String, Target As String, ParamArray ext As String()) As Boolean
        For Each Folder As String In FileIO.FileSystem.GetDirectories(Source)
            Dim Export As String = FileIO.FileSystem.GetDirectoryInfo(Folder).Name
            Call Console.WriteLine(" --->  " & Export)
            Export = Target & "/" & Export
            Call FileIO.FileSystem.CreateDirectory(Export)

            For Each extToken As String In ext
                For Each File As String In FileIO.FileSystem.GetFiles(Folder, FileIO.SearchOption.SearchTopLevelOnly, extToken)
                    Dim CopyTo As String = Export & "/" & FileIO.FileSystem.GetFileInfo(File).Name

                    If FileIO.FileSystem.FileExists(CopyTo) Then
                        If FileIO.FileSystem.GetFileInfo(File).Length <> FileIO.FileSystem.GetFileInfo(CopyTo).Length Then
                            Call FileIO.FileSystem.CopyFile(File, CopyTo)
                        End If
                    Else
                        Call FileIO.FileSystem.CopyFile(File, CopyTo)
                    End If
                Next
            Next
        Next

        Return True
    End Function

    ''' <summary>
    ''' 函数返回不成功的编号
    ''' </summary>
    ''' <param name="list"></param>
    ''' <param name="entry_info"></param>
    ''' <param name="source"></param>
    ''' <param name="copyTo"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ''' 
    <ExportAPI("Selected.Def.Source.Copy")>
    Public Function SelectByDefinition(list As IEnumerable(Of String),
                                       entry_info As IEnumerable(Of gbEntryBrief),
                                       source As String,
                                       copyTo As String) As String()
        Dim SourceList = source.LoadSourceEntryList("*.gbk", "*.gb")
        Dim LQuery = (From item In entry_info
                      Let def As String = FuzzyMatchString.StringSelection(item.Definition, list)
                      Where Not String.IsNullOrEmpty(def)
                      Select GenbankEntry = item, def).ToArray
        Dim IDList = (From item In LQuery Select item.GenbankEntry.AccessionID Distinct).ToArray
        Dim PathList = (From id As String In IDList Select SourceList(id)).ToArray
        Call SourceCopy(PathList, copyTo, True)

        '获取不成功的描述数据
        Dim Failures = (From item In LQuery Select item.def).ToArray
        Failures = (From s As String In list Where Array.IndexOf(Failures, s) = -1 Select s).ToArray
        Return Failures
    End Function

    ''' <summary>
    ''' 导出所有质粒的复制起点的序列
    ''' </summary>
    ''' <param name="source"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ''' 
    <ExportAPI("Export.Genbank.oriV", Info:="Exports the oriV site from the genbank files.")>
    Public Function ExportOriVMotif(source As String) As FastaFile
        Dim Genbank = (From path In source.LoadSourceEntryList("*.gbk", "*.gb").AsParallel
                       Let gbk = GBFF.File.Load(path.Value)
                       Where Not gbk Is Nothing AndAlso gbk.IsPlasmidSource
                       Select ID = path.Key, Gbk_data = gbk).ToArray
        Dim LQuery = (From item In Genbank
                      Let oriV As GBFF.Keywords.FEATURES.Feature() =
                          item.Gbk_data.Features.ListFeatures("rep_origin")
                      Where Not oriV.IsNullOrEmpty
                      Select ID = item.ID, oriV).ToArray
        Dim ExportFasta As FastaToken()() =
            (From item In LQuery Select __oriV(item.ID, item.oriV)).ToArray
        Dim FastaFile As New FASTA.FastaFile(ExportFasta.MatrixAsIterator)
        Return FastaFile
    End Function

    Private Function __oriV(Id As String, oriVSites As GBFF.Keywords.FEATURES.Feature()) As FASTA.FastaToken()
        Dim list As New List(Of FASTA.FastaToken)

        For i As Integer = 0 To oriVSites.Length - 1
            Dim fasta As New FASTA.FastaToken
            Dim OriV = oriVSites(i)
            fasta.Attributes = New String() {Id, OriV.Location.ToString}
            fasta.SequenceData = OriV.SequenceData

            Call list.Add(fasta)
        Next

        Return list.ToArray
    End Function

    <ExportAPI("Gene.Statics")>
    Public Function Statics(<Parameter("Dump.CDS")> CDSinfo As IEnumerable(Of GeneDumpInfo),
                            <Parameter("List.geneName")>
                            Optional lstGeneName As IEnumerable(Of String) = Nothing) As DocumentStream.File
        Dim Grouped = (From item In CDSinfo Select item Group By item.Species Into Group).ToArray
        If lstGeneName.IsNullOrEmpty Then
            lstGeneName = (From item As GeneDumpInfo
                           In CDSinfo
                           Where Not String.IsNullOrEmpty(item.GeneName)
                           Select item.GeneName
                           Distinct
                           Order By GeneName Ascending).ToArray
        End If

        Dim File As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File =
            New DocumentFormat.Csv.DocumentStream.File
        Call File.AppendLine("Organisms")
        Call File.First.AddRange(lstGeneName)

        Dim LQuery = (From Species
                      In Grouped.AsParallel
                      Select __row(lstGeneName, Species.Species, Species.Group.ToArray)).ToArray
        Call File.AppendRange(LQuery)

        Return File
    End Function

    Private Function __row(lstGeneName As Generic.IEnumerable(Of String),
                           species As String,
                           Group As LANS.SystemsBiology.Assembly.NCBI.GenBank.CsvExports.GeneDumpInfo()) As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.RowObject
        Dim row As New Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.RowObject From {species}

        For Each id As String In lstGeneName
            Dim LfffQuery = (From gene In Group Where String.Equals(gene.GeneName, id) Select 1).ToArray
            If LfffQuery.IsNullOrEmpty Then
                Call row.Add("-")
            Else
                Call row.Add("+")
            End If
        Next

        Return row
    End Function

    ''' <summary>
    ''' 函数返回去除掉重复记录之后的实际的记录数目
    ''' </summary>
    ''' <param name="source"></param>
    ''' <param name="Trim"></param> 
    ''' <returns></returns>
    ''' <remarks></remarks>
    <ExportAPI("Genbank.Source.Distinct")>
    Public Function Distinct(source As String, Optional Trim As Boolean = True) As Integer
        Return LANS.SystemsBiology.Assembly.NCBI.GenBank.Distinct(source, Trim).Count
    End Function

    <ExportAPI("Genbank.Source.Distinct")>
    Public Function Distinct(Source As Generic.IEnumerable(Of LANS.SystemsBiology.Assembly.NCBI.GenBank.CsvExports.Plasmid)) _
        As LANS.SystemsBiology.Assembly.NCBI.GenBank.CsvExports.Plasmid()
        Return LANS.SystemsBiology.Assembly.NCBI.GenBank.Distinct(Source)
    End Function

    ''' <summary>
    ''' 根据<paramref name="DataEntries"></paramref>之中的编号来复制基因组的文件数据
    ''' </summary>
    ''' <param name="DataEntries"></param>
    ''' <param name="source"></param>
    ''' <param name="copyto"></param>
    ''' <param name="ext">只需要列举出后缀名即可，不需要额外的.或者*.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <ExportAPI("Genbank.Source.Copy")>
    Public Function SelectCopyFasta(DataEntries As Generic.IEnumerable(Of LANS.SystemsBiology.Assembly.NCBI.GenBank.CsvExports.Plasmid),
                                    <MetaData.Parameter("Dir.Source", "The directory of the file data source.")> Source As String,
                                    <MetaData.Parameter("Dir.CopyTo", "The directory of the file data will be copied to.")> CopyTo As String,
                                    <MetaData.Parameter("List.Ext", "The default file type to copy between these two directory is fasta file. " &
                                        "There is no needs for the additional joiner character '.' or '*.', example as ""fasta"" for fasta file, " &
                                        "example as ""*.fasta"" will causing exceptions.")>
                                    Optional ext As Generic.IEnumerable(Of String) = Nothing) As Boolean

        If ext.IsNullOrEmpty Then ext = New String() {"fasta"}

        Call FileIO.FileSystem.CreateDirectory(CopyTo)

        Source = FileIO.FileSystem.GetDirectoryInfo(Source).FullName
        CopyTo = FileIO.FileSystem.GetDirectoryInfo(CopyTo).FullName

        For Each ID As String In (From Entry As LANS.SystemsBiology.Assembly.NCBI.GenBank.CsvExports.Plasmid
                                  In DataEntries
                                  Where Not String.IsNullOrEmpty(Entry.AccessionID)
                                  Select Entry.AccessionID
                                  Distinct).ToArray

            Call Console.Write(".")

            Dim PathList As String() = (From extType As String
                                        In ext
                                        Let Path As String = Source & "/" & ID & "." & extType
                                        Where FileIO.FileSystem.FileExists(Path)
                                        Select Path).ToArray

            For Each Path As String In PathList
                Try
                    Call FileIO.FileSystem.CopyFile(Path, CopyTo & "/" & FileIO.FileSystem.GetFileInfo(Path).Name, True)
                Catch ex As Exception
                    Call Console.WriteLine(Path.ToFileURL)
                    Call Console.WriteLine(ex.ToString)
                End Try
            Next
        Next

        Return True
    End Function

    <ExportAPI("genbank.source.get.acc_id")>
    Public Function GetAccessionID(source As Generic.IEnumerable(Of LANS.SystemsBiology.Assembly.NCBI.GenBank.CsvExports.gbEntryBrief)) As String()
        Return (From item In source Select item.AccessionID Distinct).ToArray
    End Function

    <ExportAPI("ptt.copy_genome.seq_fasta")>
    Public Function CopyGenomeSequence(source As String, copyTo As String) As Integer
        Return LANS.SystemsBiology.Assembly.NCBI.GenBank.gbExportService.CopyGenomeSequence(source, copyTo)
    End Function

    <ExportAPI("export.pubmed", Info:="Export all of the avaliable reference paper pubmed from the exported plasmid information data.")>
    Public Function ExportPubMed(data As String, export As String) As String()
        Dim datafile = data.LoadCsv(Of LANS.SystemsBiology.Assembly.NCBI.GenBank.CsvExports.Plasmid)(False)
        Dim LQuery = (From item In datafile.AsParallel Let getpubmed = Function() As String()
                                                                           Dim l As New List(Of String)
                                                                           If InStr(item.Reference1, "[PUBMED ") > 0 Then Call l.Add(item.Reference1)
                                                                           If InStr(item.Reference2, "[PUBMED ") > 0 Then Call l.Add(item.Reference2)
                                                                           If InStr(item.Reference3, "[PUBMED ") > 0 Then Call l.Add(item.Reference3)
                                                                           If InStr(item.Reference4, "[PUBMED ") > 0 Then Call l.Add(item.Reference4)
                                                                           If InStr(item.Reference5, "[PUBMED ") > 0 Then Call l.Add(item.Reference5)
                                                                           If InStr(item.Reference6, "[PUBMED ") > 0 Then Call l.Add(item.Reference6)

                                                                           Return l.ToArray
                                                                       End Function Select getpubmed()).ToArray.MatrixToVector.Distinct.ToArray
        Call System.IO.File.WriteAllLines(export, LQuery)
        Return LQuery
    End Function

    <InputDeviceHandle("Genbank.As.Ptt")>
    <ExportAPI("Genbank.As.Ptt")>
    Public Function GenbankAsPtt(GenbankFile As String) As LANS.SystemsBiology.Assembly.NCBI.GenBank.TabularFormat.PTT
        Dim Genbank = LANS.SystemsBiology.Assembly.NCBI.GenBank.GBFF.File.Load(GenbankFile)
        Dim Ptt = LANS.SystemsBiology.Assembly.NCBI.GenBank.gbExportService.GbkffExportToPTT(Genbank)
        Return Ptt
    End Function

    <InputDeviceHandle("Genbank.As.PttRes")>
    Public Function GenbankAsPttResource(GenbankFile As String) As PTTDbLoader
        Dim Genbank As GBFF.File = GBFF.File.Load(GenbankFile)
        Dim Ptt = gbExportService.GbkffExportToPTT(Genbank)
        Dim PttRes As PTTDbLoader = PTTDbLoader.CreateObject(Ptt, Genbank.Origin.ToFasta)
        For Each gene As KeyValuePair(Of String, GeneBrief) In PttRes
            gene.Value.Gene = Ptt(gene.Value.Synonym).Gene
        Next
        Return PttRes
    End Function

    ''' <summary>
    ''' Merge the ncbi annotation data. parameter source and the merged is the csv file path of the CDs ncbi annotation data export. 
    ''' notices that this function will group the duplicated accession id and onyl retrun the first object with the accessionid, 
    ''' which means this function is also distince the protein source.
    ''' </summary>
    ''' <param name="source"></param>
    ''' <param name="merged"></param>
    ''' <param name="save"></param>
    ''' <returns></returns>
    <ExportAPI("anno.merge",
               Info:="Merge the ncbi annotation data. parameter source and the merged is the csv file path of the CDs ncbi annotation data export. notices that this function will group the duplicated accession id and onyl retrun the first object with the accessionid, which means this function is also distince the protein source.")>
    Public Function MergeAnnotations(source As String, merged As String, save As String) As GeneDumpInfo()
        Dim LQuery = (From item As GeneDumpInfo
                      In source.LoadCsv(Of GeneDumpInfo)(False) +
                          merged.LoadCsv(Of GeneDumpInfo)(False).ToArray
                      Select item
                      Group By item.LocusID Into Group).ToArray
        Dim data = (From item In LQuery Select item.Group.First).ToArray
        Call data.SaveTo(save, False)
        Return data
    End Function

    <InputDeviceHandle("gene_dump")>
    <ExportAPI("read.csv.gene_dump")>
    Public Function ReadGeneDumpInformation(path As String) As LANS.SystemsBiology.Assembly.NCBI.GenBank.CsvExports.GeneDumpInfo()
        Return path.LoadCsv(Of LANS.SystemsBiology.Assembly.NCBI.GenBank.CsvExports.GeneDumpInfo)(False).ToArray
    End Function

    <InputDeviceHandle("Genbank.EntryInfo")>
    <ExportAPI("read.csv.genbank_info")>
    Public Function ReadGenbankEntryInfo(path As String) As LANS.SystemsBiology.Assembly.NCBI.GenBank.CsvExports.gbEntryBrief()
        Return path.LoadCsv(Of LANS.SystemsBiology.Assembly.NCBI.GenBank.CsvExports.gbEntryBrief)(False).ToArray
    End Function

    <InputDeviceHandle("Plasmid")>
    <ExportAPI("read.csv.genbank_plasmid_info")>
    Public Function ReadPlasmidGenbankEntryInfo(path As String) As LANS.SystemsBiology.Assembly.NCBI.GenBank.CsvExports.Plasmid()
        Return path.LoadCsv(Of LANS.SystemsBiology.Assembly.NCBI.GenBank.CsvExports.Plasmid)(False).ToArray
    End Function

    <ExportAPI("write.csv.genbank_dump")>
    <IO_DeviceHandle(GetType(Generic.IEnumerable(Of LANS.SystemsBiology.Assembly.NCBI.GenBank.CsvExports.gbEntryBrief)))>
    Public Function WriteGenbankEntryDump(data As Generic.IEnumerable(Of LANS.SystemsBiology.Assembly.NCBI.GenBank.CsvExports.gbEntryBrief), saveto As String) As Boolean
        Return data.SaveTo(saveto, False)
    End Function

    <ExportAPI("write.csv.genbank_plasmid")>
    <IO_DeviceHandle(GetType(Generic.IEnumerable(Of LANS.SystemsBiology.Assembly.NCBI.GenBank.CsvExports.Plasmid)))>
    Public Function WriteGenbankPlasmidEntryDump(data As Generic.IEnumerable(Of LANS.SystemsBiology.Assembly.NCBI.GenBank.CsvExports.Plasmid),
                                                 <MetaData.Parameter("Path.Saved", "The path of the csv file to save the data.")> SaveTo As String) As Boolean
        Return data.SaveTo(SaveTo, False)
    End Function

    <ExportAPI("write.csv.gene_dump")>
    <IO_DeviceHandle(GetType(Generic.IEnumerable(Of LANS.SystemsBiology.Assembly.NCBI.GenBank.CsvExports.GeneDumpInfo)))>
    Public Function WriteGeneDump(data As Generic.IEnumerable(Of LANS.SystemsBiology.Assembly.NCBI.GenBank.CsvExports.GeneDumpInfo), saveto As String) As Boolean
        Return data.SaveTo(saveto, False)
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="source"></param>
    ''' <param name="ExportToDir"></param>
    ''' <param name="ext">默认文件的后缀名为*.gbk和*.gb</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ''' 
    <ExportAPI("Export.Genbank",
               Info:="Batch export the genome brief statics information from the ncbi genebank database file. The genbank database file in the target input directory should have the file extension of *.gb or *.gbk")>
    Public Function Export(Source As String,
                           <MetaData.Parameter("Dir.Export")> ExportToDir As String,
                           <MetaData.Parameter("List.Load.Ext")> Optional Ext As String() = Nothing,
                           <MetaData.Parameter("WGS.Trim")> Optional Trim_WGS As Boolean = False) As Boolean
        If Ext.IsNullOrEmpty Then
            Ext = New String() {"*.gbk", "*.gb"}
        End If

        Dim FileList = (From path As String In FileIO.FileSystem.GetFiles(Source, FileIO.SearchOption.SearchAllSubDirectories, Ext).AsParallel
                        Let Genbank = LANS.SystemsBiology.Assembly.NCBI.GenBank.GBFF.File.Load(path)
                        Where Not Genbank Is Nothing AndAlso Genbank.HasSequenceData
                        Select Genbank).ToArray
        Dim GeneList As LANS.SystemsBiology.Assembly.NCBI.GenBank.CsvExports.GeneDumpInfo() = Nothing
        Dim Entrys As LANS.SystemsBiology.Assembly.NCBI.GenBank.CsvExports.gbEntryBrief() = Nothing

        If Trim_WGS Then
            FileList = (From item In FileList Where Not item.IsWGS Select item).ToArray
        End If

        Call LANS.SystemsBiology.Assembly.NCBI.GenBank.BatchExport(FileList, GeneList, Entrys, ExportToDir)
        Call GeneList.SaveTo(ExportToDir & "/ORF.Info.csv", False)
        Call Entrys.SaveTo(ExportToDir & "/Genbank.Entry.csv", False)

        Return True
    End Function

    <ExportAPI("export.plasmid_gbk")>
    Public Function ExportPlasmid(input As String, export_dir As String, Optional ext As String() = Nothing, Optional Trim_WGS As Boolean = False) As Boolean
        If ext.IsNullOrEmpty Then
            ext = New String() {"*.gbk", "*.gb"}
        End If

        Return ExportPlasmid(FileIO.FileSystem.GetFiles(input, FileIO.SearchOption.SearchAllSubDirectories, ext), export_dir)
    End Function

    <ExportAPI("export.plasmid_gbk")>
    Public Function ExportPlasmid(source As Generic.IEnumerable(Of String), export_dir As String, Optional Trim_WGS As Boolean = False) As Boolean
        Dim FileList = (From path In source.LoadSourceEntryList.AsParallel
                        Let gbk = LANS.SystemsBiology.Assembly.NCBI.GenBank.GBFF.File.Load(path.Value)
                        Where gbk.IsPlasmidSource AndAlso gbk.HasSequenceData
                        Select gbk).ToArray
        Dim GeneList As LANS.SystemsBiology.Assembly.NCBI.GenBank.CsvExports.GeneDumpInfo() = Nothing
        Dim Entrys As LANS.SystemsBiology.Assembly.NCBI.GenBank.CsvExports.Plasmid() = Nothing

        If Trim_WGS Then
            FileList = (From item In FileList Where Not item.IsWGS Select item).ToArray
        End If

        Call LANS.SystemsBiology.Assembly.NCBI.GenBank.BatchExportPlasmid(FileList, GeneList, Entrys, export_dir)
        Call GeneList.SaveTo(export_dir & "/CDS_INFO.csv", False)
        Call Entrys.SaveTo(export_dir & "/GBKFF_ENTRY_INFO.plasmid.csv", False)

        Return True
    End Function

    ''' <summary>
    ''' 从gbk文件之中加载蛋白质的dump信息
    ''' </summary>
    ''' <param name="gbk"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ''' 
    <InputDeviceHandle("gbk.Dump")>
    <ExportAPI("gbk.Load.AsDump")>
    Public Function gbkLoadAsDump(gbk As String) As GeneDumpInfo()
        Dim Gbf = GBFF.File.Load(gbk)
        Dim ChunkBuffer = gbExportService.ExportGeneAnno(Gbf)
        Return ChunkBuffer
    End Function

    ''' <summary>
    ''' 从gbk文件之中加载蛋白质的dump信息
    ''' </summary>
    ''' <param name="gbk"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>    
    <ExportAPI("gbk.Load.AsDump")>
    Public Function gbkLoadAsDump(gbk As GBFF.File) As GeneDumpInfo()
        Dim ChunkBuffer = gbExportService.ExportGeneAnno(gbk)
        Return ChunkBuffer
    End Function

    ''' <summary>
    ''' http://www.cbs.dtu.dk/services/gwBrowser/
    ''' </summary>
    ''' <param name="gbk"></param>
    ''' <param name="justCDS"></param>
    ''' <returns></returns>
    <ExportAPI("CopyBrief", Info:="Generates the custom annotation data for gwBrowser at http://www.cbs.dtu.dk/services/gwBrowser/,  
example format can be review from: http://www.cbs.dtu.dk/services/gwBrowser/CP000550.ann")>
    Public Function CopyBrief(gbk As GBFF.File, Optional justCDS As Boolean = True) As String
        Dim gbkDump As GeneDumpInfo() =
            LinqAPI.Exec(Of GeneDumpInfo) <= From site As GeneDumpInfo
                                             In FeatureDumps(gbk)
                                             Select site
                                             Order By site.Location.Left Ascending
        Dim Features As String() = gbkDump.ToArray(AddressOf __briefFormat)
        Return String.Join(vbCrLf, Features)
    End Function

    Private Function __briefFormat(feature As LANS.SystemsBiology.Assembly.NCBI.GenBank.CsvExports.GeneDumpInfo) As String
        'Return $"{feature.COG}                {feature.Location.Left}    {feature.Location.Right} {feature.Strand} {feature.GeneName}"
        Return $"{feature.COG}{vbTab}{feature.Location.Left}{vbTab}{feature.Location.Right}{vbTab}{feature.Strand}{vbTab}{feature.GeneName}"
    End Function

    <ExportAPI("Read.Gbk", Info:="If the error occurring, this function will returns the null value.")>
    Public Function LoadGenbank(path As String) As LANS.SystemsBiology.Assembly.NCBI.GenBank.GBFF.File
        Return LANS.SystemsBiology.Assembly.NCBI.GenBank.GBFF.File.Load(path)
    End Function
End Module
