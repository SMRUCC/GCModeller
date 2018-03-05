#Region "Microsoft.VisualBasic::048ba96c8636614c761a20ea41e0b0ff, meme_suite\MEME.DocParser\MAST\Fasta.vb"

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

    '     Class FastaToken
    ' 
    '         Properties: DOOR, GeneList, Location, Strand, UniqueId
    ' 
    '         Function: __createObject, Load
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text.RegularExpressions
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.Keywords.FEATURES
Imports SMRUCC.genomics.ComponentModel
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.ComponentModel.Loci.Abstract
Imports SMRUCC.genomics.ComponentModel.Loci.NucleotideLocation
Imports SMRUCC.genomics.SequenceModel

Namespace DocumentFormat.MAST.HTML

    Public Class FastaToken : Inherits FASTA.FastaSeq
        Implements ILocationNucleotideSegment

        Public ReadOnly Property Strand As Strands Implements ILocationNucleotideSegment.Strand
        Public ReadOnly Property Location As SMRUCC.genomics.ComponentModel.Loci.Location Implements ILocationSegment.Location
        Public ReadOnly Property UniqueId As String Implements ILocationSegment.UniqueId
        Public ReadOnly Property DOOR As String
        Public ReadOnly Property GeneList As String

        Public Overloads Shared Function Load(FastaFile As FASTA.FastaFile) As FastaToken()
            Dim LQuery = (From Fsa In FastaFile.AsParallel Select __createObject(Fsa)).ToArray
            Return LQuery
        End Function

        Private Shared Function __createObject(fa As FASTA.FastaSeq) As FastaToken
            Dim FsaObject As FastaToken = New FastaToken
            Dim Title As String = fa.Title
            FsaObject._UniqueId = Title.Split.First
            FsaObject._GeneList = Regex.Match(Title, "OperonGenes=[^]]+").Value.Split(CChar("=")).Last
            FsaObject._Location = New SMRUCC.genomics.ComponentModel.Loci.Location

            Dim strLocation As String = Regex.Match(Title, "\d+ ==> \d+ #(Reverse|Forward)").Value
            Dim Tokens As String() = strLocation.Split
            FsaObject._Location = New SMRUCC.genomics.ComponentModel.Loci.Location With {
                .Left = Tokens(0),
                .Right = Tokens(2)
            }
            FsaObject._Strand = If(String.Equals(Tokens.Last, "#Forward"), Strands.Forward, Strands.Reverse)
            FsaObject.SequenceData = fa.SequenceData
            FsaObject._DOOR = Regex.Match(Title, "AssociatedOperon=[^]]+").Value.Split(CChar("=")).Last
            If String.IsNullOrEmpty(FsaObject._DOOR) Then
                FsaObject._DOOR = Title.Split.First
            End If

            Return FsaObject
        End Function
    End Class
End Namespace
