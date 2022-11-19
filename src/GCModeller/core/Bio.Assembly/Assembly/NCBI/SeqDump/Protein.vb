#Region "Microsoft.VisualBasic::c2342226717f270f2052c0f8c8bde6d0, GCModeller\core\Bio.Assembly\Assembly\NCBI\SeqDump\Protein.vb"

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

    '   Total Lines: 36
    '    Code Lines: 26
    ' Comment Lines: 3
    '   Blank Lines: 7
    '     File Size: 1.09 KB


    '     Class Protein
    ' 
    '         Properties: Description, GI
    ' 
    '         Function: __createObject, LoadDocument
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.SequenceModel

Namespace Assembly.NCBI.SequenceDump

    ''' <summary>
    ''' NCBI genbank title format fasta parser
    ''' </summary>
    Public Class Protein : Inherits FASTA.FastaSeq

        Public Property GI As String
        Public Property Description As String

        Private Shared Function __createObject(Fasta As FASTA.FastaSeq) As Protein
            Dim prot As New Protein With {
                .SequenceData = Fasta.SequenceData,
                .Headers = Fasta.Headers
            }
            prot.GI = Fasta.Headers(1)
            prot.Description = Fasta.Headers(4).Trim

            Return prot
        End Function

        Public Shared Function LoadDocument(path As String) As Protein()
            Dim raw = FASTA.FastaFile.Read(path)
            Dim LQuery = LinqAPI.Exec(Of Protein) <=
 _
                From Fasta As FASTA.FastaSeq
                In raw.AsParallel
                Select Protein.__createObject(Fasta)

            Return LQuery
        End Function
    End Class
End Namespace
