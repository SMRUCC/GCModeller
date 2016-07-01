Imports SMRUCC.genomics.Assembly.SBML
Imports SMRUCC.genomics.Assembly.SBML.Specifics.MetaCyc
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataStructures
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Models.rFBA

    Public Class rFBA_ARGVS
        Public Property baseFactor As Double = 2.5
        Public Property PCCw As Double = 0.7
        Public Property sPCCw As Double = 0.25
        Public Property FluxBoundOverrides As Double = 25
        Public Property SupressImpact As Double = 1.2
        Public Property FluxOverrides As Boolean = True
        ''' <summary>
        ''' 假若反应是单向的，则乘以这个倍增系数
        ''' </summary>
        ''' <returns></returns>
        Public Property DirectedFactor As Double = 1.5
        Public Property forceEnzymeRev As Boolean = True

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class

    ''' <summary>
    ''' 基因突变的设置参数
    ''' </summary>
    Public Class Modifier : Implements sIdEnumerable

        Public Property locus As String Implements sIdEnumerable.Identifier
        ''' <summary>
        ''' 突变修饰
        ''' </summary>
        ''' <returns></returns>
        Public Property modify As Double
        Public Property Comments As String

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class
End Namespace