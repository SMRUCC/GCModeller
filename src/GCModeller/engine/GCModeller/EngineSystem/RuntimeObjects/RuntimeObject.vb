#Region "Microsoft.VisualBasic::0828b1e24f34c3809d1c4e05f7753f27, engine\GCModeller\EngineSystem\RuntimeObjects\RuntimeObject.vb"

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

    '     Class RuntimeObject
    ' 
    '         Properties: Guid
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '     Interface IRuntimeObject
    ' 
    '         Properties: Guid
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace EngineSystem.RuntimeObjects

    ''' <summary>
    ''' I'm just kidding
    ''' </summary>
    ''' <remarks></remarks>
    Public MustInherit Class RuntimeObject
        Implements IRuntimeObject

        Public ReadOnly Property Guid As Long Implements IRuntimeObject.Guid

        Sub New()
            _Guid = _GuidGenerator
            _GuidGenerator += 1
        End Sub

        Private Shared _GuidGenerator As Long = Long.MinValue
    End Class

    Public Interface IRuntimeObject
        ReadOnly Property Guid As Long
    End Interface
End Namespace
