#Region "Microsoft.VisualBasic::d253051f43c7a50b6f70f446548298cd, GCModeller\models\SBML\SBML\Level3\Extensions.vb"

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

'   Total Lines: 8
'    Code Lines: 6
' Comment Lines: 0
'   Blank Lines: 2
'     File Size: 195 B


'     Module Extensions
' 
' 
' 
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices

Namespace Level3

    <HideModuleName>
    Public Module Extensions

        Public Const sbmlXmlns As String = "http://www.sbml.org/sbml/level3/version1/core"

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function LoadSBML(filepath As String) As XmlFile(Of Reaction)
            Return XmlFile(Of Reaction).LoadDocument(filepath)
        End Function
    End Module
End Namespace
