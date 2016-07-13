Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace gast

    ''' <summary>
    ''' gast -in input_fasta -ref reference_uniques_fasta -rtax reference_dupes_taxonomy [-mp min_pct_id] [-m majority] -out output_file
    ''' </summary>
    Public Structure ARGV

        ''' <summary>
        ''' input_fasta, input fasta file
        ''' </summary>
        ''' <returns></returns>
        Public Property [in] As String
        ''' <summary>
        ''' reference_uniques_fasta, reference fasta file containing unique sequences of known taxonomy
        ''' The definition line should include the ID used In the reference taxonomy file.
        ''' Any other information On the definition line should be separated by a space Or a ``|`` symbol.
        ''' </summary>
        ''' <returns></returns>
        Public Property ref As String
        ''' <summary>
        ''' reference_dupes_taxonomy, reference taxa file with taxonomy for all copies of the sequences in 
        ''' the reference fasta file 
        ''' This Is a tab-delimited file, three columns, describing the taxonomy of the reference sequences
        ''' The ID matching the reference fasta, the taxonomy And the number Of reference sequences With this 
        ''' this same taxonomy.  
        ''' </summary>
        ''' <returns></returns>
        Public Property rtax As String
        ''' <summary>
        ''' [min_pct_id] 
        ''' </summary>
        ''' <returns></returns>
        Public Property mp As String
        ''' <summary>
        ''' [majority] 
        ''' </summary>
        ''' <returns></returns>
        Public Property m As String
        ''' <summary>
        ''' output_file, output filename
        ''' </summary>
        ''' <returns></returns>
        Public Property out As String
        ''' <summary>
        ''' -full, input data will be compared against full length 16S reference sequences [default: not full length]
        ''' </summary>
        ''' <returns></returns>
        Public Property full As Boolean
        ''' <summary>
        ''' -maj, percent majority required for taxonomic consensus [default: 66]
        ''' </summary>
        ''' <returns></returns>
        Public Property maj As Double
        ''' <summary>
        ''' -maxr, [Optional] usearch --max_rejects parameter [default: 200]
        ''' </summary>
        ''' <returns></returns>
        Public Property maxr As Double
        ''' <summary>
        ''' -maxa, [Optional] usearch --max_accepts parameter [default: 15]
        ''' </summary>
        ''' <returns></returns>
        Public Property maxa As Double
        ''' <summary>
        ''' -minp, [Optional] minimum percent identity match to a reference.
        ''' If the best match Is less Then min_pct_id, it Is Not considered a match
        ''' Default = 0.80
        ''' </summary>
        ''' <returns></returns>
        Public Property minp As Double
        ''' <summary>
        ''' -wdb, use a USearch formatted wdb indexed version of the reference for speed. 
        ''' [NO LONGER AVAILABLE with usearch6.0+]
        ''' </summary>
        ''' <returns></returns>
        Public Property wdb As String
        ''' <summary>
        ''' -udb, use a USearch formatted udb indexed version of the reference for speed. 
        ''' (see http://drive5.com/usearch/manual/udb_files.html)
        ''' </summary>
        ''' <returns></returns>
        Public Property udb As String
        ''' <summary>
        ''' -terse  minimal output, includes only ID, taxonomy, and distance
        ''' See GAST manual For description Of other fields
        ''' </summary>
        ''' <returns></returns>
        Public Property terse As String

#Region "Optional database output, MySQL imports options, if null, then result data will not imports to mysql"

        ''' <summary>
        ''' -host, mysql server host name
        ''' </summary>
        ''' <returns></returns>
        Public Property db_host As String
        ''' <summary>
        ''' -db, database name
        ''' </summary>
        ''' <returns></returns>
        Public Property db_name As String
        ''' <summary>
        ''' -table, database table to receive data
        ''' </summary>
        ''' <returns></returns>
        Public Property table As String
#End Region

        Sub New(args As CommandLine)
            [in] = args - "-in"
            ref = args - "-ref"
            rtax = args - "-rtax"
            mp = args - "-mp"
            m = args - "-m"
            out = args - "-out"
            db_host = args - "-host"
            db_name = args - "-db"
            table = args - "-table"
            full = args.GetBoolean("-full")
            maj = args.GetValue("-maj", 66.0R)
            maxr = args.GetValue("-maxr", 200.0R)
            maxa = args.GetValue("-maxa", 15.0R)
            minp = args.GetValue("-minp", 0.8)
            wdb = args("-wdb")
            udb = args("-udb")
            terse = args - "-terse"
        End Sub

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Structure
End Namespace