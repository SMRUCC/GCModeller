Imports System.ComponentModel.Composition
Imports System.Resources

''' <summary>
''' External resource data EXPORT API
''' </summary>
<Export(GetType(ResourceManager))>
Public Module Resources

    ''' <summary>
    ''' Export the resource data to the assembly LANS.SystemsBiology.Assembly.Components_v2.0_33.0.0.0__89845dcd8080cc91
    ''' </summary>
    ''' <returns></returns>
    <Export(GetType(ResourceManager))>
    Public ReadOnly Property Resources As ResourceManager
        Get
            Return My.Resources.ResourceManager
        End Get
    End Property
End Module
