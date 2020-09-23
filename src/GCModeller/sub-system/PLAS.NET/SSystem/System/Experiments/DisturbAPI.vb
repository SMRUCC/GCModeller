#Region "Microsoft.VisualBasic::b996036d032c463c406a9bd8c15abe97, sub-system\PLAS.NET\SSystem\System\Experiments\DisturbAPI.vb"

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

    '     Enum Types
    ' 
    '         ChangeTo, Decrease, Increase
    ' 
    '  
    ' 
    ' 
    ' 
    '     Module DisturbAPI
    ' 
    '         Properties: Methods
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace Kernel.ObjectModels

    Public Enum Types
        Increase
        Decrease
        ChangeTo
    End Enum

    Public Module DisturbAPI

        Public ReadOnly Property Methods As IReadOnlyDictionary(Of Types, Func(Of Double, Double, Double))

        Sub New()
            Methods = New Dictionary(Of Types, Func(Of Double, Double, Double)) From {
 _
                {Types.Increase, Function(x, d) x + d},
                {Types.Decrease, Function(x, d) x - d},
                {Types.ChangeTo, Function(x, d) x}
            }
        End Sub
    End Module
End Namespace
