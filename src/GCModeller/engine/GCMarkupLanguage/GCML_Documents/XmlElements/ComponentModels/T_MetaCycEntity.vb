Imports System.Xml.Serialization
Imports LANS.SystemsBiology.Assembly.MetaCyc.File.DataFiles
Imports LANS.SystemsBiology.GCModeller.ModellingEngine.Assembly.DocumentFormat.GCMarkupLanguage.GCML_Documents.XmlElements
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

Namespace GCML_Documents.ComponentModels

    Public MustInherit Class T_MetaCycEntity(Of T As Slots.Object)
        Implements sIdEnumerable

        <XmlIgnore> Friend BaseType As T

        ''' <summary>
        ''' Current object that define in the MetaCyc database.(当前所定义的MetaCyc数据库对象的唯一标识符)
        ''' </summary>
        ''' <remarks></remarks>
        <XmlAttribute("UniqueId")>
        Public Overridable Property Identifier As String Implements sIdEnumerable.Identifier

        Public Overrides Function ToString() As String
            Return Identifier
        End Function
    End Class

    ''' <summary>
    ''' 目标对象是一种生物过程
    ''' </summary>
    ''' <remarks></remarks>
    Public Interface I_BiologicalProcess_EventHandle
        Function get_Regulators() As SignalTransductions.Regulator()
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="internal_GUID">这个可以是调控的motif位点对象，对于反应对象而言，这个参数似乎是没有用的</param>
        ''' <param name="Regulator"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function _add_Regulator(internal_GUID As String, Regulator As SignalTransductions.Regulator) As Boolean
    End Interface
End Namespace