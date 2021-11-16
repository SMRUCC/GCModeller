#Region "Microsoft.VisualBasic::26490ae94a83eeb1e261f8271a72a8d4, RNA-Seq\RNA-seq.Data\Soap\SOAP.vb"

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

    '     Class SOAPFile
    ' 
    ' 
    ' 
    '     Class Read
    ' 
    '         Properties: Quality, Reads, RefSeq, SequenceData, Start
    '                     Strand
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace SOAP

    Public Class SOAPFile

    End Class

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>
    ''' Example line:
    ''' 
    ''' ```
    ''' SIMU_0001_00000370/2	ACGTTAACGTTGAGCCAGGCTGGCATGCACGGAAC	hhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhh	1	b	35	-	refseq	73466	2	C->31G40	G->20T40
    ''' ```
    ''' </remarks>
    Public Class Read
        Implements IPolymerSequenceModel

        Public Property Reads As String
        Public Property SequenceData As String Implements IPolymerSequenceModel.SequenceData
        Public Property Quality As String
        '1
        'b
        '35
        Public Property Strand As String
        Public Property RefSeq As String
        Public Property Start As Integer
        '0
        'C->31G40	
        'G->20T40

    End Class
End Namespace
