#Region "Microsoft.VisualBasic::c0fd385d4a3ed0691d40a7e924b2098d, GCModeller\analysis\Metagenome\Metagenome\OTUTable\BIOMExtensions.vb"

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
    '    Code Lines: 20
    ' Comment Lines: 0
    '   Blank Lines: 5
    '     File Size: 797 B


    ' Module BIOMExtensions
    ' 
    '     Function: Union
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.csv.IO
Imports SMRUCC.genomics.foundation.BIOM.v10

Public Module BIOMExtensions

    <Extension>
    Public Function Union(tables As IEnumerable(Of BIOMDataSet(Of Double))) As IEnumerable(Of DataSet)
        Dim matrix As New Dictionary(Of String, DataSet)

        For Each table As BIOMDataSet(Of Double) In tables
            For Each otu In table.PopulateRows
                If Not matrix.ContainsKey(otu.Name) Then
                    matrix(otu.Name) = New DataSet With {
                        .ID = otu.Name
                    }
                End If

                Call matrix(otu.Name).Append(otu, AddressOf Math.Max)
            Next
        Next

        Return matrix.Values
    End Function
End Module
