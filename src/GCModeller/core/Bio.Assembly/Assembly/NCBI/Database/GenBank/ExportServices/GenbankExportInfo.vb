Namespace Assembly.NCBI.GenBank.CsvExports

    Public Class GenbankExportInfo(Of T As GenBank.CsvExports.gbEntryBrief)

#Region "Csv data storage region."

        Dim _entryInfo As Microsoft.VisualBasic.ComponentModel.LazyLoader(Of T(), String)
        Dim _ORFInfo As Microsoft.VisualBasic.ComponentModel.LazyLoader(Of GeneDumpInfo(), String)

        Public Property EntryInfo As T()
            Get
                Return _entryInfo.Value
            End Get
            Set(value As T())
                _entryInfo.Value = value
            End Set
        End Property

        Public Property ORFInfo As GeneDumpInfo()
            Get
                Return _ORFInfo.Value
            End Get
            Set(value As GeneDumpInfo())
                _ORFInfo.Value = value
            End Set
        End Property
#End Region

#Region "Total fasta sequence info storage region."

        Public ReadOnly Property ORF As LANS.SystemsBiology.SequenceModel.FASTA.FastaFile
            Get
                Return LANS.SystemsBiology.SequenceModel.FASTA.FastaFile.Read(_root & "/orf.fasta")
            End Get
        End Property

        Public ReadOnly Property Gene As LANS.SystemsBiology.SequenceModel.FASTA.FastaFile
            Get
                Return LANS.SystemsBiology.SequenceModel.FASTA.FastaFile.Read(_root & "/genes.fasta")
            End Get
        End Property

        Public ReadOnly Property Genome As LANS.SystemsBiology.SequenceModel.FASTA.FastaFile
            Get
                Return LANS.SystemsBiology.SequenceModel.FASTA.FastaFile.Read(_root & "/genomes.fasta")
            End Get
        End Property
#End Region

        Public Delegate Function GenbankEntryInfoLoadMethod(Path As String) As T()
        Public Delegate Function ORFInfoLoadMethod(Path As String) As GeneDumpInfo()

        ''' <summary>
        ''' Repository root for this export source.
        ''' </summary>
        ''' <remarks></remarks>
        Dim _root As String

        ''' <summary>
        ''' 根目录
        ''' </summary>
        ''' <param name="Root"></param>
        ''' <remarks></remarks>
        Sub New(Root As String, GenbankEntryInfoLoad As GenbankEntryInfoLoadMethod, ORFInfoLoad As ORFInfoLoadMethod)
            _root = Root
            _entryInfo = New Microsoft.VisualBasic.ComponentModel.LazyLoader(Of T(), String)(Root & "/genbank.info.csv", Function(path) GenbankEntryInfoLoad(path))
            _ORFInfo = New Microsoft.VisualBasic.ComponentModel.LazyLoader(Of GeneDumpInfo(), String)(Root & "/genbank.orf.csv", Function(path) ORFInfoLoad(path))
        End Sub

        Public ReadOnly Property ORFSource As Dictionary(Of String, String)
            Get
                Return (_root & "/ORF/").LoadSourceEntryList
            End Get
        End Property

        Public ReadOnly Property GeneSource As Dictionary(Of String, String)
            Get
                Return (_root & "/Gene/").LoadSourceEntryList
            End Get
        End Property

        Public ReadOnly Property GenomeSource As Dictionary(Of String, String)
            Get
                Return (_root & "/Genomes/").LoadSourceEntryList
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return _root
        End Function
    End Class
End Namespace