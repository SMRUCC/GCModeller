#Region "Microsoft.VisualBasic::63d760bcbe2dd080bf24e32419dcf36c, analysis\RNA-Seq\Toolkits.RNA-Seq\Matrix\ExprMAT.vb"

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

    '     Interface IExprMAT
    ' 
    '         Properties: dataExpr0, LocusId
    ' 
    '     Class ExprMAT
    ' 
    '         Properties: dataExpr0, LocusId
    ' 
    '         Function: LoadMatrix, ToSample, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection

Namespace dataExprMAT

    Public Interface IExprMAT
        Property LocusId As String
        Property dataExpr0 As Dictionary(Of String, Double)
    End Interface

    Public Class ExprMAT : Implements IExprMAT

        Public Property LocusId As String Implements IExprMAT.LocusId

        <Meta(GetType(Double))>
        Public Property dataExpr0 As Dictionary(Of String, Double) Implements IExprMAT.dataExpr0

        Public Overrides Function ToString() As String
            Return LocusId
        End Function

        Public Function ToSample() As ExprSamples
            Return New ExprSamples With {
                .locusId = LocusId,
                .data = dataExpr0.Values.ToArray
            }
        End Function

        ''' <summary>
        ''' General load method.
        ''' </summary>
        ''' <param name="path"></param>
        ''' <returns></returns>
        Public Function LoadMatrix(path As String) As ExprMAT()
            Dim File As IO.File = IO.File.Load(path)
            File(Scan0, Scan0) = NameOf(LocusId)
            Dim MAT As ExprMAT() = File.AsDataSource(Of ExprMAT)(False).ToArray
            Return MAT
        End Function
    End Class
End Namespace
