#Region "Microsoft.VisualBasic::c1136847cf865f9670a5370d05dd7ad0, RDotNET\RDotNET\Utilities\REngineExtensionsAdvanced.vb"

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

    '     Module REngineExtensionsAdvanced
    ' 
    '         Function: CheckUnbound, EqualsRNilValue
    ' 
    ' 
    ' /********************************************************************************/

#End Region


Namespace Utilities
    ''' <summary>
    ''' Advanced, less usual extension methods for the R.NET REngine
    ''' </summary>
    Public Module REngineExtensionsAdvanced

        ''' <summary>
        ''' Checks the equality in native memory of a pointer against a pointer to the R 'NULL' value
        ''' </summary>
        ''' <param name="engine">R.NET Rengine</param>
        ''' <param name="pointer">Pointer to test</param>
        ''' <returns>True if the pointer and pointer to R NULL are equal</returns>
        <System.Runtime.CompilerServices.Extension>
        Public Function EqualsRNilValue(engine As REngine, pointer As IntPtr) As Boolean
            Return engine.NilValue.DangerousGetHandle() = pointer
        End Function

        ''' <summary>
        ''' Checks the equality in native memory of a pointer against a pointer to the R 'R_UnboundValue',
        ''' i.e. whether a symbol exists (i.e. functional equivalent to "exists('varName')" in R)
        ''' </summary>
        ''' <param name="engine">R.NET Rengine</param>
        ''' <param name="pointer">Pointer to test</param>
        ''' <returns>True if the pointer is not bound to a value</returns>
        <System.Runtime.CompilerServices.Extension>
        Public Function CheckUnbound(engine As REngine, pointer As IntPtr) As Boolean
            Return engine.UnboundValue.DangerousGetHandle() = pointer
        End Function
    End Module
End Namespace

