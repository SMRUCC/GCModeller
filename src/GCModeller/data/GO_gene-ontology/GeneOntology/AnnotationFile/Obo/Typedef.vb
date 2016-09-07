Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.foundation.OBO_Foundry

Namespace OBO

    Public Class Typedef : Inherits base

        ''' <summary>
        ''' !通常是有一定含义的字符串， 而不是数字
        ''' </summary>
        ''' <returns></returns>
        <Field("")> Public Property is_anonymous As String
        <Field("")> Public Property alt_id As String
        <Field("")> Public Property def As String
        <Field("")> Public Property comment As String
        <Field("")> Public Property subset As String
        <Field("")> Public Property synonym As String
        <Field("")> Public Property xref As String
        ''' <summary>
        ''' !该关系仅对domain指定术语的亚类起作用
        ''' </summary>
        ''' <returns></returns>
        <Field("")> Public Property domain As String
        ''' <summary>
        ''' !任何具有这个关系的术语都属于range指定术语的亚类
        ''' </summary>
        ''' <returns></returns>
        <Field("")> Public Property range As String
        <Field("")> Public Property is_anti_symmetric As String
        ''' <summary>
        ''' !可否构建循环作用
        ''' </summary>
        ''' <returns></returns>
        <Field("")> Public Property is_cyclic As String
        ''' <summary>
        ''' !是否自反
        ''' </summary>
        ''' <returns></returns>
        <Field("")> Public Property is_reflexive As String
        ''' <summary>
        ''' !是否对称
        ''' </summary>
        ''' <returns></returns>
        <Field("is_symmetric")> Public Property is_symmetric As String
        ''' <summary>
        ''' !传递关系？
        ''' </summary>
        ''' <returns></returns>
        <Field("is_transitive")> Public Property is_transitive As String
        <Field("")> Public Property is_a As String
        ''' <summary>
        ''' !和另一关系相反。适用于原关系的两个术语，可以反方向适用另一关系。
        ''' </summary>
        ''' <returns></returns>
        <Field("")> Public Property inverse_of As String
        ''' <summary>
        ''' !将关系传递给下一个
        ''' </summary>
        ''' <returns></returns>
        <Field("transitive_over")> Public Property transitive_over As String
        <Field("")> Public Property relationship As String
        <Field("")> Public Property is_obsolete As String
        <Field("")> Public Property replaced_by As String
        <Field("")> Public Property consider As String
        <Field("is_metadata_tag")> Public Property is_metadata_tag As String
        <Field("is_class_level")> Public Property is_class_level As String
        <Field("holds_over_chain")> Public Property holds_over_chain As String

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class
End Namespace