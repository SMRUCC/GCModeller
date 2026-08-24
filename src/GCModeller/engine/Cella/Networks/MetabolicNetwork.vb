#Region "Microsoft.VisualBasic::83fa63e900a22546e2c7581cfeacf954, engine\Cella\Networks\MetabolicNetwork.vb"

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
    '    Code Lines: 21 (67.74%)
    ' Comment Lines: 3 (9.68%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 7 (22.58%)
    '     File Size: 924 B


    ' Class MetabolicNetwork
    ' 
    '     Constructor: (+1 Overloads) Sub New
    ' 
    '     Function: GetStats
    ' 
    '     Sub: RunStep
    ' 
    ' /********************************************************************************/

#End Region


Imports Microsoft.VisualBasic.Math.Calculus.Dynamics
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Dynamics.Core

''' <summary>
''' 采用ODEs系统表示的代谢网络模型
''' </summary>
Public Class MetabolicNetwork : Inherits SubNetwork

    Protected ReadOnly core As SolverIterator
    Protected ReadOnly massEnv As MassTable
    Protected ReadOnly cellular As Vessel

    Sub New(mass As MassTable, network As IEnumerable(Of Channel), cell As VirtualCella)
        Call MyBase.New(cell)

        cellular = New Vessel() _
            .load(mass.AsEnumerable) _
            .load(network) _
            .Initialize(boost:=1)
        massEnv = mass
    End Sub

    Public Overrides Sub RunStep()
        Call core.Tick()
    End Sub

    Public Overrides Function GetStats() As Dictionary(Of String, Double)
        Throw New NotImplementedException()
    End Function
End Class

