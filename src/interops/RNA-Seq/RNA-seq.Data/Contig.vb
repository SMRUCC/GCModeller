#Region "Microsoft.VisualBasic::317b0f514eb66de9e4f558bb46ac91de, ..\interops\RNA-Seq\RNA-seq.Data\Contig.vb"

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

    Public Property Attributes As String() Implements IAbstractFastaToken.Attributes
    Public Property SequenceData As String Implements IPolymerSequenceModel.SequenceData

    Public Function ToFastaToken() As FastaToken
        Return New FastaToken With {
            .SequenceData = SequenceData,
            .Attributes = {Location.ToString, String.Join(" / ", FLAGS)}
        }
    End Function

    Protected Overrides Function __getMappingLoci() As NucleotideLocation
        Return _MappingLocation
    End Function
End Class
