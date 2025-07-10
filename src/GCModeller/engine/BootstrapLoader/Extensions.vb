#Region "Microsoft.VisualBasic::fc36bc79f81fd766a4567399d433d9d0, engine\BootstrapLoader\Extensions.vb"

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

    '   Total Lines: 24
    '    Code Lines: 16 (66.67%)
    ' Comment Lines: 6 (25.00%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 2 (8.33%)
    '     File Size: 907 B


    ' Module Extensions
    ' 
    '     Function: variables
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.GCModeller.ModellingEngine.BootstrapLoader.Engine
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Dynamics.Core
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model.Cellular.Molecule

<HideModuleName>
Public Module Extensions

    ''' <summary>
    ''' Populate all the <paramref name="complex"/> related components.
    ''' </summary>
    ''' <param name="massTable"></param>
    ''' <param name="complex"></param>
    ''' <returns></returns>
    <Extension>
    Public Iterator Function variables(massTable As MassTable, complex As Protein, cellular_id As String) As IEnumerable(Of Variable)
        For Each compound In complex.compounds
            Yield massTable.variable(compound, cellular_id)
        Next
        For Each peptide In complex.polypeptides
            Yield massTable.variable(peptide, cellular_id)
        Next
    End Function
End Module
