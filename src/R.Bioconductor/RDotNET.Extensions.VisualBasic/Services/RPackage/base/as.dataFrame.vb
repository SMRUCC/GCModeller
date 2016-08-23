#Region "Microsoft.VisualBasic::51e253ad3a49264ca59606b218f00b1d, ..\R.Bioconductor\RDotNET.Extensions.VisualBasic\Services\RPackage\base\as.dataFrame.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports RDotNET.Extensions.VisualBasic.Services.ScriptBuilder.RTypes

Namespace base.as.data

    ''' <summary>
    ''' Functions to check if an object is a data frame, or coerce it if possible.
    ''' </summary>
    Public Module DataFrameAPI

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="x">any R object.</param>
        ''' <param name="rowNames">NULL or a character vector giving the row names for the data frame. Missing values are not allowed.</param>
        ''' <param name="[optional]">logical. If TRUE, setting row names and converting column names (to syntactic names: see make.names) is optional.</param>
        ''' <returns></returns>
        Public Function frame(x As String, Optional rowNames As String = NULL, Optional [optional] As Boolean = False) As RExpression
            Return $"as.data.frame({x}, row.names={rowNames}, optional={New RBoolean([optional])})"
        End Function
    End Module
End Namespace
