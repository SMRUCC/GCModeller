#Region "Microsoft.VisualBasic::cdccaada2eb5f8183d56acd588b0ac7c, ..\GCModeller\models\Networks\Network.Regulons\MotifCluster.vb"

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
Imports Microsoft.VisualBasic.DataMining.KMeans
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Text.Levenshtein
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.MotifScans
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.Similarity.TOMQuery

''' <summary>
''' 需要先将motif映射为长度一致的实体！
''' 先进行TOM全局比对，然后取match部分，之后就可以进行聚类了？
''' </summary>
''' 
<PackageAttribute("Motif.TOMCluster", Category:=APICategories.ResearchTools)>
Public Module MotifCluster

    ''' <summary>
    ''' 首先和目标表型的LDM进行全局比对做Mapping映射为长度一致的实体对象
    ''' </summary>
    ''' <param name="source">无法进行比对的结果将会被忽略掉</param>
    ''' <param name="LDM"></param>
    ''' <param name="param">TOM全局比对的参数信息</param>
    ''' <returns></returns>
    ''' 
    <ExportAPI("Lev.Mappings")>
    Public Function Mappings(source As IEnumerable(Of AnnotationModel), LDM As AnnotationModel, param As Parameters) As Entity()
        Dim method As ColumnCompare = TomTOm.GetMethod(param.Method)
        Dim LQuery = (From x As AnnotationModel In source
                      Let edits = TomTOm.Compare(x, LDM, method, param)
                      Where Not edits Is Nothing
                      Select x,
                          edits,
                          el = edits.DistEdits.Length).ToArray
        Dim buffer As New List(Of Entity)

        For Each map In LQuery  ' 获取得到映射
            Dim mapResult As ResidueSite() = Nothing
            Call map.edits.GetMatches(map.x.PWM, LDM.PWM, mapResult, Nothing) ' 长度不一样？？？

            Dim mapObj As New Entity With {
                .uid = map.x.Uid,
                .Properties = mapResult.ToArray(Function(x) x.Bits + x.PWM.Average, where:=Function(x) Not x Is Nothing)
            }
            Call buffer.Add(mapObj)
        Next

        Dim maxL = buffer.ToArray(Function(x) x.Length).Max

        If maxL = 0 Then
            Return New Entity() {}
        End If

        For Each x In buffer
            Dim d As Integer = maxL - x.Length
            If d = 0 Then
                Continue For
            End If
            Dim v As Double() = NullSite.Repeats(d)
            x.Properties = x.Properties.Join(v).ToArray
        Next

        Return buffer.ToArray
    End Function

    ''' <summary>
    ''' x.Bits + x.PWM.Average => 1+ATGC/4
    ''' x.Bits + x.PWM.Average => 1+{0.25, 0.25, 0.25, 0.25}/4
    ''' </summary>
    Const NullSite As Double = 1 + 0.25
End Module
