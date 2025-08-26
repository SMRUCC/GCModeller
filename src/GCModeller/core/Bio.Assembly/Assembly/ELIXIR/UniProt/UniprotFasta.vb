#Region "Microsoft.VisualBasic::f8d9c614804556f20be0527de990ec6f, core\Bio.Assembly\Assembly\ELIXIR\UniProt\UniprotFasta.vb"

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

    '   Total Lines: 169
    '    Code Lines: 79 (46.75%)
    ' Comment Lines: 72 (42.60%)
    '    - Xml Docs: 95.83%
    ' 
    '   Blank Lines: 18 (10.65%)
    '     File Size: 8.30 KB


    '     Class FastaHeader
    ' 
    '         Properties: EntryName, GN, OrgnsmSpName, PE, ProtName
    '                     SV, UniprotID
    ' 
    '         Function: ToString
    ' 
    '     Class UniprotFasta
    ' 
    '         Properties: UniProtHeader, UniprotID
    ' 
    '         Function: CreateObject, LoadFasta, ParseHeader, SimpleHeaderParser
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Linq.Extensions
Imports SMRUCC.genomics.SequenceModel

Namespace Assembly.Uniprot

    Public Class FastaHeader : Implements INamedValue

        ''' <summary>
        ''' UniqueIdentifier Is the primary accession number of the UniProtKB entry.
        ''' </summary>
        ''' <returns></returns>
        Public Property UniprotID As String Implements INamedValue.Key
        ''' <summary>
        ''' EntryName Is the entry name of the UniProtKB entry.
        ''' </summary>
        ''' <returns></returns>
        Public Property EntryName As String
        ''' <summary>
        ''' OrganismName Is the scientific name of the organism of the UniProtKB entry.
        ''' </summary>
        ''' <returns></returns>
        Public Property OrgnsmSpName As String
        ''' <summary>
        ''' GeneName Is the first gene name of the UniProtKB entry. If there Is no gene name, OrderedLocusName Or ORFname, the GN field Is Not listed. GeneName(基因名称)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property GN As String
        ''' <summary>
        ''' ProteinExistence Is the numerical value describing the evidence for the existence of the protein.
        ''' </summary>
        ''' <returns></returns>
        Public Property PE As String
        ''' <summary>
        ''' SequenceVersion Is the version number of the sequence.
        ''' </summary>
        ''' <returns></returns>
        Public Property SV As String
        ''' <summary>
        ''' ProteinName Is the recommended name of the UniProtKB entry as annotated in the RecName field. For UniProtKB/TrEMBL entries without a RecName field, the SubName field Is used. 
        ''' In case of multiple SubNames, the first one Is used. The 'precursor' attribute is excluded, 'Fragment' is included with the name if applicable.
        ''' </summary>
        ''' <returns></returns>
        Public Property ProtName As String

        Public Overrides Function ToString() As String
            Return String.Format("sp|{0}|{1} {2} OS={3} GN={4} PE={5} SV={6}", EntryName, UniprotID, ProtName, OrgnsmSpName, GN, PE, SV)
        End Function
    End Class

    ''' <summary>
    ''' A fasta object which is specific for the uniprot fasta title parsing.(专门用于解析Uniprot蛋白质序列记录的Fasta对象)
    ''' 
    ''' The following is a description of FASTA headers for UniProtKB (including alternative isoforms), UniRef, UniParc and archived UniProtKB versions. 
    ''' NCBI's program formatdb (in particular its -o option) is compatible with the UniProtKB fasta headers.
    ''' 
    ''' UniProtKB
    ''' >db|UniqueIdentifier|EntryName ProteinName OS=OrganismName[ GN=GeneName]PE=ProteinExistence SV=SequenceVersion
    ''' 
    ''' Where:
    ''' db Is 'sp' for UniProtKB/Swiss-Prot and 'tr' for UniProtKB/TrEMBL.
    ''' UniqueIdentifier Is the primary accession number of the UniProtKB entry.
    ''' EntryName Is the entry name of the UniProtKB entry.
    ''' ProteinName Is the recommended name of the UniProtKB entry as annotated in the RecName field. For UniProtKB/TrEMBL entries without a RecName field, the SubName field Is used. 
    ''' In case of multiple SubNames, the first one Is used. The 'precursor' attribute is excluded, 'Fragment' is included with the name if applicable.
    ''' OrganismName Is the scientific name of the organism of the UniProtKB entry.
    ''' GeneName Is the first gene name of the UniProtKB entry. If there Is no gene name, OrderedLocusName Or ORFname, the GN field Is Not listed.
    ''' ProteinExistence Is the numerical value describing the evidence for the existence of the protein.
    ''' SequenceVersion Is the version number of the sequence.
    ''' </summary>
    ''' <remarks>http://www.uniprot.org/help/fasta-headers</remarks>
    Public Class UniprotFasta : Inherits FASTA.FastaSeq
        Implements IReadOnlyId

        Public ReadOnly Property UniprotID As String Implements IReadOnlyId.Identity
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return UniProtHeader.UniprotID
            End Get
        End Property

        Public Property UniProtHeader As FastaHeader

        ''' <summary>
        ''' 从读取的文件数据之中创建一个Uniprot序列对象
        ''' </summary>
        ''' <param name="fasta"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' >sp|Q197F8|002R_IIV3 Uncharacterized protein 002R OS=Invertebrate iridescent virus 3 GN=IIV3-002R PE=4 SV=1
        ''' </remarks>
        Public Shared Function CreateObject(fasta As FASTA.FastaSeq) As UniprotFasta
            Dim uniprotFasta As UniprotFasta = fasta.Copy(Of UniprotFasta)()
            Dim s As String = uniprotFasta.Headers(2)

            Try
                uniprotFasta.UniProtHeader = ParseHeader(s, uniprotFasta.Headers(1))
            Catch ex As Exception
                Dim msg As String = "Header parsing error at  ------> ""{0}"""
                msg = String.Format(msg & vbCrLf & vbCrLf & ex.ToString, fasta.Title)
                Throw New SyntaxErrorException(msg)
            End Try

            Return uniprotFasta
        End Function

        ''' <summary>
        ''' Parse the fasta header of the sequence from UniProt database.
        ''' </summary>
        ''' <param name="s$"></param>
        ''' <param name="id$"></param>
        ''' <returns></returns>
        Public Shared Function ParseHeader(s$, id$) As FastaHeader
            Dim uniprot As New FastaHeader

            uniprot.EntryName = s.Split.First
            uniprot.UniprotID = id
            uniprot.OrgnsmSpName = Regex.Match(s, "OS=[^=]+GN\s*=").Value
            uniprot.OrgnsmSpName = Regex.Replace(uniprot.OrgnsmSpName, "\s*GN\s*=", "")
            uniprot.GN = Regex.Replace(Regex.Match(s, "GN=[^=]+PE\s*=").Value, "\s*PE\s*=", "").Trim
            uniprot.PE = Regex.Match(s, "PE=\d+").Value
            uniprot.SV = Regex.Match(s, "SV=\d+").Value

            If Not String.IsNullOrEmpty(uniprot.OrgnsmSpName) Then s = s.Replace(uniprot.OrgnsmSpName, "")
            If Not String.IsNullOrEmpty(uniprot.PE) Then s = s.Replace(uniprot.PE, "")
            If Not String.IsNullOrEmpty(uniprot.GN) Then s = s.Replace(uniprot.GN, "")
            If Not String.IsNullOrEmpty(uniprot.SV) Then s = s.Replace(uniprot.SV, "")
            If Not String.IsNullOrEmpty(uniprot.EntryName) Then s = s.Replace(uniprot.EntryName, "").Trim

            uniprot.ProtName = s
            uniprot.OrgnsmSpName = Mid(uniprot.OrgnsmSpName, 4).Trim
            uniprot.GN = Mid(uniprot.GN, 4)
            uniprot.PE = Mid(uniprot.PE, 4)
            uniprot.SV = Mid(uniprot.SV, 4)

            Return uniprot
        End Function

        Public Shared Function SimpleHeaderParser(header$) As Dictionary(Of String, String)
            Dim headers$() = header.Split("|"c)

            Return New Dictionary(Of String, String) From {
                {"DB", headers(Scan0)},
                {"UniprotID", headers(1)},
                {"Description", headers(2)}
            }
        End Function

        ''' <summary>
        ''' Load the uniprot fasta sequence file. 
        ''' </summary>
        ''' <param name="path"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function LoadFasta(path As String) As UniprotFasta()
            Call $"Start loading the fasta sequence from file ""{path.ToFileURL}""...".debug
            Dim sw As Stopwatch = Stopwatch.StartNew
            Dim LQuery = (From fa As FASTA.FastaSeq
                          In FASTA.FastaFile.Read(path).AsParallel
                          Select UniprotFasta.CreateObject(fa)).ToArray
            Call $"Uniprot fasta data load done!   {sw.ElapsedMilliseconds}ms.".debug
            Return LQuery
        End Function
    End Class
End Namespace
