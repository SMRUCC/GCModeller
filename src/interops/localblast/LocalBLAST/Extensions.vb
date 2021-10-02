#Region "Microsoft.VisualBasic::9e418e9c7ab3ef11b1b3bc7d1fe01ea6, localblast\LocalBLAST\Extensions.vb"

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

    ' Module Extensions
    ' 
    '     Function: IsNullOrEmpty
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH

<HideModuleName>
Module Extensions

    ''' <summary>
    ''' Is this collection of the besthit data is empty or nothing?
    ''' </summary>
    ''' <param name="data"></param>
    ''' <returns></returns>
    <Extension>
    Public Function IsNullOrEmpty(data As IEnumerable(Of BestHit)) As Boolean
        If data Is Nothing Then
            Return True
        Else
            Dim notNull As BestHit = (From bh As BestHit
                                      In data
                                      Where Not bh.isMatched
                                      Select bh).FirstOrDefault
            Return notNull Is Nothing
        End If
    End Function
End Module
