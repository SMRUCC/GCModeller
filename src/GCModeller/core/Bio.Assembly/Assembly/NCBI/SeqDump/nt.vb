#Region "Microsoft.VisualBasic::49553f6d39059492c4e4d3cbb8889273, ..\GCModeller\core\Bio.Assembly\Assembly\NCBI\SeqDump\Gene.vb"

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

Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.FASTA

Namespace Assembly.NCBI.SequenceDump

    ''' <summary>
    ''' NCBI genbank title format fasta parser
    ''' </summary>
    Public Class Nucleotide : Inherits FASTA.FastaToken

#Region "ReadOnly properties"

        Public ReadOnly Property CommonName As String
        Public ReadOnly Property LocusTag As String
        Public ReadOnly Property Location As NucleotideLocation
#End Region

        Sub New(FastaObj As FASTA.FastaToken)
            Dim strTitle As String = FastaObj.Title
            Dim LocusTag As String = Regex.Match(strTitle, "locus_tag=[^]]+").Value
            Dim Location As String = Regex.Match(strTitle, "location=[^]]+").Value
            Dim CommonName As String = Regex.Match(strTitle, "gene=[^]]+").Value

            Me._LocusTag = LocusTag.Split(CChar("=")).Last
            Me._CommonName = CommonName.Split(CChar("=")).Last
            Me._Location = LociAPI.TryParse(Location)
            Me.Attributes = FastaObj.Attributes
            Me.SequenceData = FastaObj.SequenceData
        End Sub

        Public Overloads Shared Function Load(path As String) As Nucleotide()
            Dim FASTA As FastaFile = FastaFile.Read(path)
            Dim LQuery As Nucleotide() = LinqAPI.Exec(Of Nucleotide) <=
 _
                From fa As FastaToken
                In FASTA
                Select New Nucleotide(fa)

            Return LQuery
        End Function
    End Class

    ''' <summary>
    ''' The fasta header of the nt database.
    ''' </summary>
    Public Structure NTheader
        Public gi As String
        Public db As String
        Public uid As String
        Public description As String

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function

        Public Shared Function ParseNTheader(fa As FastaToken) As IEnumerable(Of NTheader)
            Try
                Dim attrs$() = fa.Attributes
                Dim splits$()() = attrs.Skip(1).Split(4%)
                Dim trimGI As Boolean = splits.Length > 1
                Dim out As New List(Of NTheader)

                For Each b As String() In splits
                    If b.Length = 1 Then
                        Dim x = out.Last
                        out(out.Count - 1) = New NTheader With {
                            .db = x.db,
                            .description = x.description & "|" & b(Scan0),
                            .gi = x.gi,
                            .uid = x.uid
                        }

                        Continue For
                    End If

                    Dim descr$ = Strings.Trim(b(3))

                    out += New NTheader With {
                        .gi = b(0),
                        .db = b(1),
                        .uid = b(2),
                        .description = If(trimGI, Trim(descr), descr)
                    }
                Next

                Return out
            Catch ex As Exception
                ex = New Exception(fa.Title, ex)
                Throw ex
            End Try
        End Function

        Private Shared Function Trim(s As String) As String
            Dim gi$ = Mid(s, s.Length - 1)
            If String.Equals("gi", gi$, StringComparison.OrdinalIgnoreCase) Then
                s = Mid(s, 1, s.Length - 2)
            End If
            Return s
        End Function
    End Structure
End Namespace
