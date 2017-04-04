#Region "Microsoft.VisualBasic::8a52fccf54a520602f3727d6faa0d01f, ..\core\Bio.Assembly\Assembly\NCBI\SeqDump\Protein.vb"

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
Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.SequenceModel

Namespace Assembly.NCBI.SequenceDump

    ''' <summary>
    ''' NCBI genbank title format fasta parser
    ''' </summary>
    Public Class Protein : Inherits FASTA.FastaToken

        Public Property GI As String
        Public Property Description As String

        Private Shared Function __createObject(Fasta As FASTA.FastaToken) As Protein
            Dim prot As New Protein With {
                .SequenceData = Fasta.SequenceData,
                .Attributes = Fasta.Attributes
            }
            prot.GI = Fasta.Attributes(1)
            prot.Description = Fasta.Attributes(4).Trim

            Return prot
        End Function

        Public Shared Function LoadDocument(path As String) As Protein()
            Dim raw = FASTA.FastaFile.Read(path)
            Dim LQuery = LinqAPI.Exec(Of Protein) <=
 _
                From Fasta As FASTA.FastaToken
                In raw.AsParallel
                Select Protein.__createObject(Fasta)

            Return LQuery
        End Function
    End Class
End Namespace
