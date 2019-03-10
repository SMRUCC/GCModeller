#Region "Microsoft.VisualBasic::0431f76fd59429038369498d47dbbacf, RNA-Seq\RNA-seq.Data\Contig.vb"

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

    ' Class Contig
    ' 
    '     Properties: FLAGS, Headers, Location, SequenceData, Title
    ' 
    '     Function: __getMappingLoci, ToFastaToken
    ' 
    ' /********************************************************************************/

#End Region

Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.SequenceModel.FASTA

Public Class Contig : Inherits NucleotideModels.Contig
    Implements IAbstractFastaToken

    Public Property FLAGS As String()
    Public Property Location As NucleotideLocation
        Get
            Return Me._MappingLocation
        End Get
        Set(value As NucleotideLocation)
            Me._MappingLocation = value
        End Set
    End Property

    Public ReadOnly Property Title As String Implements IAbstractFastaToken.Title
        Get
            Return Me.ToString
        End Get
    End Property

    Public Property Headers As String() Implements IAbstractFastaToken.Headers
    Public Property SequenceData As String Implements IPolymerSequenceModel.SequenceData

    Public Function ToFastaToken() As FastaSeq
        Return New FastaSeq With {
            .SequenceData = SequenceData,
            .Headers = {Location.ToString, String.Join(" / ", FLAGS)}
        }
    End Function

    Protected Overrides Function __getMappingLoci() As NucleotideLocation
        Return _MappingLocation
    End Function
End Class
