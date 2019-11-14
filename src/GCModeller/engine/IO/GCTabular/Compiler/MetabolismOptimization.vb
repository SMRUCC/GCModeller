#Region "Microsoft.VisualBasic::d078e72902d8141adfc916a7de5b072f, engine\IO\GCTabular\Compiler\MetabolismOptimization.vb"

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

    '     Class MetabolismOptimization
    ' 
    '         Function: ApplyOptimation, Optimization
    ' 
    '         Sub: InreversibleRule, ReversibleRule
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.Data.csv.Extensions
Imports System.Text
Imports Microsoft.VisualBasic.Data.csv.IO

Namespace Compiler.Components

    Public Class MetabolismOptimization

        Const OptimizationScript As String =
            "imports solver.fba" & vbCrLf &
            "model______________0 <- loadmodel.from_csvtabular $model" & vbCrLf &
            "objectivefunction <- get.full_objective $model______________0" & vbCrLf &
            "model______________0 <- create_model.from_csvtabular model $model______________0 objectivefunction $objectivefunction" & vbCrLf &
 _
            "result <- fba.solve model $model______________0 r_bin $r" & vbCrLf &
            "result <- write.fba_result result $result saveto $result_save"

        Public Function Optimization(Model As FileStream.IO.XmlresxLoader) As File
            Dim script As StringBuilder = New StringBuilder("rbin <- solver.fba session.new")
            Dim rBin As String = Settings.Session.SettingsFile.R_HOME
            'Call ScriptEngine.MMUDevice.Update("model", Model)
            'Call ScriptEngine.MMUDevice.Update("r", rBin)
            'Call ScriptEngine.MMUDevice.Update("result_save", String.Format("{0}/Metabolism.Optimizations.csv", Model.ModelParentDir))
            'Call ScriptEngine.Exec(OptimizationScript)
            'Dim Result As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File =
            '    DirectCast(ScriptEngine.GetValue("$result"), Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File)
            'Call ApplyOptimation(Model, (From Line In Result.Skip(1) Select New KeyValuePair(Of String, Double)(Line.First, Val(Line(1)))).ToArray)

            'Return Result
        End Function

        Public Shared Function ApplyOptimation(Model As FileStream.IO.XmlresxLoader, Result As Generic.IEnumerable(Of KeyValuePair(Of String, Double))) As FileStream.IO.XmlresxLoader
            Dim Network = Model.MetabolismModel
            For Each Line In Result
                Dim FluxModel = Network.GetItem(Line.Key)
                Dim OptimizationValue As Double = Val(Line.Value)

                If FluxModel.Reversible Then
                    Call ReversibleRule(FluxModel, OptimizationValue)
                Else
                    Call InreversibleRule(FluxModel, OptimizationValue)
                End If
            Next

            Return Model
        End Function

        ''' <summary>
        ''' 如果优化值大于10或者小于-10，则另一个方向为当前方向的0.5
        ''' 假如优化值小于10或者大于-10，则两个方向的值都为10
        ''' </summary>
        ''' <param name="Model"></param>
        ''' <param name="OptimizationValue"></param>
        ''' <remarks></remarks>
        Private Shared Sub ReversibleRule(Model As FileStream.MetabolismFlux, OptimizationValue As Double)
            If OptimizationValue > 10 Then
                Model.UPPER_Bound = OptimizationValue
                Model.LOWER_Bound = -0.5 * OptimizationValue
            ElseIf OptimizationValue < -10 Then
                Model.UPPER_Bound = OptimizationValue * -0.5
                Model.LOWER_Bound = OptimizationValue
            Else
                Model.UPPER_Bound = 10
                Model.LOWER_Bound = -10
            End If
        End Sub

        ''' <summary>
        ''' 假若优化值小于10，则优化值为10
        ''' </summary>
        ''' <param name="Model"></param>
        ''' <param name="OptimizationValue"></param>
        ''' <remarks></remarks>
        Private Shared Sub InreversibleRule(Model As FileStream.MetabolismFlux, OptimizationValue As Double)
            If OptimizationValue < 10 Then
                Model.UPPER_Bound = 10
            Else
                Model.UPPER_Bound = OptimizationValue
            End If
        End Sub
    End Class
End Namespace
