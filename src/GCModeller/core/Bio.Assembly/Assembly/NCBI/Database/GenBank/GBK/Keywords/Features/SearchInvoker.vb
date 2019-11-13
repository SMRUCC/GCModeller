#Region "Microsoft.VisualBasic::5af241b13cf572fde83e503ae1531fa7, core\Bio.Assembly\Assembly\NCBI\Database\GenBank\GBK\Keywords\Features\SearchInvoker.vb"

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

    '     Delegate Function
    ' 
    ' 
    '     Class SearchInvoker
    ' 
    '         Properties: SearchBy_GI
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: _SearchBy_GI, Search, ToString
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace Assembly.NCBI.GenBank.GBFF.Keywords.FEATURES

    Public Delegate Function SearchMethod(data As FEATURES, keyword As String) As Feature

    ''' <summary>
    ''' 可以使用本模块内的方法搜索<see cref="NCBI.GenBank.GBFF.Keywords.FEATURES"></see>模块之中的内容
    ''' </summary>
    ''' <remarks></remarks>
    Public Class SearchInvoker

        Dim GBFF As File

#Region "Delegate Methods for Delegate Invoker"

        Public Shared ReadOnly Property SearchBy_GI As SearchMethod
            Get
                Return AddressOf SearchInvoker._SearchBy_GI
            End Get
        End Property

        Protected Shared Function _SearchBy_GI(data As FEATURES, keyword As String) As Feature
            Dim LQuery = (From b As Feature
                          In data._innerList
                          Let id As String = b.Query(FeatureQualifiers.db_xref)
                          Let gi = id.Replace("GI:", "")
                          Where String.Equals(gi, keyword)
                          Select b).FirstOrDefault
            Return LQuery
        End Function
#End Region

        ''' <summary>
        ''' Delegate invoker
        ''' </summary>
        ''' <param name="method"></param>
        ''' <param name="keyword"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Search(method As SearchMethod, keyword As String) As Feature
            Return method(GBFF.Features, keyword)
        End Function

        Sub New(gbk As File)
            GBFF = gbk
        End Sub

        Public Overrides Function ToString() As String
            Return GBFF.ToString
        End Function
    End Class
End Namespace
