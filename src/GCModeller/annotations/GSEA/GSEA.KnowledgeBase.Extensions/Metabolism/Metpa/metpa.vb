Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace Metabolism.Metpa

    ''' <summary>
    ''' metpa symbol
    ''' </summary>
    Public Class metpa

        <Field("mset.list")> Public Property msetList As msetList
        <Field("rbc.list")> Public Property rbcList As rbcList
        <Field("path.ids")> Public Property pathIds As pathIds
        <Field("uniq.count")> Public Property unique_count As Integer
        <Field("path.smps")> Public Property pathSmps As pathSmps
        <Field("dgr.list")> Public Property dgrList As dgrList
        <Field("graph.list")> Public Property graphList As graphList

    End Class
End Namespace