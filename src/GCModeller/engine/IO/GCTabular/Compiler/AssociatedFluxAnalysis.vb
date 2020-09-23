#Region "Microsoft.VisualBasic::138a341be3b3b57ccf22d6624c3b6ffe, engine\IO\GCTabular\Compiler\AssociatedFluxAnalysis.vb"

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

    '     Module AssociatedFluxAnalysis
    ' 
    '         Function: __InternalCheck_MetabolismFlux, __InternalCheck_TransmembraneFlux
    ' 
    '         Sub: ApplyAnalysis, CheckConsistent
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.ApplicationServices.Debugging.Logging
Imports SMRUCC.genomics.GCModeller.Assembly

Namespace Compiler.Components

    ''' <summary>
    ''' 分析计算出<see cref="FileStream.Metabolite.n_FluxAssociated"></see>属性的值
    ''' </summary>
    ''' <remarks></remarks>
    Module AssociatedFluxAnalysis

        ''' <summary>
        ''' 在去除代谢物的时候，请注意，仅去除Compound类型，其他的类型不需要去除
        ''' </summary>
        ''' <param name="Model"></param>
        ''' <remarks></remarks>
        Public Sub ApplyAnalysis(Model As FileStream.IO.XmlresxLoader)
            Dim FluxCollection = Model.MetabolismModel
            Dim LQuery = (From Metabolite As FileStream.Metabolite
                          In Model.MetabolitesModel.Values.AsParallel
                          Let get_Factor = (From item In FluxCollection Let ac = item.get_Coefficient(Metabolite.Identifier) Where ac <> 0 Select Reversible = item.Reversible, Sto = ac).ToArray
                          Let sum = Function() As FileStream.Metabolite

                                        Dim d_sum As Integer = 0

                                        For Each Line In get_Factor
                                            If Line.Reversible Then
                                                d_sum += System.Math.Abs(Line.Sto)
                                            Else
                                                '不可逆过程仅计数底物端，即消耗项
                                                If Line.Sto < 0 Then
                                                    d_sum += System.Math.Abs(Line.Sto) * 2
                                                End If
                                            End If
                                        Next
                                        Metabolite.n_FluxAssociated = d_sum
                                        If Metabolite.n_FluxAssociated >= 2 Then
                                            Metabolite.n_FluxAssociated /= 2
                                        End If
                                        Return Metabolite
                                    End Function()
                          Select sum).ToArray

            FluxCollection = Model.TransmembraneTransportation

            LQuery = (From Metabolite As FileStream.Metabolite
                       In Model.MetabolitesModel.Values.AsParallel
                      Let get_Factor = (From item In FluxCollection Let ac = item.get_Coefficient(Metabolite.Identifier) Where ac <> 0 Select Reversible = item.Reversible, Sto = ac).ToArray
                      Let sum = Function() As FileStream.Metabolite

                                    Dim d_sum As Integer = 0

                                    For Each Line In get_Factor
                                        If Line.Reversible Then
                                            d_sum += System.Math.Abs(Line.Sto)
                                        Else
                                            '不可逆过程仅计数底物端，即消耗项
                                            If Line.Sto < 0 Then
                                                d_sum += System.Math.Abs(Line.Sto) * 2
                                            End If
                                        End If
                                    Next

                                    If d_sum >= 2 Then
                                        d_sum /= 2
                                    End If

                                    Metabolite.n_FluxAssociated += d_sum '使用赋值的话会覆盖上一个检查的结果
                                    Return Metabolite
                                End Function()
                      Select sum).ToArray
        End Sub

        Private Function __InternalCheck_MetabolismFlux(Metabolites As KeyValuePair(Of Double, String)(), ByRef model As FileStream.IO.XmlresxLoader) As KeyValuePair(Of Double, String)()
            Dim List As List(Of KeyValuePair(Of Double, String)) = New List(Of KeyValuePair(Of Double, String))

            For Each substrate In Metabolites
                Dim id As String = substrate.Value

                If Not model.MetabolitesModel.ContainsKey(id) Then  '存在的不用管，仅添加不存在的对象
                    Dim LQuery = (From item In model.MetabolitesModel.AsParallel Where String.Equals(item.Value.MetaCycId, id) Select item).ToArray
                    If LQuery.IsNullOrEmpty Then
                        LQuery = (From item In model.MetabolitesModel.AsParallel Where String.Equals(item.Value.KEGGCompound, id) Select item).ToArray
                        If LQuery.IsNullOrEmpty Then '实在找不到了，则添加一个新的
                            Dim Metabolite As New FileStream.Metabolite With {.Identifier = id, .InitialAmount = 10}
                            Call model.MetabolitesModel.Add(id, Metabolite)
                            Call List.Add(substrate)
                        Else
                            Call List.Add(New KeyValuePair(Of Double, String)(substrate.Key, value:=LQuery.First.Value.Identifier))
                        End If
                    Else
                        Call List.Add(New KeyValuePair(Of Double, String)(substrate.Key, LQuery.First.Value.Identifier))
                    End If
                Else
                    Call List.Add(substrate)
                End If
            Next

            Return List.ToArray
        End Function

        ''' <summary>
        ''' 按照代谢反应之中的底物来检查模型之中的代谢物模型是否有缺失
        ''' </summary>
        ''' <param name="Model"></param>
        ''' <param name="Logging"></param>
        ''' <remarks></remarks>
        Public Sub CheckConsistent(Model As FileStream.IO.XmlresxLoader, Logging As LogFile)

            For Each FluxModel In Model.MetabolismModel
                FluxModel._Internal_compilerLeft = __InternalCheck_MetabolismFlux(FluxModel._Internal_compilerLeft, Model)
                FluxModel._Internal_compilerRight = __InternalCheck_MetabolismFlux(FluxModel._Internal_compilerRight, Model)
                Call FluxModel.ReCreateEquation()
            Next


            For Each FluxModel In Model.TransmembraneTransportation
                FluxModel._Internal_compilerLeft = __InternalCheck_TransmembraneFlux(FluxModel._Internal_compilerLeft, Model)
                FluxModel._Internal_compilerRight = __InternalCheck_TransmembraneFlux(FluxModel._Internal_compilerRight, Model)
                Call FluxModel.ReCreateEquation()
            Next

            Dim PolypeptideLQuery = (From item In Model.Proteins Where Not Model.MetabolitesModel.ContainsKey(item.Identifier) Select item.Identifier).ToArray     '查看蛋白质是否有缺失
            For Each Id As String In PolypeptideLQuery
                Call Model.MetabolitesModel.Add(Id, New FileStream.Metabolite With {.Identifier = Id, .InitialAmount = 10, .MetaboliteType = GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.Metabolite.MetaboliteTypes.Polypeptide})
            Next
        End Sub

        Private Function __InternalCheck_TransmembraneFlux(Metabolites As KeyValuePair(Of Double, String)(), ByRef model As FileStream.IO.XmlresxLoader) As KeyValuePair(Of Double, String)()
            Dim List As List(Of KeyValuePair(Of Double, String)) = New List(Of KeyValuePair(Of Double, String))

            For Each substrate In Metabolites
                Dim id As String = substrate.Value
                Dim Compartment As String = Regex.Match(id, "\[.+?\]").Value

                If Not String.IsNullOrEmpty(Compartment) Then
                    id = id.Replace(Compartment, "").Trim
                End If

                If Not model.MetabolitesModel.ContainsKey(id) Then  '存在的不用管，仅添加不存在的对象
                    Dim LQuery = (From item In model.MetabolitesModel.AsParallel Where String.Equals(item.Value.MetaCycId, id) Select item).ToArray
                    If LQuery.IsNullOrEmpty Then
                        LQuery = (From item In model.MetabolitesModel.AsParallel Where String.Equals(item.Value.KEGGCompound, id) Select item).ToArray
                        If LQuery.IsNullOrEmpty Then '实在找不到了，则添加一个新的
                            Dim Metabolite As New FileStream.Metabolite With {.Identifier = id, .InitialAmount = 10}
                            Call model.MetabolitesModel.Add(id, Metabolite)
                            Call List.Add(substrate)
                        Else
                            Call List.Add(New KeyValuePair(Of Double, String)(substrate.Key, value:=(LQuery.First.Value.Identifier & " " & Compartment).Trim))
                        End If
                    Else
                        Call List.Add(New KeyValuePair(Of Double, String)(substrate.Key, (LQuery.First.Value.Identifier & " " & Compartment).Trim))
                    End If
                Else
                    Call List.Add(substrate)
                End If
            Next

            Return List.ToArray
        End Function
    End Module
End Namespace
