﻿#Region "Microsoft.VisualBasic::ea7233fcd1e26807d0c00747e75e6760, GCModeller.Framework.Kernel_Driver\Kinetics\KineticsLDM.vb"

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

    '     Class KineticsLDM
    ' 
    '         Operators: (+3 Overloads) -, (+3 Overloads) *, (+3 Overloads) /, (+3 Overloads) \, (+3 Overloads) ^
    '                    (+3 Overloads) +, (+6 Overloads) Mod
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace Kinetics

    Public MustInherit Class KineticsLDM

        ''' <summary>
        ''' The formulation evaluation.
        ''' </summary>
        ''' <returns></returns>
        Public MustOverride Function GetValue() As Double

#Region "LDM opr LDM"

        Public Shared Operator *(x As KineticsLDM, y As KineticsLDM) As Double
            Return x.GetValue * y.GetValue
        End Operator

        Public Shared Operator +(x As KineticsLDM, y As KineticsLDM) As Double
            Return x.GetValue + y.GetValue
        End Operator

        Public Shared Operator -(x As KineticsLDM, y As KineticsLDM) As Double
            Return x.GetValue - y.GetValue
        End Operator

        Public Shared Operator /(x As KineticsLDM, y As KineticsLDM) As Double
            Return x.GetValue / y.GetValue
        End Operator

        Public Shared Operator Mod(x As KineticsLDM, y As KineticsLDM) As Double
            Return x.GetValue Mod y.GetValue
        End Operator

        Public Shared Operator \(x As KineticsLDM, y As KineticsLDM) As Double
            Return CDbl(x.GetValue \ y.GetValue)
        End Operator

        Public Shared Operator ^(x As KineticsLDM, y As KineticsLDM) As Double
            Return x.GetValue ^ y.GetValue
        End Operator
#End Region

#Region "LDM opr <n>"

        Public Shared Operator *(x As KineticsLDM, y As Double) As Double
            Return x.GetValue * y
        End Operator

        Public Shared Operator +(x As KineticsLDM, y As Double) As Double
            Return x.GetValue + y
        End Operator

        Public Shared Operator -(x As KineticsLDM, y As Double) As Double
            Return x.GetValue - y
        End Operator

        Public Shared Operator /(x As KineticsLDM, y As Double) As Double
            Return x.GetValue / y
        End Operator

        Public Shared Operator Mod(x As KineticsLDM, y As Double) As Double
            Return x.GetValue Mod y
        End Operator

        Public Shared Operator \(x As KineticsLDM, y As Double) As Double
            Return CDbl(x.GetValue \ y)
        End Operator

        Public Shared Operator ^(x As KineticsLDM, y As Double) As Double
            Return x.GetValue ^ y
        End Operator

#End Region

#Region "<n> opr LDM"

        Public Shared Operator *(x As Double, y As KineticsLDM) As Double
            Return x * y.GetValue
        End Operator

        Public Shared Operator +(x As Double, y As KineticsLDM) As Double
            Return x + y.GetValue
        End Operator

        Public Shared Operator -(x As Double, y As KineticsLDM) As Double
            Return x - y.GetValue
        End Operator

        Public Shared Operator /(x As Double, y As KineticsLDM) As Double
            Return x / y.GetValue
        End Operator

        Public Shared Operator Mod(x As Double, y As KineticsLDM) As Double
            Return x Mod y.GetValue
        End Operator

        Public Shared Operator \(x As Double, y As KineticsLDM) As Double
            Return CDbl(x \ y.GetValue)
        End Operator

        Public Shared Operator ^(x As Double, y As KineticsLDM) As Double
            Return x ^ y.GetValue
        End Operator
#End Region

    End Class
End Namespace
