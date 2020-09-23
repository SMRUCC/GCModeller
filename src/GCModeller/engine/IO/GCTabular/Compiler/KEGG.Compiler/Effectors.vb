#Region "Microsoft.VisualBasic::2864ca0f46e254b435e46590803f45fa, engine\IO\GCTabular\Compiler\KEGG.Compiler\Effectors.vb"

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

    '     Module Effectors
    ' 
    '         Function: CreateDictionary, MappingEffectors, MappingKEGGRegprecise
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic
Imports SMRUCC.genomics.Assembly
Imports SMRUCC.genomics.Data.Regprecise

Namespace KEGG.Compiler

    ''' <summary>
    ''' 本处理过程的目的是将模型之中的Regulation关系之中的Effector替换为KEGG的经过Normalization操作之后的UniqueId
    ''' </summary>
    ''' <remarks>
    ''' 首先将KEGGCoumpound在MetaCyc_ALL之间映射
    ''' </remarks>
    Public Module Effectors

        ''' <summary>
        ''' 可能会有重复的记录，仅仅依靠拓展函数无法解决这个问题，故而专门编写本方法来解决这个问题
        ''' </summary>
        ''' <param name="MetaCycCompounds"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function CreateDictionary(MetaCycCompounds As SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.Compounds) _
            As Dictionary(Of String, SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.Slots.Compound)

            Dim MetaCycCompoundWithKEGGCompounds = (From compound As SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.Slots.Compound
                                                    In MetaCycCompounds.AsParallel
                                                    Let KEGGId As String = compound.KEGGCompound
                                                    Where Not String.IsNullOrEmpty(KEGGId)
                                                    Select compound).ToArray
            Dim KEGGIdList = (From item In MetaCycCompoundWithKEGGCompounds Select item.KEGGCompound Distinct).ToArray
            Dim DictValue As Dictionary(Of String, SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.Slots.Compound) =
                New Dictionary(Of String, SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.Slots.Compound)

            For Each strId As String In KEGGIdList
                Call DictValue.Add(strId, (From item In MetaCycCompoundWithKEGGCompounds.AsParallel Where String.Equals(strId, item.KEGGCompound) Select item).First)
            Next

            Return DictValue
        End Function

        ''' <summary>
        ''' 在当前的这个函数之中已经将MetaCyc的标识符赋值给KEGGCompound了
        ''' </summary>
        ''' <param name="MetaCyc"></param>
        ''' <param name="KEGGCompounds"></param>
        ''' <param name="Regprecise"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function MappingEffectors(MetaCyc As MetaCyc.File.FileSystem.DatabaseLoadder,
                                         KEGGCompounds As List(Of FileStream.Metabolite),
                                         Regprecise As TranscriptionFactors) As List(Of MetaCyc.Schema.EffectorMap)

            Dim MetaCycCompoundsEqualsKEGG = CreateDictionary(MetaCyc.GetCompounds) '获取到所有带有KEGGCompound属性的MetaCyc代谢物
            Dim MappingResult As List(Of MetaCyc.Schema.EffectorMap) =
                New GCTabular.Mapping(MetaCyc, Nothing).EffectorMapping(Regprecise) '将Regprecise之中的Effector和MetaCyc代谢物进行映射

            For i As Integer = 0 To KEGGCompounds.Count - 1  '利用MetaCycID，将KEGGCompound映射到结果之中
                Dim Metabolite = KEGGCompounds(i)
                If MetaCycCompoundsEqualsKEGG.ContainsKey(Metabolite.KEGGCompound) Then
                    Metabolite.MetaCycId = MetaCycCompoundsEqualsKEGG(Metabolite.KEGGCompound).Identifier
                    '接着在mappingresult里面查找，查看是否有盖MetaCycId的对象，有的话，则修改Mappingresult对象里面的映射值为当前代谢物对象的UniqueId
                    Dim LQuery = (From item In MappingResult.AsParallel Where String.Equals(item.MetaCycId, Metabolite.MetaCycId) Select item).ToArray
                    If Not LQuery.IsNullOrEmpty Then
                        Dim Map = LQuery.First
                        Map.KEGGCompound = Metabolite.KEGGCompound
                        Map.MetaCycId = Metabolite.Identifier
                    End If
                End If
            Next

            'Dim Mapping = MappingKEGGRegprecise((From item As FileStream.Metabolite
            '                                     In KEGGCompounds
            '                                     Where Not String.IsNullOrEmpty(item.MetaCycId)
            '                                     Select item).ToArray.ToDictionary(Function(item As FileStream.Metabolite) item.MetaCycId), MappingResult)
            'Return Mapping
            Return MappingResult
        End Function

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="KEGGCompounds">{MetaCycId, Metabolite}</param>
        ''' <param name="MetaCycRegpreciseMapping"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function MappingKEGGRegprecise(KEGGCompounds As Dictionary(Of String, FileStream.Metabolite),
                                               MetaCycRegpreciseMapping As List(Of MetaCyc.Schema.EffectorMap)) As List(Of MetaCyc.Schema.EffectorMap)

            For i As Integer = 0 To MetaCycRegpreciseMapping.Count - 1
                Dim Mapping = MetaCycRegpreciseMapping(i)
                If String.IsNullOrEmpty(Mapping.MetaCycId) Then
                    Continue For
                End If
                If Not KEGGCompounds.ContainsKey(Mapping.MetaCycId) Then
                    Continue For
                End If

                Dim KEGGCompound = KEGGCompounds(Mapping.MetaCycId)
                Mapping.MetaCycId = KEGGCompound.Identifier
            Next

            Return MetaCycRegpreciseMapping
        End Function
    End Module
End Namespace
