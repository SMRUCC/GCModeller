Imports System.Xml.Serialization
Imports SMRUCC.genomics.AnalysisTools.CellPhenotype.Simulation.ExpressionRegulationNetwork.KineticsModel.Regulators
Imports Microsoft.VisualBasic.DataMining.Framework.DFL_Driver
Imports Microsoft.VisualBasic

Namespace Simulation.ExpressionRegulationNetwork.KineticsModel

    ''' <summary>
    ''' This object represents a regulatory motif site in the gene promoter region.
    ''' (启动子区的一个调控位点)
    ''' </summary>
    ''' <remarks></remarks>
    Public Class SiteInfo : Inherits dflNode

        Sub New()
            Call MyBase.New(New List(Of RegulationExpression))
        End Sub

        ''' <summary>
        ''' The position id value of the current regulatory site or the ATG distance of this site to the target gene.
        ''' (调控位点的编号或者说是当前的这个调控位点距离ATG的距离)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlAttribute> Public Property Position As Integer
        ''' <summary>
        ''' 该位点之上的调控因子
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlElement> Public Property Regulators As RegulationExpression()
            Get
                Return (From item In Me.__factorList Select DirectCast(item, RegulationExpression)).ToArray
            End Get
            Set(value As RegulationExpression())
                If Not value.IsNullOrEmpty Then
                    Call Me.__factorList.Clear()
                    For Each item In value
                        Call Me.__factorList.Add(item.set_TargetSite(Me))
                    Next
                End If
            End Set
        End Property

        Public Sub set_Regulator(idx As Integer, instance As RegulationExpression)
            Me.__factorList(idx) = instance.set_TargetSite(Me)
        End Sub

        Friend Function get_RegulatorQuantitySum() As Integer
            Dim LQuery = (From Regulator In Regulators Select Regulator.Quantity).ToArray.Sum
            Return LQuery
        End Function
    End Class
End Namespace