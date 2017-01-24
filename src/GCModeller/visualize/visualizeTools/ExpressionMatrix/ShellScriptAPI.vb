#Region "Microsoft.VisualBasic::d10cda7899db25c7784d5d112a6ec421, ..\GCModeller\visualize\visualizeTools\ExpressionMatrix\ShellScriptAPI.vb"

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

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports SMRUCC.genomics.InteractionModel

Namespace ExpressionMatrix

    <[Namespace]("expression.matrix")>
    Module ShellScriptAPI

        <ExportAPI("mat.invoke_drawing")>
        Public Function DrawingImage(data As SerialsData()) As Image
            Return MatrixDrawing.InvokeDrawing(data, Nothing)
        End Function

        <ExportAPI("mat.load")>
        Public Function LoadData(data As IO.File) As SerialsData()
            Return DataServicesExtension.LoadCsv(data)
        End Function

        <ExportAPI("mat.invoke_drawing")>
        Public Function DrawingMatrix(MAT As IO.File) As Image
            Return MatrixDrawing.NormalMatrix(MAT)
        End Function

        <ExportAPI("mat.Triangular_drawing")>
        Public Function DrawingMatrixTr(MAT As IO.File) As Image
            Return MatrixDrawing.NormalMatrixTriangular(MAT)
        End Function
    End Module
End Namespace
