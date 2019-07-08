#Region "Microsoft.VisualBasic::26ecd41cd620330d42f20e3d72b97a78, WebCloud\SMRUCC.HTTPInternal\Platform\Submodule.vb"

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

    '     Class Submodule
    ' 
    '         Properties: PlatformEngine
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices

Namespace Platform

    ''' <summary>
    ''' Web App engine platform components.
    ''' </summary>
    Public MustInherit Class Submodule

        ''' <summary>
        ''' Platform engine parent host
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property PlatformEngine As PlatformEngine

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Sub New(main As PlatformEngine)
            PlatformEngine = main
        End Sub
    End Class
End Namespace
