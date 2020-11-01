#Region "Microsoft.VisualBasic::a761b0a978bc67f79081e7a6d7898eee, RDotNET\Graphics\REngineExtension.vb"

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

    '     Module REngineExtension
    ' 
    '         Sub: Install
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace Graphics

    Public Module REngineExtension

        <System.Runtime.CompilerServices.Extension>
        Public Sub Install(engine As REngine, device As IGraphicsDevice)
            Dim adapter = New GraphicsDeviceAdapter(device)
            AddHandler engine.Disposing, Sub(sender, e) Call adapter.Dispose()
            adapter.SetEngine(engine)
        End Sub
    End Module
End Namespace
