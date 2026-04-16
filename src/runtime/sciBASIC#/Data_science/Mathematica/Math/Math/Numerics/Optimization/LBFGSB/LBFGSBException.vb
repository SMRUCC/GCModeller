#Region "Microsoft.VisualBasic::f6e10c5b137a58891f0658a26f8e2595, Data_science\Mathematica\Math\Math\Numerics\Optimization\LBFGSB\LBFGSBException.vb"

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

    '   Total Lines: 18
    '    Code Lines: 13 (72.22%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 5 (27.78%)
    '     File Size: 372 B


    '     Class LBFGSBException
    ' 
    '         Constructor: (+2 Overloads) Sub New
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System

Namespace Framework.Optimization.LBFGSB
    Public Class LBFGSBException
        Inherits Exception

        Private Const serialVersionUID As Long = 1L

        Public Sub New(message As String)
            MyBase.New(message)
        End Sub
        Public Sub New()
            MyBase.New()
        End Sub

    End Class

End Namespace
