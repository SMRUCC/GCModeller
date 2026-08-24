#Region "Microsoft.VisualBasic::def7fe03c4c65259cac9b448c145fdfe, annotations\WGCNA\WGCNA\ResultTables\ModuleMembershipResult.vb"

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

    '   Total Lines: 55
    '    Code Lines: 24 (43.64%)
    ' Comment Lines: 23 (41.82%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 8 (14.55%)
    '     File Size: 1.64 KB


    ' Class ModuleMembershipResult
    ' 
    '     Properties: Correlation, GeneId, ModuleName, PValue, SampleCount
    ' 
    '     Function: ReadModuleAssignment, ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Data.Framework.StorageProvider

''' <summary>
''' 模块成员结果
''' </summary>
Public Class ModuleMembershipResult
    ''' <summary>
    ''' 基因ID
    ''' </summary>
    Public Property GeneId As String

    ''' <summary>
    ''' 模块名称
    ''' </summary>
    Public Property ModuleName As String

    ''' <summary>
    ''' 相关系数（模块成员MM）
    ''' </summary>
    Public Property Correlation As Double

    ''' <summary>
    ''' p值
    ''' </summary>
    Public Property PValue As Double

    ''' <summary>
    ''' 样本数量
    ''' </summary>
    Public Property SampleCount As Integer

    Public Overrides Function ToString() As String
        Return $"Gene '{GeneId}' in '{ModuleName}': MM={Correlation:F3}, p={PValue:F4}"
    End Function

    ''' <summary>
    ''' [geneID,moduleColor,kME]
    ''' </summary>
    ''' <param name="file"></param>
    ''' <returns></returns>
    Public Shared Iterator Function ReadModuleAssignment(file As String) As IEnumerable(Of ModuleMembershipResult)
        Dim df As DataFrameResolver = DataFrameResolver.Load(file)
        Dim geneID As Integer = df.GetOrdinal("geneID")
        Dim moduleColor As Integer = df.GetOrdinal("moduleColor")
        Dim kME As Integer = df.GetOrdinal("kME")

        Do While df.Read
            Yield New ModuleMembershipResult With {
                .GeneId = df.GetString(geneID),
                .Correlation = df.GetDouble(kME),
                .ModuleName = df.GetString(moduleColor)
            }
        Loop
    End Function
End Class
