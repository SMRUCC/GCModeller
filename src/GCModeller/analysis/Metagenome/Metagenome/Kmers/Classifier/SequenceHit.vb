#Region "Microsoft.VisualBasic::859c0f8410bfae4855d4ebda039e6da8, analysis\Metagenome\Metagenome\Kmers\Classifier\SequenceHit.vb"

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

    '   Total Lines: 31
    '    Code Lines: 24 (77.42%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 7 (22.58%)
    '     File Size: 842 B


    '     Class SequenceHit
    ' 
    '         Properties: identities, ratio, reads_title, score, total
    ' 
    '         Constructor: (+2 Overloads) Sub New
    '         Function: Unknown
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices

Namespace Kmers

    Public Class SequenceHit : Inherits SequenceSource

        Public Property reads_title As String
        Public Property identities As Double
        Public Property total As Double
        Public Property score As Double
        Public Property ratio As Double

        Sub New()
        End Sub

        Sub New(info As SequenceSource)
            id = info.id
            ncbi_taxid = info.ncbi_taxid
            name = info.name
            accession_id = info.accession_id
        End Sub

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function Unknown() As SequenceHit
            Return New SequenceHit With {
                .name = "Unknown"
            }
        End Function

    End Class
End Namespace
