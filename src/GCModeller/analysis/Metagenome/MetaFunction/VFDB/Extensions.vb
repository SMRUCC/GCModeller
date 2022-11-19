#Region "Microsoft.VisualBasic::2d40b747644856410bb6face76e2b82a, GCModeller\analysis\Metagenome\MetaFunction\VFDB\Extensions.vb"

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

    '   Total Lines: 22
    '    Code Lines: 19
    ' Comment Lines: 0
    '   Blank Lines: 3
    '     File Size: 825 B


    '     Module Extensions
    ' 
    '         Function: BuildVFDIndex
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection

Namespace VFDB

    Public Module Extensions

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function BuildVFDIndex(data As IEnumerable(Of FastaHeader)) As Dictionary(Of String, Index(Of String))
            Return data _
                .GroupBy(Function(a) a.organism) _
                .ToDictionary(Function(org) org.Key,
                              Function(genes)
                                  Return genes _
                                     .Select(Function(g) g.geneName) _
                                     .Distinct _
                                     .Indexing
                              End Function)
        End Function
    End Module
End Namespace
