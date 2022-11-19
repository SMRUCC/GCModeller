#Region "Microsoft.VisualBasic::f4afc0cf9d3610fd9e53b47e36435343, GCModeller\analysis\Metagenome\Metagenome\gast\ARGV.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
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



    ' /********************************************************************************/

    ' Summaries:


    ' Code Statistics:

    '   Total Lines: 142
    '    Code Lines: 47
    ' Comment Lines: 88
    '   Blank Lines: 7
    '     File Size: 5.44 KB


    '     Structure ARGV
    ' 
    '         Properties: [in], db_host, db_name, full, m
    '                     maj, maxa, maxr, minp, mp
    '                     out, ref, rtax, table, terse
    '                     udb, wdb
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

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
        Public Property terse As Boolean

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

        ''' <summary>
        ''' ```bash
        ''' gast -in input_fasta -ref reference_uniques_fasta -rtax reference_dupes_taxonomy [-mp min_pct_id] [-m majority] -out output_file
        ''' ```
        ''' </summary>
        ''' <param name="args"></param>
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
            terse = args.GetBoolean("-terse")
        End Sub

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Structure
End Namespace
