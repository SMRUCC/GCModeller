#Region "Microsoft.VisualBasic::e30ff0be4c2eb9a2e85004a7093551b3, src\Flute\ServerComponent.vb"

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

    '   Total Lines: 11
    '    Code Lines: 7 (63.64%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 4 (36.36%)
    '     File Size: 229 B


    ' Class ServerComponent
    ' 
    '     Constructor: (+1 Overloads) Sub New
    ' 
    ' /********************************************************************************/

#End Region

Imports Flute.Http.Configurations

''' <summary>
''' the base component class which carries the server wide configuration
''' instance for all of the derived http server components.
''' </summary>
Public MustInherit Class ServerComponent

    ''' <summary>
    ''' the shared server configuration instance that is passed down from the
    ''' host application into every derived server component.
    ''' </summary>
    Protected ReadOnly settings As Configuration

    ''' <summary>
    ''' create a new server component with the given configuration
    ''' </summary>
    ''' <param name="settings">
    ''' the server wide configuration instance that this component depends on.
    ''' </param>
    Sub New(settings As Configuration)
        Me.settings = settings
    End Sub

End Class
