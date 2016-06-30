Imports LANS.SystemsBiology.Assembly
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.DocumentFormat.Csv.Extensions
Imports Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Scripting.ShoalShell
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports LANS.SystemsBiology.Assembly.NCBI.GenBank.GBFF.Keywords.FEATURES
Imports LANS.SystemsBiology.Assembly.NCBI.GenBank
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic
Imports LANS.SystemsBiology.SequenceModel.FASTA
Imports Microsoft.VisualBasic.Terminal.Utility
Imports Microsoft.VisualBasic.Parallel
Imports Microsoft.VisualBasic.Linq

<[PackageNamespace]("LANS.SystemsBiology.WebServices",
                    Category:=APICategories.UtilityTools,
                    Description:="Some command entry in this namespace is required the bioperl was install on your computer.",
                    Publisher:="xie.guigang@live.com")>
Public Module WebServices

    <ExportAPI("ncbi.Entrez._start()")>
    Public Function NCBI_Entrez(keyword As String) As LANS.SystemsBiology.Assembly.NCBI.Entrez.QueryHandler
        Return New LANS.SystemsBiology.Assembly.NCBI.Entrez.QueryHandler(keyword)
    End Function

    <ExportAPI("Entrez.next_page")>
    Public Function EntrezGetEntries(Entrez As LANS.SystemsBiology.Assembly.NCBI.Entrez.QueryHandler) As LANS.SystemsBiology.Assembly.NCBI.Entrez.QueryHandler.Entry()
        Return Entrez.DownloadCurrentPage
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="list">AccessionID列表</param>
    ''' <param name="export">保存的文件夹</param>
    ''' <returns>返回下载成功的文件数目</returns>
    ''' <remarks></remarks>
    ''' 
    <ExportAPI("genbank.batch_download", Info:="This command required the bioperl package installed on your computer!")>
    Public Function DownloadGBK(list As Generic.IEnumerable(Of String), export As String) As Integer
        Dim InternalFileExists = Function(path As String) FileIO.FileSystem.FileExists(path) AndAlso FileIO.FileSystem.GetFileInfo(path).Length > 0
        Dim l As Integer
        Dim pb As New CBusyIndicator(_start:=True)

        For i As Integer = 0 To list.Count - 1
            Dim sw = Stopwatch.StartNew
            Dim ID As String = list(i)
            Dim saved As String = String.Format("{0}/{1}.gbk", export, ID)

            Call Console.WriteLine(ID & "   ............................. " & i / list.Count * 100 & "%")

            If InternalFileExists(saved) Then
                Continue For
            Else
                '否则在数据源之中进行查询
                Dim Paths = LANS.SystemsBiology.GCModeller.Workbench.DatabaseServices.Model_Repository.RQL.QueryGBKByLocusID(ID)
                If Not Paths.IsNullOrEmpty Then
                    '能够查询到数据，则直接从本地数据库之中复制文件
                    Call FileIO.FileSystem.CopyFile(Paths.First, saved, True)
                    Continue For
                End If
            End If

            saved = LANS.SystemsBiology.Assembly.NCBI.Entrez.QueryHandler.Entry.DownloadGBK(export, ID)
            If InternalFileExists(saved) Then
                l += 1
                Call Console.WriteLine("[DEBUG] {0} was download at ""{1}ms"".", ID, sw.ElapsedMilliseconds)
            Else
                Call Console.WriteLine("[DEBUG] {0} was download not successfully!", ID)
            End If
        Next

        Return l
    End Function

    <ExportAPI("download.uniprot")>
    Public Function DownloadUniprot(IdList As String(), TempDir As String) As LANS.SystemsBiology.SequenceModel.FASTA.FastaFile
        Dim ReturnData As LANS.SystemsBiology.SequenceModel.FASTA.FastaFile = New SequenceModel.FASTA.FastaFile

        Call Console.WriteLine("{0} sequence ready to go!", IdList.Count)

        For Each Id As String In IdList
            Dim Path As String = String.Format("{0}/{1}.fasta", TempDir, Id)
            If Not FileIO.FileSystem.FileExists(Path) Then
                Call Console.WriteLine("<<< {0}", Id)
                Dim FastaObject = LANS.SystemsBiology.Assembly.Uniprot.Web.WebServices.DownloadProtein(Id)

                If Not FastaObject.Length = 0 Then
                    Call FastaObject.SaveTo(Path)
                    Call ReturnData.Add(FastaObject)
                End If
            Else
                Call ReturnData.Add(FastaToken.Load(Path))
            End If
        Next

        Return ReturnData
    End Function

    <ExportAPI("download.expasy", Info:="Download the enzyme protein sequence fasta from uniprot database which was records in the expasy database.")>
    Public Function DownloadExpasyEnzymes(Expasy As LANS.SystemsBiology.Assembly.Expasy.Database.NomenclatureDB, TempDir As String) As LANS.SystemsBiology.SequenceModel.FASTA.FastaFile
        Dim IdList = GetExpasyEnzymeIdList(Expasy)
        Return DownloadUniprot(IdList, TempDir)
    End Function

    <ExportAPI("get.expasy_idlist", Info:="Export all of the enzyme protein swiss-prot database id which was records in the expasy database.")>
    Public Function GetExpasyEnzymeIdList(Expasy As LANS.SystemsBiology.Assembly.Expasy.Database.NomenclatureDB) As String()
        Dim EnzymeUniprotIdList = (From Enzyme In Expasy.Enzymes Select Enzyme.SwissProt).ToArray.MatrixToList
        EnzymeUniprotIdList = (From UniprotID As String In EnzymeUniprotIdList Where Not String.IsNullOrEmpty(UniprotID) Select UniprotID Distinct).ToList
        Return EnzymeUniprotIdList.ToArray
    End Function

    <ExportAPI("pdbentry.get_from_ligand")>
    Public Function GetPDBEntry(CompoundCode As String) As String()
        Return LANS.SystemsBiology.Assembly.RCSB.PDB.WebServices.PDBEntry.DownloadEntryList(CompoundCode)
    End Function

    Public Class Item
        <Column("Protein")> Public Property UniprotId As String
        Public Property AccessionId As String

        Public Property prot_desc As String
        Public Property Ratio As String
        Public Property Pvalue As String
        <Column("Subcellular Location")> Public Property SubcellularLocation As String
        Public Property biological_process As String
        Public Property molecular_function As String
        Public Property cellular_component As String
        <Column("KO No.")> Public Property KO As String
        <Column("Gene desc")> Public Property Gene As String
        <Column("Pathway desc")> Public Property Pathway As String
        <Column("Interpro ID")> Public Property InterproID As String
        <Column("Domain desc")> Public Property Domain As String

    End Class

    Public Class Item2
        Public Property Gene As String
        Public Property Accession As String
        Public Property Description As String
        Public Property Score As String
        Public Property Coverage As String
        <Column("# Proteins")> Public Property Proteins As String
        <Column("# Unique Peptides")> Public Property Uniprot As String
        <Column("# Peptides")> Public Property peptides As String
        <Column("# PSMs")> Public Property psms As String
        <Column("Molecular Function")> Public Property MolecularFunction As String
        <Column("Cellular Component")> Public Property CellularComponent As String
        <Column("Biological Process")> Public Property BiologicalProcess As String
        <Column("Pfam IDs")> Public Property PfamIDs As String
        <Column("130/131")> Public Property d1 As String
        <Column("130/131 Count")> Public Property d2 As String
        <Column("130/131 Variability [%]")> Public Property d3 As String
        <Column("# AAs")> Public Property AAs As String
        <Column("MW [kDa]")> Public Property MW As String
        <Column("calc. pI")> Public Property PI As String
    End Class

    <ExportAPI("x2")>
    Public Function to2(list As Generic.IEnumerable(Of Item2), gbk As String) As Item2()
        Dim gb As GBFF.File = GBFF.File.Read(gbk)
        Dim FeatureSearch As New SearchInvoker(gb)
        Dim setValue = New SetValue(Of Item2) <= NameOf(Item2.Gene)
        Dim LQuery = (From item As Item2
                      In list
                      Let Feature = FeatureSearch.Search(SearchInvoker.SearchBy_GI, item.Accession)
                      Let gene As String = If(Feature Is Nothing, item.Accession, Feature.Query(FeatureQualifiers.locus_tag))
                      Select setValue(item, gene)).ToArray
        Return LQuery
    End Function

    <Runtime.DeviceDriver.DriverHandles.IO_DeviceHandle(GetType(Generic.IEnumerable(Of Item2)))>
    Public Function WriteItem2(data As Generic.IEnumerable(Of Item2), saveto As String) As Boolean
        Return data.SaveTo(saveto, False)
    End Function

    <ExportAPI("read.csv.tr2")>
    Public Function ReadItem2(path As String) As Item2()
        Return path.LoadCsv(Of Item2)(False).ToArray
    End Function

    <ExportAPI("uniprot_2_ncbi")>
    Public Function UniprotToNCBIId(list As Generic.IEnumerable(Of Item)) As Item()
        Dim LQuery = (From item In list Select assign(item)).ToArray
        Return LQuery
    End Function

    Private Function assign(item As Item) As Item
        Dim ncbi As String = Regex.Match(item.prot_desc, "GN=[^ ]+", RegexOptions.IgnoreCase).Value.Split(CChar("=")).Last
        item.AccessionId = ncbi
        Return item
    End Function

    <ExportAPI("read.csv.uniprots")>
    Public Function ReadData(path As String) As Item()
        Return path.LoadCsv(Of Item)(False).ToArray
    End Function

    <ExportAPI("write.csv.uniprots")>
    Public Function WriteData(data As Generic.IEnumerable(Of Item), saveto As String) As Boolean
        Return data.SaveTo(saveto, False)
    End Function
End Module
