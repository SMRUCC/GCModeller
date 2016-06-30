Imports System.Text
Imports LANS.SystemsBiology.Assembly.KEGG.WebServices
Imports LANS.SystemsBiology.SequenceModel
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData

Namespace Regprecise

    ''' <summary>
    ''' Downloads the protein fasta sequence from the KEGG which was records in the regprecise regulation datasets.
    ''' </summary>
    <PackageNamespace("Regprecise.KEGGDownloader",
                      Publisher:="xie.guigang@gcmodeller.org",
                      Category:=APICategories.UtilityTools,
                      Description:="Sequence downloader for the regulators and the regulated genes in the regprecise database from KEGG server.")>
    Public Module KEGGDownloader

        ''' <summary>
        ''' 从KEGG数据库之中下载调控因子的蛋白质序列数据
        ''' </summary>
        ''' <param name="Regulator"></param>
        ''' <param name="Bacteria"></param>
        ''' <param name="ErrLog"></param>
        ''' <param name="DownloadDirectory"></param>
        ''' <param name="FastaSaved"></param>
        ''' <returns></returns>
        '''
        <ExportAPI("Regulator.Downloads")>
        Public Function RegulatorDownloads(Regulator As Regulator,
                                           Bacteria As BacteriaGenome,
                                           ErrLog As Logging.LogFile,
                                           DownloadDirectory As String,
                                           FastaSaved As String) As FASTA.FastaToken

            If FileIO.FileSystem.FileExists(FastaSaved) AndAlso FileIO.FileSystem.GetFileInfo(FastaSaved).Length > 0 Then
                Return FASTA.FastaToken.Load(FastaSaved)
            End If

            Dim FastaObject = RegulatorDownloads(Regulator.LocusTag.Key, ErrLog, Bacteria.BacteriaGenome.name)

            If FastaObject Is Nothing Then
                Return Nothing
            Else
                Call FastaObject.SaveTo(FastaSaved)
                Return FastaObject
            End If
        End Function

        <ExportAPI("Regulator.Downloads", Info:="Download a regulators' protein fasta sequence using the gene's locus tag")>
        Public Function RegulatorDownloads(locusTag As String,
                                           ErrLog As Logging.LogFile,
                                           <Parameter("Err.Trace.Bacteria")> Optional bacteria As String = "") As LANS.SystemsBiology.SequenceModel.FASTA.FastaToken
            Dim EntryList = WebRequest.HandleQuery(locusTag)
            If EntryList.IsNullOrEmpty Then
                Dim Msg = String.Format("[KEGG_ENTRY_NOT_FOUND] [Query_LocusTAG={0}] [Bacteria={1}]", locusTag, bacteria)
                Call ErrLog.WriteLine(Msg, NameOf(RegulatorDownloads), Logging.MSG_TYPES.DEBUG)
                Return Nothing
            End If
            EntryList = (From item In EntryList Where String.Equals(locusTag, item.LocusId, StringComparison.OrdinalIgnoreCase) Select item).ToArray
            If EntryList.IsNullOrEmpty Then
                Dim Msg = String.Format("[KEGG_ENTRY_NOT_FOUND] [Query_LocusTAG={0}] [Bacteria={1}]", locusTag, bacteria)
                Call ErrLog.WriteLine(Msg, NameOf(RegulatorDownloads), Logging.MSG_TYPES.DEBUG)
                Return Nothing
            End If

            Dim Entry = EntryList.First
            Dim FastaObject = WebRequest.FetchSeq(Entry)

            If FastaObject Is Nothing Then
                Dim Msg = String.Format("[KEGG_DATA_NOT_FOUND] [Query_LocusTAG={0}] [Bacteria={1}] KEGG not sure the object is a protein.", locusTag, bacteria)
                Call ErrLog.WriteLine(Msg, NameOf(RegulatorDownloads), Logging.MSG_TYPES.DEBUG)
                Return Nothing
            End If

            Return FastaObject
        End Function

        <ExportAPI("Regulator.Downloads")>
        Public Function RegulatorDownloads(Regulator As Regprecise.WebServices.JSONLDM.regulator, ErrLog As Logging.LogFile) As Regprecise.FastaReaders.Regulator
            Dim Fasta As FASTA.FastaToken = RegulatorDownloads(Regulator.locusTag, ErrLog, Regulator.ToString)
            If Fasta Is Nothing Then
                Return Nothing
            End If
            Dim KEGG As String = Fasta.Attributes(Scan0).Split.First
            Dim RegulatorFasta As New Regprecise.FastaReaders.Regulator With {
                .Family = Regulator.regulatorFamily,
                .KEGG = KEGG,
                .Regulog = Regulator.regulonId,
                .Definition = Fasta.Title.Replace(KEGG, "").Trim,
                .SequenceData = Fasta.SequenceData
            }
            Return RegulatorFasta
        End Function
    End Module
End Namespace