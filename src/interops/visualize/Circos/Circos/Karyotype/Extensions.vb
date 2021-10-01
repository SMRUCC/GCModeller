#Region "Microsoft.VisualBasic::a11772edb2f91eca472c2cdfb9a698cc, visualize\Circos\Circos\Karyotype\Extensions.vb"

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

    '     Module KaryotypeExtensions
    ' 
    '         Function: nt
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports SMRUCC.genomics.SequenceModel.FASTA

Namespace Karyotype

    <HideModuleName>
    Public Module KaryotypeExtensions

        ''' <summary>
        ''' nt核苷酸基因组序列拓展属性
        ''' </summary>
        ''' <param name="x"></param>
        ''' <returns></returns>
        <Extension>
        Public Function nt(x As Karyotype) As PropertyValue(Of FastaSeq)
            Return PropertyValue(Of Object).Read(Of Karyotype, FastaSeq)(x, NameOf(nt))
        End Function
    End Module
End Namespace
