Imports Microsoft.VisualBasic.ComponentModel.Settings.Inf

Namespace Definitions

    ''' <summary>
    ''' 培养基配方之中的一个组分。
    ''' 
    ''' 该对象把「培养基的角色」与「某个数据库之中的化合物编号」以及「默认浓度」绑定在一起，
    ''' 从而让同一个配方可以跨数据库(MetaCyc / BioCyc / KEGG)复用：
    ''' 
    ''' <c>role</c> 是稳定的语义键(例如 glucose / ammonium / phosphate)，
    ''' <c>id</c> 才是具体数据库之中的编号(例如 GLC / C00031)。
    ''' </summary>
    <ClassName("medium-component")>
    Public Class MediumComponent

        ''' <summary>
        ''' 组分在培养基之中所扮演的角色，例如 ``glucose``、``ammonium``、``phosphate``
        ''' </summary>
        ''' <returns></returns>
        Public Property role As String

        ''' <summary>
        ''' 该组分在目标数据库之中的化合物编号
        ''' </summary>
        ''' <returns></returns>
        Public Property id As String

        ''' <summary>
        ''' 默认浓度(mM)。小于等于 0 的组分不会出现在默认培养基之中
        ''' </summary>
        ''' <returns></returns>
        Public Property concentration As Double

        Public Overrides Function ToString() As String
            Return $"{role}:{id}={concentration}"
        End Function

    End Class
End Namespace
