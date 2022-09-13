#Region "Microsoft.VisualBasic::69d717729fcc377116b5409eb9f54d8e, GCModeller\core\Bio.Assembly\Assembly\bac-srna.org\Sequence.vb"

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

    '   Total Lines: 86
    '    Code Lines: 65
    ' Comment Lines: 8
    '   Blank Lines: 13
    '     File Size: 3.00 KB


    '     Class Sequence
    ' 
    '         Properties: Name, SequenceData, Specie, UniqueId
    ' 
    '         Constructor: (+3 Overloads) Sub New
    '         Function: [CType], __getMappingLoci, ToFasta, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.SequenceModel.NucleotideModels
Imports Microsoft.VisualBasic.Serialization
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Assembly.Bac_sRNA.org

    Public Class Sequence : Inherits Contig
        Implements IPolymerSequenceModel

        Public ReadOnly Property UniqueId As String
        Public ReadOnly Property Specie As String
        Public ReadOnly Property Name As String

        Public Property SequenceData As String Implements IPolymerSequenceModel.SequenceData
            Get
                Return __raw.SequenceData
            End Get
            Set(value As String)
                __raw.SequenceData = value
            End Set
        End Property

        Dim __raw As FastaSeq

        Sub New()
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="sid"></param>
        ''' <param name="org"></param>
        ''' <param name="name"></param>
        ''' <param name="seq">The fasta sequence data</param>
        ''' <param name="loci"></param>
        Sub New(sid As String, org As String, name As String, seq As String, loci As NucleotideLocation)
            _UniqueId = sid
            _Specie = org
            _Name = name
            _MappingLocation = loci

            Dim attrs$() = {
                UniqueId, Specie, name,
                CStr(loci.Left), CStr(loci.Right), loci.Strand.ToString
            }
            __raw = New FastaSeq(attrs, seq)
        End Sub

        Sub New(atts As String())
            Call Me.New(atts(Scan0),
                        atts(1),
                        atts(2), "",
                        New NucleotideLocation(Scripting.CTypeDynamic(Of Integer)(atts(3)),
                                               Scripting.CTypeDynamic(Of Integer)(atts(4)),
                                               atts(5).GetStrand))
        End Sub

        Public Shared Function [CType](fa As FastaSeq) As Sequence
            Return New Sequence With {
                ._UniqueId = fa.Headers(Scan0),
                ._Specie = fa.Headers(1),
                ._Name = fa.Headers(2),
                .__raw = fa
            }
        End Function

        Public Overrides Function ToString() As String
            Return __raw.GetJson
        End Function

        Public Function ToFasta() As FastaSeq
            Return New FastaSeq(__raw.Headers, __raw.SequenceData)
        End Function

        Protected Overrides Function __getMappingLoci() As NucleotideLocation
            Return New NucleotideLocation With {
                .Left = __raw.Headers(3),
                .Right = __raw.Headers(4),
                .Strand = LocusExtensions.GetStrand(__raw.Headers(5))
            }
        End Function
    End Class
End Namespace
