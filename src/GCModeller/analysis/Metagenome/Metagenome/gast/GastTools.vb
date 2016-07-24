#Region "Microsoft.VisualBasic::673b0760afb82ce78852ada79af71477, ..\Metagenome\gast\GastTools.vb"

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

Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports SMRUCC.genomics.Assembly
Imports SMRUCC.genomics.Assembly.NCBI
Imports SMRUCC.genomics.SequenceModel.FASTA

Namespace gast

    Public Module gast_tools

        <Extension>
        Public Function ExportSILVA([in] As String, EXPORT As String) As Boolean
            Dim reader As New StreamIterator([in])
            Dim out As String = EXPORT & "/" & [in].BaseName & ".fasta"
            Dim tax As String = out.TrimSuffix & ".tax"

            Call "".SaveTo(out)
            Call "".SaveTo(tax)

            Using ref As New StreamWriter(New FileStream(out, FileMode.OpenOrCreate)),
                taxon As New StreamWriter(New FileStream(tax, FileMode.OpenOrCreate))

                ref.NewLine = vbLf
                taxon.NewLine = vbLf

                For Each fa As FastaToken In reader.ReadStream
                    Dim title As String = fa.Title
                    Dim uid As String = title.Split.First
                    Dim taxnomy As String = Mid(title, uid.Length + 1).Trim

                    uid = uid.Replace(".", "_")
                    fa = New FastaToken({uid}, fa.SequenceData)
                    title = {uid, taxnomy, "1"}.JoinBy(vbTab)

                    Call ref.WriteLine(fa.GenerateDocument(60))
                    Call taxon.WriteLine(title)
                Next
            End Using

            Return True
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="[in]">file path of *.names</param>
        ''' <returns></returns>
        Public Iterator Function NamesClusterOut([in] As String) As IEnumerable(Of NamedValue(Of String()))
            Dim lines As String() = [in].ReadAllLines

            For Each line As String In lines
                Dim tokens As String() = Strings.Split(line, vbTab)

                Yield New NamedValue(Of String()) With {
                    .Name = tokens(Scan0),
                    .x = tokens(1).Split(","c)
                }
            Next
        End Function

        Public Function ExportNt(nt As String, gi2taxid As String, taxi_dmp As String, out As String) As Boolean
            Dim giTaxid As BucketDictionary(Of Integer, Integer) =
                NCBI.Taxonomy.AcquireAuto(gi2taxid)
            Dim tree As New NcbiTaxonomyTree(taxi_dmp)

            Using ref As StreamWriter = out.OpenWriter(Encodings.ASCII,),
                tax As StreamWriter = (out.TrimSuffix & ".tax").OpenWriter(Encodings.ASCII)

                For Each seq As FastaToken In New StreamIterator(nt).ReadStream
                    Dim title As String = seq.Title
                    Dim gi As Integer = __gi(title)
                    Dim taxid As Integer = giTaxid(gi)
                    Dim nodes As TaxonNode() = tree.GetAscendantsWithRanksAndNames(taxid)
                    Dim taxonomy As String = TaxonNode.Taxonomy(nodes)

                    seq = New FastaToken({title}, seq.SequenceData)
                    title = {title, taxonomy, "1"}.JoinBy(vbTab)

                    Call ref.WriteLine(seq.GenerateDocument(120))
                    Call tax.WriteLine(title)
                Next
            End Using

            Return True
        End Function

        Private Function __gi(ByRef title As String) As Integer
            title = title.Split.First
            title = title.NormalizePathString()

            Dim gi As String = Regex.Match(title, "gi(\||_)\d+", RegexICSng).Value
            Return CInt(Val(gi))
        End Function
    End Module
End Namespace
