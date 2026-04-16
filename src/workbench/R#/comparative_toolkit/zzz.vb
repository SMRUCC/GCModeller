#Region "Microsoft.VisualBasic::dd543a26186f32460f30b1f1371709b1, R#\comparative_toolkit\zzz.vb"

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

    '   Total Lines: 14
    '    Code Lines: 9 (64.29%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 5 (35.71%)
    '     File Size: 232 B


    ' Class zzz
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Sub: onLoad
    ' 
    ' /********************************************************************************/

#End Region

Imports SMRUCC.Rsharp.Runtime.Interop

<Assembly: RPackageModule>

Public NotInheritable Class zzz

    Shared Sub New()
    End Sub

    Public Shared Sub onLoad()
        Call pangenome.Main()
    End Sub

End Class

