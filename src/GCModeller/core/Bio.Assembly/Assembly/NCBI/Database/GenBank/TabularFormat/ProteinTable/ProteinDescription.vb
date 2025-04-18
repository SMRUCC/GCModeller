﻿#Region "Microsoft.VisualBasic::4e99d5ff2b3f7d3c9cdd9cea55d92fec, core\Bio.Assembly\Assembly\NCBI\Database\GenBank\TabularFormat\ProteinTable\ProteinDescription.vb"

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

    '   Total Lines: 71
    '    Code Lines: 38 (53.52%)
    ' Comment Lines: 27 (38.03%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 6 (8.45%)
    '     File Size: 2.39 KB


    '     Class ProteinDescription
    ' 
    '         Properties: [Stop], COG, GeneID, Length, Locus
    '                     Locus_tag, Product, ProteinName, RepliconAccession, RepliconName
    '                     Start, Strand
    ' 
    '         Function: GetLoci, ToPTTGene, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels
Imports SMRUCC.genomics.ComponentModel.Loci

Namespace Assembly.NCBI.GenBank.TabularFormat

    ''' <summary>
    ''' tabular data model for protein data information
    ''' </summary>
    Public Class ProteinDescription : Implements INamedValue

        ''' <summary>
        ''' #Replicon Name
        ''' </summary>
        ''' <returns></returns>
        Public Property RepliconName As String
        ''' <summary>
        ''' Replicon Accession
        ''' </summary>
        ''' <returns></returns>
        Public Property RepliconAccession As String
        Public Property Start As Integer
        Public Property [Stop] As Integer
        Public Property Strand As String
        Public Property GeneID As String
        Public Property Locus As String
        ''' <summary>
        ''' Locus tag.(基因号)
        ''' </summary>
        ''' <returns></returns>
        Public Property Locus_tag As String Implements INamedValue.Key
        ''' <summary>
        ''' Protein product
        ''' </summary>
        ''' <returns></returns>
        Public Property Product As String
        Public Property Length As Integer
        ''' <summary>
        ''' COG(s)
        ''' </summary>
        ''' <returns></returns>
        Public Property COG As String
        ''' <summary>
        ''' Protein name
        ''' </summary>
        ''' <returns></returns>
        Public Property ProteinName As String

        Public Function ToPTTGene() As GeneBrief
            Return New GeneBrief With {
                .Code = Me.Product,
                .COG = Me.COG,
                .Gene = Me.GeneID,
                .Location = GetLoci(),
                .PID = Me.GeneID,
                .Product = Me.ProteinName,
                .Synonym = Me.Locus_tag
            }
        End Function

        Public Function GetLoci() As NucleotideLocation
            Dim strand = GetStrand(Me.Strand)
            Dim loci As New NucleotideLocation(Me.Start, Me.Stop, strand)
            Return loci
        End Function

        Public Overrides Function ToString() As String
            Return Locus_tag & vbTab & GetLoci.ToString
        End Function
    End Class
End Namespace
