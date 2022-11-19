#Region "Microsoft.VisualBasic::90e87f3200f7b40d19010f94bfcc775e, GCModeller\core\Bio.Assembly\Assembly\NCBI\Database\GenBank\TabularFormat\FeatureBriefs\Fasta.vb"

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

    '   Total Lines: 90
    '    Code Lines: 63
    ' Comment Lines: 12
    '   Blank Lines: 15
    '     File Size: 3.12 KB


    '     Class Fasta
    ' 
    '         Properties: UniqueId
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: CreateObject
    ' 
    '     Class GenomeSequence
    ' 
    '         Properties: Description, GI, LocusID
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: SaveBriefData
    ' 
    '     Class GeneObject
    ' 
    '         Properties: Description, GI, Location, Locus
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '     Class Protein
    ' 
    '         Properties: Description, GI, Locus
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text.RegularExpressions
Imports SMRUCC.genomics.SequenceModel.FASTA

Namespace Assembly.NCBI.GenBank.TabularFormat.FastaObjects

    Public Class Fasta : Inherits FastaSeq

        Public ReadOnly Property UniqueId As String

        Protected Sub New()
        End Sub

        Public Shared Function CreateObject(UniqueId As String, Fasta As FastaSeq) As Fasta
            Return New Fasta With {
                ._UniqueId = UniqueId,
                .Headers = Fasta.Headers,
                .SequenceData = Fasta.SequenceData
            }
        End Function
    End Class

    ''' <summary>
    ''' *.fna 基因组序列
    ''' </summary>
    ''' <remarks></remarks>
    Public Class GenomeSequence : Inherits SequenceModel.FASTA.FastaSeq

        Public ReadOnly Property GI As String
        Public ReadOnly Property LocusID As String
        Public ReadOnly Property Description As String

        Sub New(Fasta As SequenceModel.FASTA.FastaSeq)
            _GI = Fasta.Headers(1)
            _LocusID = Regex.Replace(Fasta.Headers(3), "\.\d+", "")
            _Description = Fasta.Headers(4)
            Headers = Fasta.Headers
            Me.SequenceData = Fasta.SequenceData
        End Sub

        Public Function SaveBriefData(Path As String, Optional encoding As System.Text.Encoding = Nothing) As Boolean
            Dim Fasta As New SequenceModel.FASTA.FastaSeq With {
                .SequenceData = SequenceData,
                .Headers = New String() {LocusID}
            }
            Return Fasta.SaveTo(Path, encoding)
        End Function
    End Class

    ''' <summary>
    ''' *.ffn 基因序列
    ''' </summary>
    ''' <remarks></remarks>
    Public Class GeneObject : Inherits SequenceModel.FASTA.FastaSeq

        Public ReadOnly Property GI As String
        Public ReadOnly Property Locus As String
        Public ReadOnly Property Location As ComponentModel.Loci.Location
        Public ReadOnly Property Description As String

        Sub New(Fasta As SequenceModel.FASTA.FastaSeq)
            _GI = Fasta.Headers(1)
            _Locus = Fasta.Headers(3)
            _Description = Fasta.Headers.Last
            Headers = Fasta.Headers
            SequenceData = Fasta.SequenceData
            Dim sLoci As String = Mid(Regex.Match(Description, ":\d+-\d+").Value, 2)
            Dim Tokens As String() = Strings.Split(sLoci, ":")
            _Location = New ComponentModel.Loci.Location(Val(Tokens.First), Val(Tokens.Last))
        End Sub
    End Class

    ''' <summary>
    ''' *.faa 蛋白质序列
    ''' </summary>
    ''' <remarks></remarks>
    Public Class Protein : Inherits SequenceModel.FASTA.FastaSeq

        Public ReadOnly Property GI As String
        Public ReadOnly Property Locus As String
        Public ReadOnly Property Description As String

        Sub New(Fasta As SequenceModel.FASTA.FastaSeq)
            _GI = Fasta.Headers(1)
            _Locus = Fasta.Headers(3)
            _Description = Fasta.Headers.Last
            Headers = Fasta.Headers
            SequenceData = Fasta.SequenceData
        End Sub
    End Class
End Namespace
