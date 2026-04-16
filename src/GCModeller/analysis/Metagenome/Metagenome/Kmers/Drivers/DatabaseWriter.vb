#Region "Microsoft.VisualBasic::b528181ce314ad2e6407b1dc30abeb23, analysis\Metagenome\Metagenome\Kmers\Drivers\DatabaseWriter.vb"

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

    '   Total Lines: 25
    '    Code Lines: 10 (40.00%)
    ' Comment Lines: 10 (40.00%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 5 (20.00%)
    '     File Size: 791 B


    '     Interface DatabaseWriter
    ' 
    '         Function: AddSequenceID
    ' 
    '         Sub: (+2 Overloads) Add, SetKSize
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.SequenceModel.NucleotideModels

Namespace Kmers

    Public Interface DatabaseWriter : Inherits IDisposable

        Sub SetKSize(k As Integer)
        ''' <summary>
        ''' add sequence into database
        ''' </summary>
        ''' <param name="seq"></param>
        ''' <param name="taxid"></param>
        Sub Add(seq As IFastaProvider, taxid As UInteger)
        ''' <summary>
        ''' add sequence into database
        ''' </summary>
        ''' <param name="seq"></param>
        ''' <param name="taxid"></param>
        Sub Add(seq As ChunkedNtFasta, taxid As UInteger)

        Function AddSequenceID(taxid As UInteger, name As String) As UInteger

    End Interface
End Namespace
